using System;
using System.Collections.Generic;
using System.Web.Security;

namespace DotNetCasClient.State
{
    public interface IFormsAuthenticationStateProvider
    {
        FormsAuthenticationTicket GetTicket(string casTicket);
        void InsertTicket(FormsAuthenticationTicket ticket, DateTime expiration);
        void UpdateTicketExpiration(FormsAuthenticationTicket ticket, DateTime newExpiration);
        bool RevokeTicket(FormsAuthenticationTicket ticket);
        bool RevokeTicket(string casTicket);
        bool ContainsTicket(FormsAuthenticationTicket ticket);
        bool ContainsTicket(string casTicket);
        void RevokeUserTickets(string netId);
        IEnumerable<FormsAuthenticationTicket> GetAllTickets();
        IEnumerable<FormsAuthenticationTicket> GetUserTickets(string netId);
        IEnumerable<string> GetAllCasTickets();
        IEnumerable<string> GetUserCasTickets(string netId);
        IEnumerable<string> GetAllTicketedUsers();
        bool VerifyClientTicket(FormsAuthenticationTicket clientTicket);
    }
}
