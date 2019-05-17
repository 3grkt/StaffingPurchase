using StaffingPurchase.Web.Models.Common;
using StaffingPurchase.Web.Models.Role;

namespace StaffingPurchase.Web.Models.User
{
    public class UserGridModel : ViewModelBase
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string FullName { get; set; }

        public DepartmentModel Department { get; set; }
        public LocationModel Location { get; set; }
        public RoleModel Role { get; set; }
        
        public string LocationName { get; set; }
        public string RoleName { get; set; }
        public string DepartmentName { get; set; }
    }
}