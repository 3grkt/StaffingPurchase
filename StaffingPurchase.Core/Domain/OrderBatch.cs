using System;
using System.Collections.Generic;

namespace StaffingPurchase.Core.Domain
{
    public partial class OrderBatch : EntityBase
    {
        public OrderBatch()
        {
            Orders = new List<Order>();
        }

        public int Id { get; set; }
        public int LocationId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public short StatusId { get; set; }
        public Nullable<DateTime> ActionDate { get; set; }
        public string ActionComment { get; set; }
        public short TypeId { get; set; }
        public int? HrAdminApproverId { get; set; }
        public int? HrManagerApproverId { get; set; }
        public DateTime? HrAdminApprovalDate { get; set; }
        public DateTime? HrManagerApprovalDate { get; set; }
        public virtual Location Location { get; set; }
        public virtual ICollection<Order> Orders { get; set; }
        public virtual User HrAdminApprover { get; set; }
        public virtual User HrManagerApprover { get; set; }
    }
}
