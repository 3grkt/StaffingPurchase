using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using StaffingPurchase.Web.Models.Common;

namespace StaffingPurchase.Web.Models.Report
{
    public class SummaryDiscountProductModel : JsonList<SummaryDiscountProductRowModel>
    {
        public decimal SummaryTotalPrice { get; set; }
        public decimal SummaryTotalDiscountedPrice { get; set; }
        public decimal SummaryTotalAmount { get; set; }
    }

    public class SummaryDiscountProductRowModel : ViewModelBase
    {
        public int No { get; set; }
        public string SKU { get; set; }
        public string SKUName { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalPrice { get; set; }
        public decimal UnitDiscountedPrice { get; set; }
        public decimal TotalDiscountedPrice { get; set; }
        public string Department { get; set; }
        public string Location { get; set; }
    }
}