using System;

namespace StaffingPurchase.Core.SearchCriteria
{
    public class OrderHistorySearchCriteria : SearchCriteriaBase
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public OrderStatus? Status { get; set; }
    }
}
