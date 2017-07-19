/*
 * Licensed to Apereo under one or more contributor license
 * agreements. See the NOTICE file distributed with this work
 * for additional information regarding copyright ownership.
 * Apereo licenses this file to you under the Apache License,
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
using System.Text;
using DotNetCasClient.Security;

namespace DotNetCasClient
{
    /// <summary>
    /// Data object representing a Cas Authentication Ticket.  The ServiceTicket,
    /// stored on the client in the UserData field of a FormsAuthenticationTicket,
    /// is used as a key to retrieve the information in this class from a 
    /// ServiceTicketManager instance.  Without a ServiceTicketManager configured, the 
    /// CasAuthententicationTicket cannot be retrieved.
    /// </summary>
    /// <author>Scott Holodak</author>
    [Serializable]
    public sealed class CasAuthenticationTicket
    {
        /// <summary>
        /// The NetId username used to authenticate against the CAS server.  This
        /// information is retrieved via ticket validation and should not from 
        /// the web application.
        /// </summary>
        public string NetId { get; private set; }

        /// <summary>
        /// The CAS Service Ticket returned from the CAS Server (typically as the 
        /// ticket parameter in the URL).
        /// </summary>
        public string ServiceTicket { get; private set; }

        /// <summary>
        /// The Proxy Granting Ticket IOU used to retrieve a Proxy ticket.
        /// </summary>
        public string ProxyGrantingTicketIou { get; set; }

        /// <summary>
        /// The Proxy Granting Ticket used to generate Proxy tickets
        /// </summary>
        public string ProxyGrantingTicket { get; set; }

        /// <summary>
        /// The Proxy path associated with the user
        /// </summary>
        public List<string> Proxies { get; private set; }

        /// <summary>
        /// The ServiceName used during the initial authentication and ticket 
        /// validation.  When a Single Sign Out request is received from the CAS s
        /// server, this is used as a safety mechanism to confirm that the CAS 
        /// server's SSO request is valid.
        /// </summary>
        public string OriginatingServiceName { get; private set; }

        /// <summary>
        /// The IP address of the client that originally requested the CAS Service 
        /// Ticket.  By tracking IP addresses, this enables applications with a 
        /// ServiceTicketManager configured to detect and/or prevent multiple logins by a 
        /// user from different IP addresses.  
        /// </summary>
        public string ClientHostAddress { get; private set; }

        /// <summary>
        /// The CAS assertion associated with the service ticket.  This contains the 
        /// principal name, the validity date/times, and the attributes associated
        /// with the service ticket.
        /// </summary>
        public IAssertion Assertion { get; private set; }

        /// <summary>
        /// The ValidFromDate associated with the ticket.  This is derived from, but 
        /// not the same as the Assertion's ValidFromDate.  If the Assertion's 
        /// ValidFromDate is equal to DateTime.MinValue, the CasAuthenticationTicket's 
        /// ValidFromDate is set to DateTime.Now.
        /// </summary>
        public DateTime ValidFromDate { get; private set; }

        /// <summary>
        /// The ValidUntilDate associated with the ticket.  This is derived from, but
        /// not the same as the Assertion's ValidUntilDate.  If the Assertion's 
        /// ValidUntilDate is equal to DateTime.MinValue, the following rule is 
        /// applied:  If the Assertion's ValidUntilDate is in the future, the 
        /// ValidUntilDate is used as is.  If the Assertion's ValidUntilDate is in 
        /// the past, the ValidFromDate + FormsAuthentication.Timeout timespan is
        /// used.
        /// </summary>
        public DateTime ValidUntilDate { get; private set; }

        /// <summary>
        /// Readonly property which indicates whether or not the ValidUntilDate is in
        /// the past (i.e., the ticket is expired).  Expired tickets should/will be 
        /// purged from the ServiceTicketManager during the RemoveExpiredTickets() call,
        /// during the BeginRequest event handler.
        /// </summary>
        public bool Expired
        {
            get
            {
                return (DateTime.Now.CompareTo(ValidUntilDate) > 0);
            }
        }

        /// <summary>
        /// Empty constructor (to be used during Serialization/Deserialization)
        /// </summary>
        private CasAuthenticationTicket()
        {
            CasAuthentication.Initialize();
            Proxies = new List<string>();
        }

        /// <summary>
        /// Public CasAuthenticationTicket constructor
        /// </summary>
        /// <param name="serviceTicket">CAS Service Ticket associated with this CasAuthenticationTicket</param>
        /// <param name="originatingServiceName">ServiceName used during CAS authentication/validation</param>
        /// <param name="clientHostAddress">IP address of the client initiating the authentication request</param>
        /// <param name="assertion">CAS assertion returned from the CAS server during ticket validation</param>
        public CasAuthenticationTicket(string serviceTicket, string originatingServiceName, string clientHostAddress, IAssertion assertion)
        {
            CasAuthentication.Initialize();
            Proxies = new List<string>();

            NetId = assertion.PrincipalName;
            ServiceTicket = serviceTicket;
            OriginatingServiceName = originatingServiceName;
            ClientHostAddress = clientHostAddress;
            Assertion = assertion;

            if (DateTime.MinValue.CompareTo(assertion.ValidFromDate) != 0)
            {
                ValidFromDate = assertion.ValidFromDate;
            }
            else
            {
                ValidFromDate = DateTime.Now;
            }

            DateTime localValidUntil = ValidFromDate.Add(CasAuthentication.FormsTimeout);            
            if (DateTime.MinValue.CompareTo(assertion.ValidUntilDate) != 0)
            {
                ValidUntilDate = localValidUntil.CompareTo(assertion.ValidUntilDate) < 0 ? localValidUntil : assertion.ValidUntilDate;
            }
            else
            {
                ValidUntilDate = localValidUntil;
            }
        }

        /// <summary>
        /// Exposes the CasAuthenticationTicket and all related properties as a multi-line string.
        /// </summary>
        /// <returns>A string representation of the CasAuthenticationTicket for use in debugging</returns>
        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();

            builder.AppendFormat("[{0}]{1}", ServiceTicket, Environment.NewLine);
            builder.AppendFormat("  NetID............. {0}{1}", NetId, Environment.NewLine);
            builder.AppendLine  ("  Proxy.............");
            builder.AppendFormat("    PGT IOU......... {0}{1}", ProxyGrantingTicket ?? string.Empty, Environment.NewLine);
            builder.AppendFormat("    PGT............. {0}{1}", ProxyGrantingTicketIou ?? string.Empty, Environment.NewLine);
            builder.AppendLine  ("    Proxy Tickets...");
            foreach (string proxy in Proxies)
            {
                builder.AppendFormat("      Proxy......... {0}{1}", proxy, Environment.NewLine);
            }
            builder.AppendFormat("  Origin Service.... {0}{1}", OriginatingServiceName, Environment.NewLine);
            builder.AppendFormat("  Client Address.... {0}{1}", ClientHostAddress, Environment.NewLine);
            builder.AppendFormat("  Valid From........ {0}{1}", ValidFromDate, Environment.NewLine);
            builder.AppendFormat("  Valid Until....... {0}{1}{2}", ValidUntilDate, (Expired ? " (Expired!)" : string.Empty), Environment.NewLine);
            builder.AppendLine  ("  Assertion.........");
            builder.AppendFormat("    Principal....... {0}{1}", Assertion.PrincipalName, Environment.NewLine);
            builder.AppendFormat("    Valid From...... {0}{1}", Assertion.ValidFromDate, Environment.NewLine);
            builder.AppendFormat("    Valid Until..... {0}{1}", Assertion.ValidUntilDate, Environment.NewLine);
            builder.AppendLine  ("    Attributes......");
            foreach (KeyValuePair<string, IList<string>> attribute in Assertion.Attributes)
            {
                builder.AppendLine("      " + attribute.Key);
                foreach (string valuePart in attribute.Value)
                {
                    builder.AppendLine("        " + valuePart);
                }
            }
            return builder.ToString();
        }
    }
}