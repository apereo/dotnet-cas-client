﻿/*
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

#pragma warning disable 1591

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration.Provider;
using System.Text;
using System.Security.Principal;
using System.Web;
using System.Web.Security;

namespace DotNetCasClient.Security
{
    public class AssertionRoleProvider : RoleProvider
    {
        public const string ROLE_ATTRIBUTE_NAME = "roleAttributeName";

        private readonly static IList<string> EMPTY_LIST = new List<string>(0).AsReadOnly();

        private string roleAttribute;

        public override string ApplicationName
        {
            get { throw new NotSupportedException(); }
            set { throw new NotSupportedException(); }
        }

        public override void AddUsersToRoles(string[] usernames, string[] roleNames)
        {
            throw new NotImplementedException();
        }

        public override void Initialize(string name, NameValueCollection config)
        {
            if (config == null)
            {
                throw new ArgumentNullException("config");
            }

            // Assign the provider a default name if it doesn't have one
            if (String.IsNullOrEmpty(name))
            {
                name = "CasAssertionRoleProvider";
            }
            base.Initialize(name, config);

            roleAttribute = config[ROLE_ATTRIBUTE_NAME];
            if (roleAttribute == null)
            {
                throw new ProviderException(ROLE_ATTRIBUTE_NAME + " is required but has not been provided.");
            }
            if (roleAttribute == string.Empty)
            {
                throw new ProviderException(ROLE_ATTRIBUTE_NAME + " roleAttribute must be non-empty string.");
            }
        }

        public override void CreateRole(string roleName)
        {
            throw new NotImplementedException();
        }

        public override bool DeleteRole(string roleName, bool throwOnPopulatedRole)
        {
            throw new NotImplementedException();
        }

        public override string[] FindUsersInRole(string roleName, string usernameToMatch)
        {
            throw new NotImplementedException();
        }

		/// <summary>
		/// Gets a list of all the roles for the configured applicationName.
		/// </summary>
		/// <returns>A string array containing the names of all the roles stored in the data
		/// source for the configured applicationName.</returns>
		/// <remarks>This method will always throw a <see cref="NotImplementedException"/> as the
		/// CAS client is not able to retrieve a list of all roles from the CAS server.</remarks>
		public override string[] GetAllRoles()
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Gets a list of role names of which the specified user is a member.
		/// </summary>
		/// <param name="username">The user to get roles for</param>
		/// <returns>A string array containing the names of all the roles</returns>
		public override string[] GetRolesForUser(string username)
		{
			if (CasAuthentication.CurrentPrincipal.Identity.Name != username)
			{
				// Role membership is provided by the CAS server, not this client, so it is
				// impossible to determine the role membership for other identities
				throw new ProviderException("Cannot fetch roles for user other than that of current context.");
			}

			// Call the private method to get all roles for the specified (current) user
			IList<string> roles = GetCurrentUserRoles();

			if (roles is Array)
			{
				// The roles list can be directly cast to a string array and returned to the caller
				return (string[])roles;
			}

			// The elements of the roles list must be manually copied into a new string array
			// that can be returned to the caller
			string[] roleArray = new string[roles.Count];
			for (int i = 0; i < roles.Count; i++)
			{
				roleArray[i] = roles[i];
			}
			return roleArray;
		}

		public override string[] GetUsersInRole(string roleName)
        {
            throw new NotImplementedException();
        }

        public override bool IsUserInRole(string username, string roleName)
        {
            if (CasAuthentication.CurrentPrincipal.Identity.Name != username)
            {
                throw new ProviderException("Cannot fetch roles for user other than that of current context.");
            }
            return GetCurrentUserRoles().Count > 0;
        }

        public override void RemoveUsersFromRoles(string[] usernames, string[] roleNames)
        {
            throw new NotImplementedException();
        }

        public override bool RoleExists(string roleName)
        {
            throw new NotImplementedException();
        }

        private IList<string> GetCurrentUserRoles()
        {
            ICasPrincipal principal = CasAuthentication.CurrentPrincipal;
            if (principal == null)
            {
                return EMPTY_LIST;
            }
            IList<string> roles = principal.Assertion.Attributes[roleAttribute];
            if (roles == null)
            {
                roles = EMPTY_LIST;
            }
            return roles;
        }

    }
}

#pragma warning restore 1591