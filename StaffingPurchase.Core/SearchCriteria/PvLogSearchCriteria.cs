using System;

namespace StaffingPurchase.Core.SearchCriteria
{
    public class PvLogSearchCriteria : SearchCriteriaBase
    {
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int? UserId { get; set; }
    }
}
