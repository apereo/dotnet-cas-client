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
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Cache;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using DotNetCasClient.Security;
using DotNetCasClient.Utils;

namespace DotNetCasClient.Validation.TicketValidator
{
    /// <summary>
    /// SAML 1.1 Ticket Validator
    /// </summary>
    /// <remarks>
    /// This is the .Net port of
    ///   org.jasig.cas.client.validation.Saml11TicketValidator
    /// </remarks>
    /// <author>Scott Battaglia</author>
    /// <author>Catherine D. Winfrey (.Net)</author>
    /// <author>Marvin S. Addison</author>
    /// <author>Scott Holodak (.Net)</author>
    class Saml11TicketValidator : AbstractUrlTicketValidator
    {
        private const string SAML_ARTIFACT_PARAM = "SAMLart";
        private const string SAML_SERVICE_PARAM = "TARGET";

        #region Properties
        public override string ArtifactParameterName
        {
            get
            {
                return SAML_ARTIFACT_PARAM;
            }
        }

        public override string ServiceParameterName
        {
            get
            {
                return SAML_SERVICE_PARAM;
            }
        }

        /// <summary>
        /// The endpoint of the validation URL.  Should be relative (i.e. not start
        /// with a "/").
        /// i.e. validate or serviceValidate or samlValidate.
        /// </summary>
        public override string UrlSuffix
        {
            get
            {
                return "samlValidate";
            }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Performs Saml11TicketValidator initialization.
        /// </summary>
        public override void Initialize()
        {
            // Do nothing
        }

        /// <summary>
        /// Parses the response from the server into a CAS Assertion and includes
        /// this in a CASPrincipal.
        /// </summary>
        /// <param name="response">
        /// the response from the server, in any format.
        /// </param>
        /// <param name="ticket">The ticket used to generate the validation response</param>
        /// <returns>
        /// a Principal backed by a CAS Assertion, if one could be created from the
        /// response.
        /// </returns>
        /// <exception cref="TicketValidationException">
        /// Thrown if creation of the Assertion fails.
        /// </exception>
        protected override ICasPrincipal ParseResponseFromServer(string response, string ticket)
        {
            if (response == null)
            {
                throw new ArgumentNullException("response");
            }

            // parse Assertion element out of SAML response from CAS
            CasSaml11Response casSaml11Response = new CasSaml11Response(response, CasAuthentication.TicketTimeTolerance);

            if (casSaml11Response.HasCasSamlAssertion)
            {
                protoLogger.Debug("Valid Assertion found: " + casSaml11Response.CasPrincipal.Assertion);
                return casSaml11Response.CasPrincipal;
            }
            else
            {
                throw new TicketValidationException("CAS Server response could not be parsed.");
            }
        }

        /// <summary>
        /// Requests CAS ticket validation by the configured CAS server.
        /// </summary>
        /// <param name="validationUrl">the URL to use for ticket validation</param>
        /// <param name="ticket">
        /// the ticket returned by the CAS server from a successful authentication
        /// </param>
        /// <returns>
        /// the XML response representing the ticket validation
        /// </returns>
        protected override string RetrieveResponseFromServer(string validationUrl, string ticket)
        {
            // Create the SAML Request and populate its values.
            Schema.Saml11.Protocol.Request.RequestType samlRequest = new Schema.Saml11.Protocol.Request.RequestType
            {
                MajorVersion = "1",
                MinorVersion = "1",
                RequestId = Guid.NewGuid().ToString(),
                IssueInstant = DateTime.UtcNow,
                ItemsElementName = new[]
                {
                    Schema.Saml11.Protocol.Request.RequestType.ItemsElementNames.AssertionArtifact
                },
                Items = new object[] {ticket}
            };

            // Create the XML representation of the SAML Request.
            StringBuilder samlRequestXml = new StringBuilder();
            using (XmlWriter xmlWriter = XmlWriter.Create(samlRequestXml))
            {
                // Need to add some overrides because of conflicting element names during serialization.
                var xmlOverrides = new XmlAttributeOverrides();
                xmlOverrides.Add(typeof(Schema.XmlDsig.PgpDataType.ItemsElementNames), new XmlAttributes
                {
                    XmlType = new XmlTypeAttribute("ItemsElementNamesOfPgpDataType")
                });
                xmlOverrides.Add(typeof(Schema.XmlDsig.KeyInfoType.ItemsElementNames), new XmlAttributes
                {
                    XmlType = new XmlTypeAttribute("ItemsElementNamesOfKeyInfoType")
                });

                XmlSerializer xmlSerializer = new XmlSerializer(typeof(Schema.Saml11.Protocol.Request.RequestType), xmlOverrides);
                XmlSerializerNamespaces samlRequestNamespaces = new XmlSerializerNamespaces();
                samlRequestNamespaces.Add("samlp", "urn:oasis:names:tc:SAML:1.0:protocol");
                xmlSerializer.Serialize(xmlWriter, samlRequest, samlRequestNamespaces);
            }

            // Create the SOAP Envelope XML that will wrap around the SAML Request XML.
            Schema.SoapEnvelope.Envelope soapEnvelope = new Schema.SoapEnvelope.Envelope();
            soapEnvelope.Header = new Schema.SoapEnvelope.Header();
            soapEnvelope.Body = new Schema.SoapEnvelope.Body();

            using (XmlReader xmlReader = XmlReader.Create(new StringReader(samlRequestXml.ToString())))
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(xmlReader);

                XmlElement el = doc.DocumentElement;
                soapEnvelope.Body.Any = new[] { el };
            }

            StringBuilder samlRequestStringBuilder = new StringBuilder();
            XmlWriterSettings xmlWriterSettings = new XmlWriterSettings();
            xmlWriterSettings.OmitXmlDeclaration = true;
            xmlWriterSettings.Indent = true;
            using (XmlWriter xmlWriter = XmlWriter.Create(samlRequestStringBuilder, xmlWriterSettings))
            {
                XmlSerializer xmlSerializer = new XmlSerializer(typeof(Schema.SoapEnvelope.Envelope));
                XmlSerializerNamespaces soapEnvelopeNamespaces = new XmlSerializerNamespaces();
                soapEnvelopeNamespaces.Add("SOAP-ENV", "http://schemas.xmlsoap.org/soap/envelope/");
                xmlSerializer.Serialize(xmlWriter, soapEnvelope, soapEnvelopeNamespaces);
            }

            // Get the string representation of the SAML Request XML.
            string message = samlRequestStringBuilder.ToString();

            protoLogger.Debug("Constructed SAML request:{0}{1}", Environment.NewLine, message);

            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(validationUrl);
            req.Method = "POST";
            req.ContentType = "text/xml";
            req.ContentLength = Encoding.UTF8.GetByteCount(message);
            req.CookieContainer = new CookieContainer();
            req.Headers.Add("SOAPAction", "http://www.oasis-open.org/committees/security");
            req.CachePolicy = new RequestCachePolicy(RequestCacheLevel.NoCacheNoStore);

            protoLogger.Debug("Request URI: " + req.RequestUri);
            protoLogger.Debug("Request headers: " + req.Headers);

            byte[] payload = Encoding.UTF8.GetBytes(message);
            using (Stream requestStream = req.GetRequestStream())
            {
                requestStream.Write(payload, 0, payload.Length);
            }

            HttpWebResponse response = (HttpWebResponse)req.GetResponse();
            Stream responseStream = response.GetResponseStream();
            if (responseStream != null)
            {
                using (StreamReader responseReader = new StreamReader(responseStream))
                {
                    return responseReader.ReadToEnd();
                }
            }
            else
            {
                throw new ApplicationException("Unable to retrieve response stream.");
            }
        }
        #endregion
    }
}