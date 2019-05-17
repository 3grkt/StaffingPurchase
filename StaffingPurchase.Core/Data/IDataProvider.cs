using System;
using System.Data;
using System.Data.Common;

namespace StaffingPurchase.Core.Data
{
    public interface IDataProvider
    {
        DbParameter CreateParameter();

        DbParameter CreateParameter(string name, object value, DbType dataType,
            ParameterDirection direction = ParameterDirection.Input);

        DateTime? AddDays(DateTime? date, int? daysToAdd);
        DateTime? AddMonths(DateTime? date, int? monthsToAdd);
        DateTime? AddYears(DateTime? date, int? yearsToAdd);
        string GetMonthYearString(DateTime? date);
    }
}
