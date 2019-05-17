using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using StaffingPurchase.Core;
using StaffingPurchase.Services.Localization;
using StaffingPurchase.Services.Logging;
using static StaffingPurchase.Web.Extensions.HttpExtension;

namespace StaffingPurchase.Web.Api
{
    public class ApiControllerBase : ApiController
    {
        protected readonly ILogger _logger;
        protected readonly IResourceManager _resourceManager;

        public ApiControllerBase()
        {
            _logger = null;
            _resourceManager = null;
        }

        public ApiControllerBase(ILogger logger, IResourceManager resourceMng)
        {
            _logger = logger;
            _resourceManager = resourceMng;
        }

        /// <summary>
        /// Log exception and return response message
        /// </summary>
        /// <param name="log">Log service</param>
        /// <param name="ex">Exception</param>
        /// <param name="logMsg">Log message</param>
        /// <param name="responseMsg">Response message</param>
        /// <param name="logLevel">Log level</param>
        /// <param name="responseStatus">Response status code</param>
        /// <returns>a HTTP response message</returns>
        protected HttpResponseMessage RecordException(ILogger log, Exception ex, string logMsg, string responseMsg,
            LogLevel logLevel = LogLevel.Error, HttpStatusCode responseStatus = HttpStatusCode.NotAcceptable)
        {
            switch (logLevel)
            {
                case LogLevel.Debug:
                    log.Debug(logMsg);
                    break;
                case LogLevel.Info:
                    log.Info(logMsg);
                    break;
                case LogLevel.Warning:
                    log.Warn(logMsg);
                    break;
                case LogLevel.Error:
                    log.Error(logMsg, ex);
                    break;
                default:
                    log.Error(logMsg, ex);
                    break;
            }

            return Request.CreateErrorResponse(responseStatus, responseMsg);
        }

        /// <summary>
        /// Log exception and response HTTP message.
        /// Try to resolve the string by resource file if possible.
        /// Using the Logger instance from the inherit class.
        /// </summary>
        /// <param name="ex">Exception</param>
        /// <param name="logMsg">Out log message with log service</param>
        /// <param name="responseMsg">Response message for HTTP Response</param>
        /// <param name="logLevel">Log Level to determine log information</param>
        /// <param name="responseStatus">HTTP Response message status</param>
        /// <returns></returns>
        protected HttpResponseMessage RecordException(Exception ex, string logMsg, string responseMsg = "Error.DataException",
            LogLevel logLevel = LogLevel.Error, HttpStatusCode responseStatus = HttpStatusCode.InternalServerError)
        {
            if (_logger != null)
            {
                return _resourceManager != null
                    ? RecordException(_logger, ex, _resourceManager.GetString(logMsg), _resourceManager.GetString(responseMsg),
                        logLevel, responseStatus)
                    : RecordException(_logger, ex, logMsg, responseMsg, logLevel, responseStatus);
            }

            return Request.CreateErrorResponse(responseStatus, logMsg + ": " + responseMsg);
        }

        protected HttpResponseMessage NotFound(string message)
        {
            return Request.CreateErrorResponse(HttpStatusCode.NotFound, new HttpError(message));
        }

        protected HttpResponseMessage Error(string message, HttpStatusCode statusCode = HttpStatusCode.BadRequest)
        {
            return Request.CreateErrorResponse(statusCode, message);
        }

        protected PaginationOptions GetPaginationOptions(PaginationOptions options)
        {
            return (options != null && options.PageIndex > 0 && options.PageSize > 0) ? options : PaginationOptions.Default;
        }

        protected IHttpActionResult DownloadFile(string path)
        {
            if (!File.Exists(path))
            {
                _logger.Warn($"File '{path}' does not exist.");
                return new StaffPurchaseExceptionActionResult(_resourceManager.GetString("Common.FailedToDownloadFile"));
            }

            return new FileActionResult(new FileStream(path, FileMode.Open), Path.GetFileName(path));
        }
    }
}
