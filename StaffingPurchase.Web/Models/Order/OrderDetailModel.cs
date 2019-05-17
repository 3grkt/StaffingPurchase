namespace StaffingPurchase.Web.Models.Order
{
    public class OrderDetailModel
    {
        public int ProductId { get; set; }
        public int Volume { get; set; }

        public bool IsPrice { get; set; }
    }
}