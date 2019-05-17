using System;
using StaffingPurchase.Core;
using StaffingPurchase.Core.Domain.Logging;

namespace StaffingPurchase.Services.Logging
{
    public interface IQueriableLogger
    {
        IPagedList<LogEntry> GetLogs(string filter, PaginationOptions options, DateTime? startDate = null, DateTime? endDate = null);
        LogEntry GetLog(string id);
    }
}
