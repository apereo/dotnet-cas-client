using System;
using System.Configuration;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Configuration;
using log4net;
using DotNetCasClient.Authentication;
using DotNetCasClient.Configuration;
using DotNetCasClient.Session;
using DotNetCasClient.Validation;

namespace DotNetCasClient.Utils
{
  // <summary>
  /// Base class for all CAS HttpModules
  /// </summary>
  /// <author>Catherine Winfrey</author>
  public abstract class AbstractCasModule : IHttpModule
  {
    /// <summary>
    /// Access to the log file
    /// </summary>
    protected static readonly ILog log = 
      LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
   
    #region Properties
    /// <summary>
    /// The name of the request parameter whose value is the artifact (e.g. "ticket").
    /// </summary>
    protected string ArtifactParameterName { get; private set; }
    
    /// <summary>
    /// The name of the request parameter whose value is the service (e.g. "service")
    /// </summary>
    protected string ServiceParameterName { get; private set; }

    /// <summary>
    /// Whether to encode the session ID into the Service URL.
    /// constructed.
    /// </summary>
    protected bool EncodeServiceUrl { get; private set; }

    /// <summary>
    /// The server name of the server hosting the client application.  Service URL
    /// will be dynamically constructed using this value if Service is not specified.
    /// e.g. https://app.princeton.edu/
    /// </summary>
    protected string ServerName { get; private set; }
    
    /// <summary>
    /// The exact url of the service.
    protected string Service { get; private set; }

    /// <summary>
    /// The service URL that will be used if a Service value is
    /// configured.
    /// </summary>
    protected string DefaultServiceUrl { get; private set; }

    /// <summary>
    /// The exact CAS server login URL.
    /// </summary>
    protected string CasServerLoginUrl { get; private set; }

    /// <summary>
    /// Whether or not to redirect to the CAS server logon for a gateway request.
    /// </summary>
    protected bool Gateway { get; private set; }

    /// <summary>
    /// Whether to redirect to the same URL after ticket validation, but without the ticket
    /// in the parameter.
    /// </summary>
    protected bool RedirectAfterValidation { get; private set; }

    /// <summary>
    /// Whether renew=true should be sent to the CAS server.
    /// </summary>
    protected bool Renew { get; private set; }

    /// <summary>
    /// Specifies whether single sign out functionality should be enabled.
    /// </summary>
    protected bool SingleSignOut { get; private set; }
    #endregion

    #region Fields
    /// <summary>
    /// The IGatewayResolver to be used for gateway maintenance
    /// </summary>
    internal IGatewayResolver gatewayResolver;

    /// <summary>
    /// The ITicketValidator to be used for ticket validation.
    /// </summary>
    internal ITicketValidator ticketValidator;

    /// <summary>
    /// The SingleOutHandler to be used for sign out, including CAS server
    /// single sign out.
    /// </summary>
    internal ISingleSignOutHandler singleSignOutHandler;

    /// <summary>
    /// ConfigurationManager to provide access to the configuration data
    /// <see cref="CasClientConfiguration"/>
    /// </summary>
    internal CasClientConfiguration config = CasClientConfiguration.Config;
    #endregion


    /// <summary>
    /// Initializes the HttpModule and prepares it to handle requests.
    /// <para>
    /// Primary tasks are reading configuration settings and registering
    /// event handler(s).
    /// </para>
    /// </summary>
    /// <param name="application">the application context</param>
    public virtual void Init(HttpApplication application)
    {
      this.InitCommonBase(application);
      this.InitInternalAuthenticationBase(application);
      this.InitInternalValidationBase(application);
    }

    /// <summary>
    /// Performs initializations / startup functionality common to all the methods of
    /// this HttpModule.
    /// <param name="application">the application context</param>
    private void InitCommonBase(HttpApplication application)
    {
      this.ServerName = this.config.ServerName;
      if (log.IsInfoEnabled) {
        log.Info(string.Format("{0}:Loaded ServerName property: {1}",
          CommonUtils.MethodName, this.ServerName));
      }

      this.Service = this.config.Service;
      if (log.IsInfoEnabled) {
        log.Info(string.Format("{0}:Loaded Service property: {1}",
          CommonUtils.MethodName, this.Service));
      }

      this.ArtifactParameterName = this.config.ArtifaceParameterName;
      if (log.IsInfoEnabled) {
        log.Info(string.Format("{0}:Loaded ArtifactParameterName property: {1}",
          CommonUtils.MethodName, this.ArtifactParameterName));
      }

      this.ServiceParameterName = this.config.ServiceParameterName;
      if (log.IsInfoEnabled) {
        log.Info(string.Format("{0}:Loaded ServiceParameterName property: {1}",
          CommonUtils.MethodName, this.ServiceParameterName));
      }

      this.EncodeServiceUrl = this.config.EncodeServiceUrl;
      if (log.IsInfoEnabled) {
        log.Info(string.Format("{0}:Loaded EncodeServiceUrl property: {1}",
          CommonUtils.MethodName, this.EncodeServiceUrl));
      }
      if (this.EncodeServiceUrl) {
        throw new CasConfigurationException(
          string.Format("Encode URL with session ID functionality not yet implemented."));
      }

      this.Renew = this.config.Renew;
      if (log.IsInfoEnabled) {
        log.Info(string.Format("{0}:Loaded Renew property: {1}",
          CommonUtils.MethodName, this.Renew));
      }

      this.SingleSignOut = this.config.SingleSignOut;
      if (log.IsInfoEnabled) {
        log.Info(string.Format("{0}:Loaded SingleSignOut property: {1}",
          CommonUtils.MethodName, this.SingleSignOut));
      }

      CommonUtils.AssertTrue(CommonUtils.IsNotBlank(this.ArtifactParameterName),
                  CasClientConfiguration.ARTIFACT_PARAMETER_NAME + " cannot be null or empty.");
      CommonUtils.AssertTrue(CommonUtils.IsNotBlank(this.ServiceParameterName),
                  CasClientConfiguration.SERVICE_PARAMETER_NAME + " cannot be null or empty.");
      CommonUtils.AssertTrue( !String.IsNullOrEmpty(this.ServerName) ||
                              !String.IsNullOrEmpty(this.Service),
                              string.Format("Either {0} or {1} must be set.",
                              CasClientConfiguration.SERVER_NAME,
                              CasClientConfiguration.SERVICE));
      if (!String.IsNullOrEmpty(this.Service)) {
        this.DefaultServiceUrl = this.Service;
      }
    }

    
    /// <summary>
    /// Performs initializations / startup functionality specific to the Authentication step.
    /// </summary>
    /// <param name="application">the application context</param>
    private void InitInternalAuthenticationBase(HttpApplication application)
    {
      this.CasServerLoginUrl = this.config.CasServerLoginUrl;
      if (log.IsInfoEnabled) {
        log.Info(string.Format("{0}:Loaded CasServerLoginUrl property: {1}",
          CommonUtils.MethodName, this.CasServerLoginUrl));
      }
      
      this.Gateway = this.config.Gateway;
      if (log.IsInfoEnabled) {
        log.Info(string.Format("{0}:Loaded Gateway property: {1}",
          CommonUtils.MethodName, this.Gateway));
      }
      if (this.Gateway && this.Renew) {
        throw new CasConfigurationException(
          string.Format("Gateway and renew functionalities are mutually exclusive"));
      }
      if (this.Gateway) {
        this.gatewayResolver = new SessionAttrGatewayResolver();
      }

      CommonUtils.AssertTrue(CommonUtils.IsNotBlank(this.CasServerLoginUrl),
                  CasClientConfiguration.CAS_SERVER_LOGIN_URL + " cannot be null or empty.");
    }

   
    /// <summary>
    /// Performs initializations / startup functionality specific to the Validation step.
    /// </summary>
    /// <param name="application">the application context</param>
    /// <exception cref="CasConfigurationException">
    /// Thrown if configuration problems prevent the successful initialization of the
    /// ticket validator.
    /// </exception>
    private void InitInternalValidationBase(HttpApplication application)
    {
      this.RedirectAfterValidation = this.config.RedirectAfterValidation;
      if (log.IsInfoEnabled) {
        log.Info(string.Format("{0}:Loaded RedirectAfterValidation property: {1}",
          CommonUtils.MethodName, this.RedirectAfterValidation));
      }
      string ticketValidatorName = this.config.TicketValidatorName;
      if (log.IsInfoEnabled) {
        log.Info(string.Format("{0}:Loaded TicketValidatorName property: {1}",
          CommonUtils.MethodName, ticketValidatorName));
      }
      switch(ticketValidatorName) {
        case CasClientConfiguration.CAS10_TICKET_VALIDATOR_NAME:
          this.ticketValidator = new Cas10TicketValidator(this.config);
          break;
        case CasClientConfiguration.CAS20_TICKET_VALIDATOR_NAME:
          this.ticketValidator = new Cas20ServiceTicketValidator(this.config);
          break;
        case CasClientConfiguration.SAML11_TICKET_VALIDATOR_NAME:
          this.ticketValidator = new Saml11TicketValidator(this.config);
          break;
        default:
          throw new CasConfigurationException(
            string.Format("Ticket validator unknown: {0}", 
                          ticketValidatorName));
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
    /// <param name="request">the current HttpRequest</param>
    /// <returns>the service URI to use, not encoded</returns>
    protected Uri ConstructServiceUri(HttpRequest request)
    {
      if (!String.IsNullOrEmpty(this.DefaultServiceUrl)) {
        if (log.IsDebugEnabled) {
          log.Debug(string.Format("{0}:return DefaultServiceUrl: {1}",
            CommonUtils.MethodName, this.DefaultServiceUrl));
        }
        return new Uri(this.DefaultServiceUrl);
      }
      StringBuilder buffer = new StringBuilder();
      if (!(this.ServerName.StartsWith("https://")  ||
          this.ServerName.StartsWith("http://")))
      {
        buffer.Append(request.IsSecureConnection ? "https://" : "http://");
      }
      buffer.Append(this.ServerName);
      string absolutePath = request.Url.AbsolutePath;
      if (!absolutePath.StartsWith("/")) {
        buffer.Append("/");
      }
      buffer.Append(absolutePath);
      StringBuilder queryBuffer = new StringBuilder();
      if (request.QueryString.Count > 0) {
        string queryString = request.Url.Query;
        int indexOfTicket = queryString.IndexOf(this.ArtifactParameterName + "=");
        int indexAfterTicket = queryString.Length;
        if (indexOfTicket == -1) {
          // No ticket parameter so keep QueryString as is
          queryBuffer.Append(queryString);
        } else {
          indexAfterTicket = queryString.IndexOf("&", indexOfTicket);
          if (indexAfterTicket == -1) {
            indexAfterTicket = queryString.Length;
          } else {
            indexAfterTicket = indexAfterTicket + 1;
          }
          queryBuffer.Append(queryString.Substring(0, indexOfTicket));
          if (indexAfterTicket < queryString.Length) {
            queryBuffer.Append(queryString.Substring(indexAfterTicket));
          } else {
            queryBuffer.Length = queryBuffer.Length - 1;
          }
        }
      }
      if (queryBuffer.Length > 1) {
        buffer.Append(queryBuffer.ToString());
      }

      if (log.IsDebugEnabled) {
        log.Debug(string.Format("{0}:return generated serviceUri: {1}",
          CommonUtils.MethodName, buffer.ToString()));
      }
      return new Uri(buffer.ToString());
    }


    /// <summary>
    /// Constructs the URL to use for redirection to the CAS server for login
    /// </summary>
    /// <remarks>
    /// The server name is not parsed from the request for security reasons, which
    /// is why the service and server name configuration parameters exist.
    /// </remarks>
    /// <param name="request">the current HttpRequest</param>
    /// <param name="casServerLoginUrl">the exact CAS server login URL</param>
    /// <returns>the redirection URL to use, not encoded</returns>
    protected string ConstructRedirectUri(HttpRequest request, string casServerLoginUrl)
    {
      Uri serviceUri = this.ConstructServiceUri(request);
      string redirectToUrl = string.Format("{0}?{1}={2}{3}{4}",
        casServerLoginUrl,
        this.ServiceParameterName,
        HttpUtility.UrlEncode(serviceUri.ToString(), Encoding.UTF8),
        (this.Renew ? "&renew=true" : ""),
        (this.gatewayResolver != null ?  "&gateway=true" : ""));
      if (log.IsDebugEnabled) {
        log.Debug(string.Format("{0}: redirectToUrl=>{1}<", CommonUtils.MethodName,
          redirectToUrl));
      }
      return redirectToUrl;
    }

    /// <summary>
    /// Disposes of the resources (other than memory) used by the module
    /// </summary>
    public void Dispose()
    {
      // Intentionally left unimplemented, nothing to dispose.
    }
  }
}
