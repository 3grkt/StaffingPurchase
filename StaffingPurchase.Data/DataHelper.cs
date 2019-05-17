using System;
using System.Data.Entity.SqlServer;
using System.Data.Entity;
using System.Linq.Expressions;
using StaffingPurchase.Core;
using StaffingPurchase.Core.Domain;
using StaffingPurchase.Core.DTOs;

namespace StaffingPurchase.Data
{
    public class DataHelper : IDataHelper
    {
        public DataHelper()
        {
        }

        public Expression<Func<PVLog, PvLogGroup>> GroupLogByUserAndMonthYear()
        {
            Expression<Func<PVLog, PvLogGroup>> expression =
                x => new PvLogGroup
                {
                    MonthYear = DbFunctions.Left(SqlFunctions.DateName("month", x.LogDate), 3) + "-" + SqlFunctions.DatePart("year", x.LogDate),
                    UserId = x.UserId,
                    UserName = x.UserName
                };
            return expression;
        }

    }
}
