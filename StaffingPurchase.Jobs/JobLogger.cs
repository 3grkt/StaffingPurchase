using System;
using StaffingPurchase.Core;
using StaffingPurchase.Services.Logging;

namespace StaffingPurchase.Jobs
{
    public class JobLogger : ElmahLogger
    {
        private const int LogPrefixWidth = 12;

        protected override LogSource DefaultLogSource
        {
            get { return LogSource.BatchJob; }
        }

        protected override string FormatMessage(string message)
        {
            string logSource = DefaultLogSource.ToString().ToUpper();
            return string.Format("{0} {1}",
                string.Format("[{0}]", logSource).PadRight(LogPrefixWidth), // leave some trailing spaces for readibility
                message);
        }

        /// <summary>
        /// Overrides and downgrades log to warning level so that it can be viewed friendly on ELMAH page.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="exception"></param>
        /// <param name="source"></param>
        public override void Error(string message, Exception exception, LogSource source = LogSource.None)
        {
            var errorMessage = string.Format("{0} - Error: {1}", message, CommonHelper.GetFullExceptionDetails(exception));
            Warn(errorMessage);
        }
    }
}
