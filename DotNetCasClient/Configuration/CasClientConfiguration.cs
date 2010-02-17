using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;

namespace DotNetCasClient.Configuration
{
  /// <summary>
  /// CAS specific ConfigurationSection for Web.config.
  /// </summary>
  class CasClientConfiguration : ConfigurationSection
  {
    public static CasClientConfiguration Config
    {
      get
      {
        return ConfigurationManager.GetSection("casClientConfig")
          as CasClientConfiguration;
      }
    }
            
    /// <summary>
    /// Required Properties
    /// </summary>
    public const string CAS_SERVER_LOGIN_URL = "casServerLoginUrl";
    public const string CAS_SERVER_URL_PREFIX = "casServerUrlPrefix";
    public const string TICKET_VALIDATOR_NAME = "ticketValidatorName";

    /// <summary>
    /// One of these Properties must be set. If both are set, service takes
    /// precedence.
    /// </summary>
    public const string SERVER_NAME = "serverName";
    public const string SERVICE = "service";

    /// <summary>
    /// Optional Properties
    /// </summary>
    public const string RENEW = "renew";
    public const string GATEWAY = "gateway";
    public const string ARTIFACT_PARAMETER_NAME = "artifactParameterName";
    public const string SERVICE_PARAMETER_NAME = "serviceParameterName";
    public const string ARTIFACT_PARAMETER_NAME_VALIDATION = "artifactParameterNameValidation";
    public const string SERVICE_PARAMETER_NAME_VALIDATION = "serviceParameterNameValidation";
    public const string REDIRECT_AFTER_VALIDATION = "redirectAfterValidation";
    public const string ENCODE_SERVICE_URL = "encodeServiceUrl";
    public const string SECURE_URI_REGEX_STRING = "secureUriRegex";
    public const string SECURE_URI_EXCEPTION_REGEX_STRING = "secureUriExceptionRegex";
    public const string USE_SESSION = "useSession";
    public const string TICKET_TIME_TOLERANCE = "ticketTimeTolerance";
    public const string SINGLE_SIGN_OUT = "singleSignOut";
    public const string PROXY_GRANTING_TICKET_RECEPTOR = "proxyGrantingTicketReceptor";
    public const string PROXY_CALLBACK_URL = "proxyCallbackUrl";
    public const string PROXY_RECEPTOR_URL = "proxyReceptorUrl";
    public const string FORMS_AUTHENTICATION_STATE_PROVIDER = "formsAuthenticationStateProvider";
    public const string NOT_AUTHORIZED_URL = "notAuthorizedUrl";
  
    /// <summary>
    /// Names for the supported ticket validators
    /// </summary>
    public const string CAS10_TICKET_VALIDATOR_NAME = "Cas10";
    public const string CAS20_TICKET_VALIDATOR_NAME = "Cas20";
    public const string SAML11_TICKET_VALIDATOR_NAME = "Saml11";

    /// <summary>
    /// Names for the supported Cache authentication state provider
    /// </summary>
    public const string CACHE_AUTHENTICATION_STATE_PROVIDER = "CacheAuthenticationStateProvider";

    /// <summary>
    /// Defines the exact CAS server login URL.
    /// e.g. https://cas.princeton.edu/cas/login
    /// </summary>
    [ConfigurationProperty(CAS_SERVER_LOGIN_URL, IsRequired=true)]
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
    [ConfigurationProperty(CAS_SERVER_URL_PREFIX, IsRequired=true)]
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
    /// Currently supported values: Cas10 / Cas20 / Saml11
    /// </remarks>
    /// </summary>
    [ConfigurationProperty(TICKET_VALIDATOR_NAME, IsRequired=true)]
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
    [ConfigurationProperty(TICKET_TIME_TOLERANCE, DefaultValue=1000L, IsRequired=false)]
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
    [ConfigurationProperty(SERVICE, IsRequired=false)]
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
    [ConfigurationProperty(SERVER_NAME, IsRequired=false)]
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
    [ConfigurationProperty(RENEW, DefaultValue=false, IsRequired=false)]
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
    [ConfigurationProperty(GATEWAY, DefaultValue=false, IsRequired=false)]
    public bool Gateway
    {
      get
      {
        return Convert.ToBoolean(this[GATEWAY]);
      }
    }

    /// <summary>
    /// Specifies the name of the request parameter whose value is the artifact (e.g. "ticket").
    /// </summary>
    [ConfigurationProperty(ARTIFACT_PARAMETER_NAME, IsRequired=false)]
    public string ArtifactParameterName
    {
      get
      {
        return this[ARTIFACT_PARAMETER_NAME] as string;
      }
    }

    /// <summary>
    /// Specifies the name of the request parameter whose value is the service (e.g. "service")
    /// </summary>
    [ConfigurationProperty(SERVICE_PARAMETER_NAME, IsRequired=false)]
    public string ServiceParameterName
    {
      get
      {
        return this[SERVICE_PARAMETER_NAME] as string;
      }
    }

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
    [ConfigurationProperty(SINGLE_SIGN_OUT, DefaultValue=true, IsRequired=false)]
    public bool SingleSignOut
    {
      get
      {
        return Convert.ToBoolean(this[SINGLE_SIGN_OUT]);
      }
    }

    /// <summary>
    /// Specifies whether proxy granting ticket receptor functionality should be enabled.
    /// </summary>
    [ConfigurationProperty(PROXY_GRANTING_TICKET_RECEPTOR, DefaultValue = true, IsRequired = false)]
    public bool ProxyGrantingTicketReceptor
    {
        get
        {
            return Convert.ToBoolean(this[PROXY_GRANTING_TICKET_RECEPTOR]);
        }
    }

    /// <summary>
    /// The callback URL provided to the CAS server for receiving Proxy Granting Tickets.
    /// e.g. https://www.example.edu/cas-client-app/proxyCallback
    /// </summary>
    [ConfigurationProperty(PROXY_CALLBACK_URL, DefaultValue = null, IsRequired = false)]
    public string ProxyCallbackUrl
    {
        get
        {
            return this[PROXY_CALLBACK_URL] as string;
        }
    }

    /// <summary>
    /// The URL to watch for PGTIOU/PGT responses from the CAS server. Should be defined from
    /// the root of the context. For example, if your application is deployed in /cas-client-app
    /// and you want the proxy receptor URL to be /cas-client-app/my/receptor you need to configure
    /// proxyReceptorUrl to be /my/receptor
    /// e.g. /proxyCallback
    /// </summary>
    [ConfigurationProperty(PROXY_RECEPTOR_URL, DefaultValue = null, IsRequired = false)]
    public string ProxyReceptorUrl
    {
        get
        {
            return this[PROXY_RECEPTOR_URL] as string;
        }
    }


    /// <summary>
    /// The ticket validator to use to validate tickets returned by the CAS server.
    /// <remarks>
    /// Currently supported values: CacheAuthenticationStateProvider
    /// </remarks>
    /// </summary>
    [ConfigurationProperty(FORMS_AUTHENTICATION_STATE_PROVIDER, IsRequired = false)]
    public string FormsAuthenticationStateProvider
    {
        get
        {
            return this[FORMS_AUTHENTICATION_STATE_PROVIDER] as string;
        }
    }


    [ConfigurationProperty(NOT_AUTHORIZED_URL, IsRequired = false)]
    public string NotAuthorizedUrl
    {
        get
        {
            return this[NOT_AUTHORIZED_URL] as string;
        }
    }


    /// <summary>
    /// Specifies whether authentication based on the presence of the CAS
    /// Assertion in the session is in effect, reducing the number of
    /// round-trips to the CAS server.
    /// </summary>
    [ConfigurationProperty(USE_SESSION, DefaultValue=true, IsRequired=false)]
    public bool UseSession
    {
      get
      {
        return Convert.ToBoolean(this[USE_SESSION]);
      }
    }

    /// <summary>
    /// Regex for selection of URIs which need to be protected by CAS.  Default value
    /// is a regular expression that matches everything, so that all URIs are protected
    /// by CAS.
    [ConfigurationProperty(SECURE_URI_REGEX_STRING, DefaultValue = ".*", IsRequired=false)]
    public String SecureUriRegex
    {
      get
      {
        return this[SECURE_URI_REGEX_STRING] as string;
      }
    }

    /// <summary>
    /// Regex for selection of URIs which match the SecureUriRegex but do
    /// not need to be protected by CAS.  This is needed for things such
    /// as the Ajax resource URIs.  Default value is a regular expression
    /// that matches nothing, so that no exceptions would occur.
    /// </summary>
    [ConfigurationProperty(SECURE_URI_EXCEPTION_REGEX_STRING, DefaultValue = "a^", IsRequired=false)]
    public String SecureUriExceptionRegex
    {
      get
      {
        return this[SECURE_URI_EXCEPTION_REGEX_STRING] as string;
      }
    }
  }
}