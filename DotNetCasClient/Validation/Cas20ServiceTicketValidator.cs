using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Security.Principal;
using System.Text;
using DotNetCasClient.Authentication;
using DotNetCasClient.Configuration;
using DotNetCasClient.Security;
using DotNetCasClient.Utils;

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
  /// <author>Marvin S. Addison</author>
  class Cas20ServiceTicketValidator : AbstractCasProtocolValidator
  {
    private const string XML_USER_ELEMENT_NAME = "cas:user";
    private const string XML_AUTHENTICATION_FAILURE_ELEMENT_NAME = "cas:authenticationFailure";

    /// <summary>
    /// Constructs an ITicketValidator, initializing it with the supplied
    /// configuration data.
    /// </summary>
    /// <param name="config">
    /// ConfigurationManager to be used to obtain the settings needed by this
    /// ticket validator
    /// </param>
    public Cas20ServiceTicketValidator(CasClientConfiguration config)
      : base(config) { }

    /// <summary>
    /// The endpoint of the validation URL.  Should be relative (i.e. not start with a "/").
    /// i.e. validate or serviceValidate.
    /// </summary>
    protected override string UrlSuffix {
      get { return "serviceValidate"; }
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
      return new CasPrincipal(new Assertion(userValue));
    }
  }
}
