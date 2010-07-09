/*
 * Copyright 2007-2010 The JA-SIG Collaborative. All rights reserved. See license
 * distributed with this file and available online at
 * http://www.ja-sig.org/products/cas/overview/license/index.html
 */

using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.Configuration;
using System.Web.Security;
using System.Xml;
using DotNetCasClient.Configuration;
using DotNetCasClient.Security;
using DotNetCasClient.State;
using DotNetCasClient.Utils;
using DotNetCasClient.Validation;
using DotNetCasClient.Validation.Schema.Cas20;
using DotNetCasClient.Validation.TicketValidator;
using log4net;

namespace DotNetCasClient
{
    /// <summary>
    /// CasAuthentication exposes a public API for use in working with CAS Authentication
    /// in the .NET framework.  It also exposes all configured CAS client configuration 
    /// parameters as public static properties.
    /// </summary>
    public sealed class CasAuthentication
    {
        #region Constants
        private const string XML_SESSION_INDEX_ELEMENT_NAME = "samlp:SessionIndex";
        private const string PARAM_PROXY_GRANTING_TICKET_IOU = "pgtIou";        
        private const string PARAM_PROXY_GRANTING_TICKET = "pgtId";
        #endregion

        #region Fields
        /// <summary>
        /// Access to the log file
        /// </summary>
        private static readonly ILog Log = LogManager.GetLogger("CasAuthentication");

        private static readonly object LockObject;
        private static bool _initialized;

        // system.web/authentication and system.web/authentication/forms static classes
        internal static AuthenticationSection AuthenticationConfig;
        internal static CasClientConfiguration CasClientConfig;

        // Ticket validator support
        private static string _ticketValidatorName;
        private static AbstractUrlTicketValidator _ticketValidator;

        // Ticket manager support
        private static string _serviceTicketManagerProvider;
        private static IServiceTicketManager _serviceTicketManager;

        // Proxy ticket support
        private static string _proxyTicketManagerProvider;
        private static IProxyTicketManager _proxyTicketManager;

        // Gateway support
        private static bool _gateway;
        private static string _gatewayStatusCookieName;

        private static string _formsLoginUrl;
        private static TimeSpan _formsTimeout;
        private static string _casServerLoginUrl;
        private static string _casServerUrlPrefix;
        private static long _ticketTimeTolerance;
        private static string _serverName;
        private static bool _renew;
        private static string _artifactParameterName;
        private static string _serviceParameterName;
        private static bool _redirectAfterValidation;
        private static bool _singleSignOut;
        private static string _notAuthorizedUrl;
        private static string _cookiesRequiredUrl;
        private static string _gatewayParameterName;
        private static string _proxyCallbackParameterName;

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
        #endregion

        /*
        public string GetProxyTicketFor(Uri service)
        {
            if (this.proxyGrantingTicket != null)
            {
                return this.proxyRetriever.GetProxyTicketIdFor(
                  this.proxyGrantingTicket, service);
            }
            log.Debug(string.Format("{0}:" +
              "No ProxyGrantingTicket was supplied --> returning null",
              CommonUtils.MethodName));
            return null;
        }
        */

        #region Methods
        /// <summary>
        /// Static constructor
        /// </summary>
        static CasAuthentication()
        {
            LockObject = new object();
        }

        /// <summary>
        /// Initializes configuration-related properties and validates configuration.
        /// </summary>
        public static void Initialize()
        {
            if (!_initialized)
            {
                lock (LockObject)
                {
                    if (!_initialized)
                    {
                        FormsAuthentication.Initialize();
                        AuthenticationConfig = (AuthenticationSection)WebConfigurationManager.GetSection("system.web/authentication");
                        CasClientConfig = CasClientConfiguration.Config;

                        if (AuthenticationConfig == null)
                        {
                            if (Log.IsDebugEnabled)
                            {
                                Log.Debug("Application is not configured for Authentication");
                            }
                            throw new CasConfigurationException("The CAS authentication provider requires Forms authentication to be enabled in web.config.");
                        }

                        if (AuthenticationConfig.Mode != AuthenticationMode.Forms)
                        {
                            if (Log.IsDebugEnabled)
                            {
                                Log.Debug("Application is not configured for Forms Authentication");
                            }
                            throw new CasConfigurationException("The CAS authentication provider requires Forms authentication to be enabled in web.config.");
                        }

                        if (FormsAuthentication.CookieMode != HttpCookieMode.UseCookies)
                        {
                            throw new CasConfigurationException("CAS requires Forms Authentication to use cookies (cookieless='UseCookies').");
                        }

                        _xmlReaderSettings = new XmlReaderSettings();
                        _xmlReaderSettings.ConformanceLevel = ConformanceLevel.Auto;
                        _xmlReaderSettings.IgnoreWhitespace = true;

                        _xmlNameTable = new NameTable();

                        _xmlNamespaceManager = new XmlNamespaceManager(_xmlNameTable);
                        _xmlNamespaceManager.AddNamespace("cas", "http://www.yale.edu/tp/cas");
                        _xmlNamespaceManager.AddNamespace("saml", "urn: oasis:names:tc:SAML:1.0:assertion");
                        _xmlNamespaceManager.AddNamespace("saml2", "urn: oasis:names:tc:SAML:1.0:assertion");
                        _xmlNamespaceManager.AddNamespace("samlp", "urn: oasis:names:tc:SAML:1.0:protocol");

                        _formsLoginUrl = AuthenticationConfig.Forms.LoginUrl;
                        _formsTimeout = AuthenticationConfig.Forms.Timeout;

                        if (string.IsNullOrEmpty(CasClientConfig.CasServerUrlPrefix))
                        {
                            throw new CasConfigurationException("The CasServerUrlPrefix is required");
                        }

                        _casServerUrlPrefix = CasClientConfig.CasServerUrlPrefix;
                        _casServerLoginUrl = CasClientConfig.CasServerLoginUrl;
                        _ticketValidatorName = CasClientConfig.TicketValidatorName;
                        _ticketTimeTolerance = CasClientConfig.TicketTimeTolerance;
                        _serverName = CasClientConfig.ServerName;
                        _renew = CasClientConfig.Renew;
                        _gateway = CasClientConfig.Gateway;
                        _gatewayStatusCookieName = CasClientConfig.GatewayStatusCookieName;
                        _artifactParameterName = CasClientConfig.ArtifactParameterName;
                        _serviceParameterName = CasClientConfig.ServiceParameterName;
                        _redirectAfterValidation = CasClientConfig.RedirectAfterValidation;
                        _singleSignOut = CasClientConfig.SingleSignOut;
                        _serviceTicketManagerProvider = CasClientConfig.ServiceTicketManager;
                        _proxyTicketManagerProvider = CasClientConfig.ProxyTicketManager;
                        _notAuthorizedUrl = CasClientConfig.NotAuthorizedUrl;
                        _cookiesRequiredUrl = CasClientConfig.CookiesRequiredUrl;
                        _gatewayParameterName = CasClientConfig.GatewayParameterName;
                        _proxyCallbackParameterName = CasClientConfig.ProxyCallbackParameterName;

                        if (_gateway)
                        {
                            // throw new NotImplementedException("Gateway has not been implemented yet.");
                            // _gatewayResolver = new SessionAttrGatewayResolver();
                        }

                        // Parse "enumerated" values 
                        if (String.Compare(_ticketValidatorName, CasClientConfiguration.CAS10_TICKET_VALIDATOR_NAME, true) == 0)
                        {
                            _ticketValidator = new Cas10TicketValidator();
                        }
                        else if (String.Compare(_ticketValidatorName, CasClientConfiguration.CAS20_TICKET_VALIDATOR_NAME, true) == 0)
                        {
                            _ticketValidator = new Cas20ServiceTicketValidator();
                        }
                        else if (String.Compare(_ticketValidatorName, CasClientConfiguration.SAML11_TICKET_VALIDATOR_NAME, true) == 0)
                        {
                            _ticketValidator = new Saml11TicketValidator();
                        }
                        else
                        {
                            throw new CasConfigurationException("Unknown ticket validator " + _ticketValidatorName);
                        }

                        if (String.IsNullOrEmpty(_serviceTicketManagerProvider))
                        {
                            // Web server cannot maintain ticket state, verify tickets, perform SSO, etc.
                        }
                        else if (String.Compare(_serviceTicketManagerProvider, CasClientConfiguration.CACHE_SERVICE_TICKET_MANAGER) == 0)
                        {
                            _serviceTicketManager = new CacheServiceTicketManager();
                        }
                        else
                        {
                            throw new CasConfigurationException("Unknown service ticket manager provider: " + _serviceTicketManagerProvider);
                        }

                        if (String.IsNullOrEmpty(_proxyTicketManagerProvider))
                        {
                            // Web server cannot generate proxy tickets
                        }
                        else if (String.Compare(_proxyTicketManagerProvider, CasClientConfiguration.CACHE_PROXY_TICKET_MANAGER) == 0)
                        {
                            _proxyTicketManager = new CacheProxyTicketManager();
                        }
                        else
                        {
                            throw new CasConfigurationException("Unknown proxy ticket manager provider: " + _proxyTicketManagerProvider);
                        }

                        // Validate configuration
                        bool haveServerName = !String.IsNullOrEmpty(_serverName);
                        if (!haveServerName)
                        {
                            throw new CasConfigurationException(CasClientConfiguration.SERVER_NAME + " cannot be null or empty.");
                        }

                        if (String.IsNullOrEmpty(_casServerLoginUrl))
                        {
                            throw new CasConfigurationException(CasClientConfiguration.CAS_SERVER_LOGIN_URL + " cannot be null or empty.");
                        }

                        if (_serviceTicketManager == null && _singleSignOut)
                        {
                            throw new CasConfigurationException("Single Sign Out requires a FormsAuthenticationStateProvider.");
                        }

                        if (_gateway && _renew)
                        {
                            throw new CasConfigurationException("Gateway and Renew functionalities are mutually exclusive");
                        }

                        if (!_redirectAfterValidation)
                        {
                            throw new CasConfigurationException("Forms Authentication based modules require RedirectAfterValidation to be set to true.");
                        }

                        _initialized = true;
                    }
                }

                if (ServiceTicketManager != null) ServiceTicketManager.Initialize();
                if (ProxyTicketManager != null) ProxyTicketManager.Initialize();
                if (TicketValidator != null) TicketValidator.Initialize();
            }
        }

        /*
        public static void Authenticate(string username, string password)
        {
            // TODO: Is this too evil for inclusion?

            string postData = ConstructDirectLoginPostData(username, password, false);
            HttpWebRequest request = (HttpWebRequest) WebRequest.Create(CasServerLoginUrl);
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = postData.Length;
            
            using (StreamWriter requestWriter = new StreamWriter(request.GetRequestStream()))
            {
                requestWriter.Write(postData);
            }

            using (HttpWebResponse response = (HttpWebResponse) request.GetResponse())
            {
                using (StreamReader responseReader = new StreamReader(response.GetResponseStream()))
                {

                }
            }
        }

        private static string ConstructDirectLoginPostData(string username, string password, bool warn)
        {
            Guid cGuid = new Guid();
            Guid kGuid = new Guid();
            string lt = "_c" + cGuid + "_k" + kGuid;

            string postData = string.Format("username={0}&password={1}&lt={2}{3}",
                username,
                password,
                lt,
                (warn ? "warn=true" : string.Empty)
            );

            return postData;
        }
        */

        public static void ProxyRedirect(string url)
        {
            ProxyRedirect(url, "ticket", false);
        }

        public static void ProxyRedirect(string url, bool endResponse)
        {
            ProxyRedirect(url, "ticket", endResponse);   
        }

        public static void ProxyRedirect(string url, string proxyTicketUrlParameter)
        {
            ProxyRedirect(url, proxyTicketUrlParameter, false);
        }

        public static void ProxyRedirect(string url, string proxyTicketUrlParameter, bool endResponse)
        {
            if (url == null)
            {
                throw new ArgumentNullException("url");
            }
            else if (url.Length == 0)
            {
                throw new ArgumentException("url parameter cannot be null or empty.", "url");
            }

            if (proxyTicketUrlParameter == null)
            {
                throw new ArgumentNullException("proxyTicketUrlParameter");
            }
            else if (proxyTicketUrlParameter.Length == 0)
            {
                throw new ArgumentException("proxyTicketUrlParameter parameter cannot be null or empty.", "proxyTicketUrlParameter");
            }

            HttpContext context = HttpContext.Current;
            HttpResponse response = context.Response;

            response.Redirect(GetProxyRedirectUrl(url, proxyTicketUrlParameter), endResponse);
        }

        public static string GetProxyRedirectUrl(string targetServiceUrl)
        {
            return GetProxyRedirectUrl(targetServiceUrl, "ticket");
        }

        public static string GetProxyRedirectUrl(string targetServiceUrl, string proxyTicketUrlParameter)
        {
            string resolvedUrl = UrlUtil.ResolveUrl(targetServiceUrl);
            string proxyTicket = GetProxyTicketIdFor(resolvedUrl);
            
            EnhancedUriBuilder ub = new EnhancedUriBuilder(resolvedUrl);
            ub.QueryItems[proxyTicketUrlParameter] = proxyTicket;

            return ub.Uri.AbsoluteUri;
        }

        public static string GetProxyTicketIdFor(string targetServiceUrl)
        {
            if (targetServiceUrl == null)
            {
                throw new ArgumentNullException("targetServiceUrl");
            }

            if (targetServiceUrl.Length == 0)
            {
                throw new ArgumentException("targetServiceUrl is empty.");
            }

            if (ServiceTicketManager == null)
            {
                throw new InvalidOperationException("Proxy authentication requires a ServiceTicketManager");
            }

            FormsAuthenticationTicket formsAuthTicket = GetFormsAuthenticationTicket();
            if (formsAuthTicket == null)
            {
                throw new InvalidOperationException("The request is not authenticated (does not have a CAS Service or Proxy ticket).");
            }

            if (string.IsNullOrEmpty(formsAuthTicket.UserData))
            {
                throw new InvalidOperationException("The request does not have a CAS Service Ticket.");
            }

            CasAuthenticationTicket casTicket = ServiceTicketManager.GetTicket(formsAuthTicket.UserData);

            if (casTicket == null)
            {
                throw new InvalidOperationException("The request does not have a valid CAS Service Ticket.");
            }
            
            string proxyTicketResponse;
            try
            {
                string proxyUrl = UrlUtil.ConstructProxyTicketRequestUrl(casTicket.ProxyGrantingTicket, targetServiceUrl);
                proxyTicketResponse = PerformHttpGet(proxyUrl, true);
            }
            catch
            {
                throw new TicketValidationException("Unable to obtain CAS Proxy Ticket.");
            }

            if (String.IsNullOrEmpty(proxyTicketResponse))
            {
                throw new TicketValidationException("Unable to obtain CAS Proxy Ticket (response was empty)");
            }

            ServiceResponse serviceResponse;
            try
            {
                serviceResponse = ServiceResponse.ParseResponse(proxyTicketResponse);
            }
            catch (InvalidOperationException)
            {
                throw new TicketValidationException("CAS Server response does not conform to CAS 2.0 schema");
            }

            if (serviceResponse.IsProxyFailure)
            {
                ProxyFailure failure = (ProxyFailure)serviceResponse.Item;
                if (!String.IsNullOrEmpty(failure.Message) && !String.IsNullOrEmpty(failure.Code))
                {
                    Log.DebugFormat("Proxy failure: {0} ({1})", failure.Message, failure.Code);
                }
                else if (!String.IsNullOrEmpty(failure.Message))
                {
                    Log.DebugFormat("Proxy failure: {0}", failure.Message);
                }
                else if (!String.IsNullOrEmpty(failure.Code))
                {
                    Log.DebugFormat("Proxy failure: Code {0}", failure.Code);
                }
                return null;
            }

            if (serviceResponse.IsProxySuccess)
            {
                ProxySuccess success = (ProxySuccess) serviceResponse.Item;
                if (!String.IsNullOrEmpty(success.ProxyTicket))
                {
                    Log.DebugFormat("Proxy success: {0}", success.ProxyTicket);
                }

                return success.ProxyTicket;
            }
            
            return null;
        }

        public static void GatewayAuthenticate(bool ignoreGatewayStatusCookie)
        {
            Initialize();

            HttpContext context = HttpContext.Current;
            HttpResponse response = context.Response;
            HttpApplication application = context.ApplicationInstance;
            
            if (!ignoreGatewayStatusCookie)
            {
                if (GetGatewayStatus() != GatewayStatus.NotAttempted)
                {
                    return;
                }
            }

            SetGatewayStatusCookie(GatewayStatus.Attempting);

            string redirectUrl = UrlUtil.ConstructLoginRedirectUrl(true, false);

            response.Redirect(redirectUrl, false);
            application.CompleteRequest();
        }

        public static void InitiateSingleSignout()
        {
            Initialize();

            HttpContext context = HttpContext.Current;
            HttpResponse response = context.Response;
            HttpApplication application = context.ApplicationInstance;

            ClearAuthCookie();
            response.Redirect(UrlUtil.ConstructSingleSignOutRedirectUrl(), false);
            application.CompleteRequest();
        }

        /// <summary>
        /// Process SingleSignOut requests by removing the ticket from the state store.
        /// </summary>
        /// <returns>
        /// Boolean indicating whether the request was a SingleSignOut request, regardless of
        /// whether or not the request actually required processing (non-existent/already expired).
        /// </returns>
        internal static void ProcessSingleSignOutRequest()
        {
            HttpContext context = HttpContext.Current;
            HttpRequest request = context.Request;
            HttpResponse response = context.Response;

            if (Log.IsDebugEnabled)
            {
                Log.DebugFormat("{0}:Processing SingleSignOut request", CommonUtils.MethodName);
            }

            if (request.HttpMethod == "POST" && request.Form["logoutRequest"] != null)
            {
                // TODO: Should we be checking to make sure that this special POST is coming from a trusted source?
                //       It would be tricky to do this by IP address because there might be a white list or something.
                
                string casTicket = ExtractSingleSignOutTicketFromSamlResponse(request.Params["logoutRequest"]);
                if (Log.IsDebugEnabled)
                {
                    Log.DebugFormat("{0}:casTicket=[{1}]", CommonUtils.MethodName, casTicket);
                }
                if (!String.IsNullOrEmpty(casTicket))
                {
                    ServiceTicketManager.RevokeTicket(casTicket);

                    if (Log.IsDebugEnabled)
                    {
                        Log.DebugFormat("{0}:SingleSignOut returned true --> processed CAS logoutRequest", CommonUtils.MethodName);
                    }

                    response.StatusCode = 200;
                    response.ContentType = "text/plain";
                    response.Clear();
                    response.Write("OK");

                    context.ApplicationInstance.CompleteRequest();

                    if (Log.IsDebugEnabled)
                    {
                        Log.DebugFormat("{0}:Revoked casTicket [{1}]]", CommonUtils.MethodName, casTicket);
                    }
                }
            }
        }

        internal static bool ProcessProxyCallbackRequest()
        {
            HttpContext context = HttpContext.Current;
            HttpApplication application = context.ApplicationInstance;
            HttpRequest request = context.Request;
            HttpResponse response = context.Response;

            if (string.IsNullOrEmpty(request.QueryString[ProxyCallbackParameterName]) || request.QueryString[ProxyCallbackParameterName] != "true")
            {
                return false;
            }

            string proxyGrantingTicketIou = request.Params[PARAM_PROXY_GRANTING_TICKET_IOU];
            string proxyGrantingTicket = request.Params[PARAM_PROXY_GRANTING_TICKET];

            if (String.IsNullOrEmpty(proxyGrantingTicket) || String.IsNullOrEmpty(proxyGrantingTicketIou))
            {
                // todo log.info that we handled the callback but didn't get the pgt
                application.CompleteRequest();
                return true;
            }

            if (Log.IsDebugEnabled)
            {
                Log.Debug(string.Format("Recieved proxyGrantingTicketId [{0}] for proxyGrantingTicketIou [{1}]", proxyGrantingTicket, proxyGrantingTicketIou));
            }

            ProxyTicketManager.InsertProxyGrantingTicketMapping(proxyGrantingTicketIou, proxyGrantingTicket);

            

            response.Write("<?xml version=\"1.0\"?>");
            response.Write("<casClient:proxySuccess xmlns:casClient=\"http://www.yale.edu/tp/casClient\" />");
            application.CompleteRequest();
            return true;
        }

        internal static void ProcessTicketValidation()
        {
            HttpContext context = HttpContext.Current;
            HttpApplication app = context.ApplicationInstance;
            HttpRequest request = context.Request;

            CasAuthenticationTicket casTicket;
            ICasPrincipal principal;

            string ticket = request[TicketValidator.ArtifactParameterName];

            try
            {
                // Attempt to authenticate the ticket and resolve to an ICasPrincipal
                bool gateway = RequestEvaluator.GetRequestHasGatewayParameter();
                string serviceUrl = UrlUtil.ConstructServiceUri(gateway);
                principal = TicketValidator.Validate(ticket, serviceUrl);

                // Save the ticket in the FormsAuthTicket.  Encrypt the ticket and send it as a cookie. 
                casTicket = new CasAuthenticationTicket(
                    ticket,
                    UrlUtil.RemoveCasArtifactsFromUrl(request.Url.AbsoluteUri),
                    request.UserHostAddress,
                    principal.Assertion
                );

                if (ProxyTicketManager != null)
                {
                    casTicket.ProxyGrantingTicketIou = principal.ProxyGrantingTicket;
                    casTicket.Proxies.AddRange(principal.Proxies);
                    string proxyGrantingTicket = ProxyTicketManager.GetProxyGrantingTicket(casTicket.ProxyGrantingTicketIou);
                    if (!string.IsNullOrEmpty(proxyGrantingTicket))
                    {
                        casTicket.ProxyGrantingTicket = proxyGrantingTicket;
                    }
                }

                // TODO: Check the last 2 parameters.  We want to take the from/to dates from the FormsAuthenticationTicket.  However, we may need to do some clock drift correction.
                FormsAuthenticationTicket formsAuthTicket = CreateFormsAuthenticationTicket(principal.Identity.Name, FormsAuthentication.FormsCookiePath, ticket, null, null);
                SetAuthCookie(formsAuthTicket);

                // Also save the ticket in the server store (if configured)
                if (ServiceTicketManager != null)
                {
                    ServiceTicketManager.UpdateTicketExpiration(casTicket, formsAuthTicket.Expiration);
                }

                // Jump directly to EndRequest.  Don't allow the Page and/or Handler to execute.
                // EndRequest will redirect back without the ticket in the URL
                app.CompleteRequest();
                return;
            }
            catch (TicketValidationException)
            {
                // Leave principal null.  This might not have been a CAS service ticket.
            }
        }

        internal static void ProcessRequestAuthentication()
        {
            HttpContext context = HttpContext.Current;

            // Look for a valid FormsAuthenticationTicket encrypted in a cookie.
            CasAuthenticationTicket casTicket = null;
            FormsAuthenticationTicket formsAuthenticationTicket = GetFormsAuthenticationTicket();
            if (formsAuthenticationTicket != null)
            {
                ICasPrincipal principal;
                if (ServiceTicketManager != null)
                {
                    string serviceTicket = formsAuthenticationTicket.UserData;
                    casTicket = ServiceTicketManager.GetTicket(serviceTicket);
                    if (casTicket != null)
                    {
                        IAssertion assertion = casTicket.Assertion;

                        if (!ServiceTicketManager.VerifyClientTicket(casTicket))
                        {
                            if (Log.IsDebugEnabled)
                            {
                                Log.DebugFormat("{0}:Ticket failed verification." + Environment.NewLine, CommonUtils.MethodName);
                            }

                            // Deletes the invalid FormsAuthentication cookie from the client.
                            ClearAuthCookie();
                            ServiceTicketManager.RevokeTicket(serviceTicket);

                            // Don't give this request a User/Principal.  Remove it if it was created
                            // by the underlying FormsAuthenticationModule or another module.
                            principal = null;
                        }
                        else
                        {
                            if (ProxyTicketManager != null && !string.IsNullOrEmpty(casTicket.ProxyGrantingTicketIou) && string.IsNullOrEmpty(casTicket.ProxyGrantingTicket))
                            {
                                string proxyGrantingTicket = ProxyTicketManager.GetProxyGrantingTicket(casTicket.ProxyGrantingTicketIou);
                                if (!string.IsNullOrEmpty(proxyGrantingTicket))
                                {
                                    casTicket.ProxyGrantingTicket = proxyGrantingTicket;
                                }
                            }

                            principal = new CasPrincipal(assertion);
                        }
                    }
                    else
                    {
                        // This didn't resolve to a ticket in the TicketStore.  Revoke it.
                        ClearAuthCookie();
                        ServiceTicketManager.RevokeTicket(serviceTicket);

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
                    ClearAuthCookie();
                }
                else
                {
                    // Extend the expiration of the cookie if FormsAuthentication is configured to do so.
                    if (FormsAuthentication.SlidingExpiration)
                    {
                        FormsAuthenticationTicket newTicket = FormsAuthentication.RenewTicketIfOld(formsAuthenticationTicket);
                        if (newTicket != null && newTicket != formsAuthenticationTicket)
                        {
                            SetAuthCookie(newTicket);
                            if (ServiceTicketManager != null)
                            {
                                ServiceTicketManager.UpdateTicketExpiration(casTicket, newTicket.Expiration);
                            }
                        }
                    }
                }
            }
        }


        /// <summary>
        /// Attempts to set the GatewayStatus client cookie.  If the cookie is not
        /// present and equal to GatewayStatus.Attempting when a CAS Gateway request
        /// comes in (indicated by the presence of the 'gatewayParameterName' 
        /// defined in web.config appearing in the URL), the server knows that the 
        /// client is not accepting session cookies and will optionally redirect 
        /// the user to the 'cookiesRequiredUrl' (also defined in web.config).  If
        /// 'cookiesRequiredUrl' is not defined but 'gateway' is, every page request
        /// will result in a round-trip to the CAS server.
        /// </summary>
        /// <param name="gatewayStatus">The GatewayStatus to attempt to store</param>
        internal static void SetGatewayStatusCookie(GatewayStatus gatewayStatus)
        {
            Initialize();
            HttpContext current = HttpContext.Current;
            HttpCookie cookie = new HttpCookie(GatewayStatusCookieName, gatewayStatus.ToString());
            
            cookie.HttpOnly = false;
            cookie.Path = FormsAuthentication.FormsCookiePath;
            cookie.Secure = false;

            if (FormsAuthentication.CookieDomain != null)
            {
                cookie.Domain = FormsAuthentication.CookieDomain;
            }

            // Add it to the request collection for later processing during this request
            current.Request.Cookies.Remove(GatewayStatusCookieName);
            current.Request.Cookies.Add(cookie);

            // Add it to the response collection for delivery to client
            current.Response.Cookies.Add(cookie);
        }
        
        /// <summary>
        /// Retrieves the GatewayStatus from the client cookie.
        /// </summary>
        /// <returns>
        /// The GatewayStatus stored in the cookie if present, otherwise 
        /// GatewayStatus.NotAttempted.
        /// </returns>
        public static GatewayStatus GetGatewayStatus()
        {
            Initialize();
            HttpContext context = HttpContext.Current;
            HttpCookie cookie = context.Request.Cookies[GatewayStatusCookieName];

            GatewayStatus status;

            if (cookie != null && !string.IsNullOrEmpty(cookie.Value))
            {
                try
                {
                    // Parse the value out of the cookie
                    status = (GatewayStatus) Enum.Parse(typeof (GatewayStatus), cookie.Value);
                }
                catch (ArgumentException)
                {
                    // If the cookie contains an invalid value, clear the cookie 
                    // and return GatewayStatus.NotAttempted
                    SetGatewayStatusCookie(GatewayStatus.NotAttempted);
                    status = GatewayStatus.NotAttempted;
                }
            } 
            else
            {
                // Use the default value GatewayStatus.NotAttempted
                status = GatewayStatus.NotAttempted;
            }

            return status;
        }

        /// <summary>
        /// Sends a blank and expired FormsAuthentication cookie to the 
        /// client response.  This effectively removes the FormsAuthentication
        /// cookie and revokes the FormsAuthenticationTicket.  It also removes
        /// the cookie from the current Request object, preventing subsequent 
        /// code from being able to access it during the execution of the 
        /// current request.
        /// </summary>
        public static void ClearAuthCookie()
        {
            Initialize();
            HttpContext current = HttpContext.Current;

            // Don't let anything see the incoming cookie 
            current.Request.Cookies.Remove(FormsAuthentication.FormsCookieName);

            // Remove the cookie from the response collection (by adding an expired/empty version).
            HttpCookie cookie = new HttpCookie(FormsAuthentication.FormsCookieName);
            cookie.Expires = DateTime.Now.AddMonths(-1);
            cookie.Domain = FormsAuthentication.CookieDomain;
            cookie.Path = FormsAuthentication.FormsCookiePath;
            current.Response.Cookies.Add(cookie);
        }

        /// <summary>
        /// Encrypts a FormsAuthenticationTicket in an HttpCookie (using 
        /// GetAuthCookie) and includes it in the response.
        /// </summary>
        /// <param name="clientTicket">The FormsAuthenticationTicket to encode</param>
        public static void SetAuthCookie(FormsAuthenticationTicket clientTicket)
        {
            Initialize();
            HttpContext current = HttpContext.Current;

            if (!current.Request.IsSecureConnection && FormsAuthentication.RequireSSL)
            {
                throw new HttpException("Connection not secure while creating secure cookie");
            }

            current.Response.Cookies.Add(GetAuthCookie(clientTicket));
        }

        /// <summary>
        /// Creates an HttpCookie containing an encrypted FormsAuthenticationTicket,
        /// which in turn contains a CAS service ticket.
        /// </summary>
        /// <param name="ticket">The FormsAuthenticationTicket to encode</param>
        /// <returns>An HttpCookie containing the encrypted FormsAuthenticationTicket</returns>
        public static HttpCookie GetAuthCookie(FormsAuthenticationTicket ticket)
        {
            Initialize();

            string str = FormsAuthentication.Encrypt(ticket);

            if (String.IsNullOrEmpty(str))
            {
                throw new HttpException("Unable to encrypt cookie ticket");
            }

            HttpCookie cookie = new HttpCookie(FormsAuthentication.FormsCookieName, str);

            // Per http://support.microsoft.com/kb/900111 :
            // In ASP.NET 2.0, forms authentication cookies are HttpOnly cookies. 
            // HttpOnly cookies cannot be accessed through client script. This 
            // functionality helps reduce the chances of replay attacks.
            cookie.HttpOnly = true;

            cookie.Path = FormsAuthentication.FormsCookiePath;
            cookie.Secure = FormsAuthentication.RequireSSL;

            if (FormsAuthentication.CookieDomain != null)
            {
                cookie.Domain = FormsAuthentication.CookieDomain;
            }

            if (ticket.IsPersistent)
            {
                cookie.Expires = ticket.Expiration;
            }

            return cookie;
        }

        /// <summary>
        /// Creates a FormsAuthenticationTicket for storage on the client.
        /// The UserData field contains the CAS Service Ticket which can be 
        /// used by the server-side ServiceTicketManager to retrieve additional 
        /// details about the ticket (e.g. assertions)
        /// </summary>
        /// <param name="netId">User associated with the ticket</param>
        /// <param name="cookiePath">Relative path on server in which cookie is valid</param>
        /// <param name="serviceTicket">CAS service ticket</param>
        /// <param name="validFromDate">Ticket valid from date</param>
        /// <param name="validUntilDate">Ticket valid too date</param>
        /// <returns>Instance of a FormsAuthenticationTicket</returns>
        public static FormsAuthenticationTicket CreateFormsAuthenticationTicket(string netId, string cookiePath, string serviceTicket, DateTime? validFromDate, DateTime? validUntilDate)
        {
            if (String.IsNullOrEmpty(netId))
            {
                throw new ArgumentNullException("netId");
            }

            if (String.IsNullOrEmpty(serviceTicket))
            {
                throw new ArgumentNullException("serviceTicket");
            }

            Initialize();

            if (Log.IsDebugEnabled)
            {
                Log.DebugFormat("{0}:Incoming CAS Assertion: {1}", CommonUtils.MethodName, serviceTicket);
            }

            DateTime fromDate = validFromDate.HasValue ? validFromDate.Value : DateTime.Now;
            DateTime toDate = validUntilDate.HasValue ? validUntilDate.Value : fromDate.Add(FormsTimeout);

            FormsAuthenticationTicket ticket = new FormsAuthenticationTicket(
                2,
                netId,
                fromDate,
                toDate,
                false,
                serviceTicket,
                cookiePath ?? FormsAuthentication.FormsCookiePath
            );

            return ticket;
        }

        /// <summary>
        /// Looks for a FormsAuthentication cookie and attempts to
        /// parse a valid, non-expired FormsAuthenticationTicket.
        /// It ensures that the UserData field has a value (presumed
        /// to be a CAS Service Ticket).
        /// </summary>
        /// <returns>
        /// Returns the FormsAuthenticationTicket contained in the 
        /// cookie or null if any issues are encountered.
        /// </returns>
        public static FormsAuthenticationTicket GetFormsAuthenticationTicket()
        {
            Initialize();
            HttpContext context = HttpContext.Current;
            HttpCookie cookie = context.Request.Cookies[FormsAuthentication.FormsCookieName];

            if (cookie == null)
            {
                return null;
            }

            if (cookie.Expires != DateTime.MinValue && cookie.Expires < DateTime.Now)
            {
                ClearAuthCookie();
                return null;
            }

            if (String.IsNullOrEmpty(cookie.Value))
            {
                ClearAuthCookie();
                return null;
            }

            FormsAuthenticationTicket formsAuthTicket;
            try
            {
                formsAuthTicket = FormsAuthentication.Decrypt(cookie.Value);
            }
            catch
            {
                ClearAuthCookie();
                return null;
            }

            if (formsAuthTicket == null)
            {
                ClearAuthCookie();
                return null;
            }

            if (formsAuthTicket.Expired)
            {
                ClearAuthCookie();
                return null;
            }

            if (String.IsNullOrEmpty(formsAuthTicket.UserData))
            {
                ClearAuthCookie();
                return null;
            }

            return formsAuthTicket;
        }

        internal static string PerformHttpGet(string url, bool requireHttp200)
        {
            string responseBody = null;

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            {
                if (!requireHttp200 || response.StatusCode == HttpStatusCode.OK)
                {
                    using (StreamReader responseReader = new StreamReader(response.GetResponseStream()))
                    {
                        responseBody = responseReader.ReadToEnd();
                    }
                }
            }
            
            return responseBody;
        }

        internal static string PerformHttpPost(string url, string postData, bool requireHttp200)
        {
            string responseBody;

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = Encoding.UTF8.GetByteCount(postData);

            using (StreamWriter requestWriter = new StreamWriter(request.GetRequestStream()))
            {
                requestWriter.Write(postData);
            }

            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            {
                using (StreamReader responseReader = new StreamReader(response.GetResponseStream()))
                {
                    responseBody = responseReader.ReadToEnd();
                }
            }

            return responseBody;
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
            if (!String.IsNullOrEmpty(xmlAsString) && !String.IsNullOrEmpty(XML_SESSION_INDEX_ELEMENT_NAME))
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
        #endregion

        #region Properties
        /// <summary>
        /// Name of ticket validator that validates CAS tickets using a 
        /// particular protocol.  Valid values are Cas10, Cas20, and Saml11.
        /// </summary>
        public static string TicketValidatorName
        {
            get
            {
                Initialize();
                return _ticketValidatorName;
            }
        }

        /// <summary>
        /// An instance of the TicketValidator specified in the 
        /// TicketValidatorName property.  This will either be an instance of 
        /// a Cas10TicketValidator, Cas20TicketValidator, or 
        /// Saml11TicketValidator.
        /// </summary>
        internal static AbstractUrlTicketValidator TicketValidator
        {
            get
            {
                Initialize();
                return _ticketValidator;
            }
        }

        /// <summary>
        /// The ticket manager to use to store tickets returned by the CAS server
        /// for validation, revocation, and single sign out support.
        /// <remarks>
        /// Currently supported values: CacheServiceTicketManager
        /// </remarks>
        /// </summary>
        public static string ServiceTicketManagerProvider
        {
            get
            {
                Initialize();
                return _serviceTicketManagerProvider;
            }
        }

        /// <summary>
        /// An instance of the provider specified in the ServiceTicketManagerProvider property.
        /// ServiceTicketManager will be null if no serviceTicketManager is 
        /// defined in web.config.  If a ServiceTicketManager is defined, this will allow 
        /// access to and revocation of outstanding CAS service tickets along with 
        /// additional information about the service tickets (i.e., IP address, 
        /// assertions, etc.).
        /// </summary>
        public static IServiceTicketManager ServiceTicketManager
        {
            get
            {
                Initialize();
                return _serviceTicketManager;
            }
        }

        /// <summary>
        /// The ticket manager to use to store and resolve ProxyGrantingTicket IOUs to 
        /// ProxyGrantingTickets
        /// <remarks>
        /// Currently supported values: CacheProxyTicketManager
        /// </remarks>
        /// </summary>
        public static string ProxyTicketManagerProvider
        {
            get
            {
                Initialize();
                return _proxyTicketManagerProvider;
            }
        }

        /// <summary>
        /// An instance of the provider specified in the ProxyTicketManagerProvider property.
        /// ProxyTicketManager will be null if no proxyTicketManager is 
        /// defined in web.config.  If a ProxyTicketManager is defined, this will allow 
        /// generation of proxy tickets for external sites and services.  
        /// </summary>
        public static IProxyTicketManager ProxyTicketManager
        {
            get
            {
                Initialize();
                return _proxyTicketManager;
            }
        }

        /// <summary>
        /// Enable CAS gateway feature, see http://www.jasig.org/cas/protocol section 2.1.1.
        /// Default is false.
        /// </summary>
        public static bool Gateway
        {
            get
            {
                Initialize();
                return _gateway;
            }
        }

        /// <summary>
        /// The name of the cookie used to store the Gateway status (NotAttempted, 
        /// Success, Failed).  This cookie is used to prevent the client from 
        /// attempting to gateway authenticate every request.
        /// </summary>
        public static string GatewayStatusCookieName
        {
            get
            {
                Initialize();
                return _gatewayStatusCookieName;
            }
        }

        /// <summary>
        /// The Forms LoginUrl property set in system.web/authentication/forms
        /// </summary>
        public static string FormsLoginUrl
        {
            get
            {
                Initialize();
                return _formsLoginUrl;
            }
        }

        /// <summary>
        /// The Forms Timeout property set in system.web/authentication/forms
        /// </summary>
        public static TimeSpan FormsTimeout
        {
            get
            {
                Initialize();
                return _formsTimeout;
            }
        }

        /// <summary>
        /// URL of CAS login form.
        /// </summary>
        public static string CasServerLoginUrl
        {
            get
            {
                Initialize();
                return _casServerLoginUrl;
            }
        }

        /// <summary>
        /// URL to root of CAS server application.  For example, if your 
        /// CasServerLoginUrl is https://fed.example.com/cas/login
        /// then your CasServerUrlPrefix would be https://fed.example.com/cas/
        /// </summary>
        public static string CasServerUrlPrefix
        {
            get
            {
                Initialize();
                return _casServerUrlPrefix;
            }
        }

        /// <summary>
        /// SAML ticket validator property to allow at most the given time 
        /// difference in ms between artifact (ticket) timestamp and CAS server 
        /// system time.  Increasing this may have negative security consequences; 
        /// we recommend fixing sources of clock drift rather than increasing 
        /// this value.
        /// </summary>
        public static long TicketTimeTolerance
        {
            get
            {
                Initialize();
                return _ticketTimeTolerance;
            }
        }

        /// <summary>
        /// The server name of the server hosting the client application.  Service URL
        /// will be dynamically constructed using this value if Service is not specified.
        /// e.g. https://app.princeton.edu/
        /// </summary>
        public static string ServerName
        {
            get
            {
                Initialize();
                return _serverName;
            }
        }

        /// <summary>
        /// Force user to reauthenticate to CAS before accessing this application.
        /// This provides additional security at the cost of usability since it effectively
        /// disables SSO for this application.
        /// </summary>
        public static bool Renew
        {
            get
            {
                Initialize();
                return _renew;
            }
        }

        /// <summary>
        /// The name of the request parameter whose value is the artifact
        /// (e.g. "ticket").
        /// </summary>
        public static string ArtifactParameterName
        {
            get
            {
                Initialize();
                return _artifactParameterName;
            }
        }

        /// <summary>
        /// The name of the request parameter whose value is the service
        /// (e.g. "service")
        /// </summary>
        public static string ServiceParameterName
        {
            get
            {
                Initialize();
                return _serviceParameterName;
            }
        }

        /// <summary>
        /// Whether to redirect to the same URL after ticket validation, but without the ticket
        /// in the parameter.
        /// </summary>
        public static bool RedirectAfterValidation
        {
            get
            {
                Initialize();
                return _redirectAfterValidation;
            }
        }

        /// <summary>
        /// Specifies whether single sign out functionality should be enabled.
        /// </summary>
        public static bool SingleSignOut
        {
            get
            {
                Initialize();
                return _singleSignOut;
            }
        }

        /// <summary>
        /// The URL to redirect to when the request has a valid CAS ticket but the user is 
        /// not authorized to access the URL or resource.  If this option is set, users will
        /// be redirected to this URL.  If it is not set, the user will be redirected to the 
        /// CAS login screen with a Renew option in the URL (to force for alternate credential
        /// collection).
        /// </summary>
        public static string NotAuthorizedUrl
        {
            get
            {
                Initialize();
                return _notAuthorizedUrl;
            }
        }

        /// <summary>
        /// The URL to redirect to when the client is not accepting session 
        /// cookies.  This condition is detected only when gateway is enabled.  
        /// This will lock the users onto a specific page.  Otherwise, every 
        /// request will cause a silent round-trip to the CAS server, adding 
        /// a parameter to the URL.
        /// </summary>        
        public static string CookiesRequiredUrl
        {            
            get
            {
                Initialize();
                return _cookiesRequiredUrl;
            }
        }

        /// <summary>
        /// The URL parameter to append to outbound CAS request's ServiceName 
        /// when initiating an automatic CAS Gateway request.  This parameter 
        /// plays a role in detecting whether or not the client has cookies 
        /// enabled.  The default value is 'gatewayResponse' and only needs to 
        /// be explicitly defined if that URL parameter has a meaning elsewhere
        /// in your application.                              
        /// </summary>        
        public static string GatewayParameterName
        {
            get
            {
                Initialize();
                return _gatewayParameterName;
            }
        }

        /// <summary>
        /// The URL parameter to append to outbound CAS proxy request's pgtUrl
        /// when initiating an proxy ticket service validation.  This is used
        /// to determine whether the request is originating from the CAS server
        /// and contains a pgtIou.
        /// </summary>
        public static string ProxyCallbackParameterName
        {
            get
            {
                Initialize();
                return _proxyCallbackParameterName;
            }
        }
        #endregion
    }
}