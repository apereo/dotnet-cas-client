using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net;

namespace DotNetCasClient.Proxy
{
    class ProxyGrantingTicketStorage : IProxyGrantingTicketStorage
    {
        /// <summary>
        /// Access to the log file
        /// </summary>
        protected static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private long timeout = 60000;

        private readonly Hashtable cache = Hashtable.Synchronized(new Hashtable());

        public ProxyGrantingTicketStorage(long timeout)
        {
            this.timeout = timeout;
        }

        public void Save(string proxyGrantingTicketIou, string proxyGrantingTicket)
        {
            var holder = new ProxyGrantingTicketHolder(proxyGrantingTicket);

            if (log.IsDebugEnabled)
            {
                log.Debug("Saving ProxyGrantingTicketIOU and ProxyGrantingTicket combo: [" 
                    + proxyGrantingTicketIou + ", " + proxyGrantingTicket + "]");
            }
            this.cache.Add(proxyGrantingTicketIou, holder);
        }

        public string Retrieve(string proxyGrantingTicketIou)
        {
            if(proxyGrantingTicketIou == null)
            {
                log.Info("ProxyGrantingTicketIou is null, check ProxyCallbackUrl config");
                return null;
            }
            var holder = (ProxyGrantingTicketHolder) this.cache[proxyGrantingTicketIou];

            if (holder == null)
            {
                log.Info("No Proxy Ticket found for " + proxyGrantingTicketIou);
                return null;
            }

            this.cache.Remove(holder);

            if(log.IsDebugEnabled)
            {
                log.Debug("Returned ProxyGranting Ticket of " + holder.ProxyGrantingTicket);
            }
            return holder.ProxyGrantingTicket;
        }

        public void CleanUp()
        {
            lock (this.cache.SyncRoot)
            {

                foreach (DictionaryEntry entry in cache)
                {
                    ProxyGrantingTicketHolder holder = entry.Value as ProxyGrantingTicketHolder;

                    if (holder != null && holder.IsExpired(this.timeout))
                    {
                        cache.Remove(entry.Key);
                    }
                }
            }

        }

    }
}
