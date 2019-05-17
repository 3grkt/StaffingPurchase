using StaffingPurchase.Core;
using StaffingPurchase.Core.Domain;
using StaffingPurchase.Core.DTOs;
using StaffingPurchase.Core.SearchCriteria;
using System;

namespace StaffingPurchase.Services.PV
{
    public interface IPvLogService
    {
        /// <summary>
        /// Logs pv change to database.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="userName"></param>
        /// <param name="pv"></param>
        /// <param name="description"></param>
        /// <param name="logDate"></param>
        /// <param name="logType"></param>
        /// <param name="currentPv"></param>
        /// <param name="inTransaction"></param>
        void Log(int userId, string userName, double pv, string description = null, DateTime? logDate = null,
            PvLogType logType = PvLogType.None, double currentPv = 0, bool inTransaction = false);

        /// <summary>
        /// Searches for pv log.
        /// </summary>
        /// <param name="criteria"></param>
        /// <param name="options"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        IPagedList<PVLog> Search(PvLogSearchCriteria criteria, PaginationOptions options, WorkingUser user);

        /// <summary>
        /// Searches for pv log summary.
        /// </summary>
        /// <param name="criteria"></param>
        /// <param name="options"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        IPagedList<PvLogSummaryDto> SearchLogSummary(PvLogSearchCriteria criteria, PaginationOptions options, WorkingUser user);
    }
}
