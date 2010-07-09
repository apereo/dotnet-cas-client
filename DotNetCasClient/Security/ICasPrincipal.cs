/*
 * Copyright 2007-2010 The JA-SIG Collaborative. All rights reserved. See license
 * distributed with this file and available online at
 * http://www.ja-sig.org/products/cas/overview/license/index.html
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

        string ProxyGrantingTicket
        {
            get;
        }

        IEnumerable<string> Proxies
        {
            get;
        }
    }
}
