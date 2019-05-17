namespace StaffingPurchase.Core.SearchCriteria
{
    public class OrderWarehouseSearchCriteria : SearchCriteriaBase
    {
        public int? DepartmentId { get; set; }
        public int LocationId { get; set; }
        public int? UserId { get; set; }
    }
}
