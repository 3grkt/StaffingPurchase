namespace StaffingPurchase.Core.DTOs
{
    public class PvLogSummaryDto : EntityBase
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
