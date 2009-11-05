using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JasigCasClient.Proxy
{
    /// <summary>
    /// Interface to abstract the retrieval of a proxy ticket to make the
    /// implementation a black box to the client.
    /// <remarks>
    /// <para>
    /// This is the .Net port of org.jasig.cas.client.proxy.ProxyRetriever
    /// </para>
    /// <para>
    /// Implementations should be Serializable.
    /// </para>
    /// </remarks>
    /// </summary>
    /// <author>Scott Battaglia</author>
    /// <author>William G. Thompson, Jr. (.Net)</author>
    public interface IProxyRetriever
    {
        /// <summary>
        /// Retrieves a proxy ticket for a specific targetService.
        /// </summary>
        /// <param name="proxyGrantingTicketId">the ProxyGrantingTicketId</param>
        /// <param name="targetService">the service we want to proxy</param>
        /// <returns>Id if granted, null othewise.</returns>
        string GetProxyTicketIdFor(string proxyGrantingTicketId, Uri targetService);
    }
}
