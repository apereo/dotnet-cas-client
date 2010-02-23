using System;
using System.IO;
using System.Threading;
using System.Web;
using System.Web.Security;
using System.Xml;
using DotNetCasClient.Security;
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
        /// SAML element containing CAS ticket during SSO 
        /// request
        /// </summary>
        private const string XML_SESSION_INDEX_ELEMENT_NAME = "samlp:SessionIndex";

        /// <summary>
        /// XML Reader Settings for SAML parsing.
        /// </summary>
        private static XmlReaderSettings _xmlReaderSettings;

        /// <summary>
        /// XML Name Table for namespace resolution in SSO 
        /// SAML Parsing routine
        /// </summary>
        private static NameTable _xmlNameTable;

        /// <summary>
        /// XML Namespace Manager for namespace resolution in 
        /// SSO SAML Parsing routine
        /// </summary>
        private static XmlNamespaceManager _xmlNamespaceManager;

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
            _xmlReaderSettings = new XmlReaderSettings();
            _xmlReaderSettings.ConformanceLevel = ConformanceLevel.Auto;
            _xmlReaderSettings.IgnoreWhitespace = true;

            _xmlNameTable = new NameTable();

            _xmlNamespaceManager = new XmlNamespaceManager(_xmlNameTable);
            _xmlNamespaceManager.AddNamespace("cas", "http://www.yale.edu/tp/cas");
            _xmlNamespaceManager.AddNamespace("saml", "urn: oasis:names:tc:SAML:1.0:assertion");
            _xmlNamespaceManager.AddNamespace("saml2", "urn: oasis:names:tc:SAML:1.0:assertion");
            _xmlNamespaceManager.AddNamespace("samlp", "urn: oasis:names:tc:SAML:1.0:protocol");

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
            HttpContext context = HttpContext.Current;
            HttpResponse response = context.Response;

            CasAuthentication.Initialize();

            if (CasAuthentication.TicketManager != null)
            {
                // Cleanup the Ticket Manager
                CasAuthentication.TicketManager.RemoveExpiredTickets();

                if (CasAuthentication.SingleSignOut)
                {
                    if (ProcessSingleSignOutRequest())
                    {
                        if (Log.IsDebugEnabled)
                        {
                            Log.DebugFormat("{0}:SingleSignOut returned true --> processed CAS logoutRequest", CommonUtils.MethodName);
                        }

                        response.StatusCode = 200;
                        response.ContentType = "text/plain";
                        response.Clear();
                        response.Write("OK");

                        context.ApplicationInstance.CompleteRequest();
                    }
                    else
                    {
                        if (Log.IsDebugEnabled)
                        {
                            Log.DebugFormat("{0}:SingleSignOut returned false --> did not receive client request", CommonUtils.MethodName);
                        }
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
        private static void OnAuthenticateRequest(object sender, EventArgs e)
        {
            HttpApplication app = (HttpApplication)sender;
            HttpContext context = HttpContext.Current;
            HttpRequest request = context.Request;
            ICasPrincipal principal;

            CasAuthenticationTicket casTicket = null;

            if (Log.IsDebugEnabled)
            {
                Log.DebugFormat("{0}:starting:Summary:{1}", CommonUtils.MethodName, DebugUtils.FormsAuthRequestSummaryToString(app));
                Log.DebugFormat("{0}:starting with {1} and {2}", CommonUtils.MethodName, DebugUtils.CookieSessionIdToString(app), DebugUtils.SessionSessionIdToString(app));
            }

            // See if this request is the first request redirected from the CAS server 
            // with a Ticket parameter.
            string ticket = request[CasAuthentication.TicketValidator.ArtifactParameterName];
            if (!string.IsNullOrEmpty(ticket))
            {
                // Attempt to authenticate the ticket and resolve to an ICasPrincipal
                principal = CasAuthentication.TicketValidator.Validate(ticket, new Uri(CasAuthentication.ConstructServiceUri()));

                // Save the ticket in the FormsAuthTicket.  Encrypt the ticket and send it as a cookie. 
                casTicket = new CasAuthenticationTicket(
                    ticket,
                    RemoveQueryStringVariableFromUrl(request.RawUrl, request[CasAuthentication.TicketValidator.ArtifactParameterName]),
                    request.UserHostAddress,
                    principal.Assertion
                );

                // TODO: Check the last 2 parameters.  We want to take the from/to dates from the 
                // FormsAuthenticationTicket.  However, we may need to do some NTP-style clock
                // calibration
                FormsAuthenticationTicket formsAuthTicket = CasAuthentication.CreateFormsAuthenticationTicket(principal.Identity.Name, FormsAuthentication.FormsCookiePath, ticket, null, null);
                CasAuthentication.SetAuthCookie(formsAuthTicket);

                // Also save the ticket in the server store (if configured)
                if (CasAuthentication.TicketManager != null)
                {
                    CasAuthentication.TicketManager.UpdateTicketExpiration(casTicket, formsAuthTicket.Expiration);
                }

                int artifactIndex = request.Url.AbsoluteUri.IndexOf(CasAuthentication.TicketValidator.ArtifactParameterName);
                bool requestHasCasTicket = (request[CasAuthentication.TicketValidator.ArtifactParameterName] != null && !string.IsNullOrEmpty(request[CasAuthentication.TicketValidator.ArtifactParameterName]));
                bool requestIsInboundCasResponse = (requestHasCasTicket && artifactIndex > 0 && (request.Url.AbsoluteUri[artifactIndex - 1] == '?' || request.Url.AbsoluteUri[artifactIndex - 1] == '&'));
                if (requestIsInboundCasResponse)
                {
                    // Jump directly to EndRequest.  Don't allow the Page and/or Handler to execute
                    app.CompleteRequest();
                    return;
                }
            }

            // Look for a valid FormsAuthenticationTicket encrypted in a cookie.
            FormsAuthenticationTicket formsAuthenticationTicket = CasAuthentication.GetFormsAuthenticationTicket();
            if (formsAuthenticationTicket != null)
            {
                if (CasAuthentication.TicketManager != null)
                {
                    string serviceTicket = formsAuthenticationTicket.UserData;
                    casTicket = CasAuthentication.TicketManager.GetTicket(serviceTicket);
                    if (casTicket != null)
                    {
                        IAssertion assertion = casTicket.Assertion;

                        if (!CasAuthentication.TicketManager.VerifyClientTicket(casTicket))
                        {
                            if (Log.IsDebugEnabled)
                            {
                                Log.DebugFormat("{0}:Ticket failed verification." + Environment.NewLine, CommonUtils.MethodName);
                            }

                            // Deletes the invalid FormsAuthentication cookie from the client.
                            CasAuthentication.ClearAuthCookie();
                            CasAuthentication.TicketManager.RevokeTicket(serviceTicket);

                            // Don't give this request a User/Principal.  Remove it if it was created
                            // by the underlying FormsAuthenticationModule or another module.
                            principal = null;
                        }
                        else
                        {
                            principal = new CasPrincipal(assertion);
                        }
                    }
                    else
                    {
                        // This didn't resolve to a ticket in the TicketStore.  Revoke it.
                        CasAuthentication.ClearAuthCookie();
                        CasAuthentication.TicketManager.RevokeTicket(serviceTicket);

                        // Don't give this request a User/Principal.  Remove it if it was created
                        // by the underlying FormsAuthenticationModule or another module.
                        principal = null;
                    }
                }
                else
                {
                    principal = new CasPrincipal(new Assertion(formsAuthenticationTicket.Name));
                }

                context.User = principal;
                Thread.CurrentPrincipal = principal;

                if (principal == null)
                {
                    // Remove the cookie from the client
                    CasAuthentication.ClearAuthCookie();
                }
                else
                {
                    // Extend the expiration of the cookie if FormsAuthentication is configured to do so.
                    if (FormsAuthentication.SlidingExpiration)
                    {
                        FormsAuthenticationTicket newTicket = FormsAuthentication.RenewTicketIfOld(formsAuthenticationTicket);
                        if (newTicket != null && newTicket != formsAuthenticationTicket)
                        {
                            CasAuthentication.SetAuthCookie(newTicket);
                            if (CasAuthentication.TicketManager != null)
                            {
                                CasAuthentication.TicketManager.UpdateTicketExpiration(casTicket, newTicket.Expiration);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// EndRequest is used to trigger the appropriate redirection.  There are
        /// currently three scenarios that require special redirections.  
        /// 
        ///  - Request is unauthenticated and is being routed to the FormsLoginUrl
        ///    (typically caused by UrlAuthorizationModule).  This request needs to
        ///    be intercepted to change the 'ReturnUrl' parameter to 'serviceName'
        ///  - Request contains a CAS ticket in the URL.  This request needs to be
        ///    redirected back to itself without the 'ticket' parameter in order to
        ///    avoid potential infinite automatic ticket validation loops for when
        ///    a the ticket in the URL has expired or was revoked and the Renew 
        ///    configuration parameter is set.
        ///  - Request is authenticated, but is not authorized to access the 
        ///    requested resource (by UrlAuthorizationModule).  If the CAS is 
        ///    configured with a NotAuthorizedUrl, the request is redirected to 
        ///    that page.  Otherwise, it is redirected to the CAS login page with
        ///    a forced 'Renew' property (to prevent infinite redirect loops).
        /// </summary>
        /// <param name="sender">The HttpApplication that sent the request</param>
        /// <param name="e">Not used</param>
        private static void OnEndRequest(object sender, EventArgs e)
        {
            HttpContext context = HttpContext.Current;
            HttpRequest request = context.Request;
            HttpResponse response = context.Response;

            int artifactIndex = request.Url.AbsoluteUri.IndexOf(CasAuthentication.TicketValidator.ArtifactParameterName);

            bool responseIsRedirection = (response.StatusCode == 302);

            bool requestHasCasTicket = (request[CasAuthentication.TicketValidator.ArtifactParameterName] != null && !string.IsNullOrEmpty(request[CasAuthentication.TicketValidator.ArtifactParameterName]));
            bool requestIsInboundCasResponse = (requestHasCasTicket && artifactIndex > 0 && (request.Url.AbsoluteUri[artifactIndex - 1] == '?' || request.Url.AbsoluteUri[artifactIndex - 1] == '&'));
            bool userIsAuthenticated = (context.User != null && context.User.Identity != null && context.User.Identity.IsAuthenticated);
            bool responseIsOutboundCasRequest = (!requestHasCasTicket && response.IsRequestBeingRedirected && !string.IsNullOrEmpty(response.RedirectLocation) && response.RedirectLocation.StartsWith(CasAuthentication.FormsLoginUrl));
            bool requestIsUnAuthenticated = (!userIsAuthenticated && responseIsOutboundCasRequest);
            bool requestIsUnAuthorized = (userIsAuthenticated && responseIsRedirection && responseIsOutboundCasRequest);

            if (requestIsInboundCasResponse || requestIsUnAuthenticated || requestIsUnAuthorized)
            {
                string redirectCasUrl = CasAuthentication.ConstructLoginRedirectUrl();
                string redirectCasReturnUrl = RemoveQueryStringVariableFromUrl(request.Url.AbsoluteUri, CasAuthentication.TicketValidator.ArtifactParameterName);
                string redirectNotAuthorizedUrl = CasAuthentication.ResolveUrl(CasAuthentication.NotAuthorizedUrl);

                string redirectCasRenewUrl;

                if (CasAuthentication.Renew)
                {
                    redirectCasRenewUrl = redirectCasUrl;
                }
                else if (CasAuthentication.Gateway)
                {
                    // Gateway+Renew = bad.  We want to
                    // renew. 
                    redirectCasRenewUrl = redirectCasUrl
                        .Replace("?gateway=", "?renew=")
                        .Replace("&gateway=", "&renew=");
                }
                else
                {
                    redirectCasRenewUrl = redirectCasUrl + "&renew=true";
                }


                if (requestIsInboundCasResponse)
                {
                    // Note: redirectAfterValidation is ignored to avoid infinite 
                    //       loops in certain circumstances.
                    // Redirect the request back to itself without the ticket in the URL.
                    response.Redirect(redirectCasReturnUrl, false);
                }
                else if (requestIsUnAuthorized)
                {
                    // User is authenticated but not authorized.  If a notAuthorizedUrl 
                    // is defined, the request will be redirected there now.  If a 
                    // notAuthorizedUrl is not defined, the request will be sent to CAS 
                    // again for alternate credentials.  This forces the Renew parameter 
                    // to prevent an endless loop between this server and the CAS server.
                    response.Redirect(!string.IsNullOrEmpty(redirectNotAuthorizedUrl) ? redirectNotAuthorizedUrl : redirectCasRenewUrl, false);
                }
                else
                {
                    // If we got an HTTP 401 Error (Unauthorized) and don't have a CAS ticket
                    // in the URL, redirect to CAS.
                    response.Redirect(redirectCasUrl, false);
                }
            }
        }

        /// <summary>
        /// Process SingleSignOut requests by removing the ticket from the state store.
        /// </summary>
        /// <returns>
        /// Boolean indicating whether the request was a SingleSignOut request, regardless of
        /// whether or not the request actually required processing (non-existent/already expired).
        /// </returns>
        private static bool ProcessSingleSignOutRequest()
        {
            // TODO: Should we be checking to make sure that this special POST is coming from a trusted source?
            //       It would be tricky to do this by IP address because there might be a white list or something.

            if (!CasAuthentication.SingleSignOut || CasAuthentication.TicketManager == null)
            {
                throw new InvalidOperationException("Single Sign Out request cannot be handled without the SingleSignoutProperty set and a FormsAuthenticationStateManager configured.");
            }

            HttpContext context = HttpContext.Current;
            HttpRequest request = context.Request;

            bool logoutRequestReceived = false;
            if (request.RequestType == "POST")
            {
                string logoutRequest = request.Params["logoutRequest"];
                if (Log.IsDebugEnabled)
                {
                    Log.DebugFormat("{0}:POST logoutRequest={1}", CommonUtils.MethodName, (logoutRequest ?? "null"));
                }
                if (!string.IsNullOrEmpty(logoutRequest))
                {
                    logoutRequestReceived = true;
                    string casTicket = ExtractSingleSignOutTicketFromSamlResponse(logoutRequest);
                    if (Log.IsDebugEnabled)
                    {
                        Log.DebugFormat("{0}:casTicket=[{1}]", CommonUtils.MethodName, casTicket);
                    }
                    if (!string.IsNullOrEmpty(casTicket))
                    {
                        CasAuthentication.TicketManager.RevokeTicket(casTicket);
                        if (Log.IsDebugEnabled)
                        {
                            Log.DebugFormat("{0}:Revoked casTicket [{1}]]", CommonUtils.MethodName, casTicket);
                        }
                    }
                }
            }

            return logoutRequestReceived;
        }

        /// <summary>
        /// Extracts the CAS ticket from the SAML message supplied.
        /// </summary>
        /// <param name="xmlAsString">SAML message from CAS server</param>
        /// <returns>The CAS ticket contained in SAML message</returns>
        private static string ExtractSingleSignOutTicketFromSamlResponse(string xmlAsString)
        {
            // XmlUtils.GetTextForElement wasn't handling namespaces correctly. 
            // Existing SingleSignOut implementation wasn't working correctly.
            XmlParserContext xmlParserContext = new XmlParserContext(null, _xmlNamespaceManager, null, XmlSpace.None);

            string elementText = null;
            if (!string.IsNullOrEmpty(xmlAsString) && !string.IsNullOrEmpty(XML_SESSION_INDEX_ELEMENT_NAME))
            {
                using (TextReader textReader = new StringReader(xmlAsString))
                {
                    XmlReader reader = XmlReader.Create(textReader, _xmlReaderSettings, xmlParserContext);
                    bool foundElement = reader.ReadToFollowing(XML_SESSION_INDEX_ELEMENT_NAME);
                    if (foundElement)
                    {
                        elementText = reader.ReadElementString();
                    }

                    reader.Close();
                }
            }
            return elementText;
        }

        /// <summary>
        /// Removes a QueryString variable from a URL
        /// <remarks>
        /// Borrowed from FormsAuthenticationProvider via Reflector
        /// </remarks>
        /// </summary>
        /// <param name="strUrl">The URL to remove a variable from</param>
        /// <param name="qsVar">The variable to be removed</param>
        /// <returns></returns>
        private static string RemoveQueryStringVariableFromUrl(string strUrl, string qsVar)
        {
            if (qsVar == null)
            {
                throw new ArgumentNullException("qsVar");
            }
            int index = strUrl.IndexOf('?');
            if (index >= 0)
            {
                string sep = "&";
                string str2 = "?";
                string token = sep + qsVar + "=";
                RemoveQsVar(ref strUrl, index, token, sep, sep.Length);
                token = str2 + qsVar + "=";
                RemoveQsVar(ref strUrl, index, token, sep, str2.Length);
                sep = HttpUtility.UrlEncode("&");
                str2 = HttpUtility.UrlEncode("?");
                token = sep + HttpUtility.UrlEncode(qsVar + "=");
                if (sep != null)
                {
                    RemoveQsVar(ref strUrl, index, token, sep, sep.Length);
                }
                token = str2 + HttpUtility.UrlEncode(qsVar + "=");
                if (str2 != null)
                {
                    RemoveQsVar(ref strUrl, index, token, sep, str2.Length);
                }
            }
            return strUrl;
        }

        /// <summary>
        /// Helper function to facilitate removing a variable from a URL
        /// <remarks>
        /// Borrowed from FormsAuthenticationProvider via Reflector
        /// </remarks>
        /// </summary>
        /// <param name="strUrl">Url to process</param>
        /// <param name="posQ">Offset of beginning of querystring</param>
        /// <param name="token">?qsVar= or &qsVar=</param>
        /// <param name="sep">HTML encoded ampersand</param>
        /// <param name="lenAtStartToLeave">Substring offset</param>
        private static void RemoveQsVar(ref string strUrl, int posQ, string token, string sep, int lenAtStartToLeave)
        {
            for (int i = strUrl.LastIndexOf(token, StringComparison.Ordinal); i >= posQ; i = strUrl.LastIndexOf(token, StringComparison.Ordinal))
            {
                int startIndex = strUrl.IndexOf(sep, i + token.Length, StringComparison.Ordinal) + sep.Length;
                if ((startIndex < sep.Length) || (startIndex >= strUrl.Length))
                {
                    strUrl = strUrl.Substring(0, i);
                }
                else
                {
                    strUrl = strUrl.Substring(0, i + lenAtStartToLeave) + strUrl.Substring(startIndex);
                }
            }
        }
    }
}