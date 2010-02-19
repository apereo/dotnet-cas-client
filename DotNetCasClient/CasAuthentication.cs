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
    public static class CasAuthentication
    {
        /// <summary>
        /// Access to the log file
        /// </summary>
        private static readonly ILog Log = LogManager.GetLogger("CasAuthentication");

        internal static AuthenticationSection AuthenticationConfig;
        internal static CasClientConfiguration CasClientConfig;

        // Fields
        private static object _lockObject = new object();
        private static bool _Initialized = false;

        private static string _FormsLoginUrl;
        private static TimeSpan _FormsTimeout;        
        private static string _CasServerLoginUrl;
        private static string _CasServerUrlPrefix;
        private static string _TicketValidatorName;
        private static long _TicketTimeTolerance;
        private static string _Service;
        private static string _DefaultServiceUrl;
        private static string _ServerName;
        private static bool _Renew;
        private static bool _Gateway;
        private static string _ArtifactParameterName;
        private static string _ServiceParameterName;
        private static bool _RedirectAfterValidation;
        private static bool _EncodeServiceUrl;
        private static bool _SingleSignOut;
        private static bool _ProxyGrantingTicketReceptor;
        private static string _ProxyCallbackUrl;
        private static string _ProxyReceptorUrl;
        private static string _TicketManagerProvider;
        private static string _NotAuthorizedUrl;
        private static bool _UseSession;
        private static string _SecureUriRegex;
        private static string _SecureUriExceptionRegex;

        private static IGatewayResolver _GatewayResolver;
        private static AbstractUrlTicketValidator _TicketValidator;
        private static ProxyCallbackHandler _ProxyCallbackHandler;
        private static ITicketManager _TicketManager;

        // Methods
        public static void Initialize()
        {
            if (!_Initialized)
            {
                lock (_lockObject) {
                    if (!_Initialized)
                    {
                        FormsAuthentication.Initialize();
                        AuthenticationConfig = (AuthenticationSection) WebConfigurationManager.GetSection("system.web/authentication");
                        CasClientConfig = CasClientConfiguration.Config;

                        // Make sure we are configured for Forms Authentication Mode, do we really have 
                        // to keep doing this? isn't once enough? should we have an initConfig()?
                        if (AuthenticationConfig == null)
                        {
                            if (Log.IsDebugEnabled)
                            {
                                Log.Debug("Not configured for any Authentication");
                            }
                            throw new CasConfigurationException("The CAS authentication provider requires Forms authentication to be enabled in web.config.");
                        }

                        if (AuthenticationConfig.Mode != AuthenticationMode.Forms)
                        {
                            if (Log.IsDebugEnabled)
                            {
                                Log.Debug("Not configured for Forms Authentication");
                            }
                            throw new CasConfigurationException("The CAS authentication provider requires Forms authentication to be enabled in web.config.");
                        }

                        if (FormsAuthentication.CookieMode != HttpCookieMode.UseCookies)
                        {
                            throw new CasConfigurationException("CAS requires Forms Authentication to use cookies (cookieless='UseCookies').");
                        }

                        _FormsLoginUrl = AuthenticationConfig.Forms.LoginUrl;
                        _FormsTimeout = AuthenticationConfig.Forms.Timeout;

                        _CasServerLoginUrl = CasClientConfig.CasServerLoginUrl;
                        _CasServerUrlPrefix = CasClientConfig.CasServerUrlPrefix;
                        _TicketValidatorName = CasClientConfig.TicketValidatorName;
                        _TicketTimeTolerance = CasClientConfig.TicketTimeTolerance;
                        _Service = CasClientConfig.Service;
                        _DefaultServiceUrl = CasClientConfig.Service;
                        _ServerName = CasClientConfig.ServerName;
                        _Renew = CasClientConfig.Renew;
                        _Gateway = CasClientConfig.Gateway;
                        _ArtifactParameterName = CasClientConfig.ArtifactParameterName;
                        _ServiceParameterName = CasClientConfig.ServiceParameterName;
                        _RedirectAfterValidation = CasClientConfig.RedirectAfterValidation;
                        _EncodeServiceUrl = CasClientConfig.EncodeServiceUrl;
                        _SingleSignOut = CasClientConfig.SingleSignOut;
                        _ProxyGrantingTicketReceptor = CasClientConfig.ProxyGrantingTicketReceptor;
                        _ProxyCallbackUrl = CasClientConfig.ProxyCallbackUrl;
                        _ProxyReceptorUrl = CasClientConfig.ProxyReceptorUrl;
                        _TicketManagerProvider = CasClientConfig.TicketManager;
                        _NotAuthorizedUrl = CasClientConfig.NotAuthorizedUrl;
                        _UseSession = CasClientConfig.UseSession;
                        _SecureUriRegex = CasClientConfig.SecureUriRegex;
                        _SecureUriExceptionRegex = CasClientConfig.SecureUriExceptionRegex;

                        // Initialize default values
                        if (!string.IsNullOrEmpty(_Service))
                        {
                            _DefaultServiceUrl = _Service;
                        }

                        if (_Gateway)
                        {
                            _GatewayResolver = new SessionAttrGatewayResolver();
                        }

                        // Parse "enumerated" values 
                        if (string.Compare(_TicketValidatorName, CasClientConfiguration.CAS10_TICKET_VALIDATOR_NAME, true) == 0) 
                        {
                            _TicketValidator = new Cas10TicketValidator(CasClientConfig);
                        }
                        else if (string.Compare(_TicketValidatorName, CasClientConfiguration.CAS20_TICKET_VALIDATOR_NAME, true) == 0)
                        {
                            _TicketValidator = new Cas20ServiceTicketValidator(CasClientConfig);
                        } 
                        else if (string.Compare(_TicketValidatorName, CasClientConfiguration.SAML11_TICKET_VALIDATOR_NAME, true) == 0)
                        {
                            _TicketValidator = new Saml11TicketValidator(CasClientConfig);
                        }
                        else
                        {
                            throw new CasConfigurationException("Unknown ticket validator " + _TicketValidatorName);
                        }
                        
                        if (string.IsNullOrEmpty(_TicketManagerProvider))
                        {
                            // Web server cannot maintain ticket state, verify tickets, perform SSO, etc.
                        } 
                        else if (string.Compare(_TicketManagerProvider, CasClientConfiguration.CACHE_TICKET_MANAGER) == 0)
                        {
                            _TicketManager = new CacheTicketManager();
                        } 
                        else 
                        {
                            throw new CasConfigurationException("Unknown forms authentication state provider " + _TicketManagerProvider);
                        }

                        // Validate configuration
                        bool haveServerName = !string.IsNullOrEmpty(_ServerName);
                        bool haveService = !string.IsNullOrEmpty(_Service);
                        if ((haveServerName && haveService) || (!haveServerName && !haveService))
                        {
                            throw new CasConfigurationException(string.Format("Either {0} or {1} must be set (but not both).", CasClientConfiguration.SERVER_NAME, CasClientConfiguration.SERVICE));
                        }

                        if (string.IsNullOrEmpty(_CasServerLoginUrl))
                        {
                            throw new CasConfigurationException(CasClientConfiguration.CAS_SERVER_LOGIN_URL + " cannot be null or empty.");
                        }

                        if (_TicketManager == null && _SingleSignOut)
                        {
                            throw new CasConfigurationException("Single Sign Out requires a FormsAuthenticationStateProvider.");
                        }

                        if (_Gateway && _Renew)
                        {
                            throw new CasConfigurationException("Gateway and renew functionalities are mutually exclusive");
                        }

                        if (_EncodeServiceUrl)
                        {
                            throw new CasConfigurationException("Encode URL with session ID functionality not yet implemented.");
                        }

                        if (!_RedirectAfterValidation)
                        {
                            throw new CasConfigurationException("Forms Authentication based modules require RedirectAfterValidation to be set to true.");
                        }
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
        internal static string ConstructServiceUri()
        {
            Initialize();
            
            HttpContext context = HttpContext.Current;
            HttpRequest request = context.Request;

            if (!string.IsNullOrEmpty(DefaultServiceUrl))
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
            return buffer.ToString();
        }

        /// <summary>
        /// Constructs the URL to use for redirection to the CAS server for login
        /// </summary>
        /// <remarks>
        /// The server name is not parsed from the request for security reasons, which
        /// is why the service and server name configuration parameters exist.
        /// </remarks>
        /// <param name="casServerLoginUrl">the exact CAS server login URL</param>
        /// <returns>the redirection URL to use, not encoded</returns>
        internal static string ConstructLoginRedirectUrl()
        {
            Initialize();

            // string casServerLoginUrl = CasServerUrlPrefix + (CasServerUrlPrefix.EndsWith("/") ? string.Empty : "/") + "login";

            string casServerLoginUrl = FormsLoginUrl;
            string serviceUri = ConstructServiceUri();
            string redirectToUrl = string.Format("{0}?{1}={2}{3}{4}",
                casServerLoginUrl,
                TicketValidator.ServiceParameterName,
                HttpUtility.UrlEncode(serviceUri, Encoding.UTF8),
                (Renew ? "&renew=true" : ""),
                (Gateway ? "&gateway=true" : "")
            );

            if (Log.IsDebugEnabled)
            {
                Log.DebugFormat("{0}: redirectToUrl=>{1}<", CommonUtils.MethodName, redirectToUrl);
            }

            return redirectToUrl;
        }

        internal static string ConstructSingleSignOutRedirectUrl()
        {
            Initialize();

            string casServerLogoutUrl = CasServerUrlPrefix + (CasServerUrlPrefix.EndsWith("/") ? string.Empty : "/") + "logout";
            string serviceUri = ConstructServiceUri();
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
            
            if (string.IsNullOrEmpty(str))
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
            if (string.IsNullOrEmpty(netId))
            {
                throw new ArgumentNullException("netId");
            }

            if (string.IsNullOrEmpty(serviceTicket))
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

            if (string.IsNullOrEmpty(cookie.Value))
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

            if (string.IsNullOrEmpty(formsAuthTicket.UserData))
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
        internal static string ResolveUrl(string url) {
            if (url == null) throw new ArgumentNullException("url", "url can not be null"); 
        	if (url.Length == 0) throw new ArgumentException("The url can not be an empty string", "url"); 
        	if (url[0] != '~') return url; 

        	string applicationPath = HttpContext.Current.Request.ApplicationPath; 
        	if (url.Length == 1) return	applicationPath; 

        	// assume url looks like ~somePage 
        	int indexOfUrl = 1; 

        	// determine the middle character 
        	string midPath = (applicationPath.Length > 1 ) ? "/" : string.Empty; 

        	// if url looks like ~/ or ~\ change the indexOfUrl to 2 
        	if (url[1] == '/' || url[1] == '\\') indexOfUrl = 2; 

        	return applicationPath + midPath + url.Substring(indexOfUrl); 
        }

        // Properties
        public static string FormsLoginUrl { get { Initialize(); return _FormsLoginUrl; } }
        public static TimeSpan FormsTimeout { get { Initialize(); return _FormsTimeout; } }
        public static string CasServerLoginUrl { get { Initialize(); return _CasServerLoginUrl; } }
        public static string CasServerUrlPrefix { get { Initialize(); return _CasServerUrlPrefix; } }
        public static string TicketValidatorName { get { Initialize(); return _TicketValidatorName; } }
        public static long TicketTimeTolerance { get { Initialize(); return _TicketTimeTolerance; } }
        public static string Service { get { Initialize(); return _Service; } }
        public static string DefaultServiceUrl { get { Initialize(); return _DefaultServiceUrl; } }
        public static string ServerName { get { Initialize(); return _ServerName; } }
        public static bool Renew { get { Initialize(); return _Renew; } }
        public static bool Gateway { get { Initialize(); return _Gateway; } }
        public static string ArtifactParameterName { get { Initialize(); return _ArtifactParameterName; } }
        public static string ServiceParameterName { get { Initialize(); return _ServiceParameterName; } }
        public static bool RedirectAfterValidation { get { Initialize(); return _RedirectAfterValidation; } }
        public static bool EncodeServiceUrl { get { Initialize(); return _EncodeServiceUrl; } }
        public static bool SingleSignOut { get { Initialize(); return _SingleSignOut; } }
        public static bool ProxyGrantingTicketReceptor { get { Initialize(); return _ProxyGrantingTicketReceptor; } }
        public static string ProxyCallbackUrl { get { Initialize(); return _ProxyCallbackUrl; } }
        public static string ProxyReceptorUrl { get { Initialize(); return _ProxyReceptorUrl; } }
        public static string TicketManagerProvider { get { Initialize(); return _TicketManagerProvider; } }
        public static string NotAuthorizedUrl { get { Initialize(); return _NotAuthorizedUrl; } }
        public static bool UseSession { get { Initialize(); return _UseSession; } }
        public static string SecureUriRegex { get { Initialize(); return _SecureUriRegex; } }
        public static string SecureUriExceptionRegex { get { Initialize(); return _SecureUriExceptionRegex; } }

        public static ITicketManager TicketManager { get { Initialize(); return _TicketManager; } }
        internal static IGatewayResolver GatewayResolver { get { Initialize(); return _GatewayResolver; } }
        internal static AbstractUrlTicketValidator TicketValidator { get { Initialize(); return _TicketValidator; } }
        internal static ProxyCallbackHandler ProxyCallbackHandler { get { Initialize(); return _ProxyCallbackHandler; } }
    }
}
