using System;
using System.Text;
using System.Web;
using System.Web.Configuration;
using System.Web.Security;
using DotNetCasClient.Authentication;
using DotNetCasClient.Configuration;
using DotNetCasClient.Proxy;
using DotNetCasClient.State;
using DotNetCasClient.Utils;
using DotNetCasClient.Validation;
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
        private static string _ticketManagerProvider;
        private static ITicketManager _ticketManager;

        // Gateway support
        private static bool _gateway;
        private static IGatewayResolver _gatewayResolver;
        private static string _gatewayStatusCookieName;

        // Proxy support
        private static bool _proxyGrantingTicketReceptor;
        private static ProxyCallbackHandler _proxyCallbackHandler;
        private static string _proxyCallbackUrl;
        private static string _proxyReceptorUrl;

        private static string _formsLoginUrl;
        private static TimeSpan _formsTimeout;
        private static string _casServerLoginUrl;
        private static string _casServerUrlPrefix;
        private static long _ticketTimeTolerance;
        private static string _service;
        private static string _defaultServiceUrl;
        private static string _serverName;
        private static bool _renew;
        private static string _artifactParameterName;
        private static string _serviceParameterName;
        private static bool _redirectAfterValidation;
        private static bool _encodeServiceUrl;
        private static bool _singleSignOut;
        private static string _notAuthorizedUrl;
        private static string _cookiesRequiredUrl;
        private static string _gatewayParameterName;
        private static bool _useSession;
        private static string _secureUriRegex;
        private static string _secureUriExceptionRegex;
        #endregion

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

                        _formsLoginUrl = AuthenticationConfig.Forms.LoginUrl;
                        _formsTimeout = AuthenticationConfig.Forms.Timeout;

                        _casServerLoginUrl = CasClientConfig.CasServerLoginUrl;
                        _casServerUrlPrefix = CasClientConfig.CasServerUrlPrefix;
                        _ticketValidatorName = CasClientConfig.TicketValidatorName;
                        _ticketTimeTolerance = CasClientConfig.TicketTimeTolerance;
                        _service = CasClientConfig.Service;
                        _defaultServiceUrl = CasClientConfig.Service;
                        _serverName = CasClientConfig.ServerName;
                        _renew = CasClientConfig.Renew;
                        _gateway = CasClientConfig.Gateway;
                        _gatewayStatusCookieName = CasClientConfig.GatewayStatusCookieName;
                        _artifactParameterName = CasClientConfig.ArtifactParameterName;
                        _serviceParameterName = CasClientConfig.ServiceParameterName;
                        _redirectAfterValidation = CasClientConfig.RedirectAfterValidation;
                        _encodeServiceUrl = CasClientConfig.EncodeServiceUrl;
                        _singleSignOut = CasClientConfig.SingleSignOut;
                        _ticketManagerProvider = CasClientConfig.TicketManager;
                        _notAuthorizedUrl = CasClientConfig.NotAuthorizedUrl;
                        _cookiesRequiredUrl = CasClientConfig.CookiesRequiredUrl;
                        _gatewayParameterName = CasClientConfig.GatewayParameterName;
                        _useSession = CasClientConfig.UseSession;
                        _secureUriRegex = CasClientConfig.SecureUriRegex;
                        _secureUriExceptionRegex = CasClientConfig.SecureUriExceptionRegex;

                        if (CasClientConfig.ProxyGrantingTicketReceptor)
                        {
                            // throw new NotImplementedException("Proxy support is not implemented at this time.");
                            /*
                            _proxyGrantingTicketReceptor = CasClientConfig.ProxyGrantingTicketReceptor;
                            _proxyCallbackUrl = CasClientConfig.ProxyCallbackUrl;
                            _proxyReceptorUrl = CasClientConfig.ProxyReceptorUrl;
                            */
                        }

                        // Initialize default values
                        if (!String.IsNullOrEmpty(_service))
                        {
                            _defaultServiceUrl = _service;
                        }

                        if (_gateway)
                        {
                            // throw new NotImplementedException("Gateway has not been implemented yet.");
                            // _gatewayResolver = new SessionAttrGatewayResolver();
                        }

                        // Parse "enumerated" values 
                        if (String.Compare(_ticketValidatorName, CasClientConfiguration.CAS10_TICKET_VALIDATOR_NAME, true) == 0)
                        {
                            _ticketValidator = new Cas10TicketValidator(CasClientConfig);
                        }
                        else if (String.Compare(_ticketValidatorName, CasClientConfiguration.CAS20_TICKET_VALIDATOR_NAME, true) == 0)
                        {
                            _ticketValidator = new Cas20ServiceTicketValidator(CasClientConfig);
                        }
                        else if (String.Compare(_ticketValidatorName, CasClientConfiguration.SAML11_TICKET_VALIDATOR_NAME, true) == 0)
                        {
                            _ticketValidator = new Saml11TicketValidator(CasClientConfig);
                        }
                        else
                        {
                            throw new CasConfigurationException("Unknown ticket validator " + _ticketValidatorName);
                        }

                        if (String.IsNullOrEmpty(_ticketManagerProvider))
                        {
                            // Web server cannot maintain ticket state, verify tickets, perform SSO, etc.
                        }
                        else if (String.Compare(_ticketManagerProvider, CasClientConfiguration.CACHE_TICKET_MANAGER) == 0)
                        {
                            _ticketManager = new CacheTicketManager();
                        }
                        else
                        {
                            throw new CasConfigurationException("Unknown forms authentication state provider " + _ticketManagerProvider);
                        }

                        // Validate configuration
                        bool haveServerName = !String.IsNullOrEmpty(_serverName);
                        bool haveService = !String.IsNullOrEmpty(_service);
                        if ((haveServerName && haveService) || (!haveServerName && !haveService))
                        {
                            throw new CasConfigurationException(string.Format("Either {0} or {1} must be set (but not both).", CasClientConfiguration.SERVER_NAME, CasClientConfiguration.SERVICE));
                        }

                        if (String.IsNullOrEmpty(_casServerLoginUrl))
                        {
                            throw new CasConfigurationException(CasClientConfiguration.CAS_SERVER_LOGIN_URL + " cannot be null or empty.");
                        }

                        if (_ticketManager == null && _singleSignOut)
                        {
                            throw new CasConfigurationException("Single Sign Out requires a FormsAuthenticationStateProvider.");
                        }

                        if (_gateway && _renew)
                        {
                            throw new CasConfigurationException("Gateway and renew functionalities are mutually exclusive");
                        }

                        if (_encodeServiceUrl)
                        {
                            throw new CasConfigurationException("Encode URL with session ID functionality not yet implemented.");
                        }

                        if (!_redirectAfterValidation)
                        {
                            throw new CasConfigurationException("Forms Authentication based modules require RedirectAfterValidation to be set to true.");
                        }

                        _initialized = true;
                    }
                }
            }
        }

        /// <summary>
        /// Constructs a service uri using configured values in the following order:
        /// 1.  if not empty, the value configured for Service is used
        /// - otherwise -
        /// 2.  the value configured for ServerName is used together with HttpRequest
        ///     data
        /// </summary>
        /// <remarks>
        /// The server name is not parsed from the request for security reasons, which
        /// is why the service and server name configuration parameters exist, per Jasig
        /// website.
        /// </remarks>
        /// <returns>the service URI to use, not encoded</returns>
        internal static string ConstructServiceUri(bool gateway)
        {
            Initialize();

            HttpContext context = HttpContext.Current;
            HttpRequest request = context.Request;

            if (!String.IsNullOrEmpty(DefaultServiceUrl))
            {
                if (Log.IsDebugEnabled)
                {
                    Log.DebugFormat("{0}:return DefaultServiceUrl: {1}", CommonUtils.MethodName, DefaultServiceUrl);
                }
                return DefaultServiceUrl;
            }

            StringBuilder buffer = new StringBuilder();
            if (!(ServerName.StartsWith("https://") || ServerName.StartsWith("http://")))
            {
                buffer.Append(request.IsSecureConnection ? "https://" : "http://");
            }

            buffer.Append(ServerName);
            string absolutePath = request.Url.AbsolutePath;
            if (!absolutePath.StartsWith("/"))
            {
                buffer.Append("/");
            }
            buffer.Append(absolutePath);

            StringBuilder queryBuffer = new StringBuilder();
            if (request.QueryString.Count > 0)
            {
                string queryString = request.Url.Query;
                int indexOfTicket = queryString.IndexOf(TicketValidator.ArtifactParameterName + "=");
                if (indexOfTicket == -1)
                {
                    // No ticket parameter so keep QueryString as is
                    queryBuffer.Append(queryString);
                }
                else
                {
                    int indexAfterTicket = queryString.IndexOf("&", indexOfTicket);
                    if (indexAfterTicket == -1)
                    {
                        indexAfterTicket = queryString.Length;
                    }
                    else
                    {
                        indexAfterTicket = indexAfterTicket + 1;
                    }
                    queryBuffer.Append(queryString.Substring(0, indexOfTicket));
                    if (indexAfterTicket < queryString.Length)
                    {
                        queryBuffer.Append(queryString.Substring(indexAfterTicket));
                    }
                    else
                    {
                        queryBuffer.Length = queryBuffer.Length - 1;
                    }
                }
            }
            if (queryBuffer.Length > 1)
            {
                buffer.Append(queryBuffer.ToString());
            }

            if (Log.IsDebugEnabled)
            {
                Log.DebugFormat("{0}:return generated serviceUri: {1}", CommonUtils.MethodName, buffer);
            }

            if (gateway)
            {
                buffer.Append(buffer.ToString().Contains("?") ? "&gatewayResponse=true" : "?gatewayResponse=true");
            }

            return buffer.ToString();
        }

        /// <summary>
        /// Constructs the URL to use for redirection to the CAS server for login
        /// </summary>
        /// <remarks>
        /// The server name is not parsed from the request for security reasons, which
        /// is why the service and server name configuration parameters exist.
        /// </remarks>
        /// <returns>the redirection URL to use</returns>
        internal static string ConstructLoginRedirectUrl(bool gateway)
        {
            Initialize();

            // string casServerLoginUrl = CasServerUrlPrefix + (CasServerUrlPrefix.EndsWith("/") ? string.Empty : "/") + "login";

            string casServerLoginUrl = FormsLoginUrl;
            string serviceUri = ConstructServiceUri(gateway);
            string redirectToUrl = string.Format("{0}?{1}={2}{3}",
                casServerLoginUrl,
                TicketValidator.ServiceParameterName,
                HttpUtility.UrlEncode(serviceUri, Encoding.UTF8),
                (gateway ? "&gateway=true" : (Renew ? "&renew=true" : ""))
            );

            if (Log.IsDebugEnabled)
            {
                Log.DebugFormat("{0}: redirectToUrl=>{1}<", CommonUtils.MethodName, redirectToUrl);
            }

            return redirectToUrl;
        }

        /// <summary>
        /// Constructs the URL to use for redirection to the CAS server for single
        /// signout.  The CAS server will invalidate the ticket granting ticket and
        /// redirect back to the current page.  The web application must then call
        /// ClearAuthCookie and revoke the ticket from the TicketManager to sign 
        /// the client out.
        /// </summary>
        /// <returns>the redirection URL to use, not encoded</returns>
        internal static string ConstructSingleSignOutRedirectUrl()
        {
            Initialize();

            string casServerLogoutUrl = CasServerUrlPrefix + (CasServerUrlPrefix.EndsWith("/") ? string.Empty : "/") + "logout";
            string serviceUri = ConstructServiceUri(false);
            string redirectToUrl = string.Format("{0}?{1}={2}",
                casServerLogoutUrl,
                TicketValidator.ServiceParameterName,
                HttpUtility.UrlEncode(serviceUri, Encoding.UTF8)
            );

            if (Log.IsDebugEnabled)
            {
                Log.DebugFormat("{0}: redirectToUrl=>{1}<", CommonUtils.MethodName, redirectToUrl);
            }

            return redirectToUrl;
        }

        internal static void ClearGatewayStatusCookie()
        {
            Initialize();
            HttpContext current = HttpContext.Current;

            // Don't let anything see the incoming cookie 
            current.Request.Cookies.Remove(GatewayStatusCookieName);

            // Remove the cookie from the response collection (by adding an expired/empty version).
            HttpCookie cookie = new HttpCookie(GatewayStatusCookieName);
            cookie.Expires = DateTime.Now.AddMonths(-1);
            cookie.Domain = FormsAuthentication.CookieDomain;
            cookie.Path = FormsAuthentication.FormsCookiePath;
            current.Response.Cookies.Add(cookie);
        }
        
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
                    ClearGatewayStatusCookie();
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
        /// used by the server-side TicketManager to retrieve additional 
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

        /// <summary>
        /// Resolves a relative ~/Url to a Url that is meaningful to the
        /// client
        /// <remarks>
        /// http://weblogs.asp.net/palermo4/archive/2004/06/18/getting-the-absolute-path-in-asp-net-part-2.aspx
        /// </remarks>
        /// </summary>
        /// <param name="url">The Url to resolve</param>
        /// <returns></returns>
        internal static string ResolveUrl(string url)
        {
            if (url == null) throw new ArgumentNullException("url", "url can not be null");
            if (url.Length == 0) throw new ArgumentException("The url can not be an empty string", "url");
            if (url[0] != '~') return url;

            string applicationPath = HttpContext.Current.Request.ApplicationPath;
            if (url.Length == 1) return applicationPath;

            // assume url looks like ~somePage 
            int indexOfUrl = 1;

            // determine the middle character 
            string midPath = (applicationPath.Length > 1) ? "/" : string.Empty;

            // if url looks like ~/ or ~\ change the indexOfUrl to 2 
            if (url[1] == '/' || url[1] == '\\') indexOfUrl = 2;

            return applicationPath + midPath + url.Substring(indexOfUrl);
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
        /// Currently supported values: CacheTicketManager
        /// </remarks>
        /// </summary>
        public static string TicketManagerProvider
        {
            get
            {
                Initialize();
                return _ticketManagerProvider;
            }
        }

        /// <summary>
        /// An instance of the provider specified in the TicketManagerProvider property.
        /// CasAuthentication.TicketManager will be null if no ticketManager is 
        /// defined in web.config.  If a TicketManager is defined, this will allow 
        /// access to and revocation of outstanding CAS service tickets along with 
        /// additional information about the service tickets (i.e., IP address, 
        /// assertions, etc.).
        /// </summary>
        public static ITicketManager TicketManager
        {
            get
            {
                Initialize();
                return _ticketManager;
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
        /// Gateway resolver handles CAS gateway requests & responses. 
        /// http://www.ja-sig.org/wiki/display/CAS/gateway
        /// </summary>
        internal static IGatewayResolver GatewayResolver
        {
            get
            {
                Initialize();
                return _gatewayResolver;
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
        /// Specifies whether proxy granting tickets are being accepted.
        /// </summary>
        public static bool ProxyGrantingTicketReceptor
        {
            get
            {
                Initialize();
                return _proxyGrantingTicketReceptor;
            }
        }

        /// <summary>
        /// Proxy callback handler responsible for handling CAS proxy 
        /// tickets.
        /// </summary>
        internal static ProxyCallbackHandler ProxyCallbackHandler
        {
            get
            {
                Initialize();
                return _proxyCallbackHandler;
            }
        }

        /// <summary>
        /// The callback URL provided to the CAS server for receiving Proxy Granting Tickets.
        /// e.g. https://www.example.edu/cas-client-app/proxyCallback
        /// </summary>
        public static string ProxyCallbackUrl
        {
            get
            {
                Initialize();
                return _proxyCallbackUrl;
            }
        }

        /// <summary>
        /// The URL to watch for PGTIOU/PGT responses from the CAS server. Should be defined from
        /// the root of the context. For example, if your application is deployed in /cas-client-app
        /// and you want the proxy receptor URL to be /cas-client-app/my/receptor you need to configure
        /// proxyReceptorUrl to be /my/receptor
        /// e.g. /proxyCallback
        /// </summary>
        public static string ProxyReceptorUrl
        {
            get
            {
                Initialize();
                return _proxyReceptorUrl;
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
        /// The Service URL to send to the CAS server. 
        /// e.g. https://app.princeton.edu/example/
        /// </summary>
        public static string Service
        {
            get
            {
                Initialize();
                return _service;
            }
        }

        /// <summary>
        /// The service URL that will be used if a Service value is
        /// configured.
        /// </summary>
        public static string DefaultServiceUrl
        {
            get
            {
                Initialize();
                return _defaultServiceUrl;
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
        /// Whether to encode the session ID into the Service URL.
        /// constructed.
        /// </summary>
        public static bool EncodeServiceUrl
        {
            get
            {
                Initialize();
                return _encodeServiceUrl;
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
        /// Use session to store CAS authenticated state and principal/attribute info.
        /// Default is true.
        /// </summary>
        public static bool UseSession
        {
            get
            {
                Initialize();
                return _useSession;
            }
        }

        /// <summary>
        /// Regular expression describing URIs to be protected by CAS authentication.
        /// Default is .* to protect all application resources with CAS. 
        /// </summary>
        public static string SecureUriRegex
        {
            get
            {
                Initialize();
                return _secureUriRegex;
            }
        }

        /// <summary>
        /// Regular expression describing URIs to be specifically excluded from CAS auth.
        /// This feature originated to easily exclude resources used by .NET AJAX controls.
        /// The value in the following example illustrates how to ignore the resource used
        /// to bootstrap AJAX controls.  Default is to have no exclusions. 
        /// </summary>
        public static string SecureUriExceptionRegex
        {
            get
            {
                Initialize();
                return _secureUriExceptionRegex;
            }
        }
        #endregion
    }
}