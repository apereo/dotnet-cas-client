using System;
using DotNetCasClient;

namespace Restricted.AuthenticatedUsersOnly
{
    public partial class Default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (CasAuthentication.ServiceTicketManager != null)
            {
                YourTickets.DataSource = CasAuthentication.ServiceTicketManager.GetUserServiceTickets(User.Identity.Name);
                YourTickets.DataBind();
            }
        }
    }
}