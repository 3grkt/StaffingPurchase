using System.Collections.Generic;

namespace StaffingPurchase.Core.Domain
{
    public partial class Role : EntityBase
    {
        public Role()
        {
            Users = new List<User>();
            Permissions = new List<Permission>();
        }

        public short Id { get; set; }
        public string Name { get; set; }
        public virtual ICollection<User> Users { get; set; }
        public virtual ICollection<Permission> Permissions { get; set; }
    }
}