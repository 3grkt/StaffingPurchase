using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace StaffingPurchase.Web.Models.Order
{
    public class OrderHistory : ViewModelBase
    {
        public DateTime SessionStartDate { get; set; }
        public DateTime SessionEndDate { get; set; }
        public DateTime OrderDate { get; set; }
        public string OrderType { get; set; } // TODO: create OrderType enum
        public string OrderTypeDescription { get; set; }
        public decimal Value { get; set; }
        public string Status { get; set; } // TODO: create OrderStatus enum
        public int OrderId { get; set; }
    }
}