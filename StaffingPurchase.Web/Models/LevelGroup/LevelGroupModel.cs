using FluentValidation.Attributes;
using StaffingPurchase.Web.Validators;

namespace StaffingPurchase.Web.Models.LevelGroup
{
    [Validator(typeof(LevelGroupValidator))]
    public class LevelGroupModel : ViewModelBase
    {
        public string Name { get; set; }
        public double PV { get; set; }
        public new short Id { get; set; }
    }
}
