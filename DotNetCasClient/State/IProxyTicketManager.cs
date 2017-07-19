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

namespace DotNetCasClient.State
{
    ///<summary>
    /// Defines the interface for a ProxyTicketManager implementation.  ProxyTicketManagers
    /// are responsible for temporary storage of state information relating to Proxy Tickets.
    /// For active-active clustered/web-farm configurations, the state must be stored in a 
    /// persistent storage mechanism that is accessible from any node or server that handles
    /// web requests.
    ///</summary>
    /// <author>Scott Holodak</author>
    public interface IProxyTicketManager
    {
        /// <summary>
        /// You retrieve CasAuthentication properties in the constructor or else you will cause 
        /// a StackOverflow.  CasAuthentication.Initialize() will call Initialize() on all 
        /// relevant controls when its initialization is complete.  In Initialize(), you can 
        /// retrieve properties from CasAuthentication.
        /// </summary>
        void Initialize();

        /// <summary>
        /// Removes expired PGTIOU-PGT from the ticket store
        /// </summary>
        void RemoveExpiredMappings();

        /// <summary>
        /// Method to save the ProxyGrantingTicket to the backing storage facility.
        /// </summary>
        /// <param name="proxyGrantingTicketIou">used as the key</param>
        /// <param name="proxyGrantingTicket">used as the value</param>
        void InsertProxyGrantingTicketMapping(string proxyGrantingTicketIou, string proxyGrantingTicket);

        /// <summary>
        /// Method to retrieve a ProxyGrantingTicket based on the
        /// ProxyGrantingTicketIou.  Implementations are not guaranteed to
        /// return the same result if retieve is called twice with the same 
        /// proxyGrantingTicketIou.
        /// </summary>
        /// <param name="proxyGrantingTicketIou">used as the key</param>
        /// <returns>the ProxyGrantingTicket Id or null if it can't be found</returns>
        string GetProxyGrantingTicket(string proxyGrantingTicketIou);
    }
}
