/*
 * Licensed to Jasig under one or more contributor license
 * agreements. See the NOTICE file distributed with this work
 * for additional information regarding copyright ownership.
 * Jasig licenses this file to you under the Apache License,
 * Version 2.0 (the "License"); you may not use this file
 * except in compliance with the License. You may obtain a
 * copy of the License at:
 * 
 * http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing,
 * software distributed under the License is distributed on
 * an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
 * KIND, either express or implied. See the License for the
 * specific language governing permissions and limitations
 * under the License.
 */

using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Net;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DotNetCasClient;

namespace Controls
{
    public partial class TicketManager : UserControl
    {
        protected void OutstandingTickets_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            int index = int.Parse(e.CommandArgument.ToString());
            string ticket = null;
            if (OutstandingTickets != null && OutstandingTickets.DataKeys != null && OutstandingTickets.DataKeys[index] != null)
            {
                ticket = (OutstandingTickets.DataKeys[index].Values["ServiceTicket"].ToString());
            }

            bool isMyTicket = false;
            IEnumerable<string> allMyTickets = CasAuthentication.ServiceTicketManager.GetUserServiceTickets(HttpContext.Current.User.Identity.Name);
            foreach (string myTicket in allMyTickets)
            {
                if (myTicket == ticket)
                {
                    isMyTicket = true;
                    break;
                }
            }

            if (e.CommandName == "Revoke")
            {
                CasAuthentication.ServiceTicketManager.RevokeTicket(ticket);
                if (isMyTicket)
                {
                    CasAuthentication.ClearAuthCookie();
                }
                Page.Response.Redirect(Request.RawUrl, false);
            }
            else if (e.CommandName == "SSO")
            {
                string samlString =
                    @"<samlp:LogoutRequest ID=""" + new Random().Next(10000) + @""" Version=""2.0"" IssueInstant=""" + DateTime.Now + @""">" +
                    @"<saml:NameID>@NOT_USED@</saml:NameID>" +
                    @"<samlp:SessionIndex>" + ticket + "</samlp:SessionIndex>" +
                    @"</samlp:LogoutRequest>";

                string request = "logoutRequest=" + Server.UrlEncode(samlString);

                HttpWebRequest req = (HttpWebRequest)WebRequest.Create(Page.Request.Url.ToString());
                req.Method = "POST";
                req.ContentType = "application/x-www-form-urlencoded";
                req.ContentLength = request.Length;
                using (Stream requestStream = req.GetRequestStream())
                {
                    using (StreamWriter requestStreamWriter = new StreamWriter(requestStream))
                    {
                        requestStreamWriter.Write(request);
                    }
                }
            
                HttpWebResponse resp = (HttpWebResponse)req.GetResponse();
                using (Stream responseStream = resp.GetResponseStream())
                {
                    using (StreamReader responseStreamReader = new StreamReader(responseStream))
                    {
                        string responseBody = responseStreamReader.ReadToEnd();
                        ResponseField.Text = responseBody;
                    }
                }

                Response.Redirect(Request.RawUrl);
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                BindGrid();
            }
        }

        void BindGrid()
        {
            if (CasAuthentication.ServiceTicketManager != null)
            {
                DataTable tickets = new DataTable();
                tickets.Columns.Add("NetId", typeof(string));
                tickets.Columns.Add("ServiceTicket", typeof(string));
                tickets.Columns.Add("ValidFrom", typeof(DateTime));
                tickets.Columns.Add("ValidUntil", typeof(DateTime));
                tickets.Columns.Add("Expired", typeof(bool));

                IEnumerable<CasAuthenticationTicket> allTickets = CasAuthentication.ServiceTicketManager.GetAllTickets();
                foreach (CasAuthenticationTicket ticket in allTickets)
                {
                    DataRow dr = tickets.NewRow();
                    dr["NetId"] = ticket.NetId;
                    dr["ServiceTicket"] = ticket.ServiceTicket;
                    dr["ValidFrom"] = ticket.ValidFromDate;
                    dr["ValidUntil"] = ticket.ValidUntilDate;
                    dr["Expired"] = ticket.Expired;
                    tickets.Rows.Add(dr);
                }
                OutstandingTickets.DataSource = tickets;
                OutstandingTickets.DataBind();

                OutstandingTickets.Visible = true;
                NoTicketManagerLabel.Visible = false;
            }
            else
            {
                OutstandingTickets.Visible = false;
                NoTicketManagerLabel.Visible = true;
                NoTicketManagerLabel.Text = "You need to have your CAS provider configured with formsAuthenticationStateProvider='CacheAuthenticationStateProvider' in order to use the Ticket Manager";
            }
        }
    }
}