namespace StaffingPurchase.Web.Models.Order
{
    public class OrderDetailGridModel : ViewModelBase
    {
        public int OrderDetailId { get; set; }
        public decimal Price { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string ProductSku { get; set; }
        public double PV { get; set; }
        public int Volume { get; set; }
    }
}