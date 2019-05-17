using System;
using System.Globalization;
using System.Web.Mvc;
using StaffingPurchase.Core;
using StaffingPurchase.Core.Infrastructure;

namespace StaffingPurchase.Web.Extensions
{
    public class DateTimeModelBinderExtention : DefaultModelBinder
    {
        private readonly string _customFormat;

        public DateTimeModelBinderExtention()
        {
            _customFormat = EngineContext.Current.Resolve<IWebHelper>().DateFormat;
        }

        public override object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            var value = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);
            if (value == null)
                return DateTime.Now;
            return DateTime.ParseExact(value.AttemptedValue, _customFormat, CultureInfo.InvariantCulture);
        }
    }
}