namespace StaffingPurchase.Core.SearchCriteria
{
    public class ProductListSearchCriteria : SearchCriteriaBase
    {
        public string Sku { get; set; }
        public string Name { get; set; }
        public int CategoryId { get; set; }
        public bool ActiveOnly { get; set; }
    }
}
