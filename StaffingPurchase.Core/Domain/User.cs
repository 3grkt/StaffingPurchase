using System;
using System.Collections.Generic;

namespace StaffingPurchase.Core.Domain
{
    public partial class User : EntityBase
    {
        public User()
        {
            Orders = new List<Order>();
            PackageLogs = new List<PackageLog>();
            PVLogs = new List<PVLog>();
            Awards = new List<Award>();
        }

        public double CurrentPV { get; set; }
        public DateTime DateOfBirth { get; set; }
        public Nullable<int> DepartmentId { get; set; }
        public Nullable<DateTime> EndDate { get; set; }
        public string FullName { get; set; }
        public int Id { get; set; }
        public string CostCenter { get; set; }
        public Nullable<short> LevelId { get; set; }
        public Nullable<int> LocationId { get; set; }
        public string PasswordHash { get; set; }
        public short RoleId { get; set; }
        public Nullable<DateTime> StartDate { get; set; }
        public string UserName { get; set; }
        public string Language { get; set; }
        public string EmailAddress { get; set; }

        public virtual ICollection<Award> Awards { get; set; }
        public virtual Department Department { get; set; }
        public virtual Level Level { get; set; }
        public virtual Location Location { get; set; }
        public virtual ICollection<Order> Orders { get; set; }
        public virtual ICollection<PackageLog> PackageLogs { get; set; }
        public virtual ICollection<PurchaseLitmit> PurchaseLimits { get; set; }
        public virtual ICollection<PVLog> PVLogs { get; set; }
        public virtual Role Role { get; set; }
        public virtual ICollection<OrderBatch> BatchesApprovedByHrAdmin { get; set; }
        public virtual ICollection<OrderBatch> BatchesApprovedByHrManager { get; set; }
    }
}
