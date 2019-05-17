using System.Collections.Generic;

namespace StaffingPurchase.Core.Domain
{
    public partial class Award : EntityBase
    {
        public Award()
        {
            Users = new List<User>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public double PV { get; set; }
        public virtual ICollection<User> Users { get; set; }
    }
}
