using System.Collections.Generic;

namespace StaffingPurchase.Core.Domain
{
    public partial class Department : EntityBase
    {
        public Department()
        {
            Users = new List<User>();
            PackageLogs = new List<PackageLog>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public virtual ICollection<User> Users { get; set; }
        public virtual ICollection<PackageLog> PackageLogs { get; set; }
        public virtual ICollection<Order> Orders { get; set; }
    }
}
