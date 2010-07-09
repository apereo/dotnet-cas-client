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
