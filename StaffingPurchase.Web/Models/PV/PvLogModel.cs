using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace StaffingPurchase.Web.Models.PV
{
    public class PvLogModel : ViewModelBase
    {
        public string Description { get; set; }
        public DateTime LogDate { get; set; }
        public double PV { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; }
    }
}
