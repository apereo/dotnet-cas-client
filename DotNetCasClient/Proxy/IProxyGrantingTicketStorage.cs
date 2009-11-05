using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DotNetCasClient.Proxy
{
    /// <summary>
    /// Interface for the storage and retrieval of ProxyGrantingTicketIds by mapping
    /// them to a specific ProxyGrantingTicketIou
    /// </summary>
    interface IProxyGrantingTicketStorage
    {
        /// <summary>
        /// Method to save the ProxyGrantingTicket to the backing storage facility.
        /// </summary>
        /// <param name="proxyGrantingTicketIou">used as the key</param>
        /// <param name="proxyGrantingTicket">used as the value</param>
        void Save(string ProxyGrantingTicketIou, string proxyGrantingTicket);

        /// <summary>
        /// Method to retrieve a ProxyGrantingTicket based on the
        /// ProxyGrantingTicketIou.  Note that implementations are not guaranteed to
        /// return the same result if retieve is called twice with the same proxyGrantingTicketIou.
        /// </summary>
        /// <param name="proxyGrantingTicketIou">used as the key</param>
        /// <returns>the ProxyGrantingTicket Id or null if it can't be found</returns>
        string Retrieve(string proxyGrantingTicketIou);
    }
}
