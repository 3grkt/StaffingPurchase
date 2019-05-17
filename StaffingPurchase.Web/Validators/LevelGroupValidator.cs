using FluentValidation;
using StaffingPurchase.Services.Localization;
using StaffingPurchase.Web.Models.LevelGroup;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace StaffingPurchase.Web.Validators
{
    public class LevelGroupValidator: AbstractValidator<LevelGroupModel>
    {
        public LevelGroupValidator(IResourceManager resourceManager)
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage(resourceManager.GetString("Common.Validation.Required"));
            RuleFor(x => x.PV).NotEmpty().WithMessage(resourceManager.GetString("Common.Validation.Required"));
        }
    }
}