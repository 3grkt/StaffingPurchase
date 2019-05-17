namespace StaffingPurchase.Web.Models.LevelGroup
{
    public class LevelModel : ViewModelBase
    {
        public string Name { get; set; }
        public short? GroupId { get; set; }
        public string GroupName { get; set; }
        public new short Id { get; set; }
    }
}
