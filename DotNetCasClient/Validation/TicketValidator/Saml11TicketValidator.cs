/*
 * Copyright 2007-2010 The JA-SIG Collaborative. All rights reserved. See license
 * distributed with this file and available online at
 * http://www.ja-sig.org/products/cas/overview/license/index.html
 */

using System;
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
    class Saml11TicketValidator : AbstractUrlTicketValidator
    {
        #region Properties
        /// <summary>
        /// The default name of the request parameter whose value is the artifact
        /// for the SAML 1.1 protocol.
        /// </summary>
        protected override string DefaultArtifactParameterName
        {
            get
            {
                return "SAMLart";
            }
        }

        /// <summary>
        /// The default name of the request parameter whose value is the service
        /// for the SAML 1.1 protocol.
        /// </summary>
        protected override string DefaultServiceParameterName
        {
            get
            {
                return "TARGET";
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
                throw new TicketValidationException("CAS Server could not validate ticket.");
            }

            // parse Assertion element out of SAML response from CAS
            CasSaml11Response casSaml11Response = new CasSaml11Response(response, CasAuthentication.TicketTimeTolerance);
            
            if (casSaml11Response.HasCasSamlAssertion)
            {
                if (Log.IsDebugEnabled)
                {
                    Log.Debug(string.Format("{0}:Valid Assertion found", CommonUtils.MethodName));
                    Log.Debug(string.Format("{0}:CasAssertion:{1}", CommonUtils.MethodName, DebugUtils.IPrincipalToString(casSaml11Response.CasPrincipal, Environment.NewLine, "  ", 2, ">", true)));
                }
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

            Log.Debug(string.Format("{0}:messageBytes=>{1}< with length={2}", CommonUtils.MethodName, message, Encoding.UTF8.GetByteCount(message)));
            
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(validationUrl);
            req.Method = "POST";
            req.ContentType = "text/xml";
            req.ContentLength = Encoding.UTF8.GetByteCount(message);
            req.CookieContainer = new CookieContainer();                
            req.Headers.Add("SOAPAction", "http://www.oasis-open.org/committees/security");                
            req.CachePolicy = new RequestCachePolicy(RequestCacheLevel.NoCacheNoStore);
            
            Log.Debug(string.Format("{0}:" + "CachePolicy={1} ContentLength={2} ContentType={3} Headers={4} Method={5} RequestUri=>{6}<", CommonUtils.MethodName, req.CachePolicy, req.ContentLength, req.ContentType, req.Headers, req.Method, req.RequestUri));

            using (Stream reqPostStream = req.GetRequestStream())
            {
                using (StreamWriter requestWriter = new StreamWriter(reqPostStream, Encoding.UTF8))
                {
                    requestWriter.Write(message);
                }
            }

            HttpWebResponse res = (HttpWebResponse)req.GetResponse();

            Log.Debug(string.Format("{0} HttpWebResponse StatusCode={1} and Server=>{2}<", CommonUtils.MethodName, res.StatusCode, res.Server));

            string validateUriData;

            using (Stream resStream = res.GetResponseStream()) {
                using (StreamReader reader = new StreamReader(resStream))
                {
                    validateUriData = reader.ReadToEnd();
                }
            }
            
            return validateUriData;
        }
    }
}