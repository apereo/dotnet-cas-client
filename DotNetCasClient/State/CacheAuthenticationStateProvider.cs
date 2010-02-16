using System;
using System.Collections;
using System.Collections.Generic;
using System.Web;
using System.Web.Caching;
using System.Web.Security;
using DotNetCasClient.Utils;

namespace DotNetCasClient.State
{
    public sealed class CacheAuthenticationStateProvider : AbstractFormsAuthenticationStateProvider
    {
        private const string CacheTicketKeyPrefix = "CasTicket::";
        private Cache cache;

        internal CacheAuthenticationStateProvider()
        {
            Init(HttpContext.Current.ApplicationInstance);
        }

        public override void Init(HttpApplication application)
        {
            base.Init(application);
            cache = application.Context.Cache;
        }

        public override void RemoveExpiredTickets()
        {
            // No-op.  ASP.NET Cache provider removes expired entries automatically.
        }

        public override FormsAuthenticationTicket GetTicket(string casTicket)
        {
            string key = GetTicketKey(casTicket);            
            if (cache[key] != null)
            {
                FormsAuthenticationTicket result = cache[key] as FormsAuthenticationTicket;
                return result;
            }
            return null;
        }

        public override void InsertTicket(FormsAuthenticationTicket ticket, DateTime expiration)
        {
            cache.Insert(GetTicketKey(ticket), ticket, null, expiration, Cache.NoSlidingExpiration);
        }

        public override void UpdateTicketExpiration(FormsAuthenticationTicket ticket, DateTime newExpiration)
        {
            RevokeTicket(ticket);
            InsertTicket(ticket, newExpiration);
        }

        public override bool RevokeTicket(FormsAuthenticationTicket ticket)
        {
            return RevokeTicket(ticket.UserData);
        }

        public override bool RevokeTicket(string casTicket)
        {
            string key = GetTicketKey(casTicket);
            if (cache[key] != null)
            {
                FormsAuthenticationTicket ticket = cache[key] as FormsAuthenticationTicket;
                if (ticket != null)
                {
                    cache.Remove(key);
                    return true;
                }
            }
            return false;
        }

        public override bool ContainsTicket(FormsAuthenticationTicket ticket)
        {
            return ContainsTicket(ticket.UserData);
        }

        public override bool ContainsTicket(string casTicket)
        {
            IDictionaryEnumerator enumerator = cache.GetEnumerator();
            while (enumerator.MoveNext())
            {
                string currentKey = enumerator.Entry.Key as string;
                if (currentKey != null && currentKey.StartsWith(CacheTicketKeyPrefix))
                {
                    FormsAuthenticationTicket currentTicket = enumerator.Entry.Value as FormsAuthenticationTicket;
                    if (currentTicket != null)
                    {
                        if (currentTicket.UserData == casTicket)
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        public override void RevokeUserTickets(string netId)
        {
            IEnumerable<FormsAuthenticationTicket> allTickets = GetAllTickets();
            foreach (FormsAuthenticationTicket ticket in allTickets)
            {
                if (string.Compare(ticket.Name, netId, true) == 0)
                {
                    RevokeTicket(ticket);
                }
            }
        }

        public override IEnumerable<FormsAuthenticationTicket> GetAllTickets()
        {            
            IDictionaryEnumerator enumerator = cache.GetEnumerator();
            while (enumerator.MoveNext())
            {
                string currentKey = enumerator.Entry.Key as string;
                if (currentKey != null && currentKey.StartsWith(CacheTicketKeyPrefix))
                {
                    FormsAuthenticationTicket currentTicket = enumerator.Entry.Value as FormsAuthenticationTicket;
                    if (currentTicket != null)
                    {
                        yield return currentTicket;
                    }
                }
            }
        }

        public override IEnumerable<FormsAuthenticationTicket> GetUserTickets(string netId)
        {
            IDictionaryEnumerator enumerator = cache.GetEnumerator();
            while (enumerator.MoveNext())
            {
                string currentKey = enumerator.Entry.Key as string;
                if (currentKey != null && currentKey.StartsWith(CacheTicketKeyPrefix))
                {
                    FormsAuthenticationTicket currentTicket = enumerator.Entry.Value as FormsAuthenticationTicket;
                    if (currentTicket != null && string.Compare(currentTicket.Name, netId, true) == 0)
                    {
                        yield return currentTicket;
                    }
                }
            }
        }
        
        public override IEnumerable<string> GetAllCasTickets()
        {
            IDictionaryEnumerator enumerator = cache.GetEnumerator();
            while (enumerator.MoveNext())
            {
                string currentKey = enumerator.Entry.Key as string;
                if (currentKey != null && currentKey.StartsWith(CacheTicketKeyPrefix))
                {
                    FormsAuthenticationTicket currentTicket = enumerator.Entry.Value as FormsAuthenticationTicket;
                    if (currentTicket != null)
                    {
                        yield return currentTicket.UserData;
                    }
                }
            }
        }

        public override IEnumerable<string> GetUserCasTickets(string netId)
        {
            IDictionaryEnumerator enumerator = cache.GetEnumerator();
            while (enumerator.MoveNext())
            {
                string currentKey = enumerator.Entry.Key as string;
                if (currentKey != null && currentKey.StartsWith(CacheTicketKeyPrefix))
                {
                    FormsAuthenticationTicket currentTicket = enumerator.Entry.Value as FormsAuthenticationTicket;
                    if (currentTicket != null && string.Compare(currentTicket.Name, netId, true) == 0)
                    {
                        yield return currentTicket.UserData;
                    }
                }
            }
        }

        public override IEnumerable<string> GetAllTicketedUsers()
        {
            List<string> result = new List<string>();
            IEnumerable<FormsAuthenticationTicket> tickets = GetAllTickets();
            foreach (FormsAuthenticationTicket ticket in tickets)
            {
                if (!result.Contains(ticket.Name))
                {
                    result.Add(ticket.Name);
                }
            }
            return result.ToArray();
        }

        public override bool VerifyClientTicket(FormsAuthenticationTicket incomingFormsTicket)
        {
            if (incomingFormsTicket == null)
            {
                throw new ArgumentNullException("incomingFormsTicket");
            }

            string incomingCasTicket = incomingFormsTicket.UserData;
            FormsAuthenticationTicket cacheFormsTicket = GetTicket(incomingCasTicket);
            if (cacheFormsTicket != null)
            {
                string cacheCasTicket = cacheFormsTicket.UserData;
                if (cacheCasTicket == incomingCasTicket)
                {
                    if (incomingFormsTicket.Expired)
                    {
                        if (Log.IsDebugEnabled)
                        {
                            Log.DebugFormat("{0}:Ticket [{1}] presented is already expired but did resolved to a ticket in the cache.  Removing from cache.", CommonUtils.MethodName, incomingCasTicket);
                        }
                        RevokeTicket(incomingCasTicket);
                        return false;
                    }

                    if (cacheFormsTicket.Expired)
                    {
                        if (Log.IsDebugEnabled)
                        {
                            Log.DebugFormat("{0}:Ticket [{1}] resolved to an expired ticket in the cache.  Removing from cache.", CommonUtils.MethodName, incomingCasTicket);
                        }
                        RevokeTicket(incomingCasTicket);
                        return false;
                    }

                    if (string.Compare(cacheFormsTicket.Name, incomingFormsTicket.Name, true) == 0)
                    {
                        if (Log.IsDebugEnabled)
                        {
                            Log.DebugFormat("{0}:Ticket [{1}] successfully verified for username [{2}]", CommonUtils.MethodName, incomingCasTicket, incomingFormsTicket.Name);
                        }
                        return true;
                    }

                    if (Log.IsWarnEnabled)
                    {
                        Log.WarnFormat("{0}:Ticket [{1}] found in cache but expected username [{2}] does not match cached ticket username [{3}].  Server key compromised?", CommonUtils.MethodName, incomingCasTicket, incomingFormsTicket.Name, cacheFormsTicket.Name);
                    }
                    return false;
                }
            }
            else
            {
                if (Log.IsDebugEnabled)
                {
                    Log.DebugFormat("{0}:Ticket [{1}] not found in cache.  Never existed, expired, or removed via Single Signout", CommonUtils.MethodName, incomingCasTicket);
                }
                return false;
            }
            return false;
        }

        private static string GetTicketKey(string casTicket)
        {
            return CacheTicketKeyPrefix + casTicket;
        }

        private static string GetTicketKey(FormsAuthenticationTicket ticket)
        {
            string authoritativeCasTicket = ticket.UserData;
            return CacheTicketKeyPrefix + authoritativeCasTicket;            
        }
    }
}
