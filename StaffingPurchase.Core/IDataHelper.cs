using StaffingPurchase.Core.Domain;
using StaffingPurchase.Core.DTOs;
using System;
using System.Linq.Expressions;

namespace StaffingPurchase.Core
{
    /// <summary>
    /// Contains helper methods used to retrieve data in Data layer.
    /// </summary>
    public interface IDataHelper
    {
        Expression<Func<PVLog, PvLogGroup>> GroupLogByUserAndMonthYear();
    }
}
