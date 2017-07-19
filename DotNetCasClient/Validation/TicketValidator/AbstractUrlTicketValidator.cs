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
using System.Collections.Specialized;
using System.Diagnostics;
using DotNetCasClient.Logging;
using DotNetCasClient.Security;
using DotNetCasClient.Utils;

namespace DotNetCasClient.Validation.TicketValidator
{
    /// <summary>
    /// Abstract validator implementation for tickets that are validated against
    /// an Http server.
    /// </summary>
    /// <remarks>
    /// This is the .Net port of 
    ///   org.jasig.cas.client.validation.AbstractUrlBasedTicketValidator
    ///   must be public to allow for external assemblies to extend
    /// </remarks>
    /// <author>Scott Battaglia</author>
    /// <author>William G. Thompson, Jr. (.Net)</author>
    /// <author>Marvin S. Addison</author>
    /// <author>Scott Holodak (.Net)</author>
    public abstract class AbstractUrlTicketValidator : ITicketValidator
    {
        #region Fields
        protected static readonly Logger protoLogger = new Logger(Category.Protocol);
        private NameValueCollection _CustomParameters;
        #endregion

        #region Properties
        /// <summary>
        /// Custom parameters to pass to the validation URL.
        /// </summary>        
        public NameValueCollection CustomParameters
        {
            get
            {
                if (_CustomParameters == null)
                {
                    _CustomParameters = new NameValueCollection();
                }
                return _CustomParameters;
            }
        }

        /// <summary>
        /// The endpoint of the validation URL.  Should be relative (i.e. not start with a "/").
        /// i.e. validate or serviceValidate.
        /// <list>
        ///   <item>CAS 1.0:  validate</item>
        ///   <item>CAS 2.0:  serviceValidate or proxyValidate</item>
        ///   <item>SAML 1.1: samlValidate</item>
        /// </list>
        /// </summary>
        public abstract string UrlSuffix
        {
            get;
        }

        /// <summary>
        /// The protocol-specific name of the request parameter containing the artifact/ticket.
        /// </summary>
        public abstract string ArtifactParameterName
        {
            get;
        }

        /// <summary>
        /// The protocol-specific name of the request parameter containing the service identifier.
        /// </summary>
        public abstract string ServiceParameterName
        {
            get;
        }
        #endregion

        #region Abstract Methods
        /// <summary>
        /// Perform any initialization required for the UrlTicketValidator implementation.
        /// </summary>
        public abstract void Initialize();

        /// <summary>
        /// Parses the response from the server into a CAS Assertion and includes
        /// this in a CASPrincipal.
        /// </summary>
        /// <param name="response">
        /// the response from the server, in any format.
        /// </param>
        /// <param name="ticket">The ticket used to generate the validation response</param>
        /// <returns>
        /// a Principal backed by a CAS Assertion, if one could be parsed from the
        /// response.
        /// </returns>
        /// <exception cref="TicketValidationException">
        /// Thrown if creation of the Assertion fails.
        /// </exception>
        protected abstract ICasPrincipal ParseResponseFromServer(string response, string ticket);
        #endregion

        #region Concrete Methods
        /// <summary>
        /// Default implementation that performs an HTTP GET request to the validation URL
        /// supplied with the supplied ticket and returns the response body as a string.
        /// </summary>
        /// <param name="validationUrl">The validation URL to request</param>
        /// <param name="ticket">The ticket parameter to pass to the URL</param>
        /// <returns></returns>
        protected virtual string RetrieveResponseFromServer(string validationUrl, string ticket)
        {
            return HttpUtil.PerformHttpGet(validationUrl, true);
        }

        /// <summary>
        /// Attempts to validate a ticket for the provided service.
        /// </summary>
        /// <param name="ticket">the ticket to validate</param>
        /// <returns>
        /// The ICasPrincipal backed by the CAS Assertion included in the response
        /// from the CAS server for a successful ticket validation.
        /// </returns>
        /// <exception cref="TicketValidationException">
        /// Thrown if ticket validation fails.
        /// </exception>
        public ICasPrincipal Validate(string ticket)
        {
            string validationUrl = UrlUtil.ConstructValidateUrl(ticket, CasAuthentication.Gateway, CasAuthentication.Renew, CustomParameters);
            protoLogger.Debug("Constructed validation URL " + validationUrl);

            string serverResponse;
            try
            {
                serverResponse = RetrieveResponseFromServer(validationUrl, ticket);
            }
            catch (Exception e)
            {
                protoLogger.Info("Ticket validation failed: " + e);
                throw new TicketValidationException("CAS server ticket validation threw an Exception", e);
            }

            if (serverResponse == null)
            {
                protoLogger.Warn("CAS server returned no response");
                throw new TicketValidationException("The CAS server returned no response.");
            }

            protoLogger.Debug("Ticket validation response:{0}{1}", Environment.NewLine, serverResponse);

            return ParseResponseFromServer(serverResponse, ticket);
        }
        #endregion
    }
}