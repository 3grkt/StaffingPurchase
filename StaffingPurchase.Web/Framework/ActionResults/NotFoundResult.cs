using System.Web.Mvc;

namespace StaffingPurchase.Web.Framework
{
    public class NotFoundResult : ActionResult
    {
        private readonly string _message;

        public NotFoundResult(string message = null)
        {
            _message = message;
        }

        public override void ExecuteResult(ControllerContext context)
        {
            context.HttpContext.Response.StatusCode = (int) System.Net.HttpStatusCode.NotFound;
            context.HttpContext.Items["ErrorMessage"] = _message ?? string.Empty;
            new ViewResult {ViewName = "ErrorNotFound"}.ExecuteResult(context);
        }
    }
}