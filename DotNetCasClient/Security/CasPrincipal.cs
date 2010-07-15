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
using System.Security.Principal;
using DotNetCasClient.Utils;

namespace DotNetCasClient.Security
{
    /// <summary>
    /// Implementation of ICasPrincipal.
    /// </summary>
    /// <remarks>
    /// ICasPrincipal is the .Net port of
    ///   org.jasig.cas.client.authentication.AttributePrincipalImpl
    /// </remarks>
    /// <author>Scott Battaglia</author>
    /// <author>William G. Thompson, Jr. (.Net)</author>
    [Serializable]
    public class CasPrincipal : ICasPrincipal
    {
        /// <summary>
        /// Constant representing the IIdentity AuthenticationType for 
        /// authentications via CAS.
        /// </summary>
        public const string CAS_AUTH_TYPE = "Jasig CAS";

        #region ICasPrincipal Members
        /// <summary>
        /// The Assertion backing this Principal
        /// </summary>
        public IAssertion Assertion
        {
            get;
            private set;
        }

        /// <summary>
        /// The Proxy Granting ticket associated with this principal
        /// which is used to generate Proxy tickets to external 
        /// services.
        /// </summary>
        public string ProxyGrantingTicket
        {
            get;
            private set;
        }

        /// <summary>
        /// The chain of URL's involved in the proxy authentication of 
        /// the user on this system.  If a user starts on site A, proxy 
        /// authenticates to site B, and then proxy authenticates to this
        /// site, this will contain the URL of site A and site B.
        /// </summary>
        public IEnumerable<string> Proxies
        {
            get;
            private set;
        }
        #endregion

        # region IPrincipal Members
        /// <summary>
        /// The IIdentity associated with this IPrincipal
        /// </summary>
        public IIdentity Identity
        {
            get;
            private set;
        }

        /// <summary>
        /// Determines whether the user identified by this principal is
        /// in the role supplied.
        /// </summary>
        /// <param name="role">The role to check for membership</param>
        /// <returns>
        /// True or False indicating whether the user is in the role 
        /// specified.
        /// </returns>
        public bool IsInRole(string role)
        {
            // TODO answer this with query to Attributes???)
            throw new NotImplementedException();
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Constructs a new Principal backed by the supplied Assertion.
        /// </summary>
        /// <param name="assertion">
        /// the Assertion that backs this Principal
        /// </param>
        public CasPrincipal(IAssertion assertion) : this(assertion, null, null) { }

        /// <summary>
        /// Constructs a new Principal backed by the supplied Assertion, with
        /// proxying capabilities.
        /// </summary>
        /// <param name="assertion">
        /// the Assertion that backs this Principal
        /// </param>
        /// <param name="proxyGrantingTicket">
        /// the proxy granting ticket associated with this Principal.
        /// </param>
        public CasPrincipal(IAssertion assertion, string proxyGrantingTicket) : this(assertion, proxyGrantingTicket, null) { }

        /// <summary>
        /// Constructs a new Principal backed by the supplied Assertion, with
        /// proxying capabilities.
        /// </summary>
        /// <param name="assertion">
        /// the Assertion that backs this Principal
        /// </param>
        /// <param name="proxyGrantingTicket">
        /// the proxy granting ticket associated with this Principal.
        /// </param>
        /// <param name="proxies">
        /// The proxy path associated with this Principal
        /// </param>
        public CasPrincipal(IAssertion assertion, string proxyGrantingTicket, IEnumerable<string> proxies)
        {
            CommonUtils.AssertNotNull(assertion, "assertion cannot be null.");

            Identity = new GenericIdentity(assertion.PrincipalName, CAS_AUTH_TYPE);
            Assertion = assertion;
            ProxyGrantingTicket = proxyGrantingTicket;

            if (proxies != null)
            {
                Proxies = proxies;
            }
            else
            {
                Proxies = new List<string>();
            }
        }
        #endregion
    }
}
