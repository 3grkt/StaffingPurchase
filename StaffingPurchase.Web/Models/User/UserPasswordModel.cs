using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FluentValidation.Attributes;
using StaffingPurchase.Web.Validators;

namespace StaffingPurchase.Web.Models.User
{
    [Validator(typeof (UserPasswordValidator))]
    public class UserPasswordModel: ViewModelBase
    {
        public string OldPassword { get; set; }
        public string NewPassword { get; set; }
        public string VerifyNewPassword { get; set; }
    }
}