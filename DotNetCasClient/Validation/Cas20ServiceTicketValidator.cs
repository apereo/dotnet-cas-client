using System.Collections.Generic;
using System.Web;
using DotNetCasClient.Configuration;
using DotNetCasClient.Proxy;
using DotNetCasClient.Security;
using DotNetCasClient.Utils;
using System.Timers;

namespace DotNetCasClient.Validation
{
  /// <summary>
  /// CAS 2.0 Ticket Validator
  /// </summary>
  /// <remarks>
  /// This is the .Net port of org.jasig.cas.client.validation.Cas20ServiceTicketValidator.
  /// TODO: add proxy functionality.
  /// </remarks>
  /// <author>Scott Battaglia</author>
  /// <author>Catherine D. Winfrey (.Net)</author>
  /// <author>William G. Thompson, Jr. (.Net)</author>
  /// <author>Marvin S. Addison</author>
  class Cas20ServiceTicketValidator : AbstractCasProtocolValidator
  {
    private const string XML_USER_ELEMENT_NAME = "cas:user";
    private const string XML_AUTHENTICATION_FAILURE_ELEMENT_NAME = "cas:authenticationFailure";
    private const string XML_PROXY_GRANTING_TICKET_NAME = "cas:proxyGrantingTicket";

    private readonly string proxyCallBackUrl;

    //internal IProxyGrantingTicketStorage ProxyGrantingTicketStorage{ get; private set;}
    public static readonly ProxyGrantingTicketStorage ProxyGrantingTicketStorage
        = new ProxyGrantingTicketStorage(60000);

    private Timer timer;

    private readonly IProxyRetriever proxyRetriever;

    /// <summary>
    /// Constructs an ITicketValidator, initializing it with the supplied
    /// configuration data.
    /// </summary>
    /// <param name="config">
    /// ConfigurationManager to be used to obtain the settings needed by this
    /// ticket validator
    /// </param>
    public Cas20ServiceTicketValidator(CasClientConfiguration config)
      : base(config)
    {
        this.proxyCallBackUrl = config.ProxyCallbackUrl;
        this.proxyRetriever = new Cas20ProxyRetriever(config.CasServerUrlPrefix);
        this.timer = new Timer(60000);
        this.timer.Elapsed += new ElapsedEventHandler(CleanUpEvent);
        this.timer.Enabled = true;
    }

    private static void CleanUpEvent(object source, ElapsedEventArgs e)
    {
        ProxyGrantingTicketStorage.CleanUp();
    }


    /// <summary>
    /// The endpoint of the validation URL.  Should be relative (i.e. not start with a "/").
    /// i.e. validate or serviceValidate.
    /// </summary>
    protected override string UrlSuffix {
        get { return "serviceValidate"; }
    }

    protected override void AddParameters(IDictionary<string, string> urlParameters)
    {
        urlParameters.Add("pgtUrl", HttpUtility.UrlEncode(this.proxyCallBackUrl));
    }


    /// <summary>
    /// Parses the response from the server into a CAS Assertion and includes this in
    /// a CASPrincipal.
    /// <remarks>
    /// Parsing of a <cas:attributes> element is <b>not</b> supported.  The official
    /// CAS 2.0 protocol does include this feature.  If attributes are needed,
    /// SAML must be used.
    /// </remarks>
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
      if (CommonUtils.IsBlank(response)) {
        throw new TicketValidationException("CAS Server response was empty.");
      }
      string authErrorMessage = XmlUtils.GetTextForElement(response,
        XML_AUTHENTICATION_FAILURE_ELEMENT_NAME);
      if (CommonUtils.IsNotBlank(authErrorMessage)) {
        throw new TicketValidationException(authErrorMessage);
      }
      string userValue = XmlUtils.GetTextForElement(response,
        XML_USER_ELEMENT_NAME);
      if (CommonUtils.IsBlank(userValue)) {
        throw new TicketValidationException(
          string.Format("CAS Server response parse failure: missing {0} element.",
            XML_USER_ELEMENT_NAME));
      }

      string proxyGrantingTicketIou = XmlUtils.GetTextForElement(response, XML_PROXY_GRANTING_TICKET_NAME);
      string proxyGrantingTicket =ProxyGrantingTicketStorage != null
                                       ? ProxyGrantingTicketStorage.Retrieve(proxyGrantingTicketIou)
                                       : null;
      //string proxyGrantingTicket = this.ProxyGrantingTicketStorage != null
      //                               ? this.ProxyGrantingTicketStorage.Retrieve(proxyGrantingTicketIou)
      //                               : null;
      
      return new CasPrincipal(new Assertion(userValue), proxyGrantingTicket, this.proxyRetriever);
    }
  }
}
