using System;
using System.IO;
using System.Text;
using System.Web;
using System.Web.Configuration;
using System.Web.Security;
using System.Xml;
using DotNetCasClient.Configuration;
using DotNetCasClient.State;

namespace DotNetCasClient.Utils
{
    public abstract class AbstractFormsAuthenticationCasModule : AbstractCasModule
    {
        internal static AuthenticationSection AuthenticationConfig;
        internal static FormsAuthenticationConfiguration FormsAuthConfig;

        protected const string XmlSessionIndexElementName = "samlp:SessionIndex";

        protected static readonly string HtmlRedirectTemplate =
            "<html><head><title>Object moved</title></head><body>" + Environment.NewLine +
            "<h2>Object moved to <a href=\"{0}\">here</a>.</h2>" + Environment.NewLine +
            "</body></html>" + Environment.NewLine;

        protected static readonly int HtmlRedirectTemplateLength = HtmlRedirectTemplate.Length;

        /// <summary>
        /// Specifies whether single sign out functionality should be enabled.
        /// </summary>
        protected string FormsAuthenticationStateProvider { get; private set; }

        /// <summary>
        /// The SingleOutHandler to be used for sign out, including CAS server
        /// single sign out.
        /// </summary>
        private IFormsAuthenticationStateProvider ticketManager;
        public IFormsAuthenticationStateProvider TicketManager
        {
            get
            {
                return ticketManager;
            }
        }

        public override void Init(HttpApplication application)
        {
            base.Init(application);
            InitInternalConfiguration(application);
        }

        private void InitInternalConfiguration(HttpApplication application)
        {
            AuthenticationConfig = (AuthenticationSection)WebConfigurationManager.GetSection("system.web/authentication");

            // Make sure we are configured for Forms Authentication Mode, do we really have 
            // to keep doing this? isn't once enough? should we have an initConfig()?
            if (AuthenticationConfig == null)
            {
                if (log.IsDebugEnabled)
                {
                    log.DebugFormat("{0}:not configured for any Authentication", CommonUtils.MethodName);
                }
                throw new CasConfigurationException("The CAS authentication provider requires Forms authentication to be enabled in web.config.");
            }
            
            if (AuthenticationConfig.Mode != AuthenticationMode.Forms)
            {
                if (log.IsDebugEnabled)
                {
                    log.DebugFormat("{0}:not configured for Forms Authentication", CommonUtils.MethodName);
                }
                throw new CasConfigurationException("The CAS authentication provider requires Forms authentication to be enabled in web.config.");
            }
            
            if (AuthenticationConfig.Forms != null)
            {
                FormsAuthConfig = AuthenticationConfig.Forms;
            }
            else
            {
                throw new CasConfigurationException("The CAS Authentication provider requires a valid Forms authentication defined in web.config.");
            }

            if (FormsAuthConfig.Cookieless != HttpCookieMode.UseCookies)
            {
                throw new CasConfigurationException("CAS requires Forms Authentication to use cookies (cookieless='UseCookies').");
            }

            FormsAuthenticationStateProvider = config.FormsAuthenticationStateProvider;
            if (!string.IsNullOrEmpty(FormsAuthenticationStateProvider))
            {
                switch (FormsAuthenticationStateProvider)
                {
                    case CasClientConfiguration.CACHE_AUTHENTICATION_STATE_PROVIDER:
                        ticketManager = new CacheAuthenticationStateProvider();
                        break;
                    default:
                        throw new CasConfigurationException("Unknown forms authentication state provider " + config.FormsAuthenticationStateProvider);
                }
                log.Info("Set forms authentication state provider: " + FormsAuthenticationStateProvider.GetType().Name);
            }
            else
            {
                log.Info("Not configured to use forms authentication state provider, SSO not possible.");

                if (SingleSignOut)
                {
                    throw new CasConfigurationException("Single Sign Out requires a FormsAuthenticationStateProvider.");
                }
            }

            if (!RedirectAfterValidation)
            {
                throw new CasConfigurationException("Forms Authentication based modules require RedirectAfterValidation to be set to true.");
            }
        }

        protected string ConstructRedirectUri(HttpContext context, string casServerLoginUrl)
        {
            Uri serviceUri = ConstructServiceUri(context.Request);

            string redirectToUrl = string.Format("{0}?{1}={2}{3}{4}",
                  casServerLoginUrl,
                  ticketValidator.ServiceParameterName,
                  HttpUtility.UrlEncode(serviceUri.ToString(), Encoding.UTF8),
                  (config.Renew ? "&renew=true" : ""),
                  (config.Gateway ? "&gateway=true" : "")
                );

            if (log.IsDebugEnabled)
            {
                log.DebugFormat("{0}: redirectToUrl=>{1}<", CommonUtils.MethodName, redirectToUrl);
            }

            return redirectToUrl;
        }

        internal static HttpCookie GetAuthCookie(FormsAuthenticationTicket ticket)
        {
            FormsAuthentication.Initialize();

            string str = FormsAuthentication.Encrypt(ticket);
            if (string.IsNullOrEmpty(str))
            {
                throw new HttpException("Unable to encrypt cookie ticket");
            }

            HttpCookie cookie = new HttpCookie(FormsAuthentication.FormsCookieName, str);
            cookie.HttpOnly = true;
            cookie.Path = FormsAuthConfig.Path;
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

        public static void ClearAuthCookie()
        {
            FormsAuthentication.Initialize();
            HttpContext current = HttpContext.Current;

            // Don't let anything see the incoming cookie 
            current.Request.Cookies.Remove(FormsAuthentication.FormsCookieName);

            // Remove the cookie from the response collection (by adding an expired/empty version).
            HttpCookie cookie = new HttpCookie(FormsAuthentication.FormsCookieName);
            cookie.Expires = DateTime.Now.AddMonths(-1);
            cookie.Domain = FormsAuthentication.CookieDomain;
            cookie.Path = FormsAuthConfig.Path;
            current.Response.Cookies.Add(cookie);
        }

        protected void SetAuthCookie(FormsAuthenticationTicket clientTicket)
        {
            FormsAuthentication.Initialize();
            HttpContext current = HttpContext.Current;

            if (!current.Request.IsSecureConnection && FormsAuthentication.RequireSSL)
            {
                throw new HttpException("Connection not secure while creating secure cookie");
            }

            current.Response.Cookies.Add(GetAuthCookie(clientTicket));
        }

        internal static FormsAuthenticationTicket CreateFormsAuthenticationTicket(string userName, bool createPersistentCookie, string cookiePath, string casTicket)
        {
            FormsAuthentication.Initialize();
            FormsAuthenticationTicket ticket = new FormsAuthenticationTicket(
                2,
                userName ?? string.Empty,
                DateTime.Now,
                DateTime.Now.AddMinutes(FormsAuthConfig.Timeout.TotalMinutes),
                createPersistentCookie,
                casTicket,
                string.IsNullOrEmpty(cookiePath) ? FormsAuthentication.FormsCookiePath : cookiePath
            );

            // return GetAuthCookie(ticket, strCookiePath);
            return ticket;
        }

        protected static FormsAuthenticationTicket GetFormsAuthenticationTicket(HttpContext context)
        {
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

            if (CommonUtils.IsBlank(cookie.Value))
            {
                ClearAuthCookie();
                return null;
            }

            FormsAuthenticationTicket formsAuthTicket;
            try
            {
                formsAuthTicket = FormsAuthentication.Decrypt(cookie.Value);
            }
            catch (ArgumentException)
            {
                ClearAuthCookie();
                return null;
            }

            if (formsAuthTicket == null || formsAuthTicket.Expired)
            {
                ClearAuthCookie();
                return null;
            }

            return formsAuthTicket;
        }

        // Borrowed from FormsAuthenticationProvider via Reflector
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

        // Borrowed from FormsAuthenticationProvider via Reflector
        protected static string RemoveQueryStringVariableFromUrl(string strUrl, string qsVar)
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
        /// Extracts the CAS ticket from the SAML message supplied.
        /// </summary>
        /// <param name="xmlAsString">SAML message from CAS server</param>
        /// <returns>The CAS ticket contained in SAML message</returns>
        protected static string ExtractSingleSignOutTicketFromSamlResponse(string xmlAsString)
        {
            // XmlUtils.GetTextForElement wasn't handling namespaces correctly. 
            // Existing SingleSignOut implementation wasn't working correctly.

            string elementText = null;
            if (CommonUtils.IsNotBlank(xmlAsString) &&
                CommonUtils.IsNotBlank(XmlSessionIndexElementName))
            {
                using (TextReader textReader = new StringReader(xmlAsString))
                {
                    XmlReaderSettings settings = new XmlReaderSettings();
                    settings.ConformanceLevel = ConformanceLevel.Auto;
                    settings.IgnoreWhitespace = true;

                    NameTable nt = new NameTable();

                    XmlNamespaceManager nsmgr = new XmlNamespaceManager(nt);
                    nsmgr.AddNamespace("saml", "urn:saml");
                    nsmgr.AddNamespace("samlp", "urn:samlp");

                    XmlParserContext context = new XmlParserContext(null, nsmgr, null, XmlSpace.None);

                    XmlReader reader = XmlReader.Create(textReader, settings, context);
                    bool foundElement = reader.ReadToFollowing(XmlSessionIndexElementName);
                    if (foundElement)
                    {
                        elementText = reader.ReadElementString();
                    }

                    reader.Close();
                }
            }
            return elementText;
        }
    }
}
