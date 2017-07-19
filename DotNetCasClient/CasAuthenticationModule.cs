/*
 * Licensed to Apereo under one or more contributor license
 * agreements. See the NOTICE file distributed with this work
 * for additional information regarding copyright ownership.
 * Apereo licenses this file to you under the Apache License,
 * Version 2.0 (the "License"); you may not use this file
 * except in compliance with the License. You may obtain a
 * copy of the License at:
 * 
 * http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing,
 * software distributed under the License is distributed on
 * an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
 * KIND, either express or implied. See the License for the
 * specific language governing permissions and limitations
 * under the License.
 */

using System;
using System.Web;
using DotNetCasClient.Logging;
using DotNetCasClient.Utils;

namespace DotNetCasClient
{
    /// <summary>
    /// HttpModule implementation to intercept requests and perform authentication via CAS.
    /// </summary>
    /// <author>Marvin S. Addison</author>
    /// <author>Scott Holodak</author>
    /// <author>William G. Thompson, Jr.</author>
    /// <author>Catherine D. Winfrey</author>
    public sealed class CasAuthenticationModule : IHttpModule
    {
        private static readonly Logger logger = new Logger(Category.HttpModule);

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
        /// circuiting all event processing and firing EndRequest directly 
        /// (via CompleteRequest()).
        /// </summary>
        /// <param name="sender">The HttpApplication that sent the request</param>
        /// <param name="e">Not used</param>
        private static void OnBeginRequest(object sender, EventArgs e)
        {
            CasAuthentication.Initialize();

            HttpContext context = HttpContext.Current;
            HttpRequest request = context.Request;

            logger.Debug("Starting BeginRequest for " + request.RawUrl);
            
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

            // Detect & process inbound Single SignOut Requests from the CAS server
            if (CasAuthentication.ServiceTicketManager != null && CasAuthentication.ProcessIncomingSingleSignOutRequests && RequestEvaluator.GetRequestIsCasSingleSignOut())
            {
                logger.Info("Processing inbound Single Sign Out request.");
                CasAuthentication.ProcessSingleSignOutRequest();
                return;
            }

            // Detect & process inbound proxy callback verifications from the CAS server
            if (CasAuthentication.ProxyTicketManager != null && RequestEvaluator.GetRequestIsProxyResponse())
            {
                logger.Info("Processing Proxy Callback request");
                CasAuthentication.ProcessProxyCallbackRequest();
                return;
            }

            logger.Debug("Ending BeginRequest for " + request.RawUrl);
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
                logger.Debug("AuthenticateRequest bypassed for " + request.RawUrl);
                return;
            }

            // Validate the ticket coming back from the CAS server
            if (RequestEvaluator.GetRequestHasCasTicket())
            {
                logger.Info("Processing Proxy Callback request");
                CasAuthentication.ProcessTicketValidation();
            }

            logger.Debug("Starting AuthenticateRequest for " + request.RawUrl);
            CasAuthentication.ProcessRequestAuthentication();
            logger.Debug("Ending AuthenticateRequest for " + request.RawUrl);
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
                logger.Debug("Starting EndRequest for " + request.RawUrl);

                if (RequestEvaluator.GetRequestRequiresGateway())
                {
                    logger.Info("  Performing Gateway Authentication");
                    CasAuthentication.GatewayAuthenticate(true);
                }
                else if (RequestEvaluator.GetUserDoesNotAllowSessionCookies())
                {
                    logger.Info("  Cookies not supported.  Redirecting to Cookies Required Page");
                    CasAuthentication.RedirectToCookiesRequiredPage();
                }
                else if (RequestEvaluator.GetRequestHasCasTicket())
                {
                    logger.Info("  Redirecting from login callback");
                    CasAuthentication.RedirectFromLoginCallback();
                }
                else if (RequestEvaluator.GetRequestHasGatewayParameter()) 
                {
                    logger.Info("  Redirecting from failed gateway callback");
                    CasAuthentication.RedirectFromFailedGatewayCallback();
                }
                else if (RequestEvaluator.GetRequestIsUnauthorized() && !String.IsNullOrEmpty(CasAuthentication.NotAuthorizedUrl))
                {
                    logger.Info("  Redirecting to Unauthorized Page");
                    CasAuthentication.RedirectToNotAuthorizedPage();
                }
                else if (RequestEvaluator.GetRequestIsUnauthorized())
                {
                    logger.Info("  Redirecting to CAS Login Page (Unauthorized without NotAuthorizedUrl defined)");
                    CasAuthentication.RedirectToLoginPage(true);
                }
                else if (RequestEvaluator.GetRequestIsUnAuthenticated())
                {
                    logger.Info("  Redirecting to CAS Login Page");
                    CasAuthentication.RedirectToLoginPage();
                }

                logger.Debug("Ending EndRequest for " + request.RawUrl);
            }
            else
            {
                logger.Debug("No EndRequest processing for " + request.RawUrl);
            }
        }
   }
}