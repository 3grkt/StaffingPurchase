using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace StaffingPurchase.Web.Models.PV
{
    public class PvLogSummaryModel : ViewModelBase
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string Month { get; set; }
        public string OrderSession { get; set; }
        public double PreviousSessionPv { get; set; }
        public double MonthlyRewardedPv { get; set; }
        public double BirthdayRewardedPv { get; set; }
        public double AwardedPv { get; set; }
        public double OrderingPv { get; set; }
        public double RemainingPv { get; set; }
    }
}
