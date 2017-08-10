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

using System.IO;
using DotNetCasClient.Security;

namespace DotNetCasClient.Validation.TicketValidator
{
    /// <summary>
    /// CAS 1.0 Ticket Validator
    /// </summary>
    /// <remarks>
    /// This is the .Net port of org.jasig.cas.client.validation.Cas10TicketValidator
    /// </remarks>
    /// <author>Scott Battaglia</author>
    /// <author>William G. Thompson, Jr. (.Net)</author>
    /// <author>Marvin S. Addison</author>
    /// <author>Scott Holodak (.Net)</author>
    class Cas10TicketValidator : AbstractCasProtocolTicketValidator
    {
        #region Properties
        /// <summary>
        /// The endpoint of the validation URL.  Should be relative (i.e. not start with a "/").
        /// i.e. validate or serviceValidate.
        /// </summary>
        public override string UrlSuffix
        {
            get
            {
                return "validate";
            }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Performs Cas10TicketValidator initialization.
        /// </summary>
        public override void Initialize() { /* Do nothing */ }

        /// <summary>
        /// Parses the response from the server into a CAS Assertion and includes this in
        /// a CASPrincipal.
        /// </summary>
        /// <param name="response">the response from the server, in any format.</param>
        /// <param name="ticket">The ticket used to generate the validation response</param>
        /// <returns>
        /// a Principal backed by a CAS Assertion, if one could be created from the response.
        /// </returns>
        /// <exception cref="TicketValidationException">
        /// Thrown if creation of the Assertion fails.
        /// </exception>
        protected override ICasPrincipal ParseResponseFromServer(string response, string ticket)
        {
            if (response == null || !response.StartsWith("yes"))
            {
                throw new TicketValidationException("CAS Server could not validate ticket.");
            }

            try
            {
                StringReader reader = new StringReader(response);
                reader.ReadLine();
                string name = reader.ReadLine();
                return new CasPrincipal(new Assertion(name));
            }
            catch (IOException e)
            {
                throw new TicketValidationException("CAS Server response could not be parsed.", e);
            }
        }
        #endregion
    }
}