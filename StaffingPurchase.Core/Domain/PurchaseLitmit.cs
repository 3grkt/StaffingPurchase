namespace StaffingPurchase.Core.Domain
{
    public class PurchaseLitmit : EntityBase
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public virtual User User { get; set; }
        public int Year { get; set; }
        public short BoughtCount { get; set; }
    }
}