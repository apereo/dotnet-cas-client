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

using System.Collections.Generic;
using System.Security.Principal;

namespace DotNetCasClient.Security
{
    /// <summary>
    /// Extension to the standard .Net IPrincipal that includes access to the
    /// Assertions for the associated user and a way to retrieve Proxy Tickets.
    /// for that user.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Developers who don't want their code tied to CAS merely need to work
    /// with the .Net IPrincipal. However, in order to take advantabge of CAS
    /// specific features like Proxy Tickets and Attributes, ICasPrincipal must
    /// be used.
    /// </para>
    /// <para>
    /// ICasPrincipal is the .Net port of
    ///   org.jasig.cas.client.authentication.AttributePrincipal
    /// </para>
    /// <para>
    /// Implementors should be Serializable
    /// </para>
    /// </remarks>
    /// <author>Scott Battaglia</author>
    /// <author>William G. Thompson, Jr. (.Net)</author>
    /// <author>Scott Holodak (.Net)</author>
    public interface ICasPrincipal : IPrincipal
    {
        /// <summary>
        /// The Assertion backing this Principal
        /// </summary>
        IAssertion Assertion
        {
            get;
        }

        /// <summary>
        /// The Proxy Granting ticket associated with this principal
        /// which is used to generate Proxy tickets to external 
        /// services.
        /// </summary>
        string ProxyGrantingTicket
        {
            get;
        }

        /// <summary>
        /// The chain of URL's involved in the proxy authentication of 
        /// the user on this system.  If a user starts on site A, proxy 
        /// authenticates to site B, and then proxy authenticates to this
        /// site, this will contain the URL of site A and site B.
        /// </summary>
        IEnumerable<string> Proxies
        {
            get;
        }

        /// <summary>
        /// Allows for a mechanism to retrieve the principal's password.
        /// </summary>
        string GetPassword();
    }
}
