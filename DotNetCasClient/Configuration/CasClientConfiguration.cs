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
using System.Configuration;

namespace DotNetCasClient.Configuration
{
    /// <summary>
    /// CAS specific ConfigurationSection for Web.config.
    /// </summary>
    /// <author>Scott Holodak</author>
    /// <author>Marvin S. Addison</author>
    public class CasClientConfiguration : ConfigurationSection
    {
        #region Fields
        // Required Properties
        public const string CAS_SERVER_LOGIN_URL = "casServerLoginUrl";
        public const string CAS_SERVER_URL_PREFIX = "casServerUrlPrefix";
        public const string TICKET_VALIDATOR_NAME = "ticketValidatorName";

        // One of these Properties must be set. If both are set, service takes
        // precedence.
        public const string SERVER_NAME = "serverName";
        public const string SERVICE = "service";

        // Optional Properties
        public const string RENEW = "renew";
        public const string GATEWAY = "gateway";
        public const string GATEWAY_STATUS_COOKIE_NAME = "gatewayStatusCookieName";
        public const string ARTIFACT_PARAMETER_NAME = "artifactParameterName";
        public const string SERVICE_PARAMETER_NAME = "serviceParameterName";
        public const string REQUIRE_CAS_FOR_MISSING_CONTENT_TYPES_PARAMETER_NAME = "requireCasForMissingContentTypes";
        public const string REQUIRE_CAS_FOR_CONTENT_TYPES_PARAMETER_NAME = "requireCasForContentTypes";
        public const string BYPASS_CAS_FOR_HANDLERS_PARAMETER_NAME = "bypassCasForHandlers";
		public const string AUTHENTICATION_TYPE = "authenticationType";

        // NETC-20 - Not sure whether these attributes are relevant.
        // public const string ARTIFACT_PARAMETER_NAME_VALIDATION = "artifactParameterNameValidation";
        // public const string SERVICE_PARAMETER_NAME_VALIDATION = "serviceParameterNameValidation";
        
        public const string REDIRECT_AFTER_VALIDATION = "redirectAfterValidation";
        public const string ENCODE_SERVICE_URL = "encodeServiceUrl";
        public const string SECURE_URI_REGEX_STRING = "secureUriRegex";
        public const string SECURE_URI_EXCEPTION_REGEX_STRING = "secureUriExceptionRegex";
        public const string USE_SESSION = "useSession";
        public const string TICKET_TIME_TOLERANCE = "ticketTimeTolerance";
        public const string SINGLE_SIGN_OUT = "singleSignOut";
        public const string SERVICE_TICKET_MANAGER = "serviceTicketManager";
        public const string PROXY_TICKET_MANAGER = "proxyTicketManager";
        public const string NOT_AUTHORIZED_URL = "notAuthorizedUrl";
        public const string COOKIES_REQUIRED_URL = "cookiesRequiredUrl";
        public const string GATEWAY_PARAMETER_NAME = "gatewayParameterName";
        public const string PROXY_CALLBACK_PARAMETER_NAME = "proxyCallbackParameterName";
        public const string PROXY_CALLBACK_URL = "proxyCallbackUrl";

        // Names for the supported ticket validators
        public const string CAS10_TICKET_VALIDATOR_NAME = "Cas10";
        public const string CAS20_TICKET_VALIDATOR_NAME = "Cas20";
        public const string SAML11_TICKET_VALIDATOR_NAME = "Saml11";

        // Names for the supported Service Ticket state provider
        public const string CACHE_SERVICE_TICKET_MANAGER = "CacheServiceTicketManager";

        // Names for the supported Cache Ticket state provider
        public const string CACHE_PROXY_TICKET_MANAGER = "CacheProxyTicketManager";
        #endregion

        #region Properties
        /// <summary>
        /// The CasClientConfiguration configuration element defined 
        /// in web.config
        /// </summary>
        public static CasClientConfiguration Config
        {
            get
            {
                return ConfigurationManager.GetSection("casClientConfig") as CasClientConfiguration;
            }
        }

        /// <summary>
        /// Defines the exact CAS server login URL.
        /// e.g. https://cas.princeton.edu/cas/login
        /// </summary>
        [ConfigurationProperty(CAS_SERVER_LOGIN_URL, IsRequired = true)]
        public string CasServerLoginUrl
        {
            get
            {
                return this[CAS_SERVER_LOGIN_URL] as string;
            }
        }

        /// <summary>
        /// Defines the prefix for the CAS server. Should be everything up to the URL endpoint,
        /// including the /.
        /// e.g. http://cas.princeton.edu/
        /// </summary>
        [ConfigurationProperty(CAS_SERVER_URL_PREFIX, IsRequired = true)]
        public string CasServerUrlPrefix
        {
            get
            {
                return this[CAS_SERVER_URL_PREFIX] as string;
            }
        }

        /// <summary>
        /// The ticket validator to use to validate tickets returned by the CAS server.
        /// <remarks>
        /// Currently supported values: Cas10 / Cas20 / Saml11 or any fully qualified type which extends AbstractCasProtocolTicketValidator
        /// </remarks>
        /// </summary>
        [ConfigurationProperty(TICKET_VALIDATOR_NAME, IsRequired = true)]
        public string TicketValidatorName
        {
            get
            {
                return this[TICKET_VALIDATOR_NAME] as string;
            }
        }

        /// <summary>
        /// Tolerance milliseconds for checking the current time against the SAML Assertion
        /// valid times.
        /// </summary>
        [ConfigurationProperty(TICKET_TIME_TOLERANCE, DefaultValue = 30000L, IsRequired = false)]
        public long TicketTimeTolerance
        {
            get
            {
                return Convert.ToInt64(this[TICKET_TIME_TOLERANCE]);
            }
        }

        /// <summary>
        /// The Service URL to send to the CAS server.
        /// e.g. https://app.princeton.edu/example/
        /// </summary>
        [ConfigurationProperty(SERVICE, IsRequired = false)]
        public string Service
        {
            get
            {
                return this[SERVICE] as string;
            }
        }

        /// <summary>
        /// The server name of the server hosting the client application.  Service URL
        /// will be dynamically constructed using this value if Service is not specified.
        /// e.g. https://app.princeton.edu/
        /// </summary>
        [ConfigurationProperty(SERVER_NAME, IsRequired = false)]
        public string ServerName
        {
            get
            {
                return this[SERVER_NAME] as string;
            }
        }

        /// <summary>
        /// Specifies whether renew=true should be sent to URL's directed to the
        /// CAS server.
        /// </summary>
        [ConfigurationProperty(RENEW, DefaultValue = false, IsRequired = false)]
        public bool Renew
        {
            get
            {
                return Convert.ToBoolean(this[RENEW]);
            }
        }

        /// <summary>
        /// Specifies whether or not to redirect to the CAS server logon for a gateway request.
        /// </summary>
        [ConfigurationProperty(GATEWAY, DefaultValue = false, IsRequired = false)]
        public bool Gateway
        {
            get
            {
                return Convert.ToBoolean(this[GATEWAY]);
            }
        }

        /// <summary>
        /// The name of the cookie used to store the Gateway status (NotAttempted, 
        /// Success, Failed).  This cookie is used to prevent the client from 
        /// attempting to gateway authenticate every request.
        /// </summary>
        [ConfigurationProperty(GATEWAY_STATUS_COOKIE_NAME, IsRequired = false, DefaultValue = "cas_gateway_status")]
        public string GatewayStatusCookieName
        {
            get
            {
                return this[GATEWAY_STATUS_COOKIE_NAME] as string;
            }
        }

        /// <summary>
        /// Specifies the name of the request parameter whose value is the artifact (e.g. "ticket").
        /// </summary>
        [ConfigurationProperty(ARTIFACT_PARAMETER_NAME, IsRequired = false, DefaultValue = "ticket")]
        public string ArtifactParameterName
        {
            get
            {
                return this[ARTIFACT_PARAMETER_NAME] as string ?? "ticket";
            }
        }

        /// <summary>
        /// Specifies the name of the request parameter whose value is the service (e.g. "service")
        /// </summary>
        [ConfigurationProperty(SERVICE_PARAMETER_NAME, IsRequired = false, DefaultValue = "service")]
        public string ServiceParameterName
        {
            get
            {
                return this[SERVICE_PARAMETER_NAME] as string ?? "service";
            }
        }

        /// <summary>
        /// Specifies whether to require CAS for requests that have null/empty content-types
        /// </summary>
        [ConfigurationProperty(REQUIRE_CAS_FOR_MISSING_CONTENT_TYPES_PARAMETER_NAME, IsRequired = false, DefaultValue = true)]
        public bool RequireCasForMissingContentTypes
        {
            get
            {
                return Convert.ToBoolean(this[REQUIRE_CAS_FOR_MISSING_CONTENT_TYPES_PARAMETER_NAME]);
            }
        }

        /// <summary>
        /// Content-types for which CAS authentication will be required
        /// </summary>
        [ConfigurationProperty(REQUIRE_CAS_FOR_CONTENT_TYPES_PARAMETER_NAME, IsRequired = false, DefaultValue = new[] { "text/plain", "text/html" })]
        public string[] RequireCasForContentTypes { 
            get
            {
                string[] types = ((this[REQUIRE_CAS_FOR_CONTENT_TYPES_PARAMETER_NAME] as string) ?? "text/plain,text/html").Split(',');
                for (int i = 0; i < types.Length; i++)
                {
                    string type = types[i];
                    if (type.StartsWith(" ") || type.EndsWith(" "))
                    {
                        types[i] = type.Trim();
                    }
                }
                return types;
            }
        }

        /// <summary>
        /// Handlers for which CAS authentication will be bypassed.
        /// </summary>
        [ConfigurationProperty(BYPASS_CAS_FOR_HANDLERS_PARAMETER_NAME, IsRequired = false, DefaultValue = new[] { "trace.axd", "webresource.axd" })]
        public string[] BypassCasForHandlers
        {
            get
            {
                string[] types = ((this[REQUIRE_CAS_FOR_CONTENT_TYPES_PARAMETER_NAME] as string) ?? "trace.axd,webresource.axd").Split(',');
                for (int i = 0; i < types.Length; i++)
                {
                    string type = types[i];
                    if (type.StartsWith(" ") || type.EndsWith(" "))
                    {
                        types[i] = type.Trim();
                    }
                }
                return types;
            }
        }

        // public const string REQUIRE_CAS_FOR_CONTENT_TYPES_PARAMETER_NAME = "requireCasForContentTypes";
        // public const string BYPASS_CAS_FOR_HANDLERS_PARAMETER_NAME = "bypassCasForHandlers";

        /// <summary>
        /// Whether to redirect to the same URL after ticket validation, but without the ticket
        /// in the parameter.
        /// </summary>
        [ConfigurationProperty(REDIRECT_AFTER_VALIDATION, DefaultValue = false, IsRequired = false)]
        public bool RedirectAfterValidation
        {
            get
            {
                return Convert.ToBoolean(this[REDIRECT_AFTER_VALIDATION]);
            }
        }

        /// <summary>
        /// Whether to encode the session ID into the Service URL.
        /// </summary>
        [ConfigurationProperty(ENCODE_SERVICE_URL, DefaultValue = false, IsRequired = false)]
        public bool EncodeServiceUrl
        {
            get
            {
                return Convert.ToBoolean(this[ENCODE_SERVICE_URL]);
            }
        }

        /// <summary>
        /// Specifies whether single sign out functionality should be enabled.
        /// </summary>
        [ConfigurationProperty(SINGLE_SIGN_OUT, DefaultValue = true, IsRequired = false)]
        public bool SingleSignOut
        {
            get
            {
                return Convert.ToBoolean(this[SINGLE_SIGN_OUT]);
            }
        }

        /// <summary>
        /// The service ticket manager to use to store tickets returned by the 
        /// CAS server for validation, revocation, and single sign out support.
        /// <remarks>
        /// Currently supported values: A fully qualified type name supporting IServiceTicketManager or the short name of a type in DotNetCasClient.State
        /// </remarks>
        /// </summary>
        [ConfigurationProperty(SERVICE_TICKET_MANAGER, IsRequired = false)]
        public string ServiceTicketManager
        {
            get
            {
                return this[SERVICE_TICKET_MANAGER] as string;
            }
        }

        /// <summary>
        /// The proxy ticket manager to use to store and resolve 
        /// ProxyGrantingTicket IOUs to ProxyGrantingTickets
        /// <remarks>
        /// Currently supported values: A fully qualified type name supporting IProxyTicketManager or the short name of a type in DotNetCasClient.State
        /// </remarks>
        /// </summary>
        [ConfigurationProperty(PROXY_TICKET_MANAGER, IsRequired = false)]
        public string ProxyTicketManager
        {
            get
            {
                return this[PROXY_TICKET_MANAGER] as string;
            }
        }

        /// <summary>
        /// URL to redirect to when the request has a validated and verified 
        /// CAS Authentication Ticket, but the identity associated with that 
        /// ticket is not authorized to access the requested resource.  If this 
        /// option is omitted, the request will be redirected to the CAS server
        /// for alternate credentials (with the 'renew' argument set). 
        /// </summary>
        [ConfigurationProperty(NOT_AUTHORIZED_URL, IsRequired = false)]
        public string NotAuthorizedUrl
        {
            get
            {
                return this[NOT_AUTHORIZED_URL] as string;
            }
        }

        /// <summary>
        /// The URL to redirect to when the client is not accepting session 
        /// cookies.  This condition is detected only when gateway is enabled.  
        /// This will lock the users onto a specific page.  Otherwise, every 
        /// request will cause a silent round-trip to the CAS server, adding 
        /// a parameter to the URL.
        /// </summary>
        [ConfigurationProperty(COOKIES_REQUIRED_URL, IsRequired = false, DefaultValue = null)]
        public string CookiesRequiredUrl
        {
            get
            {
                return this[COOKIES_REQUIRED_URL] as string;
            }
        }

        /// <summary>
        /// The URL parameter to append to outbound CAS request's ServiceName 
        /// when initiating an automatic CAS Gateway request.  This parameter 
        /// plays a role in detecting whether or not the client has cookies 
        /// enabled.  The default value is 'gatewayResponse' and only needs to 
        /// be explicitly defined if that URL parameter has a meaning elsewhere
        /// in your application.  If you choose not define the CookiesRequiredUrl,
        /// you can detect that session cookies are not enabled in your application
        /// by testing for this parameter in the Request.QueryString having the 
        /// value 'true'.
        /// </summary>
        [ConfigurationProperty(GATEWAY_PARAMETER_NAME, IsRequired = false, DefaultValue = "gatewayResponse")]
        public string GatewayParameterName
        {
            get
            {
                return this[GATEWAY_PARAMETER_NAME] as string;
            }
        }

        /// <summary>
        /// The URL parameter to append to outbound CAS proxy request's pgtUrl
        /// when initiating an proxy ticket service validation.  This is used
        /// to determine whether the request is originating from the CAS server
        /// and contains a pgtIou.
        /// </summary>
        [ConfigurationProperty(PROXY_CALLBACK_PARAMETER_NAME, IsRequired = false, DefaultValue = "proxyResponse")]
        public string ProxyCallbackParameterName
        {
            get
            {
                return this[PROXY_CALLBACK_PARAMETER_NAME] as string;
            }
        }

        /// <summary>
        /// Defines the exact proxy call back url
        /// </summary>
        [ConfigurationProperty(PROXY_CALLBACK_URL, IsRequired = false)]
        public string ProxyCallbackUrl
        {
            get
            {
                return this[PROXY_CALLBACK_URL] as string;
            }
        }

		/// <summary>
        /// Sets the AuthenticationType for IIdentity
        /// </summary>
        [ConfigurationProperty(AUTHENTICATION_TYPE, IsRequired = false, DefaultValue = "Apereo CAS")]
        public string AuthenticationType
        {
	    	get
			{
				return this[AUTHENTICATION_TYPE] as string ?? "Apereo CAS";
			}
	    }
        #endregion
    }
}