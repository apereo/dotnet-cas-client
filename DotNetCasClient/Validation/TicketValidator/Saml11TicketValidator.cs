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
            /*
            TODO: Test this 
             
            Schema.Saml11.Protocol.Request.RequestType samlRequest = new Schema.Saml11.Protocol.Request.RequestType();
            samlRequest.MajorVersion = "1";
            samlRequest.MinorVersion = "1";
            samlRequest.RequestId = new Guid().ToString();
            samlRequest.IssueInstant = DateTime.UtcNow;
            samlRequest.ItemsElementName = new[]
            {
                RequestType.ItemsElementNames.AssertionArtifact
            };
            samlRequest.Items = new object[]
            {
                ticket
            };

            StringBuilder reqXml = new StringBuilder();
            using (XmlWriter writer = XmlWriter.Create(reqXml))
            {
                XmlSerializer xs = new XmlSerializer(typeof (Schema.Saml11.Protocol.Request.RequestType));
                if (writer != null)
                {
                    xs.Serialize(writer, samlRequest);
                }
            }

            Schema.SoapEnvelope.Envelope env = new Envelope();
            env.Header = new Schema.SoapEnvelope.Header();
            env.Body = new Schema.SoapEnvelope.Body();

            using (XmlReader xr = XmlReader.Create(reqXml.ToString()))
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(xr);
                
                XmlElement el = doc.DocumentElement;
                env.Body.Any = new[] { el };
            }

            StringBuilder samlRequestBuilder = new StringBuilder();
            using (XmlWriter writer = XmlWriter.Create(samlRequestBuilder))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(Schema.SoapEnvelope.Envelope));
                if (writer != null)
                {
                    serializer.Serialize(writer, env);
                }
            }
            */

            StringBuilder messageBuilder = new StringBuilder();
            messageBuilder.AppendLine(@"<SOAP-ENV:Envelope xmlns:SOAP-ENV=""http://schemas.xmlsoap.org/soap/envelope/"">");
            messageBuilder.AppendLine(@"<SOAP-ENV:Header/><SOAP-ENV:Body>");
            messageBuilder.AppendLine(@"<samlp:Request xmlns:samlp=""urn:oasis:names:tc:SAML:1.0:protocol"" ");
            messageBuilder.AppendLine(@"MajorVersion=""1"" MinorVersion=""1"" RequestID=""_192.168.16.51.1024506224022"" ");
            messageBuilder.AppendLine(@"IssueInstant=""2002-06-19T17:03:44.022Z"">");
            messageBuilder.AppendLine(@"<samlp:AssertionArtifact>" + ticket);
            messageBuilder.AppendLine(@"</samlp:AssertionArtifact></samlp:Request></SOAP-ENV:Body></SOAP-ENV:Envelope>");
            string message = messageBuilder.ToString();

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