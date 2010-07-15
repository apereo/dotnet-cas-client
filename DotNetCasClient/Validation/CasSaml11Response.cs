/*
 * Licensed to Jasig under one or more contributor license
 * agreements. See the NOTICE file distributed with this work
 * for additional information regarding copyright ownership.
 * Jasig licenses this file to you under the Apache License,
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
using System.Collections.Generic;
using System.IO;
using System.Xml;
using DotNetCasClient.Security;
using DotNetCasClient.Utils;
using log4net;

namespace DotNetCasClient.Validation
{
#if DOT_NET_3
    /// <summary>
    /// Represents a CAS SAML 1.1 response from a CAS server, using
    /// SamlSecurityToken methods for parsing to populate the object.
    /// </summary>
    class CasSaml11Response : System.IdentityModel.Tokens.SamlSecurityToken
    {
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// The SAML 1.1 Assertion namespace
        /// </summary>
        const string SAML11_ASSERTION_NAMESPACE = "urn:oasis:names:tc:SAML:1.0:assertion";

        /// <summary>
        /// Tolerance ticks for checking the current time against the SAML
        /// Assertion valid times.
        /// </summary>
        private readonly long _ToleranceTicks = 1000L * TimeSpan.TicksPerMillisecond;

        // The raw response received from the CAS server
        private readonly string _CasResponse;

        // The assertion from the CAS server response used to populate this instance
        System.IdentityModel.Tokens.SamlAssertion _CasSamlAssertion;

        #region Properties
        /// <summary>
        ///  Whether a valid SAML Assertion was found for processing
        /// </summary>
        public bool HasCasSamlAssertion { get; private set; }

        /// <summary>
        /// The JaSig CAS ICasPrincipal assertion built from the received CAS
        /// SAML 1.1 response
        /// </summary>
        public ICasPrincipal CasPrincipal { get; private set; }
        #endregion

        /// <summary>
        /// Creates a CasSaml11Response from the response returned by the CAS
        /// server. The SAMLAssertion processed is the first valid SAML Asssertion
        /// found in the server response.
        /// </summary>
        /// <param name="response">
        /// the xml for the SAML 1.1 response received in response to the
        /// samlValidate query to the CAS server
        /// </param>
        /// <param name="tolerance">
        /// Tolerance milliseconds for checking the current time against the SAML
        /// Assertion valid times.
        /// </param>
        public CasSaml11Response(string response, long tolerance)
        {
            _ToleranceTicks = tolerance * TimeSpan.TicksPerMillisecond;
            HasCasSamlAssertion = false;
            _CasResponse = response;
            ProcessValidAssertion();
            HasCasSamlAssertion = true;

            // Initialize the SamlSecurityToken with the SamlAssertion.  Hopefully
            // this will be useful as the SAML support in .NET advances.
            Initialize(_CasSamlAssertion);
        }

        /// <summary>
        /// Initializes the CAS IAssertion for this instance from the first valid
        /// Assertion node in the CAS server response.  First the Assertion node is
        /// identified and stored in this instance as a .NET SamlAssertion.  This
        /// SamlAssertion is then used to create the Iassertion instance.
        /// </summary>
        /// <exception cref="TicketValidationException">
        /// Throwsn when data problems are encountered parsing
        /// the CAS server response that contains the Assertion, such as
        /// no valid Assertion found or no Authentication statment found in the
        /// the valid Assertion.
        /// </exception>
        private void ProcessValidAssertion()
        {
            Log.Debug(string.Format("{0}:starting .Net3 version", CommonUtils.MethodName));

            XmlDocument document = new XmlDocument();
            document.Load(new StringReader(_CasResponse));

            XmlNamespaceManager nsmgr = new XmlNamespaceManager(document.NameTable);
            nsmgr.AddNamespace("assertion", SAML11_ASSERTION_NAMESPACE);
            if (document.DocumentElement != null)
            {
                XmlNodeList assertions = document.DocumentElement.SelectNodes("descendant::assertion:Assertion", nsmgr);
                if (assertions == null || assertions.Count < 1)
                {
                    throw new TicketValidationException("No assertions found.");
                }

                XmlReaderSettings xmlReaderSettings = new XmlReaderSettings();
                xmlReaderSettings.IgnoreWhitespace = true;
                xmlReaderSettings.IgnoreComments = true;
                xmlReaderSettings.CloseInput = true;

                foreach (XmlNode assertionNode in assertions)
                {
                    XmlTextReader xmlReader = new XmlTextReader(new StringReader(assertionNode.OuterXml));
                    XmlDictionaryReader xmlDicReader = XmlDictionaryReader.CreateDictionaryReader(xmlReader);
                    System.IdentityModel.Tokens.SamlSerializer samlSerializer = new System.IdentityModel.Tokens.SamlSerializer();
                    System.IdentityModel.Selectors.SecurityTokenSerializer keyInfoSerializer = null;
                    System.IdentityModel.Selectors.SecurityTokenResolver outOfBandTokenResolver = null;
                    System.IdentityModel.Tokens.SamlAssertion assertion = samlSerializer.LoadAssertion(xmlDicReader, keyInfoSerializer, outOfBandTokenResolver);

                    if (!SamlUtils.IsValidAssertion(assertion, _ToleranceTicks))
                    {
                        continue;
                    }

                    System.IdentityModel.Tokens.SamlAuthenticationStatement authenticationStatement = SamlUtils.GetSAMLAuthenticationStatement(assertion);
                    if (authenticationStatement == null)
                    {
                        throw new TicketValidationException("No AuthenticationStatement found in the CAS response.");
                    }

                    System.IdentityModel.Tokens.SamlSubject subject = authenticationStatement.SamlSubject;
                    if (subject == null)
                    {
                        throw new TicketValidationException("No Subject found in the CAS response.");
                    }

                    _CasSamlAssertion = assertion;
                    IList<System.IdentityModel.Tokens.SamlAttribute> attributes = SamlUtils.GetAttributesFor(_CasSamlAssertion, subject);
                    IDictionary<string, IList<string>> personAttributes = new Dictionary<string, IList<string>>();
                    foreach (System.IdentityModel.Tokens.SamlAttribute samlAttribute in attributes)
                    {
                        IList<string> values = SamlUtils.GetValuesFor(samlAttribute);
                        personAttributes.Add(samlAttribute.Name, values);
                    }

                    IDictionary<string, IList<string>> authenticationAttributes = new Dictionary<string, IList<string>>();
                    IList<string> authValues = new List<string>();
                    authValues.Add(authenticationStatement.AuthenticationMethod);
                    authenticationAttributes.Add("samlAuthenticationStatement::authMethod", authValues);
                    IAssertion casAssertion = new Assertion(subject.Name, _CasSamlAssertion.Conditions.NotBefore, _CasSamlAssertion.Conditions.NotOnOrAfter, personAttributes);

                    CasPrincipal = new CasPrincipal(casAssertion, null);
                    return;
                }
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
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// The SAML 1.1 Assertion namespace
        /// </summary>
        const string SAML11_ASSERTION_NAMESPACE = "urn:oasis:names:tc:SAML:1.0:assertion";

        /// <summary>
        ///  Tolerance ticks for checking the current time against the SAML
        ///  Assertion valid times.
        /// </summary>
        private readonly long _ToleranceTicks = 1000L * TimeSpan.TicksPerMillisecond;

        // The raw response received from the CAS server
        private readonly string _CasResponse;

        #region Properties
        /// <summary>
        ///  Whether a valid SAML Assertion was found for processing
        /// </summary>
        public bool HasCasSamlAssertion { get; private set; }

        /// <summary>
        ///  The JaSig CAS ICasPrincipal assertion built from the received CAS
        ///  SAML 1.1 response
        /// </summary>
        public ICasPrincipal CasPrincipal { get; private set; }
        #endregion

        /// <summary>
        /// Creates a CasSaml11Response from the response returned by the CAS server.
        /// The SAMLAssertion processed is the first valid SAML Asssertion found in
        /// the server response.
        /// </summary>
        /// <param name="response">
        /// the xml for the SAML 1.1 response received in response to the
        /// samlValidate query to the CAS server
        /// </param>
        /// <param name="tolerance">
        /// Tolerance milliseconds for checking the current time against the SAML
        /// Assertion valid times.
        /// </param>
        public CasSaml11Response(string response, long tolerance)
        {
            _ToleranceTicks = tolerance * TimeSpan.TicksPerMillisecond;
            HasCasSamlAssertion = false;
            _CasResponse = response;
            ProcessValidAssertion();
            HasCasSamlAssertion = true;
        }

        /// <summary>
        /// Initializes the CAS IAssertion for this instance from the first valid
        /// Assertion node in the CAS server response.
        /// </summary>
        /// <exception cref="TicketValidationException">
        /// Thrown when data problems are encountered parsing
        /// the CAS server response that contains the Assertion, such as
        /// no valid Assertion found or no Authentication statment found in the
        /// the valid Assertion.
        /// </exception>
        private void ProcessValidAssertion()
        {
            Log.Debug(string.Format("{0}:starting .Net2 version", CommonUtils.MethodName));
            
            XmlDocument document = new XmlDocument();
            document.Load(new StringReader(_CasResponse));
            
            XmlNamespaceManager nsmgr = new XmlNamespaceManager(document.NameTable);
            nsmgr.AddNamespace("assertion", SAML11_ASSERTION_NAMESPACE);

            if (document.DocumentElement != null) {
                XmlNodeList assertions = document.DocumentElement.SelectNodes("descendant::assertion:Assertion", nsmgr);

                if (assertions == null || assertions.Count < 1)
                {
                    throw new TicketValidationException("No assertions found.");
                }

                XmlReaderSettings xmlReaderSettings = new XmlReaderSettings();
                xmlReaderSettings.ConformanceLevel = ConformanceLevel.Auto;
                xmlReaderSettings.IgnoreWhitespace = true;
                xmlReaderSettings.IgnoreComments = true;
                xmlReaderSettings.CloseInput = true;

                foreach (XmlNode assertionNode in assertions)
                {
                    XmlNode conditionsNode = assertionNode.SelectSingleNode("descendant::assertion:Conditions", nsmgr);
                    if (conditionsNode == null)
                    {
                        continue;
                    }

                    DateTime notBefore;
                    DateTime notOnOrAfter;
                    try
                    {
                        notBefore = SamlUtils.GetAttributeValueAsDateTime(conditionsNode, "NotBefore");
                        notOnOrAfter = SamlUtils.GetAttributeValueAsDateTime(conditionsNode, "NotOnOrAfter");
                        if (!SamlUtils.IsValidAssertion(notBefore, notOnOrAfter, _ToleranceTicks))
                        {
                            continue;
                        }
                    }
                    catch (Exception)
                    {
                        continue;
                    }

                    XmlNode authenticationStmtNode = assertionNode.SelectSingleNode("descendant::assertion:AuthenticationStatement", nsmgr);
                    if (authenticationStmtNode == null)
                    {
                        throw new TicketValidationException("No AuthenticationStatement found in the CAS response.");
                    }
                    
                    string authMethod = SamlUtils.GetAttributeValue(authenticationStmtNode.Attributes, "AuthenticationMethod");
                    
                    XmlNode nameIdentifierNode = assertionNode.SelectSingleNode("child::assertion:AuthenticationStatement/child::assertion:Subject/child::assertion:NameIdentifier", nsmgr);
                    if (nameIdentifierNode == null)
                    {
                        throw new TicketValidationException("No NameIdentifier found in AuthenticationStatement of the CAS response.");
                    }
                    
                    string subject = nameIdentifierNode.FirstChild.Value;

                    XmlNode attributeStmtNode = assertionNode.SelectSingleNode("descendant::assertion:AttributeStatement", nsmgr);
                    IDictionary<string, IList<string>> personAttributes = SamlUtils.GetAttributesFor(attributeStmtNode, nsmgr, subject);
                    IDictionary<string, IList<string>> authenticationAttributes = new Dictionary<string, IList<string>>();
                    IList<string> authValues = new List<string>();
                    
                    authValues.Add(authMethod);
                    
                    authenticationAttributes.Add("samlAuthenticationStatement::authMethod", authValues);
                    IAssertion casAssertion = new Assertion(subject, notBefore, notOnOrAfter, personAttributes);
                    
                    CasPrincipal = new CasPrincipal(casAssertion, null, null);
                    
                    return;
                }
            }
            throw new TicketValidationException("No valid assertions found in the CAS response.");
        }
    }
#endif
}
