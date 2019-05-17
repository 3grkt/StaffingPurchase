using System.Web.Mvc;

namespace StaffingPurchase.Web.Framework
{
    public class ErrorResult : ActionResult
    {
        public override void ExecuteResult(ControllerContext context)
        {
            context.HttpContext.Response.StatusCode = (int) System.Net.HttpStatusCode.InternalServerError;
            new ViewResult {ViewName = "Error"}.ExecuteResult(context);
        }
    }
}