namespace StaffingPurchase.Core.Domain
{
    public partial class Status : EntityBase
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string StatusType { get; set; }
    }
}