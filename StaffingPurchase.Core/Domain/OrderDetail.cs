namespace StaffingPurchase.Core.Domain
{
    public partial class OrderDetail : EntityBase
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public int ProductId { get; set; }
        public int Volume { get; set; }
        public virtual Order Order { get; set; }
        public virtual Product Product { get; set; }
    }
}