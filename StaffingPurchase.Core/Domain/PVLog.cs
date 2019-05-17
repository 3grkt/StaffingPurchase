using System;

namespace StaffingPurchase.Core.Domain
{
    public partial class PVLog : EntityBase
    {
        public string Description { get; set; }
        public int Id { get; set; }
        public DateTime LogDate { get; set; }
        public double PV { get; set; }
        public virtual User User { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; }
        public short LogTypeId { get; set; }
        public string OrderSession { get; set; }
        public double? CurrentPV { get; set; }
    }
}
