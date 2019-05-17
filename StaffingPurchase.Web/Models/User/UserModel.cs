using FluentValidation.Attributes;
using StaffingPurchase.Web.Validators;

namespace StaffingPurchase.Web.Models.User
{
    [Validator(typeof(UserValidator))]
    public class UserModel : ViewModelBase
    {
        public string UserName { get; set; }

        public string FullName { get; set; }

        public int LocationId { get; set; }

        public int RoleId { get; set; }
        public string RoleName { get; set; }

        public int DepartmentId { get; set; }
    }
}
