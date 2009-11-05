using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Security.Principal;
using System.Text;
using DotNetCasClient.Authentication;
using DotNetCasClient.Configuration;
using DotNetCasClient.Security;

namespace DotNetCasClient.Validation
{
  /// <summary>
  /// CAS 1.0 Ticket Validator
  /// </summary>
  /// <remarks>
  /// This is the .Net port of org.jasig.cas.client.validation.Cas10TicketValidator
  /// </remarks>
  /// <author>Scott Battaglia</author>
  /// <author>William G. Thompson, Jr. (.Net)</author>
  class Cas10TicketValidator : AbstractUrlTicketValidator
  {
    /// <summary>
    /// Constructs an ITicketValidator, initializing it with the supplied
    /// configuration data.
    /// </summary>
    /// <param name="config">
    /// ConfigurationManager to be used to obtain the settings needed by this
    /// ticket validator
    /// </param>
    public Cas10TicketValidator(CasClientConfiguration config)
      : base(config) { }

    /// <summary>
    /// The endpoint of the validation URL.  Should be relative (i.e. not start with a "/").
    /// i.e. validate or serviceValidate.
    /// </summary>
    protected override string UrlSuffix {
      get { return "validate"; }
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
      if (response == null  ||  !response.StartsWith("yes")) {
      //if (response == null  ||  !response.StartsWith("test")) {
        throw new TicketValidationException("CAS Server could not validate ticket.");
      }

      try {
        StringReader reader = new StringReader(response);
        reader.ReadLine();
        string name = reader.ReadLine();
        return new CasPrincipal(new Assertion(name));
      }
      catch (IOException e) {
        throw new TicketValidationException("CAS Server response could not be parsed.", e);
      }
    }
  }
}
