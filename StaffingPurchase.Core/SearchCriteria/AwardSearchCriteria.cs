namespace StaffingPurchase.Core.SearchCriteria
{
    public class AwardSearchCriteria : SearchCriteriaBase
    {
        public string Name { get; set; }
        public double Pv { get; set; }
        public bool NoPaging { get; set; }
    }
}
