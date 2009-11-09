using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Cache;
using System.Security.Principal;
using System.Text;
using DotNetCasClient.Authentication;
using DotNetCasClient.Configuration;
using DotNetCasClient.Security;
using DotNetCasClient.Utils;

namespace DotNetCasClient.Validation
{
  /// <summary>
  /// SAML 1.1 Ticket Validator
  /// </summary>
  /// <remarks>
  /// This is the .Net port of org.jasig.cas.client.validation.Saml11TicketValidator
  /// </remarks>
  /// <author>Scott Battaglia</author>
  /// <author>Catherine D. Winfrey (.Net)</author>
  /// <author>Marvin S. Addison</author>
  class Saml11TicketValidator : AbstractUrlTicketValidator
  {
    private const string DEFAULT_ARTIFACT = "SAMLart";
    private const string DEFAULT_SERVICE = "TARGET";

    #region Properties
    /// <summary>
    /// Tolerance milliseconds for checking the current time against the SAML Assertion
    /// valid times.
    /// </summary>
    protected long TicketTimeTolerance { get; private set; }

    protected override string DefaultArtifactParameterName
    {
      get { return DEFAULT_ARTIFACT; }
    }
  
    protected override string DefaultServiceParameterName
    {
    	get { return DEFAULT_SERVICE; }
    }
    #endregion

    /// <summary>
    /// Constructs an ITicketValidator for SAML 1.1 responses from the CAS server,
    /// initializing it with the supplied configuration data.
    /// </summary>
    /// <param name="config">
    /// ConfigurationManager to be used to obtain the settings needed by this
    /// ticket validator
    /// </param>
    public Saml11TicketValidator(CasClientConfiguration config) : base(config)
    {
      this.TicketTimeTolerance = config.TicketTimeTolerance;
      log.Info("Set TicketTimeTolerance property: " + this.TicketTimeTolerance);
    }

    /// <summary>
    /// The endpoint of the validation URL.  Should be relative (i.e. not start with a "/").
    /// i.e. validate or serviceValidate.
    /// </summary>
    protected override string UrlSuffix {
      get { return "samlValidate"; }
    }

    /// <summary>
    /// Adjust the parameters for the validation URL being constructed.
    /// </summary>
    /// <param name="urlParameters">
    /// The validation URL parameters created by the base class.
    /// </param>
    protected override void AddParameters(IDictionary<string, string> urlParameters)
    {
      urlParameters.Remove(this.ArtifactParameterName);
    }


    /// <summary>
    /// Parses the response from the server into a CAS Assertion and includes this in
    /// a CASPrincipal.
    /// </summary>
    /// <param name="response">the response from the server, in any format.</param>
    /// <returns>
    /// a Principal backed by a CAS Assertion, if one could be created from the response.
    /// </returns>
    /// <exception cref="TicketValidationException">
    /// Thrown if creation of the Assertion fails.
    /// </exception>
    protected override ICasPrincipal ParseResponseFromServer(string response)
    {
      if (response == null) {
        throw new TicketValidationException("CAS Server could not validate ticket.");
      }

      // parse Assertion element out of SAML response from CAS
      CasSaml11Response casSaml11Response = new CasSaml11Response(response,
        this.TicketTimeTolerance);
      if (casSaml11Response.HasCasSamlAssertion) {
        if (log.IsDebugEnabled) {
          log.Debug(string.Format("{0}:Valid Assertion found", CommonUtils.MethodName));
          log.Debug(string.Format("{0}:CasAssertion:{1}",
            CommonUtils.MethodName,
            DebugUtils.IPrincipalToString(casSaml11Response.CasPrincipal,
            DebugUtils.CR, "  ", 2, ">", true)));
        }
        return casSaml11Response.CasPrincipal;
      } else {
        throw new TicketValidationException("CAS Server response could not be parsed.");
      }
    }

    protected override string RetrieveResponseFromServer(Uri validationUrl, string ticket)
    {
      string msg1 = @"<SOAP-ENV:Envelope xmlns:SOAP-ENV=""http://schemas.xmlsoap.org/soap/envelope/"">";
      string msg2 = @"<SOAP-ENV:Header/><SOAP-ENV:Body>";
      string msg3 = @"<samlp:Request xmlns:samlp=""urn:oasis:names:tc:SAML:1.0:protocol"" ";
      string msg4 = @"MajorVersion=""1"" MinorVersion=""1"" RequestID=""_192.168.16.51.1024506224022"" ";
      string msg5 = @"IssueInstant=""2002-06-19T17:03:44.022Z"">";
      string msg6 = @"<samlp:AssertionArtifact>" + ticket;
      string msg7 = @"</samlp:AssertionArtifact></samlp:Request></SOAP-ENV:Body></SOAP-ENV:Envelope>";
      string message = msg1 + msg2 + msg3 + msg4 + msg5 + msg6 + msg7;
      StreamReader reader = null;
      Stream reqPostStream = null;
      try {
        byte[] messageBytes = Encoding.ASCII.GetBytes(message);
        if (log.IsDebugEnabled) {
          log.Debug(string.Format("{0}:messageBytes=>{1}< with length={2}", CommonUtils.MethodName,
            Encoding.ASCII.GetString(messageBytes), messageBytes.Length));
        }
        HttpWebRequest req = (HttpWebRequest) WebRequest.Create(validationUrl);
        req.Method = "POST";
        //req.ContentType = "application/x-www-form-urlencoded";
        req.ContentType = "text/xml";
        req.ContentLength = messageBytes.Length;
        // enable cookies in case response wants to set any
        req.CookieContainer = new CookieContainer();
        WebHeaderCollection reqHeaders = req.Headers;
        reqHeaders.Add("SOAPAction", "http://www.oasis-open.org/committees/security");
        WebRequest webReq = (WebRequest) req;
        webReq.CachePolicy = new RequestCachePolicy(RequestCacheLevel.NoCacheNoStore);
        if (log.IsDebugEnabled) {
          log.Debug(string.Format("{0}:" +
            "CachePolicy={1} ContentLength={2} ContentType={3} Headers={4} Method={5} " +
            " RequestUri=>{6}<", CommonUtils.MethodName,
            webReq.CachePolicy, webReq.ContentLength, webReq.ContentType,
            webReq.Headers, webReq.Method, webReq.RequestUri));
        }
        reqPostStream = req.GetRequestStream();
        reqPostStream.Write(messageBytes, 0, messageBytes.Length);
        reqPostStream.Flush();
        reqPostStream.Close();

        HttpWebResponse res = (HttpWebResponse) req.GetResponse();
        if (log.IsDebugEnabled) {
          log.Debug(string.Format("{0} HttpWebResponse StatusCode={1} and Server=>{2}<",
            CommonUtils.MethodName, res.StatusCode, res.Server));
        }
        Stream resStream = res.GetResponseStream();
        reader = new StreamReader(resStream);
        string validateUriData = reader.ReadToEnd();
        return validateUriData;
  	  } finally {
				if ( reader != null ) {
          reader.Close();
        }
        if (reqPostStream != null) {
          reqPostStream.Close();
        }
      }
    }
  }
}
