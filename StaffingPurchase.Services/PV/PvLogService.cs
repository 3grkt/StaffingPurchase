using System;
using System.Linq;
using System.Linq.Expressions;
using StaffingPurchase.Core;
using StaffingPurchase.Core.Data;
using StaffingPurchase.Core.Domain;
using StaffingPurchase.Core.DTOs;
using StaffingPurchase.Core.SearchCriteria;
using StaffingPurchase.Services.Configurations;

namespace StaffingPurchase.Services.PV
{
    public class PvLogService : ServiceBase, IPvLogService
    {
        private readonly IRepository<PVLog> _pvLogRepository;
        private readonly IDataHelper _dataHelper;

        public PvLogService(IRepository<PVLog> pvLogRepository, IDataHelper dataHelper, IAppSettings appSettings, IAppPolicy appPolicy)
            : base(appSettings, appPolicy)
        {
            _pvLogRepository = pvLogRepository;
            _dataHelper = dataHelper;
        }

        public void Log(int userId, string userName, double pv, string description = null, DateTime? logDate = null,
            PvLogType logType = PvLogType.None, double currentPv = 0, bool inTransaction = false)
        {
            var pvLog = new PVLog
            {
                UserId = userId,
                UserName = userName,
                PV = pv,
                Description = description,
                LogDate = logDate ?? DateTime.Now,
                LogTypeId = (short)logType,
                CurrentPV = currentPv,
                OrderSession = GetOrderSessionMonthYear(logDate)
            };

            _pvLogRepository.Insert(pvLog, !inTransaction);
        }

        public IPagedList<PVLog> Search(PvLogSearchCriteria criteria, PaginationOptions options, WorkingUser user)
        {
            var query = _pvLogRepository.TableNoTracking;

            query = FilterLog(query, criteria, user);

            query = string.IsNullOrEmpty(options.Sort)
                ? query.OrderByDescending(x => x.LogDate)
                : query.SortBy(options.SortExpression);

            return new PagedList<PVLog>(query, options.PageIndex, options.PageSize);
        }

        public IPagedList<PvLogSummaryDto> SearchLogSummary(PvLogSearchCriteria criteria, PaginationOptions options, WorkingUser user)
        {
            var query = _pvLogRepository.TableNoTracking;
            query = FilterLog(query, criteria, user);

            var result = query
                .GroupBy(_dataHelper.GroupLogByUserAndMonthYear())
                .Select(group => new PvLogSummaryDto
                {
                    UserId = group.Key.UserId,
                    UserName = group.Key.UserName,
                    Month = group.Key.MonthYear,
                    MonthlyRewardedPv = group.Where(x => x.LogTypeId == (short)PvLogType.MonthlyReward).Sum(x => (double?)x.PV) ?? 0,
                    AwardedPv = group.Where(x => x.LogTypeId == (short)PvLogType.Award).Sum(x => (double?)x.PV) ?? 0,
                    OrderingPv = group.Where(x => x.LogTypeId == (short)PvLogType.Ordering).Sum(x => (double?)x.PV) ?? 0,
                    BirthdayRewardedPv = group.Where(x => x.LogTypeId == (short)PvLogType.Birthday).Sum(x => (double?)x.PV) ?? 0,
                    RemainingPv = group.OrderByDescending(x => x.LogDate).FirstOrDefault().CurrentPV ?? 0,
                    PreviousSessionPv = (group.FirstOrDefault().CurrentPV ?? 0) - group.FirstOrDefault().PV
                }).
                OrderBy(x => x.UserName);

            return new PagedList<PvLogSummaryDto>(result, options.PageIndex, options.PageSize);
        }

        #region Utility
        private static IQueryable<PVLog> FilterLog(IQueryable<PVLog> query, PvLogSearchCriteria criteria, WorkingUser user)
        {
            if (criteria.StartDate.HasValue)
            {
                var startDate = criteria.StartDate.Value.StartOfDate();
                query = query.Where(x => x.LogDate >= startDate);
            }

            if (criteria.EndDate.HasValue)
            {
                var endDate = criteria.EndDate.Value.EndOfDate();
                query = query.Where(x => x.LogDate <= endDate);
            }

            if (criteria.UserId.HasValue)
            {
                query = query.Where(x => x.UserId == criteria.UserId);
            }

            return query;
        }

        #endregion
    }
}
