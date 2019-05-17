using System.Collections.Generic;

namespace StaffingPurchase.Web.Models.Order
{
    public class OrderApprovalModel
    {
        public OrderBatchModel Batch { get; set; }
        public IList<OrderModel> Orders { get; set; }
        public int TotalOrders { get; set; }
    }
}