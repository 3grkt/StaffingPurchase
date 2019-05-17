using System.Web.Http;
using System.Web.Http.ExceptionHandling;
using Elmah.Contrib.WebApi;
using StaffingPurchase.Web.Framework.Filters;

namespace StaffingPurchase.Web
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // ELMAH - TODO: check if this works
            config.Services.Add(typeof(IExceptionLogger), new ElmahExceptionLogger());

            // Filters
            config.Filters.Add(new RoleAuthorizeAttribute());
            config.Filters.Add(new PermissionAuthorizeAttribute());

            // Routing
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "CommonApi",
                routeTemplate: "api/common/{action}",
                defaults: new { controller = "Common" }
                );

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
                );
            //config.Routes.MapHttpRoute(
            //    name: "DefaultApiWithAction",
            //    routeTemplate: "api/{controller}/{action}/{id}",
            //    defaults: new { id = RouteParameter.Optional }
            //);


            // Uncomment the following line of code to enable query support for actions with an IQueryable or IQueryable<T> return type.
            // To avoid processing unexpected or malicious queries, use the validation settings on QueryableAttribute to validate incoming queries.
            // For more information, visit http://go.microsoft.com/fwlink/?LinkId=279712.
            //config.EnableQuerySupport();
            
            // Json formatter
            config.Formatters.JsonFormatter.SerializerSettings.DateTimeZoneHandling = Newtonsoft.Json.DateTimeZoneHandling.Local; // ensure date is parsed correctly in client side
            //config.Formatters.JsonFormatter.SerializerSettings.ContractResolver =
            //    new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver();
            //config.Formatters.JsonFormatter.SerializerSettings.DateFormatHandling = Newtonsoft.Json.DateFormatHandling.MicrosoftDateFormat;
        }
    }
}
