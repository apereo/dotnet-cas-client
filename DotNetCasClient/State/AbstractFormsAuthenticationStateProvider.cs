using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Security;
using log4net;

namespace DotNetCasClient.State
{
    public abstract class AbstractFormsAuthenticationStateProvider : IFormsAuthenticationStateProvider
    {
        /// <summary>
        /// Access to the log file
        /// </summary>
        protected static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public virtual void Init(HttpApplication application)
        {
            RemoveExpiredTickets();
        }

        public abstract void RemoveExpiredTickets();
        public abstract FormsAuthenticationTicket GetTicket(string casTicket);
        public abstract void InsertTicket(FormsAuthenticationTicket ticket, DateTime expiration);
        public abstract void UpdateTicketExpiration(FormsAuthenticationTicket ticket, DateTime newExpiration);
        public abstract bool RevokeTicket(FormsAuthenticationTicket ticket);
        public abstract bool RevokeTicket(string casTicket);
        public abstract bool ContainsTicket(FormsAuthenticationTicket ticket);
        public abstract bool ContainsTicket(string casTicket);
        public abstract void RevokeUserTickets(string netId);
        public abstract IEnumerable<FormsAuthenticationTicket> GetAllTickets();
        public abstract IEnumerable<FormsAuthenticationTicket> GetUserTickets(string netId);
        public abstract IEnumerable<string> GetAllCasTickets();
        public abstract IEnumerable<string> GetUserCasTickets(string netId);
        public abstract IEnumerable<string> GetAllTicketedUsers();
        public abstract bool VerifyClientTicket(FormsAuthenticationTicket incomingTicket);
    }
}
