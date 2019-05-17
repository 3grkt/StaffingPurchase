using System;
using System.Collections.Generic;

namespace StaffingPurchase.Core.Domain
{
    public partial class PackageLog : EntityBase
    {
        public PackageLog()
        {
            Orders = new List<Order>();
        }

        public int Id { get; set; }
        public int WarehouseUserId { get; set; }
        public string WarehouseUserName { get; set; }
        public int DepartmentId { get; set; }
        public int LocationId { get; set; }
        public int OrderType { get; set; }
        public DateTime PackedDate { get; set; }
        public bool FullPacked { get; set; }
        public string Comment { get; set; }
        public virtual Department Department { get; set; }
        public virtual Location Location { get; set; }
        public virtual ICollection<Order> Orders { get; set; }
        public virtual User WarehouseUser { get; set; }
    }
}