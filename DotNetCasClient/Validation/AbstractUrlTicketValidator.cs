using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using DotNetCasClient.Authentication;
using DotNetCasClient.Configuration;
using DotNetCasClient.Security;
using DotNetCasClient.Utils;
using log4net;

namespace DotNetCasClient.Validation
{
  /// <summary>
  /// Abstract validator implementation for tickets that are validated against an Http server.
  /// </summary>
  /// <remarks>
  /// This is the .Net port of org.jasig.cas.client.validation.AbstractUrlBasedTicketValidator
  /// </remarks>
  /// <author>Scott Battaglia</author>
  /// <author>William G. Thompson, Jr. (.Net)</author>
  abstract class AbstractUrlTicketValidator : ITicketValidator
  {
    /// <summary>
    /// Access to the log file
    /// </summary>
    protected static readonly ILog log = 
      LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

    #region Properties
    /// <summary>
    /// The prefix for the CAS server. Should be everything up to the URL endpoint,
    /// including the /.
    /// </summary>
    protected string CasServerUrlPrefix { get; private set; }

    /// <summary>
    /// The name of the request parameter whose value is the artifact (e.g. "ticket").
    /// </summary>
    protected string ArtifactParameterName { get; private set; }
    
    /// <summary>
    /// The name of the request parameter whose value is the service (e.g. "service")
    /// </summary>
    protected string ServiceParameterName { get; private set; }

    /// <summary>
    /// Whether renew=true should be sent to the CAS server.
    /// </summary>
    protected bool Renew { get; private set; }

    /// <summary>
    /// Whether to encode the session ID into the Service URL.
    /// </summary>
    protected bool EncodeServiceUrl { get; private set; }
    #endregion

    #region Fields

    /// <summary>
    /// Custom parameters to pass to the validation URL.
    /// </summary>
    ILookup<string, string> customParameters;
    #endregion

    #region Constructor

    /// <summary>
    /// Constructs an ITicketValidator, initializing it with the supplied
    /// configuration data.
    /// </summary>
    /// <param name="config">
    /// ConfigurationManager to be used to obtain the settings needed by this
    /// ticket validator
    /// </param>
    protected AbstractUrlTicketValidator(CasClientConfiguration config)
    {
      this.CasServerUrlPrefix = config.CasServerUrlPrefix;
      if (log.IsInfoEnabled) {
        log.Info(string.Format("{0}:Loaded CasServerUrlPrefix property: {1}",
          CommonUtils.MethodName, this.CasServerUrlPrefix));
      }

      this.ArtifactParameterName = config.ArtifactParameterNameValidation;
      if (log.IsInfoEnabled) {
        log.Info(string.Format("{0}:Loaded ArtifactParameterName property: {1}",
          CommonUtils.MethodName, this.ArtifactParameterName));
      }

      this.ServiceParameterName = config.ServiceParameterNameValidation;
      if (log.IsInfoEnabled) {
        log.Info(string.Format("{0}:Loaded ServiceParameterName property: {1}",
          CommonUtils.MethodName, this.ServiceParameterName));
      }

      this.EncodeServiceUrl = config.EncodeServiceUrl;
      if (log.IsInfoEnabled) {
        log.Info(string.Format("{0}:Loaded EncodeServiceUrl property: {1}",
          CommonUtils.MethodName, this.EncodeServiceUrl));
      }

      this.Renew = config.Renew;
      if (log.IsInfoEnabled) {
        log.Info(string.Format("{0}:Loaded Renew property: {1}",
          CommonUtils.MethodName, this.Renew));
      }

      CommonUtils.AssertTrue(CommonUtils.IsNotBlank(this.CasServerUrlPrefix),
                  CasClientConfiguration.CAS_SERVER_URL_PREFIX + " cannot be null or empty.");
      CommonUtils.AssertTrue(CommonUtils.IsNotBlank(this.ArtifactParameterName),
                  CasClientConfiguration.ARTIFACT_PARAMETER_NAME + " cannot be null or empty.");
      CommonUtils.AssertTrue(CommonUtils.IsNotBlank(this.ServiceParameterName),
                  CasClientConfiguration.SERVICE_PARAMETER_NAME + " cannot be null or empty.");

      if (log.IsDebugEnabled) {
        log.Debug(string.Format("{0}:Construct using casServerUrlPrefix>{1}<",
                  CommonUtils.MethodName, this.CasServerUrlPrefix));
      }
    }

    #endregion
    
    /// <summary>
    /// Template method for ticket validators that need to provide additional parameters
    /// to the validation URL.
    /// </summary>
    /// <param name="urlParameters">dictionary of parameters</param>
    protected virtual void AddParameters(IDictionary<string, string> urlParameters)
    {
      // nothing to do
    }

    /// <summary>
    /// The endpoint of the validation URL.  Should be relative (i.e. not start with a "/").
    /// i.e. validate or serviceValidate.
    /// </summary>
    protected abstract String UrlSuffix { get; }

    /// <summary>
    /// Constructs the URL queried to submit the validation request.
    /// </summary>
    /// <param name="ticket">the ticket to be validate.</param>
    /// <param name="serviceUri">the service identifier</param>
    /// <returns>the fully constructed URL.</returns>
    protected string ConstructValidationUrl(string ticket, Uri serviceUri)
    {
      IDictionary<string, string> urlParameters = new Dictionary<string, string>();
      urlParameters.Add(this.ArtifactParameterName, ticket);
      urlParameters.Add(this.ServiceParameterName, this.EncodeUrl(serviceUri.ToString()));

      if (this.Renew) {
        urlParameters.Add("renew", "true");
      }

      AddParameters(urlParameters);

      if (this.customParameters != null) {
        foreach (IGrouping<string, string> entry in this.customParameters) {
          foreach (string value in entry) {
           urlParameters.Add(entry.Key, value);
          }
        }
      }

      int i = 0;
      StringBuilder builder = new StringBuilder();

      builder.Append(this.CasServerUrlPrefix);
      if (!this.CasServerUrlPrefix.EndsWith("/")) {
        builder.Append("/");
      }
      builder.Append(this.UrlSuffix);

      foreach (KeyValuePair<string, string> entry in urlParameters) {
        if (entry.Value != null) {
          builder.Append(i++ == 0 ? "?" : "&");
          builder.Append(entry.Key.ToString());
          builder.Append("=");
          builder.Append(entry.Value.ToString());
        }
                
      }
      return builder.ToString();
    }


    /// <summary>
    /// Encodes a URL using the UTF-8 character encoding.
    /// </summary>
    /// <param name="url">URL to encode.</param>
    /// <returns>the encoded Url</returns>
    protected string EncodeUrl(string url)
    {
      return String.IsNullOrEmpty(url) ? url : 
        HttpUtility.UrlEncode(url, new UTF8Encoding());
    }

    /// <summary>
    /// Parses the response from the server into a CAS Assertion and includes this in
    /// a CASPrincipal.
    /// </summary>
    /// <param name="response">the response from the server, in any format.</param>
    /// <returns>
    /// a Principal backed by a CAS Assertion, if one could be parsed from the response.
    /// </returns>
    /// <exception cref="TicketValidationException">
    /// Thrown if creation of the Assertion fails.
    /// </exception>
    protected abstract ICasPrincipal ParseResponseFromServer(string response);

    /// <summary>
    /// Contacts the CAS Server to retrieve the response for the ticket validation.
    /// </summary>
    /// <param name="validationUrl">the URL for the validation request submission.</param>
    /// <param name="ticket">the ticket to validate.</param>
    /// <returns>the response from the CAS server.</returns>
    protected virtual string RetrieveResponseFromServer(Uri validationUrl, string ticket)
    {
			StreamReader reader = null;
      string validateUrlResponse = null;
			try {
				reader = new StreamReader(new WebClient().OpenRead(validationUrl));
        validateUrlResponse = reader.ReadToEnd();
			} finally {
				if ( reader != null ) {
          reader.Close();
        }
			}
      return validateUrlResponse;
    }

    /// <summary>
    /// Attempts to validate a ticket for the provided service.
    /// </summary>
    /// <param name="ticket">the ticket to validate</param>
    /// <param name="service">the service associated with this ticket</param>
    /// <returns>
    /// The ICasPrincipal backed by the CAS Assertion included in the response from
    /// the CAS server for a successful ticket validation.
    /// </returns>
    /// <exception cref="TicketValidationException">
    /// Thrown if ticket validation fails.
    /// </exception>
    public ICasPrincipal Validate(string ticket, Uri service)
    {
      string validationUrl = this.ConstructValidationUrl(ticket, service);
      if (log.IsDebugEnabled) {
        log.Debug(string.Format("{0}:Constructed validation url:{1}",
          CommonUtils.MethodName, validationUrl));
      }
      string serverResponse = null;
      try {
        serverResponse = this.RetrieveResponseFromServer(new Uri(validationUrl), ticket);
      } catch (Exception e) {
        throw new TicketValidationException(
          "CAS server ticket validation threw an Exception", e);
      }
      if (serverResponse == null) {
        throw new TicketValidationException("The CAS server returned no response.");
      }

      if (log.IsDebugEnabled) {
        log.Debug(string.Format("{0}:Ticket validation server response:>{1}<",
          CommonUtils.MethodName, serverResponse));
      }

      return this.ParseResponseFromServer(serverResponse);
    }


    /// <summary>
    /// Stores custom parameters to be included in the validation URL.
    /// </summary>
    /// <param name="customParameters">custom parameters for the validation</param>
    public void setCustomParameters(IDictionary<string, string> customParameters)
    {
      this.customParameters = customParameters.ToLookup(x => x.Key, x => x.Value);
    }
  }
}
