using StaffingPurchase.Web.Framework.UI;
using System.Web.Mvc;

namespace StaffingPurchase.Web.Framework.Controllers
{
    [HandleError]
    public abstract class WebControllerBase : Controller
    {
        #region Actions

        public ActionResult PageError()
        {
            return new ErrorResult();
        }

        public ActionResult PageNotFound(string message = null)
        {
            return new NotFoundResult(message);
        }

        public ActionResult PageForbidden()
        {
            return new ForbiddenResult();
        }

        public JsonResult AjaxResult(AjaxProcessResult result)
        {
            return Json(new { result = result.ToString() });
        }

        #endregion Actions
    }
}