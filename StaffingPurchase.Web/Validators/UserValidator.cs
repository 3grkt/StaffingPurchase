using FluentValidation;
using StaffingPurchase.Services.Localization;
using StaffingPurchase.Web.Models.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace StaffingPurchase.Web.Validators
{
    public class UserValidator : AbstractValidator<UserModel>
    {
        public UserValidator(IResourceManager resourceManager)
        {
            RuleFor(x => x.UserName).NotEmpty().WithMessage(resourceManager.GetString("Common.Validation.Required"));
            RuleFor(x => x.FullName).NotEmpty().WithMessage(resourceManager.GetString("Common.Validation.Required"));
            RuleFor(x => x.LocationId)
                .GreaterThan(0)
                .WithMessage(resourceManager.GetString("Common.Validation.Required"));
            RuleFor(x => x.RoleId).GreaterThan(0).WithMessage(resourceManager.GetString("Common.Validation.Required"));
            RuleFor(x => x.DepartmentId).GreaterThan(0).WithMessage(resourceManager.GetString("Common.Validation.Required"));
            //RuleFor(x => x.UsedUser).Length(1, 200).WithMessage(
            //    resourceManager.GetString("Common.Validation.StringMaxLength"),
            //    resourceManager.GetString("Asset.UsedUser"),
            //    8);
        }
    }
}
