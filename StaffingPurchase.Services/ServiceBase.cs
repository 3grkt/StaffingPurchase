using System;
using System.Globalization;
using StaffingPurchase.Core;
using StaffingPurchase.Services.Configurations;

namespace StaffingPurchase.Services
{
    public abstract class ServiceBase
    {
        protected readonly IAppSettings _appSettings;
        protected readonly IAppPolicy _appPolicy;

        protected ServiceBase()
        {
        }

        protected ServiceBase(IAppSettings appSettings, IAppPolicy appPolicy)
        {
            _appSettings = appSettings;
            _appPolicy = appPolicy;
        }

        protected DateTime GetNearestOrderSessionEndDate(DateTime? basedDate = null)
        {
            var date = basedDate ?? DateTime.Now;
            if (date.Month % 2 == 0)// odd month -> get it; otherwise, minus 1
            {
                date = date.AddMonths(-1);
            }
            return new DateTime(date.Year, date.Month, _appPolicy.OrderSessionEndDayOfMonth);
        }

        protected DateTime GetNearestOrderSessionStartDate(DateTime? basedDate = null)
        {
            var date = basedDate ?? DateTime.Now;
            var endMonth = GetNearestOrderSessionEndDate(date);
            var startDate = endMonth.AddMonths(-2);

            return new DateTime(startDate.Year, startDate.Month, _appPolicy.OrderSessionStartDayOfMonth);
        }

        protected DateTime GetCurrentOrderSessionEndDate(DateTime? basedDate = null)
        {
            var date = basedDate ?? DateTime.Now;
            if (date.Month % 2 == 0)
            {
                date = date.AddMonths(1);
                return new DateTime(date.Year, date.Month, _appPolicy.OrderSessionEndDayOfMonth);
            }

            if (date.Day <= _appPolicy.OrderSessionEndDayOfMonth)
            {
                return new DateTime(date.Year, date.Month, _appPolicy.OrderSessionEndDayOfMonth);
            }

            if (date.Day >= _appPolicy.OrderSessionStartDayOfMonth)
            {
                date = date.AddMonths(2);
                return new DateTime(date.Year, date.Month, _appPolicy.OrderSessionEndDayOfMonth);
            }

            return new DateTime();
        }

        protected DateTime GetCurrentOrderSessionStartDate(DateTime? basedDate = null)
        {
            var date = basedDate ?? DateTime.Now;
            if (date.Month % 2 == 0)
            {
                date = date.AddMonths(-1);
                return new DateTime(date.Year, date.Month, _appPolicy.OrderSessionStartDayOfMonth);
            }

            if (date.Day <= _appPolicy.OrderSessionEndDayOfMonth)
            {
                date = date.AddMonths(-2);
                return new DateTime(date.Year, date.Month, _appPolicy.OrderSessionStartDayOfMonth);
            }

            if (date.Day >= _appPolicy.OrderSessionStartDayOfMonth)
            {
                return new DateTime(date.Year, date.Month, _appPolicy.OrderSessionStartDayOfMonth);
            }

            return new DateTime();
        }

        /// <summary>
        /// Gets string representing Month & Year of order session.
        /// </summary>
        /// <param name="basedDate"></param>
        /// <returns></returns>
        protected string GetOrderSessionMonthYear(DateTime? basedDate = null)
        {
            return GetCurrentOrderSessionStartDate(basedDate).ToString("MMM-yyyy", CultureInfo.InvariantCulture);
        }
    }
}
