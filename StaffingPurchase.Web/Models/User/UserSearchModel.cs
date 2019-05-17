using System.Collections.Generic;
using System.Web.Mvc;
using StaffingPurchase.Web.Framework;

namespace StaffingPurchase.Web.Models.User
{
    public class UserSearchModel : ViewModelBase
    {
        [ResourceDisplayName("User.Warehouse")]
        public string WarehouseId { get; set; }

        [ResourceDisplayName("User.Name")]
        public int? UserId { get; set; }

        public IList<SelectListItem> WarehouseList { get; set; }
        public IList<SelectListItem> Users { get; set; }
    }
}