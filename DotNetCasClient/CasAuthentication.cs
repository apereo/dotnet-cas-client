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
using System.IO;
using System.Threading;
using System.Web;
using System.Web.Configuration;
using System.Web.Security;
using System.Xml;
using DotNetCasClient.Configuration;
using DotNetCasClient.Logging;
using DotNetCasClient.Security;
using DotNetCasClient.State;
using DotNetCasClient.Utils;
using DotNetCasClient.Validation;
using DotNetCasClient.Validation.Schema.Cas20;
using DotNetCasClient.Validation.TicketValidator;
using System.Collections.Generic;

namespace DotNetCasClient
{
    /// <summary>
    /// CasAuthentication exposes a public API for use in working with CAS Authentication
    /// in the .NET framework.  It also exposes all configured CAS client configuration 
    /// parameters as public static properties.
    /// </summary>
    /// <author>Marvin S. Addison</author>
    /// <author>Scott Holodak</author>
    /// <author>William G. Thompson, Jr.</author>
    /// <author>Catherine D. Winfrey</author>
    public sealed class CasAuthentication
    {
        #region Constants
        private const string XML_SESSION_INDEX_ELEMENT_NAME = "samlp:SessionIndex";
        private const string PARAM_PROXY_GRANTING_TICKET_IOU = "pgtIou";        
        private const string PARAM_PROXY_GRANTING_TICKET = "pgtId";
        #endregion

        #region Fields
        // Loggers
        private static readonly Logger configLogger = new Logger(Category.Config);
        private static readonly Logger protoLogger = new Logger(Category.Protocol);
        private static readonly Logger securityLogger = new Logger(Category.Security);

        // Thread-safe initialization
        private static readonly object LockObject;
        private static bool initialized;

        // System.Web/Authentication and System.Web/Authentication/Forms static classes
        internal static AuthenticationSection AuthenticationConfig;
        internal static CasClientConfiguration CasClientConfig;

        // Ticket validator fields
        private static string ticketValidatorName;
		private static ITicketValidator ticketValidator;

        // Ticket manager fields
        private static string serviceTicketManagerProvider;
        private static IServiceTicketManager serviceTicketManager;

        // Proxy ticket fields
        private static string proxyTicketManagerProvider;
        private static IProxyTicketManager proxyTicketManager;

        // Gateway fields
        private static bool gateway;
        private static string gatewayStatusCookieName;

        // Configuration fields
        private static string formsLoginUrl;
        private static TimeSpan formsTimeout;
        private static string casServerLoginUrl;
        private static string casServerUrlPrefix;
        private static long ticketTimeTolerance;
        private static string serverName;
        private static bool renew;
        private static bool redirectAfterValidation;
        private static bool singleSignOut;
        private static string notAuthorizedUrl;
        private static string cookiesRequiredUrl;
        private static string gatewayParameterName;
        private static string proxyCallbackParameterName;
        private static string casProxyCallbackUrl;
        private static bool requireCasForMissingContentTypes;
        private static string[] requireCasForContentTypes;
        private static string[] bypassCasForHandlers;

        // Provide reliable way for arbitrary components in forms
        // authentication pipeline to access CAS principal
        [ThreadStatic]
        private static ICasPrincipal currentPrincipal;

        // XML Reader Settings for SAML parsing.
        private static XmlReaderSettings xmlReaderSettings;

        // XML Name Table for namespace resolution in SSO SAML Parsing routine
        private static NameTable xmlNameTable;

        /// XML Namespace Manager for namespace resolution in SSO SAML Parsing routine
        private static XmlNamespaceManager xmlNamespaceManager;
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
        /// Current authenticated principal or null if current user is unauthenticated.
        /// </summary>
        public static ICasPrincipal CurrentPrincipal
        {
            get { return currentPrincipal; }
        }   

        /// <summary>
        /// Initializes configuration-related properties and validates configuration.
        /// </summary>        
        public static void Initialize()
        {
            if (!initialized)
            {
                lock (LockObject)
                {
                    if (!initialized)
                    {
                        FormsAuthentication.Initialize();
                        AuthenticationConfig = (AuthenticationSection)WebConfigurationManager.GetSection("system.web/authentication");
                        CasClientConfig = CasClientConfiguration.Config;

                        if (AuthenticationConfig == null)
                        {
                            LogAndThrowConfigurationException(
                                "The CAS authentication provider requires Forms authentication to be enabled in web.config.");
                        }

                        if (AuthenticationConfig.Mode != AuthenticationMode.Forms)
                        {
                            LogAndThrowConfigurationException(
                                "The CAS authentication provider requires Forms authentication to be enabled in web.config.");
                        }

                        if (FormsAuthentication.CookieMode != HttpCookieMode.UseCookies)
                        {
                            LogAndThrowConfigurationException(
                                "CAS requires Forms Authentication to use cookies (cookieless='UseCookies').");
                        }

                        xmlReaderSettings = new XmlReaderSettings();
                        xmlReaderSettings.ConformanceLevel = ConformanceLevel.Auto;
                        xmlReaderSettings.IgnoreWhitespace = true;

                        xmlNameTable = new NameTable();

                        xmlNamespaceManager = new XmlNamespaceManager(xmlNameTable);
                        xmlNamespaceManager.AddNamespace("cas", "http://www.yale.edu/tp/cas");
                        xmlNamespaceManager.AddNamespace("saml", "urn: oasis:names:tc:SAML:1.0:assertion");
                        xmlNamespaceManager.AddNamespace("saml2", "urn: oasis:names:tc:SAML:1.0:assertion");
                        xmlNamespaceManager.AddNamespace("samlp", "urn: oasis:names:tc:SAML:1.0:protocol");

                        formsLoginUrl = AuthenticationConfig.Forms.LoginUrl;
                        formsTimeout = AuthenticationConfig.Forms.Timeout;

                        if (string.IsNullOrEmpty(CasClientConfig.CasServerUrlPrefix))
                        {
                            LogAndThrowConfigurationException("The CasServerUrlPrefix is required");
                        }

                        casServerUrlPrefix = CasClientConfig.CasServerUrlPrefix;
                        configLogger.Info("casServerUrlPrefix = " + casServerUrlPrefix);
                        
                        casServerLoginUrl = CasClientConfig.CasServerLoginUrl;
                        configLogger.Info("casServerLoginUrl = " + casServerLoginUrl);
                        
                        ticketValidatorName = CasClientConfig.TicketValidatorName;
                        configLogger.Info("ticketValidatorName = " + ticketValidatorName);
                        
                        ticketTimeTolerance = CasClientConfig.TicketTimeTolerance;
                        configLogger.Info("ticketTimeTolerance = " + ticketTimeTolerance);
                        
                        serverName = CasClientConfig.ServerName;
                        configLogger.Info("serverName = " + serverName);
                        
                        renew = CasClientConfig.Renew;
                        configLogger.Info("renew = " + renew);
                        
                        gateway = CasClientConfig.Gateway;
                        configLogger.Info("gateway = " + gateway);
                        
                        gatewayStatusCookieName = CasClientConfig.GatewayStatusCookieName;
                        configLogger.Info("gatewayStatusCookieName = " + gatewayStatusCookieName);
                        
                        redirectAfterValidation = CasClientConfig.RedirectAfterValidation;
                        configLogger.Info("redirectAfterValidation = " + redirectAfterValidation);
                        
                        singleSignOut = CasClientConfig.SingleSignOut;
                        configLogger.Info("singleSignOut = " + singleSignOut);
                        
                        serviceTicketManagerProvider = CasClientConfig.ServiceTicketManager;
                        configLogger.Info("serviceTicketManagerProvider = " + serviceTicketManagerProvider);
                        
                        proxyTicketManagerProvider = CasClientConfig.ProxyTicketManager;
                        configLogger.Info("proxyTicketManagerProvider = " + proxyTicketManagerProvider);
                        
                        notAuthorizedUrl = CasClientConfig.NotAuthorizedUrl;
                        configLogger.Info("notAuthorizedUrl = " + notAuthorizedUrl);
                        
                        cookiesRequiredUrl = CasClientConfig.CookiesRequiredUrl;
                        configLogger.Info("cookiesRequiredUrl = " + cookiesRequiredUrl);
                        
                        gatewayParameterName = CasClientConfig.GatewayParameterName;
                        configLogger.Info("gatewayParameterName = " + gatewayParameterName);
                        
                        proxyCallbackParameterName = CasClientConfig.ProxyCallbackParameterName;
                        configLogger.Info("proxyCallbackParameterName = " + proxyCallbackParameterName);

                        casProxyCallbackUrl = CasClientConfig.ProxyCallbackUrl;
                        configLogger.Info("proxyCallbackUrl = " + casProxyCallbackUrl);

                        requireCasForMissingContentTypes = CasClientConfig.RequireCasForMissingContentTypes;
                        configLogger.Info("requireCasForMissingContentTypes = " + requireCasForMissingContentTypes);

                        requireCasForContentTypes = CasClientConfig.RequireCasForContentTypes;
                        configLogger.Info("requireCasForContentTypes = " + requireCasForContentTypes);

                        bypassCasForHandlers = CasClientConfig.BypassCasForHandlers;
                        configLogger.Info("bypassCasForHandlers = " + bypassCasForHandlers);
                        
                        if (!String.IsNullOrEmpty(ticketValidatorName))
                        {
                            if (String.Compare(CasClientConfiguration.CAS10_TICKET_VALIDATOR_NAME,ticketValidatorName) == 0)                            
                                ticketValidator = new Cas10TicketValidator();
                            else if (String.Compare(CasClientConfiguration.CAS20_TICKET_VALIDATOR_NAME, ticketValidatorName) == 0)
                                ticketValidator = new Cas20ServiceTicketValidator();
                            else if (String.Compare(CasClientConfiguration.SAML11_TICKET_VALIDATOR_NAME, ticketValidatorName) == 0)
                                ticketValidator = new Saml11TicketValidator();                            
                            else
                            {
                                // the ticket validator name is not recognized, let's try to get it using Reflection then                                
                                Type ticketValidatorType = Type.GetType(ticketValidatorName, false, true);
                                if (ticketValidatorType != null)
                                {
									if (typeof(ITicketValidator).IsAssignableFrom(ticketValidatorType))
										ticketValidator = (ITicketValidator)Activator.CreateInstance(ticketValidatorType);                                    
                                    else
                                        LogAndThrowConfigurationException("Ticket validator type is not correct " + ticketValidatorName);
                                }
                                else
                                    LogAndThrowConfigurationException("Could not find ticket validatory type " + ticketValidatorName);
                            }
                            configLogger.Info("TicketValidator type = " + ticketValidator.GetType().ToString());
                        }
                        else                                                    
                            LogAndThrowConfigurationException("Ticket validator name missing");
                        
                        
                        
                        if (String.IsNullOrEmpty(serviceTicketManagerProvider))
                        {
                            // Web server cannot maintain ticket state, verify tickets, perform SSO, etc.
                        }
                        else
                        {
                            if (String.Compare(CasClientConfiguration.CACHE_SERVICE_TICKET_MANAGER, serviceTicketManagerProvider) == 0)
                            {
#if NET20 || NET35
                                // Use the service ticket manager that implements an in-memory cache supported by .NET 2.0/3.5.
                                serviceTicketManager = new CacheServiceTicketManager();
#endif

#if NET40 || NET45
                                // Use the service ticket manager that implements an in-memory cache supported by .NET 4.x.
                                serviceTicketManager = new MemoryCacheServiceTicketManager();
#endif
                            }
                            else
                            {
                                // the service ticket manager  is not recognized, let's try to get it using Reflection then
                                Type serviceTicketManagerType = Type.GetType(serviceTicketManagerProvider, false, true);
                                if (serviceTicketManagerType != null)
                                {
                                    if (typeof(IServiceTicketManager).IsAssignableFrom(serviceTicketManagerType))
                                        serviceTicketManager = (IServiceTicketManager)Activator.CreateInstance(serviceTicketManagerType);
                                    else
                                        LogAndThrowConfigurationException("Service Ticket Manager type is not correct " + serviceTicketManagerProvider);
                                }
                                else
                                    LogAndThrowConfigurationException("Could not find Service Ticket Manager type " + serviceTicketManagerProvider);
                            }
                            configLogger.Info("ServiceTicketManager type = " + serviceTicketManager.GetType().ToString());
                        }

                        if (String.IsNullOrEmpty(proxyTicketManagerProvider))
                        {
                            // Web server cannot generate proxy tickets
                        }
                        else
                        {
                            if (String.Compare(CasClientConfiguration.CACHE_PROXY_TICKET_MANAGER, proxyTicketManagerProvider) == 0)
                            {
#if NET20 || NET35
                                // Use the proxy ticket manager that implements an in-memory cache supported by .NET 2.0/3.5.
                                proxyTicketManager = new CacheProxyTicketManager();
#endif

#if NET40 || NET45
                                // Use the proxy ticket manager that implements an in-memory cache supported by .NET 4.x.
                                proxyTicketManager = new MemoryCacheProxyTicketManager();
#endif
                            }
                            else
                            {
                                // the proxy ticket manager  is not recognized, let's try to get it using Reflection then
                                Type proxyTicketManagerType = Type.GetType(proxyTicketManagerProvider, false, true);
                                if (proxyTicketManagerType != null)
                                {
                                    if (typeof(IProxyTicketManager).IsAssignableFrom(proxyTicketManagerType))
                                        proxyTicketManager = (IProxyTicketManager)Activator.CreateInstance(proxyTicketManagerType);
                                    else
                                        LogAndThrowConfigurationException("Proxy Ticket Manager type is not correct " + proxyTicketManagerProvider);
                                }
                                else
                                    LogAndThrowConfigurationException("Could not find Proxy Ticket Manager type " + proxyTicketManagerProvider);
                            }
                            configLogger.Info("ProxyTicketManager type = " + proxyTicketManager.GetType().ToString());
                        }

                        // Validate configuration
                        bool haveServerName = !String.IsNullOrEmpty(serverName);
                        if (!haveServerName)
                        {
                            LogAndThrowConfigurationException(CasClientConfiguration.SERVER_NAME + " cannot be null or empty.");
                        }

                        if (String.IsNullOrEmpty(casServerLoginUrl))
                        {
                            LogAndThrowConfigurationException(CasClientConfiguration.CAS_SERVER_LOGIN_URL + " cannot be null or empty.");
                        }

                        if (serviceTicketManager == null && singleSignOut)
                        {
                            LogAndThrowConfigurationException("Single Sign Out support requires a ServiceTicketManager.");
                        }

                        if (gateway && renew)
                        {
                            LogAndThrowConfigurationException("Gateway and Renew functionalities are mutually exclusive");
                        }

                        if (!redirectAfterValidation)
                        {
                            LogAndThrowConfigurationException(
                                "Forms Authentication based modules require RedirectAfterValidation to be set to true.");
                        }

                        initialized = true;
                    }
                }

                if (ServiceTicketManager != null) ServiceTicketManager.Initialize();
                if (ProxyTicketManager != null) ProxyTicketManager.Initialize();
                if (TicketValidator != null) TicketValidator.Initialize();
            }
        }

        /// <summary>
        /// Obtain a Proxy ticket and redirect to the foreign service url with 
        /// that ticket included in the url.  The foreign service must be configured 
        /// to accept the ticket.
        /// </summary>
        /// <param name="url">The foreign service to redirect to</param>
        /// <exception cref="ArgumentNullException">The url supplied is null</exception>
        /// <exception cref="ArgumentException">The url supplied is empty</exception>
        public static void ProxyRedirect(string url)
        {
            ProxyRedirect(url, "ticket", false);
        }

        /// <summary>
        /// Obtain a Proxy ticket and redirect to the foreign service url with 
        /// that ticket included in the url.  The foreign service must be configured 
        /// to accept the ticket.
        /// </summary>
        /// <param name="url">The foreign service to redirect to</param>
        /// <param name="endResponse">
        /// Boolean indicating whether or not to short circuit the remaining request 
        /// pipeline events
        /// </param>
        /// <exception cref="ArgumentNullException">The url supplied is null</exception>
        /// <exception cref="ArgumentException">The url supplied is empty</exception>
        public static void ProxyRedirect(string url, bool endResponse)
        {
            ProxyRedirect(url, "ticket", endResponse);   
        }

        /// <summary>
        /// Obtain a Proxy ticket and redirect to the foreign service url with 
        /// that ticket included in the url.  The foreign service must be configured 
        /// to accept the ticket.
        /// </summary>
        /// <param name="url">The foreign service to redirect to</param>
        /// <param name="proxyTicketUrlParameter">
        /// The ticket parameter to include in the remote service Url.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// The url or proxyTicketUrlParameter supplied is null
        /// </exception>
        /// <exception cref="ArgumentException">
        /// The url or proxyTicketUrlParametersupplied is empty
        /// </exception>
        public static void ProxyRedirect(string url, string proxyTicketUrlParameter)
        {
            ProxyRedirect(url, proxyTicketUrlParameter, false);
        }

        /// <summary>
        /// </summary>
        /// <param name="url">The foreign service to redirect to</param>
        /// <param name="proxyTicketUrlParameter">
        /// The ticket parameter to include in the remote service Url.
        /// </param>
        /// <param name="endResponse">
        /// Boolean indicating whether or not to short circuit the remaining request 
        /// pipeline events
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// The url or proxyTicketUrlParameter supplied is null
        /// </exception>
        /// <exception cref="ArgumentException">
        /// The url or proxyTicketUrlParametersupplied is empty
        /// </exception>
        public static void ProxyRedirect(string url, string proxyTicketUrlParameter, bool endResponse)
        {
            CommonUtils.AssertNotNullOrEmpty(url, "url parameter cannot be null or empty.");
            CommonUtils.AssertNotNull(proxyTicketUrlParameter, "proxyTicketUrlParameter parameter cannot be null or empty.");

            HttpContext context = HttpContext.Current;
            HttpResponse response = context.Response;

            string proxyRedirectUrl = UrlUtil.GetProxyRedirectUrl(url, proxyTicketUrlParameter);
            response.Redirect(proxyRedirectUrl, endResponse);
        }

        /// <summary>
        /// Attempts to connect to the CAS server to retrieve a proxy ticket 
        /// for the target URL specified.
        /// </summary>
        /// <remarks>
        /// Problems retrieving proxy tickets are generally caused by SSL misconfiguration.
        /// The CAS server must be configured to trust the SSL certificate on the web application's 
        /// server.  The CAS server will attempt to establish an SSL connection to this web 
        /// application server to confirm that the proxy ticket request is legitimate.  If the 
        /// server does not trust the SSL certificate or the certificate authority/chain of the SSL
        /// certificate, the request will fail.
        /// </remarks>
        /// <param name="targetServiceUrl">The target Url to obtain a proxy ticket for</param>
        /// <returns>
        /// A proxy ticket for the target Url or an empty string if the request failed.
        /// </returns>
        public static string GetProxyTicketIdFor(string targetServiceUrl)
        {
            CommonUtils.AssertNotNullOrEmpty(targetServiceUrl, "targetServiceUrl parameter cannot be null or empty.");

            if (ServiceTicketManager == null)
            {
                LogAndThrowConfigurationException("Proxy authentication requires a ServiceTicketManager");
            }

            FormsAuthenticationTicket formsAuthTicket = GetFormsAuthenticationTicket();

            if (formsAuthTicket == null)
            {
                LogAndThrowOperationException("The request is not authenticated (does not have a CAS Service or Proxy ticket).");
            }

            if (string.IsNullOrEmpty(formsAuthTicket.UserData))
            {
                LogAndThrowOperationException("The request does not have a CAS Service Ticket.");
            }

            CasAuthenticationTicket casTicket = ServiceTicketManager.GetTicket(formsAuthTicket.UserData);

            if (casTicket == null)
            {
                LogAndThrowOperationException("The request does not have a valid CAS Service Ticket.");
            }
            
            string proxyTicketResponse = null;
            try
            {
                string proxyUrl = UrlUtil.ConstructProxyTicketRequestUrl(casTicket.ProxyGrantingTicket, targetServiceUrl);
                proxyTicketResponse = HttpUtil.PerformHttpGet(proxyUrl, true);
            }
            catch
            {
                LogAndThrowOperationException("Unable to obtain CAS Proxy Ticket.");
            }

            if (String.IsNullOrEmpty(proxyTicketResponse))
            {
                LogAndThrowOperationException("Unable to obtain CAS Proxy Ticket (response was empty)");
            }

            string proxyTicket = null;
            try
            {
                ServiceResponse serviceResponse = ServiceResponse.ParseResponse(proxyTicketResponse);
                if (serviceResponse.IsProxySuccess)
                {
                    ProxySuccess success = (ProxySuccess)serviceResponse.Item;
                    if (!String.IsNullOrEmpty(success.ProxyTicket))
                    {
                        protoLogger.Info(String.Format("Proxy success: {0}", success.ProxyTicket));
                    }
                    proxyTicket = success.ProxyTicket;
                }
                else
                {
                    ProxyFailure failure = (ProxyFailure)serviceResponse.Item;
                    if (!String.IsNullOrEmpty(failure.Message) && !String.IsNullOrEmpty(failure.Code))
                    {
                       protoLogger.Info(String.Format("Proxy failure: {0} ({1})", failure.Message, failure.Code));
                    }
                    else if (!String.IsNullOrEmpty(failure.Message))
                    {
                        protoLogger.Info(String.Format("Proxy failure: {0}", failure.Message));
                    }
                    else if (!String.IsNullOrEmpty(failure.Code))
                    {
                        protoLogger.Info(String.Format("Proxy failure: Code {0}", failure.Code));
                    }
                }
            }
            catch (InvalidOperationException)
            {
                LogAndThrowOperationException("CAS Server response does not conform to CAS 2.0 schema");
            }
            return proxyTicket;
        }

        /// <summary>
        /// Redirects the current request to the CAS Login page
        /// </summary>
        public static void RedirectToLoginPage()
        {
            RedirectToLoginPage(Renew);
        }

        /// <summary>
        /// Redirects the current request to the Login page and requires renewed
        /// CAS credentials
        /// </summary>
        public static void RedirectToLoginPage(bool forceRenew)
        {
            Initialize();

            HttpContext context = HttpContext.Current;
            HttpResponse response = context.Response;

            string redirectUrl = UrlUtil.ConstructLoginRedirectUrl(false, forceRenew);
            protoLogger.Info("Redirecting to " + redirectUrl);
            response.Redirect(redirectUrl, false);
        }

        /// <summary>
        /// Redirects the current request to the Cookies Required page
        /// </summary>
        public static void RedirectToCookiesRequiredPage()
        {
            Initialize();

            HttpContext context = HttpContext.Current;
            HttpResponse response = context.Response;

            response.Redirect(UrlUtil.ResolveUrl(CookiesRequiredUrl), false);
        }

        /// <summary>
        /// Redirects the current request to the Not Authorized page
        /// </summary>
        public static void RedirectToNotAuthorizedPage()
        {
            Initialize();

            HttpContext context = HttpContext.Current;
            HttpResponse response = context.Response;

            response.Redirect(UrlUtil.ResolveUrl(NotAuthorizedUrl), false);
        }

        /// <summary>
        /// Redirects the current request back to the requested page without
        /// the CAS ticket artifact in the URL.
        /// </summary>
        internal static void RedirectFromLoginCallback()
        {
            Initialize();

            HttpContext context = HttpContext.Current;
            HttpRequest request = context.Request;
            HttpResponse response = context.Response;

            if (RequestEvaluator.GetRequestHasGatewayParameter())
            {
                // TODO: Only set Success if request is authenticated?  Otherwise Failure.  
                // Doesn't make a difference from a security perspective, but may be clearer for users
                SetGatewayStatusCookie(GatewayStatus.Success);
            }

            response.Redirect(UrlUtil.RemoveCasArtifactsFromUrl(request.Url.AbsoluteUri), false);
        }

        /// <summary>
        /// Redirects the current request back to the requested page without
        /// the gateway callback artifact in the URL.
        /// </summary>
        internal static void RedirectFromFailedGatewayCallback()
        {
            Initialize();

            HttpContext context = HttpContext.Current;
            HttpRequest request = context.Request;
            HttpResponse response = context.Response;
            
            SetGatewayStatusCookie(GatewayStatus.Failed);

            string urlWithoutCasArtifact = UrlUtil.RemoveCasArtifactsFromUrl(request.Url.AbsoluteUri);
            response.Redirect(urlWithoutCasArtifact, false);
        }

        /// <summary>
        /// Attempt to perform a CAS gateway authentication.  This causes a transparent
        /// redirection out to the CAS server and back to the requesting page with or 
        /// without a CAS service ticket.  If the user has already authenticated for 
        /// another service against the CAS server and the CAS server supports Single 
        /// Sign On, this will result in the user being automatically authenticated.
        /// Otherwise, the user will remain anonymous.
        /// </summary>
        /// <param name="ignoreGatewayStatusCookie">
        /// The Gateway Status Cookie reflects whether a gateway authentication has 
        /// already been attempted, in which case the redirection is generally 
        /// unnecessary.  This property allows you to override the behavior and 
        /// perform a redirection regardless of whether it has already been attempted.
        /// </param>
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
            protoLogger.Info("Performing gateway redirect to " + redirectUrl);
            response.Redirect(redirectUrl, false);
            application.CompleteRequest();
        }

        /// <summary>
        /// Logs the user out of the application and attempts to perform a Single Sign 
        /// Out against the CAS server.  If the CAS server is configured to support 
        /// Single Sign Out, this will prevent users from gateway authenticating 
        /// to other services.  The CAS server will attempt to notify any other 
        /// applications to revoke the session.  Each of the applications must be 
        /// configured to maintain session state on the server.  In the case of 
        /// ASP.NET web applications using DotNetCasClient, this requires defining a 
        /// serviceTicketManager.  The configuration for other client types (Java, 
        /// PHP) varies based on the client implementation.  Consult the Apereo wiki
        /// for more details.
        /// </summary>
        public static void SingleSignOut()
        {
            Initialize();

            HttpContext context = HttpContext.Current;
            HttpResponse response = context.Response;


            // Necessary for ASP.NET MVC Support.
            if (context.User.Identity.IsAuthenticated)
            {
                ClearAuthCookie();
                string singleSignOutRedirectUrl = UrlUtil.ConstructSingleSignOutRedirectUrl();
                
                // Leave endResponse as true.  This will throw a handled ThreadAbortException
                // but it is necessary to support SingleSignOut in ASP.NET MVC applications.
                response.Redirect(singleSignOutRedirectUrl, true);
            }
        }

        /// <summary>
        /// Process SingleSignOut requests originating from another web application by removing the ticket 
        /// from the ServiceTicketManager (assuming one is configured).  Without a ServiceTicketManager
        /// configured, this method will not execute and this web application cannot respect external 
        /// SingleSignOut requests.
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

            protoLogger.Debug("Examining request for single sign-out signature");

            if (request.HttpMethod == "POST" && request.Form["logoutRequest"] != null)
            {
                protoLogger.Debug("Attempting to get CAS service ticket from request");
                // TODO: Should we be checking to make sure that this special POST is coming from a trusted source?
                //       It would be tricky to do this by IP address because there might be a white list or something.
                
                string casTicket = ExtractSingleSignOutTicketFromSamlResponse(request.Params["logoutRequest"]);
                if (!String.IsNullOrEmpty(casTicket))
                {
                    protoLogger.Info("Processing single sign-out request for " + casTicket);
                    ServiceTicketManager.RevokeTicket(casTicket);
                    protoLogger.Debug("Successfully removed " + casTicket);

                    response.StatusCode = 200;
                    response.ContentType = "text/plain";
                    response.Clear();
                    response.Write("OK");

                    context.ApplicationInstance.CompleteRequest();
                }
            }
        }

        /// <summary>
        /// Process a Proxy Callback request from the CAS server.  Proxy Callback requests occur as a part
        /// of a proxy ticket request.  When the web application requests a proxy ticket for a third party
        /// service from the CAS server, the CAS server attempts to connect back to the web application 
        /// over an HTTPS connection.  The success of this callback is essential for the proxy ticket 
        /// request to succeed.  Failures are generally caused by SSL configuration errors.  See the 
        /// description of the SingleSignOut method for more details.  Assuming the SSL configuration is 
        /// correct, this method is responsible for handling the callback from the CAS server.  For 
        /// more details, see the CAS protocol specification.
        /// </summary>
        /// <returns>
        /// A Boolean indicating whether or not the proxy callback request is valid and mapped to a valid,
        /// outstanding Proxy Granting Ticket IOU.
        /// </returns>
        internal static bool ProcessProxyCallbackRequest()
        {
            HttpContext context = HttpContext.Current;
            HttpApplication application = context.ApplicationInstance;
            HttpRequest request = context.Request;
            HttpResponse response = context.Response;

            string proxyGrantingTicketIou = request.Params[PARAM_PROXY_GRANTING_TICKET_IOU];
            string proxyGrantingTicket = request.Params[PARAM_PROXY_GRANTING_TICKET];
            if (String.IsNullOrEmpty(proxyGrantingTicket))
            {
                protoLogger.Info("Invalid request - {0} parameter not found", PARAM_PROXY_GRANTING_TICKET);
                return false;
            }
            else if (String.IsNullOrEmpty(proxyGrantingTicketIou))
            {
                protoLogger.Info("Invalid request - {0} parameter not found", PARAM_PROXY_GRANTING_TICKET_IOU);
                return false;
            }

            protoLogger.Info("Recieved proxyGrantingTicketId [{0}] for proxyGrantingTicketIou [{1}]", proxyGrantingTicket, proxyGrantingTicketIou);

            ProxyTicketManager.InsertProxyGrantingTicketMapping(proxyGrantingTicketIou, proxyGrantingTicket);

            // TODO: Consider creating a DotNetCasClient.Validation.Schema.Cas20.ProxySuccess object and serializing it.

            response.Write("<?xml version=\"1.0\"?>");
            response.Write("<casClient:proxySuccess xmlns:casClient=\"http://www.yale.edu/tp/casClient\" />");
            application.CompleteRequest();

            return true;
        }
        
        /// <summary>
        /// Validates a ticket contained in the URL, presumably generated by
        /// the CAS server after a successful authentication.  The actual ticket
        /// validation is performed by the configured TicketValidator 
        /// (i.e., CAS 1.0, CAS 2.0, SAML 1.0).  If the validation succeeds, the
        /// request is authenticated and a FormsAuthenticationCookie and 
        /// corresponding CasAuthenticationTicket are created for the purpose of 
        /// authenticating subsequent requests (see ProcessTicketValidation 
        /// method).  If the validation fails, the authentication status remains 
        /// unchanged (generally the user is and remains anonymous).
        /// </summary>
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
                principal = TicketValidator.Validate(ticket);

                // Save the ticket in the FormsAuthTicket.  Encrypt the ticket and send it as a cookie. 
                casTicket = new CasAuthenticationTicket(
                    ticket,
                    UrlUtil.RemoveCasArtifactsFromUrl(request.Url.AbsoluteUri),
                    request.UserHostAddress,
                    principal.Assertion
                );

                if (ProxyTicketManager != null && !string.IsNullOrEmpty(principal.ProxyGrantingTicket))
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
            catch (TicketValidationException e)
            {
                // Leave principal null.  This might not have been a CAS service ticket.
                protoLogger.Error("Ticket validation error: " + e);
            }
        }

        /// <summary>
        /// Attempts to authenticate requests subsequent to the initial authentication
        /// request (handled by ProcessTicketValidation).  This method looks for a 
        /// FormsAuthenticationCookie containing a FormsAuthenticationTicket and attempts
        /// to confirms its validitiy.  It either contains the CAS service ticket or a 
        /// reference to a CasAuthenticationTicket stored in the ServiceTicketManager 
        /// (if configured).  If it succeeds, the context.User and Thread.CurrentPrincipal 
        /// are set with a ICasPrincipal and the current request is considered 
        /// authenticated.  Otherwise, the current request is effectively anonymous.
        /// </summary>
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

                            securityLogger.Warn("CasAuthenticationTicket failed verification: " + casTicket);

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
                        securityLogger.Debug("Revoking ticket " + serviceTicket);
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
                currentPrincipal = principal;

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
            Initialize();

            protoLogger.Debug("Creating FormsAuthenticationTicket for " + serviceTicket);

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
        /// Extracts the CAS ticket from the SAML message supplied.
        /// </summary>
        /// <param name="xmlAsString">SAML message from CAS server</param>
        /// <returns>The CAS ticket contained in SAML message</returns>
        private static string ExtractSingleSignOutTicketFromSamlResponse(string xmlAsString)
        {
            XmlParserContext xmlParserContext = new XmlParserContext(null, xmlNamespaceManager, null, XmlSpace.None);

            string elementText = null;
            if (!String.IsNullOrEmpty(xmlAsString) && !String.IsNullOrEmpty(XML_SESSION_INDEX_ELEMENT_NAME))
            {
                using (TextReader textReader = new StringReader(xmlAsString))
                {
                    XmlReader reader = XmlReader.Create(textReader, xmlReaderSettings, xmlParserContext);
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

        private static void LogAndThrowConfigurationException(string message)
        {
            configLogger.Error(message);
            throw new CasConfigurationException(message);
        }

        private static void LogAndThrowOperationException(string message)
        {
            protoLogger.Error(message);
            throw new InvalidOperationException(message);
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
                return ticketValidatorName;
            }
        }

        /// <summary>
        /// An instance of the TicketValidator specified in the 
        /// TicketValidatorName property.  This will either be an instance of 
        /// a Cas10TicketValidator, Cas20TicketValidator, or 
        /// Saml11TicketValidator.
        /// </summary>
		internal static ITicketValidator TicketValidator
        {
            get
            {
                Initialize();
                return ticketValidator;
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
                return serviceTicketManagerProvider;
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
                return serviceTicketManager;
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
                return proxyTicketManagerProvider;
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
                return proxyTicketManager;
            }
        }

        /// <summary>
        /// Enable CAS gateway feature, see https://apereo.github.io/cas/5.1.x/protocol/CAS-Protocol-Specification.html section 2.1.1.
        /// Default is false.
        /// </summary>
        public static bool Gateway
        {
            get
            {
                Initialize();
                return gateway;
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
                return gatewayStatusCookieName;
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
                return formsLoginUrl;
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
                return formsTimeout;
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
                return casServerLoginUrl;
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
                return casServerUrlPrefix;
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
                return ticketTimeTolerance;
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
                return serverName;
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
                return renew;
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
                return redirectAfterValidation;
            }
        }

        /// <summary>
        /// Specifies whether external single sign out requests should be processed.
        /// </summary>
        public static bool ProcessIncomingSingleSignOutRequests
        {
            get
            {
                Initialize();
                return singleSignOut;
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
                return notAuthorizedUrl;
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
                return cookiesRequiredUrl;
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
                return gatewayParameterName;
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
                return proxyCallbackParameterName;
            }
        }

        /// <summary>
        /// URL for CAS Proxy callback
        /// </summary>
        public static String CasProxyCallbackUrl
        {
            get
            {
                Initialize();
                return casProxyCallbackUrl;
            }
        }

        /// <summary>
        /// Specifies whether to require CAS for requests that have null/empty content-types
        /// </summary>
        public static bool RequireCasForMissingContentTypes
        {
            get
            {
                Initialize();
                return requireCasForMissingContentTypes;
            }
        }

        /// <summary>
        /// Content-types for which CAS authentication will be required
        /// </summary>
        public static string[] RequireCasForContentTypes
        {
            get
            {
                Initialize();
                return requireCasForContentTypes;
            }
        }

        /// <summary>
        /// Handlers for which CAS authentication will be bypassed.
        /// </summary>
        public static string[] BypassCasForHandlers
        {
            get
            {
                Initialize();
                return bypassCasForHandlers;
            }
        }

#endregion
    }
}