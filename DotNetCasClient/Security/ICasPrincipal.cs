using System;
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
    public interface ICasPrincipal : IPrincipal
    {
        /// <summary>
        /// Retrieves a CAS proxy ticket for this Principal.
        /// </summary>
        /// <param Name="service">
        /// the service to which this user is to be proxied.
        /// </param>
        /// <returns>a string representing the proxy ticket.</returns>
        string GetProxyTicketFor(Uri service);

        /// <summary>
        /// The Assertion backing this Principal
        /// </summary>
        IAssertion Assertion { get; }
    }
}
