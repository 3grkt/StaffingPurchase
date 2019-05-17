using System.Web.Http;
using StaffingPurchase.Core;
using StaffingPurchase.Core.SearchCriteria;
using StaffingPurchase.Services.PV;
using StaffingPurchase.Services.Localization;
using StaffingPurchase.Services.Logging;
using StaffingPurchase.Web.Extensions;
using StaffingPurchase.Web.Models.Common;
using StaffingPurchase.Web.Models.PV;
using StaffingPurchase.Core.Domain;
using StaffingPurchase.Core.DTOs;
using System;
using System.Linq;

namespace StaffingPurchase.Web.Api
{
    [RoutePrefix("api/pvlog")]
    public class PvLogController : ApiControllerBase
    {
        private readonly IPvLogService _pvLogService;
        private readonly IWorkContext _workContext;

        public PvLogController(
            ILogger logger,
            IResourceManager resourceMng,
            IPvLogService pvLogService,
            IWorkContext workContext)
            : base(logger, resourceMng)
        {
            _pvLogService = pvLogService;
            _workContext = workContext;
        }

        [HttpGet]
        [Route("search")]
        public JsonList<PvLogModel> Search(
            [FromUri] PvLogSearchCriteria criteria,
            [FromUri] PaginationOptions options)
        {
            options = GetPaginationOptions(options);

            var logs = _pvLogService.Search(criteria, options, _workContext.User);
            return new JsonList<PvLogModel>
            {
                Data = logs.ToModelList<PVLog, PvLogModel>(),
                TotalItems = logs.TotalCount
            };
        }

        [HttpGet]
        [Route("logsummary")]
        public JsonList<PvLogSummaryModel> SearchLogSummary(
            [FromUri] PvLogSearchCriteria criteria,
            [FromUri] PaginationOptions options)
        {
            options = GetPaginationOptions(options);

            // If user don't have permission to view others' log; set userid to himself
            if (!_workContext.User.HasPermission(UserPermission.ViewUserPvLog))
            {
                criteria.UserId = _workContext.User.Id;
            }

            var logs = _pvLogService.SearchLogSummary(criteria, options, _workContext.User);
            var data = logs.ToModelList<PvLogSummaryDto, PvLogSummaryModel>().OrderBy(x=>ConvertMonthYearToDateTime(x.Month));
            return new JsonList<PvLogSummaryModel>
            {
                Data = data,
                TotalItems = logs.TotalCount
            };
        }

        #region Utils
        private static DateTime ConvertMonthYearToDateTime(string monthYear)
        {
            DateTime date = DateTime.ParseExact(monthYear, "MMM-yyyy", System.Globalization.CultureInfo.InvariantCulture);
            return date;
        } 
        #endregion
    }
}
