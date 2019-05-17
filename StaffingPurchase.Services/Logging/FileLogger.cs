using System;
using log4net;
using StaffingPurchase.Core;

namespace StaffingPurchase.Services.Logging
{
    public class FileLogger : ILogger
    {
        //private static readonly ILog _log = LogManager.GetLogger("AOBLogger");

        private static readonly Lazy<ILog> _lazyObject = new Lazy<ILog>(() =>
        {
            //log4net.Config.XmlConfigurator.Configure(new FileInfo(HttpContext.Current.Server.MapPath("~/web.config")));
            log4net.Config.XmlConfigurator.Configure();
            System.Diagnostics.Debug.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") +
                                               ": log4net config was loaded.");

            return LogManager.GetLogger("StaffingPurchaseLogger");
        });

        protected static ILog Logger
        {
            get { return _lazyObject.Value; }
        }

        public void Debug(string message, LogSource source = LogSource.None)
        {
            WriteLog(message, LogLevel.Debug, source: source);
        }

        public void Error(string message, Exception ex = null, LogSource source = LogSource.None)
        {
            WriteLog(message, LogLevel.Error, ex, source: source);
        }

        public void Info(string message, LogSource source = LogSource.None)
        {
            WriteLog(message, LogLevel.Info, source: source);
        }

        public void Warn(string message, LogSource source = LogSource.None)
        {
            WriteLog(message, LogLevel.Warning, source: source);
        }

        public void WriteLog(string message, Exception ex = null, LogSource source = LogSource.None, object data = null)
        {
            if (ex != null)
                WriteLog(message, LogLevel.Error, ex, source, data);
            else
                WriteLog(message, LogLevel.Debug, ex, source, data);
        }

        public void WriteLog(string message, LogLevel level, Exception ex = null, LogSource source = LogSource.None, object data = null)
        {
            // TODO: extend log capabitility (e.g. logLevel, logType)
            // TODO: add working user
            switch (level)
            {
                case LogLevel.Debug:
                    Logger.Debug(message);
                    break;
                case LogLevel.Info:
                    Logger.Info(message);
                    break;
                case LogLevel.Warning:
                    Logger.Warn(message);
                    break;
                case LogLevel.Error:
                    Logger.Error(message, ex);
                    break;
            }
        }
    }
}
