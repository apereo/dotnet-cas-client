using System;
using System.Collections.Generic;
using System.IO;
using System.IdentityModel.Selectors;
using System.IdentityModel.Tokens;
using System.Xml;
using log4net;
using JasigCasClient.Security;
using JasigCasClient.Utils;

namespace JasigCasClient.Validation
{
#if DOT_NET_3
  /// <summary>
  /// Represents a CAS SAML 1.1 response from a CAS server, using SamlSecurityToken
  /// methods for parsing to populate the object.
  /// </summary>
  class CasSaml11Response : SamlSecurityToken
  {
    private static readonly ILog log = LogManager.GetLogger(
      System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

    const string SAML11_ASSERTION_NAMESPACE = "urn:oasis:names:tc:SAML:1.0:assertion";

    /// <summary>
    ///  Tolerance ticks for checking the current time against the SAML Assertion valid times.
    /// </summary>
    long toleranceTicks = 1000L * TimeSpan.TicksPerMillisecond;

    #region Properties
    /// <summary>
    ///  Whether a valid SAML Assertion was found for processing
    /// </summary>
    public bool HasCasSamlAssertion { get; private set; }

    /// <summary>
    ///  The JaSig CAS ICasPrincipal assertion built from the received CAS SAML 1.1 response
    /// </summary>
    public ICasPrincipal CasPrincipal { get; private set; }
    #endregion

    // The raw response received from the CAS server
    private string casResponse;

    // The assertion from the CAS server response used to populate this instance
    SamlAssertion casSamlAssertion;

    /// <summary>
    /// Creates a CasSaml11Response from the response returned by the CAS server.
    /// The SAMLAssertion processed is the first valid SAML Asssertion found in the
    /// server response.
    /// </summary>
    /// <param name="response">
    /// the xml for the SAML 1.1 response received in response to the samlValidate
    /// query to the CAS server
    /// </param>
    /// <param name="tolerance">
    /// Tolerance milliseconds for checking the current time against the SAML Assertion
    /// valid times.
    /// </param>
    public CasSaml11Response(string response, long tolerance)
    {
      this.toleranceTicks = tolerance * TimeSpan.TicksPerMillisecond;
      this.HasCasSamlAssertion = false;
      this.casResponse = response;
      this.ProcessValidAssertion();
      this.HasCasSamlAssertion = true;
      /*
       Initialize the SamlSecurityToken with the SamlAssertion.  Hopefully this will be useful
       as the SAML support in .NET advances.
      */
      Initialize(this.casSamlAssertion);
    }

    /*
      Initializes the CAS IAssertion for this instance from the first valid Assertion node in the 
      CAS server response.  First the Assertion node is identified and stored in this instance as a
      .NET SamlAssertion.  This SamlAssertion is then used to create the Iassertion instance.
    */
    private void ProcessValidAssertion()
    {
      if (log.IsDebugEnabled) {
        log.Debug(string.Format("{0}:starting .Net3 version", CommonUtils.MethodName));
      }
      XmlDocument document = new XmlDocument();
      document.Load(new StringReader(this.casResponse));
      XmlNamespaceManager nsmgr = new XmlNamespaceManager(document.NameTable);
      nsmgr.AddNamespace("assertion", SAML11_ASSERTION_NAMESPACE);
      XmlNodeList assertions =
        document.DocumentElement.SelectNodes("descendant::assertion:Assertion", nsmgr);
      if (assertions.Count < 1) {
        throw new TicketValidationException("No assertions found.");
      }
      XmlReaderSettings xmlReaderSettings = new XmlReaderSettings();
      xmlReaderSettings.IgnoreWhitespace = true;
      xmlReaderSettings.IgnoreComments = true;
      xmlReaderSettings.CloseInput = true;
      foreach (XmlNode assertionNode in assertions) {
        XmlTextReader xmlReader = new XmlTextReader(new StringReader(assertionNode.OuterXml));
        XmlDictionaryReader xmlDicReader = XmlDictionaryReader.CreateDictionaryReader(xmlReader);
        SamlSerializer samlSerializer = new SamlSerializer();
        SecurityTokenSerializer keyInfoSerializer = null;
        SecurityTokenResolver outOfBandTokenResolver = null;
        SamlAssertion assertion = samlSerializer.LoadAssertion(xmlDicReader,
                                                              keyInfoSerializer,
                                                              outOfBandTokenResolver);

        if (!SamlUtils.IsValidAssertion(assertion, this.toleranceTicks)) {
          continue;
        }

        SamlAuthenticationStatement authenticationStatement =
          SamlUtils.GetSAMLAuthenticationStatement(assertion);
        if (authenticationStatement == null) {
          throw new TicketValidationException("No AuthenticationStatment found in the CAS response.");
        }
        SamlSubject subject = authenticationStatement.SamlSubject;
        if (subject == null) {
          throw new TicketValidationException("No Subject found in the CAS response.");
        }
        this.casSamlAssertion = assertion;
        IList<SamlAttribute> attributes = SamlUtils.GetAttributesFor(this.casSamlAssertion, subject);
        IDictionary<string, IList<string>> personAttributes = new Dictionary<string, IList<string>>();
        foreach (SamlAttribute samlAttribute in attributes) {
          IList<string> values = SamlUtils.GetValuesFor(samlAttribute);
          personAttributes.Add(samlAttribute.Name, values);
        }
        IDictionary<string, IList<string>> authenticationAttributes =
          new Dictionary<string, IList<string>>();
        IList<string> authValues = new List<string>();
        authValues.Add(authenticationStatement.AuthenticationMethod);
        authenticationAttributes.Add("samlAuthenticationStatement::authMethod", authValues);
        IAssertion casAssertion = new Assertion(subject.Name,
                                          casSamlAssertion.Conditions.NotBefore,
                                          casSamlAssertion.Conditions.NotOnOrAfter,
                                          personAttributes);
        this.CasPrincipal = new CasPrincipal(casAssertion, null, null);
        return;
      }
      throw new TicketValidationException("No valid assertions found in the CAS response.");
    }
  }
#else
  /// <summary>
  /// Represents a CAS SAML 1.1 response from a CAS server, using Xml parsing to
  /// populate the object.
  /// </summary>
  class CasSaml11Response
  {
    private static readonly ILog log = LogManager.GetLogger(
      System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

    const string SAML11_ASSERTION_NAMESPACE = "urn:oasis:names:tc:SAML:1.0:assertion";

    /// <summary>
    ///  Tolerance ticks for checking the current time against the SAML Assertion valid times.
    /// </summary>
    long toleranceTicks = 1000L * TimeSpan.TicksPerMillisecond;

    #region Properties
    /// <summary>
    ///  Whether a valid SAML Assertion was found for processing
    /// </summary>
    public bool HasCasSamlAssertion { get; private set; }

    /// <summary>
    ///  The JaSig CAS ICasPrincipal assertion built from the received CAS SAML 1.1 response
    /// </summary>
    public ICasPrincipal CasPrincipal { get; private set; }
    #endregion

    // The raw response received from the CAS server
    private string casResponse;


    /// <summary>
    /// Creates a CasSaml11Response from the response returned by the CAS server.
    /// The SAMLAssertion processed is the first valid SAML Asssertion found in the
    /// server response.
    /// </summary>
    /// <param name="response">
    /// the xml for the SAML 1.1 response received in response to the samlValidate
    /// query to the CAS server
    /// </param>
    /// <param name="tolerance">
    /// Tolerance milliseconds for checking the current time against the SAML Assertion
    /// valid times.
    /// </param>
    public CasSaml11Response(string response, long tolerance)
    {
      this.toleranceTicks = tolerance * TimeSpan.TicksPerMillisecond;
      this.HasCasSamlAssertion = false;
      this.casResponse = response;
      this.ProcessValidAssertion();
      this.HasCasSamlAssertion = true;
    }

    /*
      Initializes the CAS IAssertion for this instance from the first valid Assertion node in the 
      CAS server response.
    */
    private void ProcessValidAssertion()
    {
      DateTime notBefore;
      DateTime notOnOrAfter;
      string subject;
      string authMethod;
      if (log.IsDebugEnabled) {
        log.Debug(string.Format("{0}:starting .Net2 version", CommonUtils.MethodName));
      }
      XmlDocument document = new XmlDocument();
      document.Load(new StringReader(this.casResponse));
      XmlNamespaceManager nsmgr = new XmlNamespaceManager(document.NameTable);
      nsmgr.AddNamespace("assertion", SAML11_ASSERTION_NAMESPACE);
      XmlNodeList assertions =
        document.DocumentElement.SelectNodes("descendant::assertion:Assertion", nsmgr);
      if (assertions.Count < 1) {
        throw new TicketValidationException("No assertions found.");
      }
      XmlReaderSettings xmlReaderSettings = new XmlReaderSettings();
      xmlReaderSettings.ConformanceLevel = ConformanceLevel.Auto;
      xmlReaderSettings.IgnoreWhitespace = true;
      xmlReaderSettings.IgnoreComments = true;
      xmlReaderSettings.CloseInput = true;
      foreach (XmlNode assertionNode in assertions) {
        XmlNode conditionsNode = 
          assertionNode.SelectSingleNode("descendant::assertion:Conditions", nsmgr);
        if (conditionsNode == null) {
          continue;
        }
        try {
          notBefore = SamlUtils.GetAttributeValueAsDateTime(conditionsNode, "NotBefore");
          notOnOrAfter = SamlUtils.GetAttributeValueAsDateTime(conditionsNode, "NotOnOrAfter");
          if (!SamlUtils.IsValidAssertion(notBefore, notOnOrAfter, this.toleranceTicks)) {
            continue;
          }
        } catch (Exception) {
          continue;
        }
        XmlNode authenticationStmtNode = 
          assertionNode.SelectSingleNode("descendant::assertion:AuthenticationStatement", nsmgr);
        if (authenticationStmtNode == null) {
          throw new TicketValidationException("No AuthenticationStatement found in the CAS response.");
        }
        authMethod = 
          SamlUtils.GetAttributeValue(authenticationStmtNode.Attributes, "AuthenticationMethod");

        XmlNode nameIdentifierNode =
          assertionNode.SelectSingleNode("child::" +
            "assertion:AuthenticationStatement/child::" +
            "assertion:Subject/child::" +
            "assertion:NameIdentifier", nsmgr);
        if (nameIdentifierNode == null) {
          throw new TicketValidationException(
            "No NameIdentifier found in AuthenticationStatement of the CAS response.");
        }
        subject = nameIdentifierNode.FirstChild.Value;

        XmlNode attributeStmtNode =
          assertionNode.SelectSingleNode("descendant::assertion:AttributeStatement", nsmgr);
        IDictionary<string, IList<string>> personAttributes =
          SamlUtils.GetAttributesFor(attributeStmtNode, nsmgr, subject);
        IDictionary<string, IList<string>> authenticationAttributes =
          new Dictionary<string, IList<string>>();
        IList<string> authValues = new List<string>();
        authValues.Add(authMethod);
        authenticationAttributes.Add("samlAuthenticationStatement::authMethod", authValues);
        IAssertion casAssertion = new Assertion(subject,
                                                notBefore,
                                                notOnOrAfter,
                                                personAttributes);
        this.CasPrincipal = new CasPrincipal(casAssertion, null, null);
        return;
      }
      throw new TicketValidationException("No valid assertions found in the CAS response.");
    }
  }
#endif
}
