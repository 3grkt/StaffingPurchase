using System;

namespace StaffingPurchase.Core.Domain.Logging
{
    public class LogEntry : EntityBase
    {
        public Guid ErrorId { get; set; }
        public string Type { get; set; }
        public DateTime TimeUtc { get; set; }
        public string Message { get; set; }
        public string Detail { get; set; }
    }
}
