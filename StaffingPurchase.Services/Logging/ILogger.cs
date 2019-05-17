using System;
using StaffingPurchase.Core;

namespace StaffingPurchase.Services.Logging
{
    public interface ILogger
    {
        void WriteLog(string message, LogLevel level, Exception ex = null, LogSource source = LogSource.None, object data = null);
        void WriteLog(string message, Exception ex = null, LogSource source = LogSource.None, object data = null);
        void Debug(string message, LogSource source = LogSource.None);
        void Info(string message, LogSource source = LogSource.None);
        void Warn(string message, LogSource source = LogSource.None);
        void Error(string message, Exception ex = null, LogSource source = LogSource.None);
    }
}
