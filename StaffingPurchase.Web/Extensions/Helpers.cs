using StaffingPurchase.Core.Infrastructure;
using StaffingPurchase.Services.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace StaffingPurchase.Web.Extensions
{
    /// <summary>
    /// Contains extension methods for Enum.
    /// </summary>
    public class WebUtility
    {
        /// <summary>
        /// Converts Enum to select list.
        /// </summary>
        /// <remarks>
        /// The 'Text' property is get from resource file. Resouce key has format of Enums.'EnumType'.'Value'.</remarks>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static IList<SelectListItem> ConvertEnumToSelectList<T>(bool includeEmptyValue = false)
            where T : struct, IFormattable
        {
            var list = new List<SelectListItem>();
            var enumType = typeof(T);
            var resourcePrefix = "Enums." + enumType.Name + ".";
            var resourceManager = EngineContext.Current.Resolve<IResourceManager>();

            if (includeEmptyValue)
            {
                list.InsertEmptyValue();
            }

            foreach (var value in Enum.GetValues(enumType).Cast<T>())
            {
                list.Add(new SelectListItem()
                {
                    Text = resourceManager.GetString(resourcePrefix + value.ToString()),
                    Value = value.ToString("D", System.Globalization.CultureInfo.InvariantCulture),
                });
            }
            return list;
        }

        public static IList<SelectListItem> ConvertDictionaryToSelectList(Dictionary<string, string> data,
            bool includeEmptyValue = false)
        {
            var list = new List<SelectListItem>();
            if (includeEmptyValue)
            {
                list.InsertEmptyValue();
            }
            foreach (var item in data)
            {
                list.Add(new SelectListItem()
                {
                    Text = item.Value,
                    Value = item.Key
                });
            }
            return list;
        }

        /// <summary>
        /// Gets resource key for enum.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string GetEnumResourceKey<T>(T value)
            where T : struct
        {
            return string.Format("Enums.{0}.{1}", typeof(T).Name, value.ToString());
        }

        /// <summary>
        /// Gets localized string for enum.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string GetLocalizedStringForEnum<T>(T value)
            where T : struct
        {
            return EngineContext.Current.Resolve<IResourceManager>().GetString(GetEnumResourceKey(value));
        }
    }

    public static class CommonExtensions
    {
        /// <summary>
        /// Inserts empty value to list of SelectListItem.
        /// </summary>
        /// <param name="list"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static IList<SelectListItem> InsertEmptyValue(this IList<SelectListItem> list, string value = "")
        {
            list.Insert(0, new SelectListItem()
            {
                Text = EngineContext.Current.Resolve<IResourceManager>().GetString("Common.All"),
                Value = value,
            });
            return list;
        }
    }
}
