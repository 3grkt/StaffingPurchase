using StaffingPurchase.Core;
using System;
using System.Collections.Generic;

namespace StaffingPurchase.Web.Models.Order
{
    public class OrderUpdateModel : ViewModelBase
    {
        public IEnumerable<OrderDetailGridModel> OrderDetails { get; set; }
        public OrderStatus Status { get; set; }
        public OrderType Type { get; set; }
        public int UserId { get; set; }

        // Metedata
        public string Location { get; set; }
        public string Department { get; set; }
        public string User { get; set; }
        public string OrderSession { get; set; }
    }
}
