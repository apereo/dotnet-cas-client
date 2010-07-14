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
using System.Collections.Specialized;
using System.Web;
using DotNetCasClient.Security;
using DotNetCasClient.Utils;
using log4net;

namespace DotNetCasClient.Validation.TicketValidator
{
    /// <summary>
    /// Abstract validator implementation for tickets that are validated against
    /// an Http server.
    /// </summary>
    /// <remarks>
    /// This is the .Net port of 
    ///   org.jasig.cas.client.validation.AbstractUrlBasedTicketValidator
    /// </remarks>
    /// <author>Scott Battaglia</author>
    /// <author>William G. Thompson, Jr. (.Net)</author>
    /// <author>Marvin S. Addison</author>
    abstract class AbstractUrlTicketValidator : ITicketValidator
    {
        private NameValueCollection _CustomParameters;

        /// <summary>
        /// Access to the log file
        /// </summary>
        protected static readonly ILog Log = LogManager.GetLogger("AbstractUrlTicketValidator");

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

        public abstract void Initialize();

        public abstract string UrlSuffix
        {
            get;
        }

        protected abstract string DefaultArtifactParameterName
        {
            get;
        }

        protected abstract string DefaultServiceParameterName
        {
            get;
        }

        public string ArtifactParameterName
        {
            get
            {
                if (String.IsNullOrEmpty(CasAuthentication.ArtifactParameterName))
                {
                    return DefaultArtifactParameterName;
                } 
                else
                {
                    return CasAuthentication.ArtifactParameterName;
                }
            }
        }

        public string ServiceParameterName
        {
            get
            {
                if (String.IsNullOrEmpty(CasAuthentication.ServiceParameterName))
                {
                    return DefaultServiceParameterName;
                }
                else
                {
                    return CasAuthentication.ServiceParameterName;
                }
            }
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
        /// a Principal backed by a CAS Assertion, if one could be parsed from the
        /// response.
        /// </returns>
        /// <exception cref="TicketValidationException">
        /// Thrown if creation of the Assertion fails.
        /// </exception>
        protected abstract ICasPrincipal ParseResponseFromServer(string response, string ticket);

        protected virtual string RetrieveResponseFromServer(string validationUrl, string ticket)
        {
            return CasAuthentication.PerformHttpGet(validationUrl, true);
        }

        /// <summary>
        /// Constructs the URL queried to submit the validation request.
        /// </summary>
        /// <param name="ticket">the ticket to be validate.</param>
        /// <param name="service">the service identifier</param>
        /// <param name="customParameters">custom parameters to add to the validation URL</param>
        /// <returns>the fully constructed URL.</returns>
        protected string ConstructValidationUrl(string service, string ticket, NameValueCollection customParameters)
        {
            EnhancedUriBuilder ub = new EnhancedUriBuilder(EnhancedUriBuilder.Combine(CasAuthentication.CasServerUrlPrefix, UrlSuffix));
            ub.QueryItems.Add(ArtifactParameterName, ticket);
            ub.QueryItems.Add(ServiceParameterName, HttpUtility.UrlEncode(service));

            if (CustomParameters != null)
            {
                for (int i = 0; i < CustomParameters.Count; i++)
                {
                    string key = CustomParameters.AllKeys[i];
                    string value = CustomParameters[i];

                    ub.QueryItems.Add(key, value);
                }
            }

            return ub.Uri.AbsoluteUri;
        }

        /// <summary>
        /// Attempts to validate a ticket for the provided service.
        /// </summary>
        /// <param name="ticket">the ticket to validate</param>
        /// <param name="service">the service associated with this ticket</param>
        /// <returns>
        /// The ICasPrincipal backed by the CAS Assertion included in the response
        /// from the CAS server for a successful ticket validation.
        /// </returns>
        /// <exception cref="TicketValidationException">
        /// Thrown if ticket validation fails.
        /// </exception>
        public ICasPrincipal Validate(string ticket, string service)
        {
            string validationUrl = ConstructValidationUrl(service, ticket, CustomParameters);
            
            if (Log.IsDebugEnabled)
            {
                Log.Debug(string.Format("{0}:Constructed validation url:{1}", CommonUtils.MethodName, validationUrl));
            }
            
            string serverResponse;
            
            try
            {
                serverResponse = RetrieveResponseFromServer(validationUrl, ticket);
            }
            catch (Exception e)
            {
                throw new TicketValidationException("CAS server ticket validation threw an Exception", e);
            }
            
            if (serverResponse == null)
            {
                throw new TicketValidationException("The CAS server returned no response.");
            }

            Log.Debug(string.Format("{0}:Ticket validation server response:>{1}<", CommonUtils.MethodName, serverResponse));

            return ParseResponseFromServer(serverResponse, ticket);
        }
    }
}


