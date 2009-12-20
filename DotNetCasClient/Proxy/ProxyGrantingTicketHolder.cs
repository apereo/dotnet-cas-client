using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DotNetCasClient.Proxy
{
    class ProxyGrantingTicketHolder
    {
        public string ProxyGrantingTicket { get; private set;}

        public DateTime TimeInserted { get; private set; }

        public ProxyGrantingTicketHolder(string proxyGrantingTicket)
        {
            this.ProxyGrantingTicket = proxyGrantingTicket;
            this.TimeInserted = System.DateTime.Now;

        }

        public bool IsExpired(double timeout)
        {
            double age = (System.DateTime.Now - this.TimeInserted).TotalMilliseconds;
            if (age > timeout)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
