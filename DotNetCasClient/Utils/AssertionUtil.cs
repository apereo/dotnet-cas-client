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
using System.Web;

namespace DotNetCasClient.Utils
{
    /// <summary>
    /// Public utility class with helper methods for common operations on assertions.
    /// Arguably the most common operation is retrieving attributes provided by the CAS
    /// ticket validation response.
    /// </summary>
    /// <author>Marvin S. Addison</author>
    public sealed class AssertionUtil
    {
        /// <summary>
        /// Gets a list of values for the given attribute from the CAS assertion bound to the
        /// first active ticket of the authenticated user.
        /// </summary>
        /// <param name="attributeName">Attribute name</param>
        /// <returns>List of attribute values. An empty list is returned if the attribute does not exist.</returns>
        public static IList<string> GetAttributes(string attributeName)
        {
            foreach (CasAuthenticationTicket ticket in CasAuthentication.ServiceTicketManager.GetUserTickets(HttpContext.Current.User.Identity.Name))
            {
                if (!ticket.Expired)
                {
                    return ticket.Assertion.Attributes[attributeName];
                }
            }
            return new string[0];
        }

        /// <summary>
        /// Gets the first value of the given attribute from the CAS assertion bound to the
        /// first active ticket of the authenticated user.
        /// </summary>
        /// <param name="attributeName">Attribute name</param>
        /// <returns>List of attribute values. An empty list is returned if the attribute does not exist.</returns>
        public static string GetAttribute(string attributeName)
        {
            IList<string> attributes = GetAttributes(attributeName);
            return attributes.Count > 0 ? attributes[0] : null;
        }
    }
}
