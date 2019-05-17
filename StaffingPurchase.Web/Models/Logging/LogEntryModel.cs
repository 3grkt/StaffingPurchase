using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace StaffingPurchase.Web.Models.Logging
{
    public class LogEntryModel : ViewModelBase
    {
        public string ErrorId { get; set; }
        public string Type { get; set; }
        public DateTime Time { get; set; }
        public string Message { get; set; }
        public string Detail { get; set; }
    }
}
