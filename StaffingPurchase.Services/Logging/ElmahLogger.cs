using System;
using System.Data;
using System.Web;
using Elmah;
using StaffingPurchase.Core;
using StaffingPurchase.Core.Data;
using StaffingPurchase.Core.Domain.Logging;
using StaffingPurchase.Data;
using ApplicationException = Elmah.ApplicationException;

namespace StaffingPurchase.Services.Logging
{
    public class ElmahLogger : ILogger, IQueriableLogger
    {
        #region Fields and Properties
        private readonly IDbContext _dbContext;
        private readonly IDataProvider _dataProvider;

        protected virtual LogSource DefaultLogSource
        {
            get { return LogSource.None; }
        }

        #endregion

        #region Ctor.

        public ElmahLogger(IDbContext dbContext, IDataProvider dataProvider)
        {
            _dbContext = dbContext;
            _dataProvider = dataProvider;
        }

        public ElmahLogger() { } // Keep default ctor.

        #endregion

        #region Services
        public void WriteLog(string message, Exception ex = null, LogSource source = LogSource.None, object data = null)
        {
            if (ex != null)
            {
                WriteLog(message, LogLevel.Error, ex, source, data);
            }
            else
            {
                WriteLog(message, LogLevel.Debug, ex, source, data);
            }
        }

        public void WriteLog(string message, LogLevel level, Exception ex = null, LogSource source = LogSource.None, object data = null)
        {
            switch (level)
            {
                case LogLevel.Debug:
                    Debug(message);
                    break;
                case LogLevel.Info:
                    Info(message);
                    break;
                case LogLevel.Warning:
                    Warn(message);
                    break;
                case LogLevel.Error:
                    Error(message, ex);
                    break;
            }
        }

        public void Info(string message, LogSource source = LogSource.None)
        {
            RaiseErrorSignal(new InfoException(FormatMessage(message)));
        }

        public void Debug(string message, LogSource source = LogSource.None)
        {
            RaiseErrorSignal(new DebugException(FormatMessage(message)));
        }

        public void Warn(string message, LogSource source = LogSource.None)
        {
            RaiseErrorSignal(new WarningException(FormatMessage(message)));
        }

        public virtual void Error(string message, Exception exception, LogSource source = LogSource.None)
        {
            RaiseErrorSignal(new ErrorException(FormatMessage(message), exception));
        }

        public IPagedList<LogEntry> GetLogs(string filter, PaginationOptions options, DateTime? startDate = null, DateTime? endDate = null)
        {
            var applicationParam = _dataProvider.CreateParameter("Application", "StaffingPurchase", DbType.String);
            var whereClauseParam = _dataProvider.CreateParameter("MessageFilter", filter, DbType.String);
            var startDateParam = _dataProvider.CreateParameter("StartDate", ConvertToUtc(startDate), DbType.DateTime);
            var endDateParam = _dataProvider.CreateParameter("EndDate", ConvertToUtc(endDate, true), DbType.DateTime);
            var pageIndexParam = _dataProvider.CreateParameter("PageIndex", options.PageIndex, DbType.Int32);
            var pageSizeParam = _dataProvider.CreateParameter("PageSize", options.PageSize, DbType.Int32);
            var totalCountParam = _dataProvider.CreateParameter("TotalCount", 0, DbType.Int32, ParameterDirection.Output);

            var data = _dbContext.ExecuteStoredProcedureList<LogEntry>("ELMAH_Search", new object[] {
                applicationParam,
                whereClauseParam,
                startDateParam,
                endDateParam,
                pageIndexParam,
                pageSizeParam,
                totalCountParam
            });

            return new PagedList<LogEntry>(data, options.PageIndex, options.PageSize, (int)totalCountParam.Value);
        }

        public LogEntry GetLog(string id)
        {
            var log = ErrorLog.GetDefault(HttpContext.Current).GetError(id);
            if (log != null)
            {
                return new LogEntry()
                {
                    ErrorId = new Guid(log.Id),
                    Message = log.Error.Message,
                    Detail = log.Error.Detail,
                    TimeUtc = log.Error.Time,
                    Type = log.Error.Type
                };
            }
            return null;
        }
        #endregion

        #region Utlitiy

        protected virtual string FormatMessage(string message)
        {
            return message; // by default, no format
        }

        private void RaiseErrorSignal(Exception exception)
        {
            if (HttpContext.Current != null)
            {
                ErrorSignal.FromCurrentContext().Raise(exception);
            }
            else
            {
                ErrorLog.GetDefault(null).Log(new Error(exception));
            }
        }

        private DateTime? ConvertToUtc(DateTime? dt, bool endOfDate = false)
        {
            if (dt.HasValue)
            {
                var dtUtc = endOfDate ? dt.Value.EndOfDate() : dt.Value.StartOfDate();
                return dtUtc.ToUniversalTime();
            }
            return null;
        }

        #endregion
    }


    #region Elmah support
    public class DebugException : ApplicationException
    {
        public DebugException() : base() { }

        public DebugException(string message) : base(message) { }

        public DebugException(string message, Exception innerException) : base(message, innerException) { }
    }

    public class InfoException : ApplicationException
    {
        public InfoException() : base() { }

        public InfoException(string message) : base(message) { }

        public InfoException(string message, Exception innerException) : base(message, innerException) { }
    }

    public class WarningException : ApplicationException
    {
        public WarningException() : base() { }

        public WarningException(string message) : base(message) { }

        public WarningException(string message, Exception innerException) : base(message, innerException) { }
    }

    public class ErrorException : ApplicationException
    {
        public ErrorException() : base() { }

        public ErrorException(string message) : base(message) { }

        public ErrorException(string message, Exception innerException) : base(message, innerException) { }
    }
    #endregion
}
