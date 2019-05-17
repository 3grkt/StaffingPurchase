using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http.ModelBinding;

namespace StaffingPurchase.Web.Extensions
{
    public static class ModelStateDictionaryExtensions
    {
        /// <summary>
        /// Gets first error message from ModelState.
        /// </summary>
        /// <param name="modelState"></param>
        /// <returns></returns>
        public static string FirstError(this ModelStateDictionary modelState)
        {
            var firstValue = modelState.Values.FirstOrDefault();
            if (firstValue != null)
            {
                var firstError = firstValue.Errors.FirstOrDefault();
                if (firstError != null)
                {
                    return !string.IsNullOrEmpty(firstError.ErrorMessage) ? firstError.ErrorMessage : firstError.Exception.Message;
                }
            }
            return string.Empty;
        }
    }
}
