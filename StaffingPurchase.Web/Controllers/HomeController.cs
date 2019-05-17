using System.Web.Mvc;
using StaffingPurchase.Web.Framework.Controllers;
using StaffingPurchase.Web.Framework.Filters;

namespace StaffingPurchase.Web.Controllers
{
    public class HomeController : WebControllerBase
    {
        [ClaimsAuthorize]
        public ActionResult Index()
        {
            return View();
        }

        [AllowAnonymous]
        public ActionResult Login()
        {
            return View();
        }
    }
}
