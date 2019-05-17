using FluentValidation;
using StaffingPurchase.Services.Localization;
using StaffingPurchase.Web.Models.Order;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace StaffingPurchase.Web.Validators
{
    public class OrderDetailValidator : AbstractValidator<OrderDetailModel>
    {
        public OrderDetailValidator(IResourceManager resourceManager)
        {
            RuleFor(x => x.Volume).LessThanOrEqualTo(3).WithMessage(resourceManager.GetString("OrderDetail.Validation.ProductVolume"));
        }
    }
}