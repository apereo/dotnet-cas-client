/*
 * Copyright 2007-2010 The JA-SIG Collaborative. All rights reserved. See license
 * distributed with this file and available online at
 * http://www.ja-sig.org/products/cas/overview/license/index.html
 */

using System;
using System.Web;
using System.Web.Caching;

namespace DotNetCasClient.State
{
    public class CacheProxyTicketManager : IProxyTicketManager
    {
        private static readonly TimeSpan DefaultExpiration = new TimeSpan(0, 0, 3, 0); // 180 seconds

        public void Initialize()
        {
            // Do nothing
        }

        /// <summary>
        /// Removes expired entries from the ticket store
        /// </summary>
        public void RemoveExpiredMappings()
        {
            // No-op.  ASP.NET Cache provider removes expired entries automatically.
        }

        /// <summary>
        /// Method to save the ProxyGrantingTicket to the backing storage facility.
        /// </summary>
        /// <param name="proxyGrantingTicketIou">used as the key</param>
        /// <param name="proxyGrantingTicket">used as the value</param>
        public void InsertProxyGrantingTicketMapping(string proxyGrantingTicketIou, string proxyGrantingTicket)
        {
            HttpContext.Current.Cache.Insert(proxyGrantingTicketIou, proxyGrantingTicket, null, DateTime.Now.Add(DefaultExpiration), Cache.NoSlidingExpiration);
        }

        /// <summary>
        /// Method to retrieve a ProxyGrantingTicket based on the
        /// ProxyGrantingTicketIou.  Implementations are not guaranteed to
        /// return the same result if retieve is called twice with the same 
        /// proxyGrantingTicketIou.
        /// </summary>
        /// <param name="proxyGrantingTicketIou">used as the key</param>
        /// <returns>the ProxyGrantingTicket Id or null if it can't be found</returns>
        public string GetProxyGrantingTicket(string proxyGrantingTicketIou)
        {
            if (HttpContext.Current.Cache[proxyGrantingTicketIou] != null && HttpContext.Current.Cache[proxyGrantingTicketIou].ToString().Length > 0)
            {
                return HttpContext.Current.Cache[proxyGrantingTicketIou].ToString();
            }

            return null;
        }
    }
}
