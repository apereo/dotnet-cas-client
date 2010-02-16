using System;
using System.Threading;
using System.Web;
using System.Web.Security;
using DotNetCasClient.Security;
using DotNetCasClient.Utils;

namespace DotNetCasClient
{
    /// <summary>
    /// HttpModule implementation to intercept requests and perform authentication via CAS.
    /// </summary>
    public sealed class CasAuthenticationModule : AbstractFormsAuthenticationCasModule
    {
        private bool requestAuthenticated;

        /// <summary>
        /// Performs initializations / startup functionality when an instance of this HttpModule
        /// is being created.
        /// </summary>
        /// <param name="context">the current HttpApplication</param>        
        public override void Init(HttpApplication context)
        {
            base.Init(context);
            
            // Register our event handlers.  These are fired on every HttpRequest.
            context.BeginRequest += OnBeginRequest;
            context.AuthenticateRequest += OnAuthenticateRequest;
            context.PreSendRequestHeaders += OnPreSendRequestHeaders;
            context.PreSendRequestContent += OnPreSendRequestContent;
            context.EndRequest += OnEndRequest;
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
        private void OnBeginRequest(object sender, EventArgs e)
        {
            HttpApplication app = (HttpApplication)sender;
            HttpContext context = app.Context;
            HttpResponse response = context.Response;

            if (SingleSignOut && TicketManager != null)
            {
                if (ProcessSingleSignOutRequest(app))
                {
                    if (log.IsDebugEnabled)
                    {
                        log.DebugFormat("{0}:SingleSignOut returned true --> processed CAS logoutRequest", CommonUtils.MethodName);
                    }

                    response.StatusCode = 200;
                    response.ContentType = "text/plain";
                    response.Clear();
                    response.Write("OK");

                    app.CompleteRequest();
                }
                else
                {
                    if (log.IsDebugEnabled)
                    {
                        log.DebugFormat("{0}:SingleSignOut returned false --> did not receive client request", CommonUtils.MethodName);
                    }
                }
            }
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
        /// exists in the StateProvider/TicketManager, and assigns a Principal to the 
        /// thread and context.User properties.  All events after this request become 
        /// authenticated.
        /// </summary>
        /// <param name="sender">The HttpApplication that sent the request</param>
        /// <param name="e">Not used</param>
        private void OnAuthenticateRequest(object sender, EventArgs e)
        {
            requestAuthenticated = true;

            if (log.IsDebugEnabled)
            {
                log.DebugFormat("{0}:starting:Summary:{1}", CommonUtils.MethodName, DebugUtils.FormsAuthRequestSummaryToString((HttpApplication)sender));
                log.DebugFormat("{0}:starting with {1} and {2}", CommonUtils.MethodName, DebugUtils.CookieSessionIdToString((HttpApplication)sender), DebugUtils.SessionSessionIdToString((HttpApplication)sender));
            }

            HttpApplication app = (HttpApplication)sender;
            HttpContext context = app.Context;
            HttpRequest request = context.Request;
            HttpResponse response = context.Response;            
            ICasPrincipal principal;

            // Construct Service URL and save as request Item in case it is needed.
            context.Items[CommonUtils.CAS_KEY_REDIRECT_URI] = ConstructRedirectUri(context, CasServerLoginUrl); ;

            // See if this request is the first request redirected from the CAS server 
            // with a Ticket parameter.
            string ticket = request[ticketValidator.ArtifactParameterName];
            if (ticket != null && CommonUtils.IsNotBlank(ticket))
            {
                // Attempt to authenticate the ticket and resolve to an ICasPrincipal
                principal = ticketValidator.Validate(ticket, ConstructServiceUri(app.Request));

                // Save the ticket as UserData in a FormsAuthTicket.  Encrypt the ticket and send it as a cookie. 
                FormsAuthenticationTicket formsAuthTicket = CreateFormsAuthenticationTicket(principal.Identity.Name, false, FormsAuthentication.FormsCookiePath, ticket);
                SetAuthCookie(formsAuthTicket);

                // Also save the ticket in the server store (if configured)
                if (TicketManager != null)
                {
                    TicketManager.RevokeTicket(ticket);
                    TicketManager.InsertTicket(formsAuthTicket, formsAuthTicket.Expiration);
                }

                // Remove the ticket from the URL and redirect if configured to RedirectAfterValidation.
                int artifactIndex = request.Url.AbsoluteUri.IndexOf(ticketValidator.ArtifactParameterName);
                if (artifactIndex > 0 && (request.Url.AbsoluteUri[artifactIndex - 1] == '?' || request.Url.AbsoluteUri[artifactIndex - 1] == '&'))
                {
                    response.Redirect(RemoveQueryStringVariableFromUrl(request.Url.AbsoluteUri, ticketValidator.ArtifactParameterName));
                }
            }

            // Look for a valid FormsAuthenticationTicket encrypted in a cookie.
            FormsAuthenticationTicket formsAuthenticationTicket = GetFormsAuthenticationTicket(context);
            if (formsAuthenticationTicket != null)
            {
                if (TicketManager != null)
                {
                    if (!TicketManager.VerifyClientTicket(formsAuthenticationTicket))
                    {
                        if (log.IsDebugEnabled)
                        {
                            log.DebugFormat("{0}:Ticket [{1}] failed verification.", CommonUtils.MethodName, formsAuthenticationTicket.UserData);
                        }

                        // Deletes the invalid FormsAuthentication cookie from the client.
                        ClearAuthCookie();
                        TicketManager.RevokeTicket(formsAuthenticationTicket);

                        // Don't give this request a User/Principal.
                        return;
                    }
                }

                // If the ticket exists & is still valid, create a new CasPrincipal with the NetID
                principal = new CasPrincipal(new Assertion(formsAuthenticationTicket.Name));
                context.User = principal;
                Thread.CurrentPrincipal = principal;

                // Extend the expiration of the cookie if FormsAuthentication is configured to do so.
                if (FormsAuthentication.SlidingExpiration)
                {
                    FormsAuthenticationTicket newTicket = FormsAuthentication.RenewTicketIfOld(formsAuthenticationTicket);
                    if (newTicket != null && newTicket != formsAuthenticationTicket)
                    {
                        SetAuthCookie(newTicket);
                        if (TicketManager != null)
                        {
                            TicketManager.UpdateTicketExpiration(newTicket, newTicket.Expiration);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// This checks to see whether request authorization failed (typically caused by the 
        /// UrlAuthorizationModule during AuthorizeRequest).  That would indicate that the user
        /// is attempting to access a protected resource with either no credentials or 
        /// insufficient privileges.  The ASP.NET behavior is to send users to the login page 
        /// again.
        /// </summary>
        /// <param name="sender">The HttpApplication that sent the request</param>
        /// <param name="e">Not used</param>        
        private void OnEndRequest(object sender, EventArgs args)
        {
            // TODO: See what happens when a valid user accesses a page they aren't authorized for.

            if (requestAuthenticated)
            {
                requestAuthenticated = false;
            }
            else
            {
                return;
            }

            HttpApplication app = (HttpApplication)sender;
            HttpRequest request = app.Context.Request;
            HttpResponse response = app.Context.Response;

            // If we got an HTTP 401 Error (Unauthorized) and don't have a CAS ticket
            // in the URL, redirect to CAS.

            // TODO: We can implement a NotAuthorizedPage handler here.  CustomErrors 
            //       doesn't allow us to attach to the 401 error code, but we can 
            //       bypass the redirect to login page here.
            if ((response.StatusCode == 401 && request.QueryString[ticketValidator.ArtifactParameterName] == null))
            {
                // construct CAS RedirectURL and do redirect
                FormsAuthentication.RedirectToLoginPage();
            }
        }

        /// <summary>
        /// FormsAuthentication will attempt to redirect to the configured
        /// login page with a ReturnUrl parameter in the URL.  Since we are
        /// trying to go directly to the correct CAS login page, this will
        /// intercept that Location header and replace it with the correctly
        /// configured CAS redirect URL.
        /// </summary>
        /// <param name="sender">The <code>HttpApplication</code> executing the request</param>
        /// <param name="e">Unused</param>
        private void OnPreSendRequestHeaders(object sender, EventArgs e)
        {
            HttpApplication app = (HttpApplication)sender;
            HttpContext context = app.Context;
            HttpResponse response = context.Response;
            if (!string.IsNullOrEmpty(response.Headers["Location"]) && response.Headers["Location"].StartsWith(FormsAuthConfig.LoginUrl))
            {
                string redirectUrl = context.Items[CommonUtils.CAS_KEY_REDIRECT_URI].ToString();
                int newContentLength = HtmlRedirectTemplateLength + redirectUrl.Length;
                response.Headers["Location"] = redirectUrl;
                response.Headers["Content-Length"] = newContentLength.ToString();
            }
        }

        /// <summary>
        /// FormsAuthentication sends a minimal HTML response to the browser
        /// with a link to the login page for browsers that don't support or 
        /// disabled HTTP redirects.  We need to change the URL to the proper
        /// CAS URL.  To eliminate the need for installing a Filter in the 
        /// pipeline, the HTML that it renders is defined as a constant.  
        /// </summary>
        /// <param name="sender">The <code>HttpApplication</code> executing the request</param>
        /// <param name="e">Unused</param>
        private void OnPreSendRequestContent(object sender, EventArgs e)
        {
            HttpApplication app = (HttpApplication)sender;
            HttpContext context = app.Context;
            HttpResponse response = context.Response;
            if (!string.IsNullOrEmpty(response.Headers["Location"]) && response.Headers["Location"].StartsWith(FormsAuthConfig.LoginUrl))
            {
                response.Clear();
                response.Write(string.Format(HtmlRedirectTemplate, context.Items[CommonUtils.CAS_KEY_REDIRECT_URI]));
                
                // No need to call response.End() or application.CompleteRequest() 
                // because the EndRequest event was already fired.
            }
        }

        /// <summary>
        /// Process SingleSignOut requests by removing the ticket from the state store.
        /// </summary>
        /// <param name="application"></param>
        /// <returns>
        /// Boolean indicating whether the request was a SingleSignOut request, regardless of
        /// whether or not the request actually required processing (non-existent/already expired).
        /// </returns>
        public bool ProcessSingleSignOutRequest(HttpApplication application)
        {
            // TODO: Should we be checking to make sure that this special POST is coming from a trusted source?
            //       It would be tricky to do this by IP address because there might be a white list or something.

            // TODO: What about confirming that the original serviceName when the ticket was requested is the URL
            //       that CAS' SSO is connecting to now.  This would require storing more than just the CAS ticket
            //       in the FormsAuthenticationTicket's UserData property (possibly XML or URL Encoded?)

            if (!SingleSignOut || TicketManager == null)
            {
                throw new InvalidOperationException("Single Sign Out request cannot be handled without the SingleSignoutProperty set and a FormsAuthenticationStateManager configured.");
            }

            HttpRequest request = application.Request;

            bool logoutRequestReceived = false;
            if (request.RequestType == "POST")
            {
                string logoutRequest = request.Params["logoutRequest"];
                if (log.IsDebugEnabled)
                {
                    log.DebugFormat("{0}:POST logoutRequest={1}", CommonUtils.MethodName, (logoutRequest ?? "null"));
                }
                if (CommonUtils.IsNotBlank(logoutRequest))
                {
                    logoutRequestReceived = true;
                    string casTicket = ExtractSingleSignOutTicketFromSamlResponse(logoutRequest);
                    if (log.IsDebugEnabled)
                    {
                        log.DebugFormat("{0}:casTicket=[{1}]", CommonUtils.MethodName, casTicket);
                    }
                    if (CommonUtils.IsNotBlank(casTicket))
                    {
                        FormsAuthenticationTicket ticket = TicketManager.GetTicket(casTicket);
                        if (ticket != null)
                        {
                            if (log.IsDebugEnabled)
                            {
                                log.DebugFormat("{0}:Revoked casTicket [{1}]]", CommonUtils.MethodName, casTicket);
                            }
                            TicketManager.RevokeTicket(casTicket);
                        }
                        else
                        {
                            if (log.IsDebugEnabled)
                            {
                                log.DebugFormat("{0}:Unable to revoke casTicket [{1}]", CommonUtils.MethodName, casTicket);
                            }
                        }
                    }
                }
            }

            return logoutRequestReceived;
        }
    }
}