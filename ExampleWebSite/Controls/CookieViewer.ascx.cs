/*
 * Copyright 2007-2010 The JA-SIG Collaborative. All rights reserved. See license
 * distributed with this file and available online at
 * http://www.ja-sig.org/products/cas/overview/license/index.html
 */

using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Web.Security;
using System.Web.UI.WebControls;
using DotNetCasClient;

public partial class Controls_CookieViewer : System.Web.UI.UserControl
{
    protected void Page_Load(object sender, EventArgs e)
    {
        HttpCookie ticketCookie = Request.Cookies[FormsAuthentication.FormsCookieName];
        if (ticketCookie != null)
        {
            CookieDomain.Text = ticketCookie.Domain;
            CookieExpires.Text = ticketCookie.Expires.ToString();
            CookieName.Text = ticketCookie.Name;
            CookiePath.Text = ticketCookie.Path;
            CookieSecure.Text = ticketCookie.Secure.ToString();

            if (!string.IsNullOrEmpty(ticketCookie.Value))
            {
                int i = 0;
                StringBuilder cookieValueBuilder = new StringBuilder();
                while (i < ticketCookie.Value.Length)
                {
                    string line = ticketCookie.Value.Substring(i, Math.Min(ticketCookie.Value.Length - i, 50));
                    cookieValueBuilder.Append(line + "<br />");
                    i += line.Length;
                }
                CookieValue.Text = cookieValueBuilder.ToString();
            }

            if (!string.IsNullOrEmpty(ticketCookie.Value))
            {
                FormsAuthenticationTicket ticket = FormsAuthentication.Decrypt(ticketCookie.Value);
                if (ticket != null)
                {
                    TicketCookiePath.Text = ticket.CookiePath;
                    TicketExpiration.Text = ticket.Expiration.ToString();
                    TicketExpired.Text = ticket.Expired.ToString();
                    TicketIsPersistent.Text = ticket.IsPersistent.ToString();
                    TicketIssueDate.Text = ticket.IssueDate.ToString();
                    TicketName.Text = ticket.Name;
                    TicketUserData.Text = ticket.UserData;
                    TicketVersion.Text = ticket.Version.ToString();
                }

                if (CasAuthentication.ServiceTicketManager != null)
                {
                    CasAuthenticationTicket casTicket = CasAuthentication.ServiceTicketManager.GetTicket(ticket.UserData);
                    if (casTicket != null)
                    {
                        CasNetId.Text = casTicket.NetId;
                        CasServiceTicket.Text = casTicket.ServiceTicket;
                        CasOriginatingServiceName.Text = casTicket.OriginatingServiceName;
                        CasClientHostAddress.Text = casTicket.ClientHostAddress;
                        CasValidFromDate.Text = casTicket.ValidFromDate.ToString();
                        CasValidUntilDate.Text = casTicket.ValidUntilDate.ToString();
                        ProxyGrantingTicket.Text = casTicket.ProxyGrantingTicket;
                        ProxyGrantingTicketIou.Text = casTicket.ProxyGrantingTicketIou;
                        
                        StringBuilder proxiesBuilder = new StringBuilder();
                        foreach (string proxy in casTicket.Proxies)
                        {
                            proxiesBuilder.AppendLine(proxy + "<br />");
                        }
                        Proxies.Text = proxiesBuilder.ToString();

                        AssertionPrincipalName.Text = casTicket.Assertion.PrincipalName;
                        AssertionValidFromDate.Text = casTicket.Assertion.ValidFromDate.ToString();
                        AssertionValidUntilDate.Text = casTicket.Assertion.ValidUntilDate.ToString();

                        AssertionAttributesTable.Rows.Clear();
                        string newLine = "<br />";
                        StringBuilder assertionValuesBuilder = new StringBuilder();
                        foreach (KeyValuePair<string, IList<string>> item in casTicket.Assertion.Attributes)
                        {
                            TableRow assertionRow = new TableRow();
                            
                            TableCell assertionKeyCell = new TableCell();
                            assertionKeyCell.VerticalAlign = VerticalAlign.Top;
                            assertionKeyCell.Text = item.Key;

                            TableCell assertionValuesCell = new TableCell();
                            assertionValuesCell.VerticalAlign = VerticalAlign.Top;

                            foreach (string value in item.Value)
                            {
                                assertionValuesBuilder.Append(value + newLine);
                            }
                            
                            if (assertionValuesBuilder.Length > newLine.Length)
                            {
                                assertionValuesBuilder.Length -= newLine.Length;
                            }

                            assertionValuesCell.Text = assertionValuesBuilder.ToString();

                            assertionRow.Cells.Add(assertionKeyCell);
                            assertionRow.Cells.Add(assertionValuesCell);

                            AssertionAttributesTable.Rows.Add(assertionRow);
                            assertionValuesBuilder.Length = 0;
                        }
                    }
                }
            }
        }
    }
}