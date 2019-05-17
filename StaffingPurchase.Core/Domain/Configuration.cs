using System;

namespace StaffingPurchase.Core.Domain
{
    public partial class Configuration : EntityBase
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
        public string Description { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
    }
}
