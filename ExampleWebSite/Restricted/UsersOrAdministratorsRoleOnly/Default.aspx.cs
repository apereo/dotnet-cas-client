using System;
using System.Web;
using DotNetCasClient;
using DotNetCasClient.Utils;
using DotNetCasClient.Validation;

namespace Restricted.UsersOrAdministratorsRoleOnly
{
    public partial class RestrictedAuthenticatedUsersOnlyDefault : System.Web.UI.Page
    {
        private string url;

        protected void Page_Load(object sender, EventArgs e)
        {
            string prefix = Request.Url.Scheme + "://" + Request.Url.DnsSafeHost + (!Request.Url.IsDefaultPort ? ":" + Request.Url.Port : string.Empty);
            url = prefix + ResolveUrl("DotNetCasProxyDemoApp.application");

            TargetUrl.Text = url;
        }

        protected void GetProxyTicketButton_Click(object sender, EventArgs e)
        {
            try
            {
                string ticket = CasAuthentication.GetProxyTicketIdFor(url);
                string appUrl = "DotNetCasProxyDemoApp.application?ticket=" + ticket + "&verifyUrl=" + Server.UrlEncode(EnhancedUriBuilder.Combine(CasAuthentication.CasServerUrlPrefix, "proxyValidate"));

                Response.Redirect(appUrl, false);
            } 
            catch (InvalidOperationException ioe)
            {
                ProxyTicket.Text = "Invalid Request: " + ioe.Message;
            }
            catch (TicketValidationException tve)
            {
                ProxyTicket.Text = "Ticket Exception: " + tve.Message;
            }
        }
    }
}