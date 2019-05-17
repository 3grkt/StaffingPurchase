using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Web.Http;
using StaffingPurchase.Core;
using StaffingPurchase.Core.Infrastructure;
using StaffingPurchase.Core.Domain.Logging;
using StaffingPurchase.Jobs.Workers;
using StaffingPurchase.Services.Localization;
using StaffingPurchase.Services.Logging;
using StaffingPurchase.Web.Extensions;
using StaffingPurchase.Web.Framework.Filters;
using StaffingPurchase.Web.Models.Common;
using StaffingPurchase.Web.Models.Logging;

namespace StaffingPurchase.Web.Api
{
    public class BatchJobController : ApiControllerBase
    {
        private readonly IAppSettings _appSettings;
        private readonly IQueriableLogger _queriableLogger;

        public BatchJobController(IAppSettings appSettings, IResourceManager resourceManager, ILogger logger, IQueriableLogger queriableLogger)
            : base(logger, resourceManager)
        {
            _appSettings = appSettings;
            _queriableLogger = queriableLogger;
        }

        [HttpPost]
        public HttpResponseMessage Execute(string username, string key)
        {
            if (username != _appSettings.BatchJobRunUser || key != _appSettings.BatchJobRunKey)
            {
                return Request.CreateErrorResponse(
                    HttpStatusCode.BadRequest,
                    new HttpError(_resourceManager.GetString("BatchJob.RunInfoInvalid")));
            }

            try
            {
                RunJobs();
            }
            catch (Exception ex)
            {
                _logger.WriteLog("Failed to execute batchjob.", ex);
                return Request.CreateErrorResponse(
                    HttpStatusCode.BadRequest,
                    new HttpError(ex.Message + ex.StackTrace));
            }

            return Request.CreateResponse(HttpStatusCode.OK);
        }

        [HttpGet]
        [Route("api/batchjob/getlogs")]
        [PermissionAuthorize(UserPermission.ViewBatchjobLog)]
        public JsonList<LogEntryModel> GetLogs(DateTime? startDate, DateTime? endDate, [FromUri] PaginationOptions options)
        {
            var logs = _queriableLogger.GetLogs("\\[BATCHJOB\\]", GetPaginationOptions(options), startDate, endDate);
            return new JsonList<LogEntryModel>()
            {
                Data = logs.ToModelList<LogEntry, LogEntryModel>(),
                TotalItems = logs.TotalCount
            };
        }

        [HttpGet]
        [Route("api/batchjob/getlog/{id}")]
        [PermissionAuthorize(UserPermission.ViewBatchjobLog)]
        public LogEntryModel GetLog(string id)
        {
            var log = _queriableLogger.GetLog(id);
            if (log == null)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(
                    HttpStatusCode.NotFound,
                    new HttpError(_resourceManager.GetString("Common.DataNotFound"))));
            }

            return log.ToModel<LogEntryModel>();
        }

        private void RunJobs()
        {
            // Get registered IWorker types
            var workerType = typeof(IWorker);
            var foundTypes = Assembly.Load(_appSettings.BatchJobAssemblyName).GetTypes()
                .Where(t => workerType.IsAssignableFrom(t) && t.IsClass && !t.IsAbstract)
                .ToList();

            // Create IWorker instance and add to list
            var workers = new List<IWorker>();
            foreach (var type in foundTypes)
            {
                //var worker = EngineContext.Current.ContainerManager.ResolveUnregistered(type) as IWorker;
                var worker = EngineContext.Current.Resolve(type) as IWorker;
                workers.Add(worker);
            }

            // Sort and execute
            var sortedWorkers = workers.OrderBy(w => w.Order);
            foreach (var worker in sortedWorkers)
            {
                if (worker.CanWork)
                {
                    worker.DoWork();
                }
                else
                {
                    _logger.Warn(string.Format("Job {0} is currently turned off", worker.GetType().Name));
                }
            }
        }
    }
}
