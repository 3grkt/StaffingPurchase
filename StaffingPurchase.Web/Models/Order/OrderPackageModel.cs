using StaffingPurchase.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using StaffingPurchase.Web.Models.Report;

namespace StaffingPurchase.Web.Models.Order
{
    public class OrderPackageModel
    {
        public int? DepartmentId { get; set; }
        public int LocationId { get; set; }
        public InternalRequisitionFormModel PVDetails { get; set; }
        public SummaryDiscountProductModel DiscountDetails { get; set; }
    }
}