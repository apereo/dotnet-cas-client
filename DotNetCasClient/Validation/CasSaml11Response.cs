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
using System.Collections.Generic;
using System.IO;
using System.Xml;
using DotNetCasClient.Logging;
using DotNetCasClient.Security;

namespace DotNetCasClient.Validation
{
    /// <summary>
    /// Represents a CAS SAML 1.1 response from a CAS server, using Xml parsing to
    /// populate the object.
    /// </summary>
    class CasSaml11Response
    {
        #region Fields
        // The SAML 1.1 Assertion namespace
        const string SAML11_ASSERTION_NAMESPACE = "urn:oasis:names:tc:SAML:1.0:assertion";

        private static readonly Logger protoLogger = new Logger(Category.Protocol); 

        // Tolerance ticks for checking the current time against the SAML
        // Assertion valid times.
        private readonly long _ToleranceTicks = 1000L * TimeSpan.TicksPerMillisecond;

        // The raw response received from the CAS server
        private readonly string _CasResponse;
        #endregion

        #region Properties
        /// <summary>
        ///  Whether a valid SAML Assertion was found for processing
        /// </summary>
        public bool HasCasSamlAssertion { get; private set; }

        /// <summary>
        ///  The Apereo CAS ICasPrincipal assertion built from the received CAS
        ///  SAML 1.1 response
        /// </summary>
        public ICasPrincipal CasPrincipal { get; private set; }
        #endregion

        #region Constructor
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
        #endregion

        #region Methods
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
            protoLogger.Debug("Unmarshalling SAML response");
            XmlDocument document = new XmlDocument();
            document.Load(new StringReader(_CasResponse));
            
            XmlNamespaceManager nsmgr = new XmlNamespaceManager(document.NameTable);
            nsmgr.AddNamespace("assertion", SAML11_ASSERTION_NAMESPACE);

            if (document.DocumentElement != null) {
                XmlNodeList assertions = document.DocumentElement.SelectNodes("descendant::assertion:Assertion", nsmgr);

                if (assertions == null || assertions.Count < 1)
                {
                    protoLogger.Debug("No assertions found in SAML response.");
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
                        protoLogger.Debug("No AuthenticationStatement found in SAML response.");
                        throw new TicketValidationException("No AuthenticationStatement found in the CAS response.");
                    }
                    
                    string authMethod = SamlUtils.GetAttributeValue(authenticationStmtNode.Attributes, "AuthenticationMethod");
                    
                    XmlNode nameIdentifierNode = assertionNode.SelectSingleNode("child::assertion:AuthenticationStatement/child::assertion:Subject/child::assertion:NameIdentifier", nsmgr);
                    if (nameIdentifierNode == null)
                    {
                        protoLogger.Debug("No NameIdentifier found in SAML response.");
                        throw new TicketValidationException("No NameIdentifier found in AuthenticationStatement of the CAS response.");
                    }
                    
                    string subject = nameIdentifierNode.FirstChild.Value;

                    IList<string> authValues = new List<string>();
                    IDictionary<string, IList<string>> authenticationAttributes = new Dictionary<string, IList<string>>();
                    authValues.Add(authMethod);
                    authenticationAttributes.Add("samlAuthenticationStatement::authMethod", authValues);
                    
                    IAssertion casAssertion;

                    XmlNode attributeStmtNode = assertionNode.SelectSingleNode("descendant::assertion:AttributeStatement", nsmgr);
                    if (attributeStmtNode != null)
                    {
                        IDictionary<string, IList<string>> personAttributes = SamlUtils.GetAttributesFor(attributeStmtNode, nsmgr, subject);
                        casAssertion = new Assertion(subject, notBefore, notOnOrAfter, personAttributes);
                    }
                    else
                    {
                        casAssertion = new Assertion(subject, notBefore, notOnOrAfter);
                    }
                    
                    CasPrincipal = new CasPrincipal(casAssertion, null, null);
                    
                    return;
                }
            }
            protoLogger.Debug("No assertions found in SAML response.");
            throw new TicketValidationException("No valid assertions found in the CAS response.");
        }
        #endregion
    }
}
