using System.Web.Mvc;
using StaffingPurchase.Web.Framework.Controllers;

namespace StaffingPurchase.Web.Controllers
{
    public class ErrorController : WebControllerBase
    {
        public ActionResult Index()
        {
            return PageError();
        }

        public ActionResult NotFound()
        {
            return PageNotFound();
        }

        public ActionResult Forbidden()
        {
            return PageForbidden();
        }
    }
}