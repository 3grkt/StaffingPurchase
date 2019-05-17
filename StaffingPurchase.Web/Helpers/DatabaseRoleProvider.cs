using System;
using System.Linq;
using System.Web.Security;
using StaffingPurchase.Core.Infrastructure;
using StaffingPurchase.Services.Users;

namespace StaffingPurchase.Web
{
    public class DatabaseRoleProvider : RoleProvider
    {
        private readonly IUserService _userService = EngineContext.Current.Resolve<IUserService>();

        public override void AddUsersToRoles(string[] usernames, string[] roleNames)
        {
            throw new NotImplementedException();
        }

        public override string ApplicationName
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
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

        public override string[] GetAllRoles()
        {
            throw new NotImplementedException();
        }

        public override string[] GetRolesForUser(string username)
        {
            System.Diagnostics.Debug.WriteLine("Get roles for user: " + username);
            return _userService.GetRolesByUserName(username).Select(x => x.ToString()).ToArray();
        }

        public override string[] GetUsersInRole(string roleName)
        {
            throw new NotImplementedException();
        }

        public override bool IsUserInRole(string username, string roleName)
        {
            System.Diagnostics.Debug.WriteLine("Is user in role: " + roleName);
            var userRoles = GetRolesForUser(username);
            return userRoles.Contains(roleName);
        }

        public override void RemoveUsersFromRoles(string[] usernames, string[] roleNames)
        {
            throw new NotImplementedException();
        }

        public override bool RoleExists(string roleName)
        {
            throw new NotImplementedException();
        }
    }
}