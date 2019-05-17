using FluentValidation;
using StaffingPurchase.Core.Infrastructure;
using StaffingPurchase.Services.Localization;
using StaffingPurchase.Services.Products;
using StaffingPurchase.Web.Models.Product;

namespace StaffingPurchase.Web.Validators
{
    public class ProductValidator : AbstractValidator<ProductModel>
    {
        public ProductValidator(IResourceManager resourceManager)
        {
            RuleFor(x => x.Sku)
                .NotEmpty().WithMessage(resourceManager.GetString("Common.Validation.Required"))
                .Must((p, sku) => !EngineContext.Current.Resolve<IProductService>().IsSkuExisted(p.Id, sku.Trim())).WithMessage(resourceManager.GetString("Product.Validation.SkuExisted"));

            RuleFor(x => x.Name).NotEmpty().WithMessage(resourceManager.GetString("Common.Validation.Required"));

            RuleFor(x => x.CategoryId).NotEmpty().WithMessage(resourceManager.GetString("Common.Validation.Required"));
        }
    }
}
