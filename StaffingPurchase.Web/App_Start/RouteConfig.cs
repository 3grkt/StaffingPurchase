using System.Web.Mvc;
using System.Web.Routing;

namespace StaffingPurchase.Web
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            // Login
            routes.MapRoute(
                name: "Login",
                url: "login",
                defaults: new { controller = "Home", action = "Login" }
                );

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new {controller = "Home", action = "Index", id = UrlParameter.Optional}
                );
        }
    }
}