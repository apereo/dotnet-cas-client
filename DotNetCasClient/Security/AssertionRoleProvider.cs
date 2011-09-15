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
        /// Get all the roles we know about from the current user's CAS assertion.
        /// This operation is synonymous with <see cref="GetRolesForUser"/>.
        /// </summary>
        /// <returns>List of all roles belonging to current user.</returns>
        public override string[] GetAllRoles()
        {
            IList<string> roles = GetCurrentUserRoles();
            if (roles is Array)
            {
                return (string[])roles;
            }
            string[] roleArray = new string[roles.Count];
            for (int i = 0; i < roles.Count; i++)
            {
                roleArray[i] = roles[i];
            }
            return roleArray;
        }

        public override string[] GetRolesForUser(string username)
        {
            if (CasAuthentication.CurrentPrincipal.Identity.Name != username)
            {
                throw new ProviderException("Cannot fetch roles for user other than that of current context.");
            }
            return GetAllRoles();
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
