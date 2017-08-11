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

using DotNetCasClient.Security;

namespace DotNetCasClient.Validation.TicketValidator
{
    /// <summary>
    /// Contract for a validator that will confirm the validity of a supplied ticket.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Validator makes no statement about how to validate the ticket or the format 
    /// of the ticket (other than that it must be a String).
    /// </para>
    /// <para>
    /// This is the .Net port of org.jasig.cas.client.validation.TicketValidator
    /// </para>
    /// </remarks>
    /// <author>Scott Battaglia</author>
    /// <author>William G. Thompson, Jr. (.Net)</author>
    /// <author>Scott Holodak (.Net)</author>
    public interface ITicketValidator
    {
        #region Properties
        /// <summary>
        /// The name of the request parameter whose value is the artifact for the
        /// cas protocol.  The default values are defined in DefaultArtifactParameterName,
        /// but these can be overridden in web.config.
        /// </summary>
        string ArtifactParameterName
        {
            get;
        }

        /// <summary>
        /// The default name of the request parameter whose value is the service
        /// for the protocol.  The default values are defined in 
        /// DefaultServiceParameterName, but these can be overridden in web.config.
        /// </summary>
        string ServiceParameterName
        {
            get;
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
        string UrlSuffix
        {
            get;
        }
        #endregion

        #region Methods
        /// <summary>
        /// You retrieve CasAuthentication properties in the constructor or else you will cause 
        /// a StackOverflow.  CasAuthentication.Initialize() will call Initialize() on all 
        /// relevant controls when its initialization is complete.  In Initialize(), you can 
        /// retrieve properties from CasAuthentication.
        /// </summary>
        void Initialize();

        /// <summary>
        /// Attempts to validate a ticket for the provided service.
        /// </summary>
        /// <param name="ticket">the ticket to validate</param>
        /// <returns>
        /// The ICasPrincipal backed by the CAS Assertion included in the response from
        /// the CAS server for a successful ticket validation.
        /// </returns>
        /// <exception cref="TicketValidationException">
        /// Thrown if ticket validation fails.
        /// </exception>
        ICasPrincipal Validate(string ticket);
        #endregion
    }
}
