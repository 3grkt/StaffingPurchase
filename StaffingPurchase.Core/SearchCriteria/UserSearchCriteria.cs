namespace StaffingPurchase.Core.SearchCriteria
{
    public class UserSearchCriteria
    {
        public int? DepartmentId { get; set; }
        public int? LocationId { get; set; }
        public int? RoleId { get; set; }
        public string UserName { get; set; }
        public bool RestrictedByRole { get; set; }
    }
}
