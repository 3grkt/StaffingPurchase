using System.Collections.Generic;

namespace StaffingPurchase.Core.Domain
{
    public partial class Location : EntityBase
    {
        public Location()
        {
            Users = new List<User>();
            OrderBatches = new List<OrderBatch>();
            PackageLogs = new List<PackageLog>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public virtual ICollection<User> Users { get; set; }
        public virtual ICollection<OrderBatch> OrderBatches { get; set; }
        public virtual ICollection<PackageLog> PackageLogs { get; set; }
        public virtual ICollection<Order> Orders { get; set; }
    }
}