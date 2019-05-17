using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace StaffingPurchase.Web.Models.Order
{
    public class OrderViewModel: ViewModelBase
    {
        public OrderHistory Order { get; set; }
        public IList<OrderDetailGridModel> OrderDetails { get; set; }  
    }
}