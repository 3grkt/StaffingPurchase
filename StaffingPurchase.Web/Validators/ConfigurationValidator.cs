using FluentValidation;
using StaffingPurchase.Services.Localization;
using StaffingPurchase.Web.Models.Configurations;

namespace StaffingPurchase.Web.Validators
{
    public class ConfigurationValidator : AbstractValidator<ConfigurationModel>
    {
        public ConfigurationValidator(IResourceManager resourceManager)
        {
            // Maxlength
            const int maxLength = 9;
            RuleFor(x => x.Value)
                .Length(1, maxLength)
                .WithMessage(string.Format(resourceManager.GetString("Policy.Validation.MaxLengthExceed"), maxLength));

            // Valid number
            RuleFor(x => x.Value)
                .Must(IsPositiveNumber)
                .WithMessage(resourceManager.GetString("Policy.Validation.PositiveNumberRequired"));

            // Day of month
            RuleFor(x => x.Value)
                .Must(IsValidDayOfMonth)
                .When(x => x.Name == "OrderSessionStartDayOfMonth" || x.Name == "OrderSessionEndDayOfMonth")
                .WithMessage(resourceManager.GetString("Policy.Validation.StartOrEndDayInvalid"));
        }

        private bool IsPositiveNumber(string value)
        {
            int number;
            bool result = int.TryParse(value, out number);
            if (result)
            {
                result = number >= 0;
            }
            return result;
        }

        private bool IsValidDayOfMonth(string value)
        {
            int number;
            bool result = int.TryParse(value, out number);
            if (result)
            {
                result = number >= 1 && number <= 31;
            }
            return result;
        }
    }
}
