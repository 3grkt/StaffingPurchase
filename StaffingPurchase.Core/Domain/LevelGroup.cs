using System.Collections.Generic;

namespace StaffingPurchase.Core.Domain
{
    public partial class LevelGroup : EntityBase
    {
        public LevelGroup()
        {
            Levels = new List<Level>();
        }

        public short Id { get; set; }
        public virtual ICollection<Level> Levels { get; set; }
        public string Name { get; set; }
        public double PV { get; set; }
    }
}