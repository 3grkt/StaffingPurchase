namespace StaffingPurchase.Core.SearchCriteria
{
    public class OrderBatchSearchCriteria : SearchCriteriaBase
    {
        public int LocationId { get; set; }
        public int DepartmentId { get; set; }
        public OrderType OrderType { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
    }
}
