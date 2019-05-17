using StaffingPurchase.Core;
using StaffingPurchase.Web.Extensions;
using System;

namespace StaffingPurchase.Web.Models.Order
{
    public class OrderModel : ViewModelBase
    {
        public int UserId { get; set; }
        public DateTime OrderDate { get; set; }
        public int TypeId { get; set; }
        public short StatusId { get; set; }
        public decimal Value { get; set; }
        public int? BatchId { get; set; }
        public int? PackageLogId { get; set; }
        
        public string UserName { get; set; }
        public string OrderType { get { return WebUtility.GetLocalizedStringForEnum((OrderType)TypeId); } }
        public string LocationName { get; set; }
        public string DepartmentName { get; set; }
    }
}
