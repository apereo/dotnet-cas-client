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
using Enyim.Caching;
using Enyim.Caching.Memcached;

namespace DotNetCasClient.State
{
    ///<summary>
    /// An IProxyTicketManager implementation in .NET 4.5 that leverages
    /// Couchbase for proxy ticket storage in a clustered environment.
    /// 
    /// Based on the original CacheProxyTicketManager to address the limitations thereof
    /// where distributed caching is supported in Memcached environment (i.e. Couchbase).
    ///</summary>
    /// <author>Matt Borja</author>
    public class MemcachedProxyTicketManager : IProxyTicketManager
    {
        private static MemcachedClient _cache;
        private static readonly TimeSpan DefaultExpiration = new TimeSpan(0, 0, 3, 0); // 180 seconds

        public MemcachedProxyTicketManager()
        {
            _cache = new MemcachedClient();
        }

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
            // No-op.  ASP.NET Cache provider removes expired entries automatically.
        }

        /// <summary>
        /// Method to save the ProxyGrantingTicket to the backing storage facility.
        /// </summary>
        /// <param name="proxyGrantingTicketIou">used as the key</param>
        /// <param name="proxyGrantingTicket">used as the value</param>
        public void InsertProxyGrantingTicketMapping(string proxyGrantingTicketIou, string proxyGrantingTicket)
        {
            if (String.IsNullOrEmpty(proxyGrantingTicketIou))
                throw new ArgumentNullException("proxyGrantingTicketIou");

            if (String.IsNullOrEmpty(proxyGrantingTicket))
                throw new ArgumentNullException("proxyGrantingTicket");

            _cache.Store(StoreMode.Set, proxyGrantingTicketIou, proxyGrantingTicket, DateTime.Now.Add(DefaultExpiration));
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
            return _cache.Get<string>(proxyGrantingTicketIou);
        }
    }
}
