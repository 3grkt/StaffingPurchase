using FluentValidation;
using StaffingPurchase.Core;
using StaffingPurchase.Core.Infrastructure;
using StaffingPurchase.Services.Localization;
using StaffingPurchase.Services.Users;
using StaffingPurchase.Web.Models.User;

namespace StaffingPurchase.Web.Validators
{
    public class UserPasswordValidator : AbstractValidator<UserPasswordModel>
    {
        private readonly IResourceManager _resourceManager;

        public UserPasswordValidator(IResourceManager resourceManager)
        {
            _resourceManager = resourceManager;

            Validate();
        }

        private void Validate()
        {
            var currentContainer = EngineContext.Current.ContainerManager;
            RuleFor(x => x.OldPassword).NotEmpty().WithMessage(_resourceManager.GetString("Common.Validation.Required"));
            RuleFor(x => x.NewPassword).NotEmpty().WithMessage(_resourceManager.GetString("Common.Validation.Required"));
            RuleFor(x => x.NewPassword)
                .Must(x => x.Length >= 6)
                .WithMessage(string.Format(_resourceManager.GetString("User.Validation.PasswordMin"), 6));
            RuleFor(x => x.NewPassword)
                .NotEqual(x => x.OldPassword)
                .WithMessage(_resourceManager.GetString("User.Validation.SameOldPassword"));
            RuleFor(x => x.VerifyNewPassword)
                .Equal(x => x.NewPassword)
                .WithMessage(_resourceManager.GetString("User.Validation.NotSameNewPassword"));
            RuleFor(x => x.OldPassword).Must(
                oldPwd =>
                    CommonHelper.HashString(oldPwd) ==
                    currentContainer.Resolve<IUserService>()
                        .GetUserById(currentContainer.Resolve<IWorkContext>().User.Id)
                        .PasswordHash)
                .WithMessage(_resourceManager.GetString("User.Validation.WrongOldPassword"));
        }
    }
}