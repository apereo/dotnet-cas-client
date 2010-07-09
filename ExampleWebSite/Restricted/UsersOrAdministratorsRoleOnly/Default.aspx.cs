/*
 * Copyright 2007-2010 The JA-SIG Collaborative. All rights reserved. See license
 * distributed with this file and available online at
 * http://www.ja-sig.org/products/cas/overview/license/index.html
 */

using System;
using System.Text;
using System.Web.Security;
using DotNetCasClient;
using DotNetCasClient.Utils;
using DotNetCasClient.Validation;

namespace Restricted.UsersOrAdministratorsRoleOnly
{
    public partial class RestrictedAuthenticatedUsersOnlyDefault : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                FormsAuthenticationTicket formsAuthTicket = CasAuthentication.GetFormsAuthenticationTicket();
                CasAuthenticationTicket casTicket = CasAuthentication.ServiceTicketManager.GetTicket(formsAuthTicket.UserData);

                string validateUrl = EnhancedUriBuilder.Combine(CasAuthentication.CasServerUrlPrefix, "proxyValidate");

                Uri url = new UriBuilder(Request.Url.Scheme, Request.Url.DnsSafeHost, Request.Url.Port, ResolveUrl("DotNetCasProxyDemoApp.application")).Uri;
                string proxyGrantingTicket = casTicket.ProxyGrantingTicket;
                string proxyUrl = UrlUtil.ConstructProxyTicketRequestUrl(casTicket.ProxyGrantingTicket, url.AbsoluteUri);

                string ticket;
                try
                {
                    ticket = CasAuthentication.GetProxyTicketIdFor(url.AbsoluteUri);
                }
                catch (InvalidOperationException ioe)
                {
                    ticket = "Invalid Request: " + ioe.Message;
                }
                catch (TicketValidationException tve)
                {
                    ticket = "Ticket Exception: " + tve.Message;
                }

                string clickOnceValidation = validateUrl + "?service=" + Server.UrlEncode(url.AbsoluteUri) + "&proxyTicket=" + ticket;
                string appUrl = new UriBuilder(Request.Url.Scheme, Request.Url.DnsSafeHost, Request.Url.Port, ResolveUrl("DotNetCasProxyDemoApp.application"), "?proxyTicket=" + ticket + "&verifyUrl=" + Server.UrlEncode(validateUrl)).Uri.AbsoluteUri;

                StringBuilder debugText = new StringBuilder();
                debugText.AppendLine("Your PGT");
                debugText.AppendLine(proxyGrantingTicket);
                debugText.AppendLine();

                debugText.AppendLine("Target Service URL");
                debugText.AppendLine(url.AbsoluteUri);
                debugText.AppendLine();

                debugText.AppendLine("Proxy Ticket URL");
                debugText.AppendLine(proxyUrl);
                debugText.AppendLine();
                
                debugText.AppendLine("Proxy Ticket");
                debugText.AppendLine(ticket);
                debugText.AppendLine();

                debugText.AppendLine("Validate URL");
                debugText.AppendLine(validateUrl);
                debugText.AppendLine();

                debugText.AppendLine("ClickOnce URL");
                debugText.AppendLine(appUrl);
                debugText.AppendLine();

                debugText.AppendLine("ClickOnce Validation");
                debugText.AppendLine(clickOnceValidation);

                DebugField.Text = debugText.ToString();
                ClickOnceUrl.Text = appUrl;
            }
        }

        protected void GetProxyTicketButton_Click(object sender, EventArgs e)
        {
            Response.Redirect(ClickOnceUrl.Text, false);
        }
    }
}