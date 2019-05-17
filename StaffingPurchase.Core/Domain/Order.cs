using System;
using System.Collections.Generic;

namespace StaffingPurchase.Core.Domain
{
    public enum OrderStatusType
    {
        Draft = 101,
        Submitted = 102,
        Approved = 103,
        Packaged = 104
    }

    public partial class Order : EntityBase
    {
        public Order()
        {
            OrderDetails = new List<OrderDetail>();
        }

        public int Id { get; set; }
        public int UserId { get; set; }
        public DateTime OrderDate { get; set; }
        public int TypeId { get; set; }
        public short StatusId { get; set; }
        public decimal Value { get; set; }
        public Nullable<int> BatchId { get; set; }
        public Nullable<int> PackageLogId { get; set; }
        public Nullable<int> DepartmentId { get; set; }
        public Nullable<int> LocationId { get; set; }
        public virtual User User { get; set; }
        public virtual OrderBatch OrderBatch { get; set; }
        public virtual PackageLog PackageLog { get; set; }
        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
        public virtual Department Department { get; set; }
        public virtual Location Location { get; set; }
    }
}