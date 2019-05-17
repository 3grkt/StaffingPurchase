using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace StaffingPurchase.Web.Models.Awards
{
    public class AwardModel : ViewModelBase
    {
        public string Name { get; set; }
        public double PV { get; set; }
    }
}
