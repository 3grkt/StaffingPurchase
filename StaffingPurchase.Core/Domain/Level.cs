using System.Collections.Generic;

namespace StaffingPurchase.Core.Domain
{
    public partial class Level : EntityBase
    {
        public Level()
        {
            Users = new List<User>();
        }

        public short Id { get; set; }
        public string Name { get; set; }
        public short? GroupId { get; set; }
        public virtual ICollection<User> Users { get; set; }
        public virtual LevelGroup LevelGroup { get; set; }
    }
}
