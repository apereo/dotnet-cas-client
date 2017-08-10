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

#if NET40 || NET45
using System;

namespace DotNetCasClient.State
{///<summary>
    /// An IProxyTicketManager implementation that relies on the System.Runtime.Caching caching model for ticket 
    /// storage.  Generally this implies that the ticket storage is maintained locally on the web
    /// server (either in memory or on disk).  A limitation of this model is that it will not 
    /// support clustered, load balanced, or round-robin style configurations.
    ///</summary>
    /// <author>Jason Kanaris</author>
    public class MemoryCacheProxyTicketManager : IProxyTicketManager
    {
        private static readonly TimeSpan DefaultExpiration = new TimeSpan(0, 0, 3, 0); // 180 seconds

        /// <summary>
        /// You retrieve CasAuthentication properties in the constructor or else you will cause 
        /// a StackOverflow.  CasAuthentication.Initialize() will call Initialize() on all 
        /// relevant controls when its initialization is complete.  In Initialize(), you can 
        /// retrieve properties from CasAuthentication.
        /// </summary>
        public void Initialize()
        {
            // Do nothing
        }

        /// <summary>
        /// Removes expired PGTIOU-PGT from the ticket store
        /// </summary>
        public void RemoveExpiredMappings()
        {
            // No-op.  The System.Runtime.Caching.ObjectCache provider removes expired entries automatically.
        }

        /// <summary>
        /// Method to save the ProxyGrantingTicket to the backing storage facility.
        /// </summary>
        /// <param name="proxyGrantingTicketIou">used as the key</param>
        /// <param name="proxyGrantingTicket">used as the value</param>
        public void InsertProxyGrantingTicketMapping(string proxyGrantingTicketIou, string proxyGrantingTicket)
        {
            MemoryCacheManager.Instance.Set(proxyGrantingTicketIou, proxyGrantingTicket, DefaultExpiration);
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
            
            if (MemoryCacheManager.Instance.Contains(proxyGrantingTicketIou) && !string.IsNullOrWhiteSpace(MemoryCacheManager.Instance.Get<string>(proxyGrantingTicketIou)))
            {
                return MemoryCacheManager.Instance.Get<string>(proxyGrantingTicketIou);
            }

            return null;
        }
    }
}
#endif