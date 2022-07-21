using System;
using System.Linq;
using System.Web.Mvc;
using System.Web.Security;
using Pluritech.Authentication.Abstract;
using Pluritech.Services;

namespace Flexpage.Providers
{
    public class PwRoleProvider : RoleProvider
    {
        private IRoleProvider _roleProvider
        {
            get
            {
                return DependencyResolver.Current.GetService<IRoleProvider>();
            }
        }

        public override void Initialize(string name, System.Collections.Specialized.NameValueCollection config)
        {
            if (config == null)
                throw new ArgumentNullException("config");

            if (name == null || name.Length == 0)
                name = "PwRoleProvider";

            if (String.IsNullOrEmpty(config["description"]))
            {
                config.Remove("description");
                config.Add("description", "Pluriworks Distribution List Role provider");
            }

            base.Initialize(name, config);
        }

        public override string[] GetAllRoles()
        {
            return _roleProvider.GetAllRoles();
        }

        public override string[] GetRolesForUser(string username)
        {
            return _roleProvider.GetRolesForUser(username);
        }

        public override string[] GetUsersInRole(string roleName)
        {
            return _roleProvider.GetUsersInRole(roleName);
        }

        public override bool IsUserInRole(string username, string roleName)
        {
            return _roleProvider.IsUserInRole(username, roleName);
        }

        public override void AddUsersToRoles(string[] usernames, string[] roleNames)
        {
            //Not implemented. Just nothing to do
        }

        public override void CreateRole(string roleName)
        {
            //Not implemented. Just nothing to do
        }

        public override bool DeleteRole(string roleName, bool throwOnPopulatedRole)
        {
            //Not implemented. Just nothing to do
            return true;
        }

        public override string[] FindUsersInRole(string roleName, string usernameToMatch)
        {
            return GetUsersInRole(roleName).Where(x => x.Equals(usernameToMatch)).ToArray();
        }

        public override void RemoveUsersFromRoles(string[] usernames, string[] roleNames)
        {
            //Not implemented. Just nothing to do
        }

        public override bool RoleExists(string roleName)
        {
            //Not implemented. Just nothing to do
            return true;
        }

        public override string ApplicationName { get; set; }
    }
}
