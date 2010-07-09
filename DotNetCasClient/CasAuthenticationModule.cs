/*
 * Copyright 2007-2010 The JA-SIG Collaborative. All rights reserved. See license
 * distributed with this file and available online at
 * http://www.ja-sig.org/products/cas/overview/license/index.html
 */

using System;
using System.Web;
using DotNetCasClient.Utils;
using log4net;

namespace DotNetCasClient
{
    /// <summary>
    /// HttpModule implementation to intercept requests and perform authentication via CAS.
    /// </summary>
    public sealed class CasAuthenticationModule : IHttpModule
    {
        /// <summary>
        /// Access to the log file
        /// </summary>
        private static readonly ILog Log = LogManager.GetLogger("CasAuthenticationModule");

        /// <summary>
        /// Performs initializations / startup functionality when an instance of this HttpModule
        /// is being created.
        /// </summary>
        /// <param name="context">the current HttpApplication</param>        
        public void Init(HttpApplication context)
        {
            // Register our event handlers.  These are fired on every HttpRequest.
            context.BeginRequest += OnBeginRequest;
            context.AuthenticateRequest += OnAuthenticateRequest;
            context.EndRequest += OnEndRequest;
        }

        /// <summary>
        /// Performs cleanup when an instance of this HttpModule is being destroyed.
        /// </summary>
        public void Dispose()
        {
        }

        /// <summary>
        /// Intercepts the beginning of the request pipeline.  This will detect SingleSignOut 
        /// requests.  SingleSignOut requests are posted back to the serviceName URL that
        /// was passed when the CAS session was established.  Since the behavior of the script
        /// at that URL is unknown, a POST back by the CAS server could have unexpected 
        /// consequences.  We want to prevent this request from authenticating and from 
        /// executing the HttpHandler typically associated with that URL.  So we are handling 
        /// this by sending back an HTTP 200 (OK) message with a blank body and short 
        /// circuiting all event processing & firing EndRequest directly 
        /// (via CompleteRequest()).
        /// </summary>
        /// <param name="sender">The HttpApplication that sent the request</param>
        /// <param name="e">Not used</param>
        private static void OnBeginRequest(object sender, EventArgs e)
        {
            CasAuthentication.Initialize();

            HttpContext context = HttpContext.Current;
            HttpRequest request = context.Request;

            Log.DebugFormat("Starting BeginRequest for {0}", request.RawUrl);
            
            // Cleanup expired ServiceTickets in the ServiceTicketManager
            if (CasAuthentication.ServiceTicketManager != null)
            {
                CasAuthentication.ServiceTicketManager.RemoveExpiredTickets();
            }

            // Cleanup expired ProxyTicket mappings in the ProxyTicketManager
            if (CasAuthentication.ProxyTicketManager != null)
            {
                CasAuthentication.ProxyTicketManager.RemoveExpiredMappings();
            }

            // Detect & process inbound Single Signout Requests from the CAS server
            if (CasAuthentication.ServiceTicketManager != null && CasAuthentication.SingleSignOut && RequestEvaluator.GetRequestIsCasSingleSignout())
            {
                Log.Debug("Processing inbound Single Sign Out request.");
                CasAuthentication.ProcessSingleSignOutRequest();
                return;
            }

            // Detect & process inbound proxy callback verifications from the CAS server
            if (CasAuthentication.ProxyTicketManager != null && RequestEvaluator.GetRequestIsProxyResponse())
            {
                Log.Debug("Processing Proxy Callback request");
                CasAuthentication.ProcessProxyCallbackRequest();
                return;
            }

            Log.DebugFormat("Ending BeginRequest for {0}", request.RawUrl);
        }

        /// <summary>
        /// Handles the authentication of the request.  
        /// 
        /// If the request contains a ticket, this will validate the ticket and create a 
        /// FormsAuthenticationTicket and encrypted cookie container for it.  It will redirect 
        /// to remove the ticket from the URL.  With Forms-based authentication, this is 
        /// required to prevent the client from automatically/silently re-authenticating on a 
        /// refresh or after logout.  
        /// 
        /// If the request does not contain a ticket, it checks for a FormsAuthentication 
        /// cookie, decrypts it, extracts the FormsAuthenticationTicket, verifies that it 
        /// exists in the StateProvider/ServiceTicketManager, and assigns a Principal to the 
        /// thread and context.User properties.  All events after this request become 
        /// authenticated.
        /// </summary>
        /// <param name="sender">The HttpApplication that sent the request</param>
        /// <param name="e">Not used</param>
        private static void OnAuthenticateRequest(object sender, EventArgs e)
        {
            HttpContext context = HttpContext.Current;
            HttpRequest request = context.Request;

            // Validate the ticket coming back from the CAS server
            if (!RequestEvaluator.GetRequestIsAppropriateForCasAuthentication())
            {
                Log.DebugFormat("AuthenticateRequest bypassed for {0}", request.RawUrl);
                return;
            }

            // Validate the ticket coming back from the CAS server
            if (RequestEvaluator.GetRequestHasCasTicket())
            {
                Log.Debug("Processing Proxy Callback request");
                CasAuthentication.ProcessTicketValidation();
            }


            Log.DebugFormat("Starting AuthenticateRequest for {0}", request.RawUrl);                
            CasAuthentication.ProcessRequestAuthentication();              
            Log.DebugFormat("Ending AuthenticateRequest for {0}", request.RawUrl);
        }


        /// <summary>
        /// EndRequest is used to trigger the appropriate redirection.  There are
        /// currently three scenarios that require special redirections.  
        /// <list>
        ///     <item>
        ///         Request is unauthenticated and is being routed to the FormsLoginUrl
        ///         (typically caused by UrlAuthorizationModule).  This request needs to
        ///         be intercepted to change the 'ReturnUrl' parameter to 'serviceName'
        ///     </item>
        ///     <item>
        ///         Request contains a CAS ticket in the URL.  This request needs to be
        ///         redirected back to itself without the 'ticket' parameter in order to
        ///         avoid potential infinite automatic ticket validation loops for when
        ///         a the ticket in the URL has expired or was revoked and the Renew 
        ///         configuration parameter is set.
        ///     </item>
        ///     <item>
        ///         Request is authenticated, but is not authorized to access the 
        ///         requested resource (by UrlAuthorizationModule).  If the CAS is 
        ///         configured with a NotAuthorizedUrl, the request is redirected to 
        ///         that page.  Otherwise, it is redirected to the CAS login page with
        ///         a forced 'Renew' property (to prevent infinite redirect loops).
        ///     </item>
        /// </list>
        /// </summary>
        /// <param name="sender">The HttpApplication that sent the request</param>
        /// <param name="e">Not used</param>
        private static void OnEndRequest(object sender, EventArgs e)
        {
            HttpContext context = HttpContext.Current;
            HttpRequest request = context.Request;

            if (RequestEvaluator.GetRequestIsAppropriateForCasAuthentication())
            {
                Log.DebugFormat("Starting EndRequest for {0}", request.RawUrl);

                if (RequestEvaluator.GetRequestRequiresGateway())
                {
                    Log.DebugFormat("  Performing Gateway Authentication");
                    CasAuthentication.GatewayAuthenticate(true);
                }
                else if (RequestEvaluator.GetUserDoesNotAllowSessionCookies())
                {
                    Log.DebugFormat("  Cookies not supported.  Redirecting to Cookies Required Page");
                    RedirectUtil.RedirectToCookiesRequiredPage();
                }
                else if (RequestEvaluator.GetRequestHasCasTicket())
                {
                    Log.DebugFormat("  Redirecting from login callback");
                    RedirectUtil.RedirectFromLoginCallback();
                }
                else if (RequestEvaluator.GetRequestHasGatewayParameter()) 
                {
                    Log.DebugFormat("  Redirecting from failed gateway callback");
                    RedirectUtil.RedirectFromFailedGatewayCallback();
                }
                else if (RequestEvaluator.GetRequestIsUnauthorized() && !String.IsNullOrEmpty(CasAuthentication.NotAuthorizedUrl))
                {
                    Log.DebugFormat("  Redirecting to Unauthorized Page");
                    RedirectUtil.RedirectToUnauthorizedPage();
                }
                else if (RequestEvaluator.GetRequestIsUnauthorized())
                {
                    Log.DebugFormat("  Redirecting to CAS Login Page (Unauthorized without NotAuthorizedUrl defined)");
                    RedirectUtil.RedirectToLoginPage(true);
                }
                else if (RequestEvaluator.GetRequestIsUnAuthenticated())
                {
                    Log.DebugFormat("  Redirecting to CAS Login Page");
                    RedirectUtil.RedirectToLoginPage();
                }

                Log.DebugFormat("Ending EndRequest for {0}", request.RawUrl);
            }
            else
            {
                Log.DebugFormat("No EndRequest processing for {0}", request.RawUrl);
            }
        }
   }
}