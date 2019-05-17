using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using StaffingPurchase.Web.Models.Common;

namespace StaffingPurchase.Web.Models.Report
{
    public class InternalRequisitionFormModel : JsonList<InternalRequisitionRowModel>
    {
        public decimal SummaryTotalPV { get; set; }
        public decimal SummaryTotalPrice { get; set; }
        public decimal SummaryTotalAmount { get; set; }
    }

    public class InternalRequisitionRowModel : ViewModelBase
    {
        public int No { get; set; }
        public string SKU { get; set; }
        public string SKUName { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalPrice { get; set; }
        public decimal PV { get; set; }
        public decimal TotalPV { get; set; }
        public string Department { get; set; }
        public string Location { get; set; }
    }
}