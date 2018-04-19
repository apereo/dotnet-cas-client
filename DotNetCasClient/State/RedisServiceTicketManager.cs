﻿#if NET45
using DotNetCasClient.Configuration;
using DotNetCasClient.Logging;
using DotNetCasClient.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNetCasClient.State
{
    /// <summary>
    /// An IServiceTicketManager implementation that uses Redis for ticket storage.  
    /// It allows to work in distributed enviroment
    /// </summary>
    /// <author>Yerassyl Shalabayev</author>
    public sealed class RedisServiceTicketManager : IServiceTicketManager
    {
        private static readonly Logger securityLogger = new Logger(Category.Security);

        /// <summary>
        /// Indicates whether or not the ticket store contains the supplied serviceTicket
        /// </summary>
        /// <param name="serviceTicket">The service ticket to check for</param>
        /// <returns>True if the ticket is contained in the store</returns>
        /// <exception cref="ArgumentNullException">serviceTicket is null</exception>
        /// <exception cref="ArgumentException">serviceTicket is empty</exception>
        public bool ContainsTicket(string serviceTicket)
        {
            CommonUtils.AssertNotNullOrEmpty(serviceTicket, "serviceTicket parameter cannot be null or empty.");
            return RedisManager.Instance.GetAuthTicket(serviceTicket) != null;
        }

        /// <summary>
        /// Retrieves all CAS Service Tickets in the ticket store that have not already
        /// expired.
        /// </summary>
        /// <returns>An enumerable collection of service tickets</returns>
        public IEnumerable<string> GetAllServiceTickets()
        {
            return RedisManager.Instance.GetAllTickets().Select(t => t.ServiceTicket);
        }

        /// <summary>
        /// Retrieves a list of all users that have non-expired CAS authentication 
        /// tickets.
        /// </summary>
        /// <returns>An enumerable collection of NetId's</returns>
        public IEnumerable<string> GetAllTicketedUsers()
        {            
            return GetAllTickets().Select(t => t.NetId).Distinct();
        }

        /// <summary>
        /// Retrieves all tickets in the ticket store that have not already expired.
        /// </summary>
        /// <returns>An enumerable collection of CasAuthenticationTickets</returns>
        public IEnumerable<CasAuthenticationTicket> GetAllTickets()
        {
            return RedisManager.Instance.GetAllTickets();
        }

        /// <summary>
        /// Retrieve a CasAuthenticationTicket from the ticket store 
        /// by it's CAS Service Ticket
        /// </summary>
        /// <param name="serviceTicket">The service ticket generated by the CAS server</param>
        /// <returns>The CasAuthenticationTicket or null if no matching ticket is found</returns>
        /// <exception cref="ArgumentNullException">serviceTicket is null</exception>
        /// <exception cref="ArgumentException">serviceTicket is empty</exception>
        public CasAuthenticationTicket GetTicket(string serviceTicket)
        {
            CommonUtils.AssertNotNullOrEmpty(serviceTicket, "serviceTicket parameter cannot be null or empty.");

            return RedisManager.Instance.GetAuthTicket(serviceTicket);
        }

        /// <summary>
        /// Retrieves all non-expired CAS Service Tickets in the ticket store associated 
        /// with the netId supplied.
        /// </summary>
        /// <param name="netId">The netId to search the collection for</param>
        /// <returns>An enumerable collection of service tickets</returns>
        /// <exception cref="ArgumentNullException">netId is null</exception>
        /// <exception cref="ArgumentException">netId is empty</exception>
        public IEnumerable<string> GetUserServiceTickets(string netId)
        {
            CommonUtils.AssertNotNullOrEmpty(netId, "netId parameter cannot be null or empty.");

            var tickets = GetAllTickets();
            foreach (var ticket in tickets)
            {                                                
                if (String.Compare(ticket.NetId, netId, true) == 0)
                {
                    yield return ticket.ServiceTicket;
                }                
            }
        }

        /// <summary>
        /// Retrieves all non-expired tickets in the ticket store associated with the 
        /// netId supplied.
        /// </summary>
        /// <param name="netId">The NetId to search the collection for</param>
        /// <returns>An enumerable collection of CasAuthenticationTickets</returns>
        /// <exception cref="ArgumentNullException">netId is null</exception>
        /// <exception cref="ArgumentException">netId is empty</exception>
        public IEnumerable<CasAuthenticationTicket> GetUserTickets(string netId)
        {
            CommonUtils.AssertNotNullOrEmpty(netId, "netId parameter cannot be null or empty.");

            var tickets = GetAllTickets();
            foreach (var ticket in tickets)
            {
                if (String.Compare(ticket.NetId, netId, true) == 0)
                {
                    yield return ticket;
                }
            }
        }

        /// <summary>
        /// You retrieve CasAuthentication properties in the constructor or else you will cause 
        /// a StackOverflow.  CasAuthentication.Initialize() will call Initialize() on all 
        /// relevant controls when its initialization is complete.  In Initialize(), you can 
        /// retrieve properties from CasAuthentication.
        public void Initialize()
        {
            
        }

        /// <summary>
        /// Inserts a CasAuthenticationTicket to the ticket store with a corresponding 
        /// ticket expiration date.
        /// </summary>
        /// <param name="casAuthenticationTicket">The CasAuthenticationTicket to insert</param>
        /// <param name="expiration">The date and time at which the ticket expires</param>
        /// <exception cref="ArgumentNullException">casAuthenticationTicket is null</exception>
        public void InsertTicket(CasAuthenticationTicket casAuthenticationTicket, DateTime expiration)
        {
            RedisManager.Instance.Add(casAuthenticationTicket);
        }

        /// <summary>
        /// Removes expired entries from the ticket store
        /// </summary>
        public void RemoveExpiredTickets()
        {
            var dictionary = RedisManager.Instance.GetAllTickets();
            var ticketsForRemove = dictionary.Where(d => d.Expired)
                                        .Select(d => d.ServiceTicket);            

            RedisManager.Instance.Remove(ticketsForRemove);
        }

        /// <summary>
        /// Removes the ticket from the collection if it exists.  If the ticket does not
        /// exist in the ticket store, just return (do not throw an exception).
        /// </summary>
        /// <param name="serviceTicket">The ticket to remove from the ticket store</param>
        /// <exception cref="ArgumentNullException">serviceTicket is null</exception>
        /// <exception cref="ArgumentException">serviceTicket is empty</exception>
        public void RevokeTicket(string serviceTicket)
        {
            CommonUtils.AssertNotNullOrEmpty(serviceTicket, "serviceTicket parameter cannot be null or empty.");

            RedisManager.Instance.Remove(new string[] { serviceTicket });
        }

        /// <summary>
        /// Revoke all tickets corresponding to the supplied NetId.
        /// </summary>
        /// <param name="netId">The NetId to revoke tickets for</param>
        /// <exception cref="ArgumentNullException">The netId supplied is null</exception>
        /// <exception cref="ArgumentException">The netId supplied is empty</exception>
        public void RevokeUserTickets(string netId)
        {
            var dictionary = RedisManager.Instance.GetAllTickets();
            var ticketsForRemove = dictionary.Where(d => d.NetId == netId)
                                            .Select(d => d.ServiceTicket);

            RedisManager.Instance.Remove(ticketsForRemove);
        }

        /// <summary>
        /// Updates the expiration date and time for an existing ticket.  If the ticket does
        /// not exist in the ticket store, just return (do not throw an exception).
        /// </summary>
        /// <param name="casAuthenticationTicket">The CasAuthenticationTicket to insert</param>
        /// <param name="newExpiration">The new expiration date and time</param>
        /// <exception cref="ArgumentNullException">casAuthenticationTicket is null</exception>
        public void UpdateTicketExpiration(CasAuthenticationTicket casAuthenticationTicket, DateTime newExpiration)
        {
            CommonUtils.AssertNotNull(casAuthenticationTicket, "casAuthenticationTicket parameter cannot be null.");
                        
            RevokeTicket(casAuthenticationTicket.ServiceTicket);
            InsertTicket(casAuthenticationTicket, newExpiration);
        }

        /// <summary>
        /// Verify that the supplied casAuthenticationTicket exists in the ticket store
        /// </summary>
        /// <param name="casAuthenticationTicket">The casAuthenticationTicket to verify</param>
        /// <returns>
        /// True if the ticket exists in the ticket store and the properties of that 
        /// ticket match the properties of the ticket in the ticket store.
        /// </returns>
        public bool VerifyClientTicket(CasAuthenticationTicket casAuthenticationTicket)
        {
            CommonUtils.AssertNotNull(casAuthenticationTicket, "casAuthenticationTicket parameter cannot be null.");

            string incomingServiceTicket = casAuthenticationTicket.ServiceTicket;
            CasAuthenticationTicket cacheAuthTicket = GetTicket(incomingServiceTicket);
            if (cacheAuthTicket != null)
            {
                string cacheServiceTicket = cacheAuthTicket.ServiceTicket;
                if (cacheServiceTicket == incomingServiceTicket)
                {
                    if (String.Compare(cacheAuthTicket.NetId, casAuthenticationTicket.NetId, true) != 0)
                    {
                        securityLogger.Info("Username {0} in ticket {1} does not match cached value.",
                            casAuthenticationTicket.NetId, incomingServiceTicket);
                        return false;
                    }

                    if (String.Compare(cacheAuthTicket.Assertion.PrincipalName, casAuthenticationTicket.Assertion.PrincipalName, true) != 0)
                    {
                        securityLogger.Info("Principal name {0} in assertion of ticket {1} does not match cached value.",
                            casAuthenticationTicket.NetId, casAuthenticationTicket.Assertion.PrincipalName);
                        return false;
                    }

                    return true;
                }
            }
            else
            {
                securityLogger.Info("Ticket {0} not found in cache.  Never existed, expired, or removed via single sign out",
                    incomingServiceTicket);
                return false;
            }
            return false;
        }
    }
}
#endif