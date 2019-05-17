using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Web.Routing;
using StaffingPurchase.Core;
using StaffingPurchase.Core.Infrastructure;

namespace StaffingPurchase.Web.Extensions
{
    public static class HtmlExtensions
    {
        /// <summary>
        /// Gets css class setting code for active menu tab.
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="tabName"></param>
        /// <param name="includeClass"></param>
        /// <returns></returns>
        public static string GetActiveMenuTab(this HtmlHelper helper, string tabName, bool includeClass = true)
        {
            var active = helper.ViewContext.RouteData.Values["controller"].ToString() == tabName;
            return active ? (includeClass ? "class=active" : " active") : string.Empty;
        }

        /// <summary>
        /// Gets css class setting code for active menu tab.
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="tabNames"></param>
        /// <param name="includeClass"></param>
        /// <returns></returns>
        public static string GetActiveMenuTab(this HtmlHelper helper, string[] tabNames, bool includeClass = true)
        {
            var active = tabNames.Contains(helper.ViewContext.RouteData.Values["controller"].ToString());
            return active ? (includeClass ? "class=active" : " active") : string.Empty;
        }

        ///// <summary>
        ///// Gets grid pager.
        ///// </summary>
        ///// <remarks>Use Shared/_GridPager.cshtml as view.</remarks>
        ///// <param name="helper"></param>
        ///// <param name="pager"></param>
        ///// <returns></returns>
        //public static MvcHtmlString GetPager(this HtmlHelper helper, IPagination pager)
        //{
        //    if (pager == null)
        //        return MvcHtmlString.Empty;
        //    return helper.Partial("_GridPager", pager);
        //}

        /// <summary>
        /// Gets dropdown containing page size options.
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="name"></param>
        /// <param name="selectedValue"></param>
        /// <param name="htmlAttributes"></param>
        /// <returns></returns>
        public static MvcHtmlString GetPageSizeOptions(this HtmlHelper helper, string name, int selectedValue = 5,
            object htmlAttributes = null)
        {
            var options = new int[] {5, 10, 20, 30, 50};
            var list = options.Select(x => new SelectListItem()
            {
                Text = x.ToString(),
                Value = x.ToString(),
                Selected = (x == selectedValue)
            });
            return helper.DropDownList(name, list, htmlAttributes);
        }

        /// <summary>
        /// Renders readonly textbox.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TProperty"></typeparam>
        /// <param name="helper"></param>
        /// <param name="expression"></param>
        /// <returns></returns>
        public static MvcHtmlString ReadOnlyTextBoxFor<T, TProperty>(this HtmlHelper<T> helper,
            Expression<Func<T, TProperty>> expression)
        {
            return ReadOnlyTextBoxFor(helper, expression, null);
        }

        /// <summary>
        /// Renders readonly textbox.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TProperty"></typeparam>
        /// <param name="helper"></param>
        /// <param name="expression"></param>
        /// <param name="htmlAttributes"></param>
        /// <returns></returns>
        public static MvcHtmlString ReadOnlyTextBoxFor<T, TProperty>(this HtmlHelper<T> helper,
            Expression<Func<T, TProperty>> expression, object htmlAttributes)
        {
            return ReadOnlyTextBoxFor(helper, expression, null, htmlAttributes);
        }

        /// <summary>
        /// Renders readonly textbox.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TProperty"></typeparam>
        /// <param name="helper"></param>
        /// <param name="expression"></param>
        /// <param name="format"></param>
        /// <param name="htmlAttributes"></param>
        /// <returns></returns>
        public static MvcHtmlString ReadOnlyTextBoxFor<T, TProperty>(this HtmlHelper<T> helper,
            Expression<Func<T, TProperty>> expression, string format, object htmlAttributes)
        {
            var dctAttribute = HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes);
            dctAttribute["readonly"] = "readonly";
            return helper.TextBoxFor(expression, format, dctAttribute);
        }

        /// <summary>
        /// Renders readonly dropdown.
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <typeparam name="TProperty"></typeparam>
        /// <param name="htmlHelper"></param>
        /// <param name="expression"></param>
        /// <param name="selectList"></param>
        /// <param name="htmlAttributes"></param>
        /// <returns></returns>
        public static MvcHtmlString ReadOnlyDropDownListFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper,
            Expression<Func<TModel, TProperty>> expression, IEnumerable<SelectListItem> selectList,
            object htmlAttributes)
        {
            var dctAttribute = HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes);
            dctAttribute["disabled"] = "disabled";
            return htmlHelper.DropDownListFor(expression, selectList, dctAttribute);
        }

        /// <summary>
        /// Gets string represents given date in predefined format.
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <param name="htmlHelper"></param>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static string GetDateString<TModel>(this HtmlHelper<TModel> htmlHelper, DateTime? dateTime)
        {
            return EngineContext.Current.Resolve<IWebHelper>().GetDateString(dateTime);
        }

        public static string GetDateTimeString<TModel>(this HtmlHelper<TModel> htmlHelper, DateTime? dateTime)
        {
            return EngineContext.Current.Resolve<IWebHelper>().GetDateTimeString(dateTime);
        }

        /// <summary>
        /// Gets formatted number.
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <param name="htmlHelper"></param>
        /// <param name="number"></param>
        /// <returns></returns>
        public static string GetFormattedNumber<TModel>(this HtmlHelper<TModel> htmlHelper, int? number)
        {
            number = number ?? 0;
            return number.Value.ToString(EngineContext.Current.Resolve<IWebHelper>().NumberFormat);
        }

        /// <summary>
        /// Checks if current user has given permision(s).
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <param name="htmlHelper"></param>
        /// <param name="permissions"></param>
        /// <returns></returns>
        public static bool HasPermission<TModel>(this HtmlHelper<TModel> htmlHelper, params UserPermission[] permissions)
        {
            return EngineContext.Current.Resolve<IWorkContext>().User.HasPermission(permissions);
        }
    }

    public static class UrlExtensions
    {
        //Builds URL by finding the best matching route that corresponds to the current URL,
        //with given parameters added or replaced.
        public static MvcHtmlString Current(this UrlHelper helper, object substitutes)
        {
            //get the route data for the current URL e.g. /Research/InvestmentModelling/RiskComparison
            //this is needed because unlike UrlHelper.Action, UrlHelper.RouteUrl sets includeImplicitMvcValues to false
            //which causes it to ignore current ViewContext.RouteData.Values
            var rd = new RouteValueDictionary(helper.RequestContext.RouteData.Values);

            //get the current query string e.g. ?BucketID=17371&amp;compareTo=123
            var qs = helper.RequestContext.HttpContext.Request.QueryString;

            //add query string parameters to the route value dictionary
            foreach (string param in qs)
            {
                if (!string.IsNullOrEmpty(qs[param]))
                {
                    if (qs[param].Contains(",")) // HACK: if multiple value detect, just get the first one
                    {
                        rd[param] = qs[param].Split(new char[] {','}, StringSplitOptions.RemoveEmptyEntries)[0];
                    }
                    else
                    {
                        rd[param] = qs[param];
                    }
                }
            }

            //override parameters we're changing
            foreach (PropertyDescriptor property in TypeDescriptor.GetProperties(substitutes.GetType()))
            {
                var value = property.GetValue(substitutes);
                if (string.IsNullOrEmpty(Convert.ToString(value)))
                    rd.Remove(property.Name);
                else
                    rd[property.Name] = value;
            }

            //UrlHelper will find the first matching route
            //(the routes are searched in the order they were registered).
            //The unmatched parameters will be added as query string.
            var url = helper.RouteUrl(rd);
            return new MvcHtmlString(url);
        }

        public static MvcHtmlString Paging(this UrlHelper helper, int pageIndex, int? pageSize = null)
        {
            if (pageSize == null)
                return helper.Current(new {paging = 1, page = pageIndex});
            return helper.Current(new {paging = 1, page = pageIndex, pageSize = pageSize});
        }
    }
}
