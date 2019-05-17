using System;
using System.Data;
using System.Data.Common;
using System.Data.Entity;
using System.Data.SqlClient;
using StaffingPurchase.Core.Data;
using System.Data.Entity.SqlServer;

namespace StaffingPurchase.Data
{
    public class SqlServerDataProvider : IDataProvider
    {
        public DbParameter CreateParameter()
        {
            return new SqlParameter();
        }

        public DbParameter CreateParameter(string name, object value, DbType dataType,
            ParameterDirection direction = ParameterDirection.Input)
        {
            var parameter = new SqlParameter(name, value);
            parameter.DbType = dataType;
            parameter.Direction = direction;
            return parameter;
        }

        public DateTime? AddDays(DateTime? date, int? dateToAdd)
        {
            return DbFunctions.AddDays(date, dateToAdd);
        }

        public DateTime? AddMonths(DateTime? date, int? monthsToAdd)
        {
            return DbFunctions.AddMonths(date, monthsToAdd);
        }

        public DateTime? AddYears(DateTime? date, int? yearsToAdd)
        {
            return DbFunctions.AddYears(date, yearsToAdd);
        }

        public string GetMonthYearString(DateTime? date)
        {
            return $"{DbFunctions.Left(SqlFunctions.DateName("month", date), 3)}-{SqlFunctions.DatePart("year", date)}";
        }
    }
}
