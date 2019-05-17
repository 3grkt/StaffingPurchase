using System.Collections.Generic;
using System.Linq;

namespace StaffingPurchase.Core
{
    /// <summary>
    /// Represent current working user.
    /// </summary>
    public class WorkingUser
    {
        public WorkingUser()
        {
            Roles = new UserRole[] { };
            Permissions = new UserPermission[] { };
        }

        public int Id { get; set; }
        public string UserName { get; set; }
        public string FullName { get; set; }
        public int? LocationId { get; set; }
        //public string LocationName { get; set; }
        public int? DepartmentId { get; set; }

        public IList<UserRole> Roles { get; set; }
        public IList<UserPermission> Permissions { get; set; }

        /// <summary>
        /// Gets string used for logging purpose (including user id and username).
        /// </summary>
        /// <returns></returns>
        public string GetLogString()
        {
            return string.Format("{0} - {1}", Id, UserName);
        }

        public bool HasPermission(params UserPermission[] permissions)
        {
            return Permissions.Intersect(permissions).Any();
        }

        public bool IsInRole(UserRole role)
        {
            return Roles.Contains(role);
        }

        public bool IsInRole(params UserRole[] roles)
        {
            return roles.Any(userRole => Roles.Contains(userRole));
        }
    }
}
