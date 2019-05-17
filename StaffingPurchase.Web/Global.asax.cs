using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Security.Principal;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web;
using System.Web.Caching;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.Security;
using FluentValidation.WebApi;
using StaffingPurchase.Core;
using StaffingPurchase.Core.Infrastructure;
using StaffingPurchase.Services.Logging;
using StaffingPurchase.Web.Framework;
using StaffingPurchase.Web.Infrastructure;

namespace StaffingPurchase.Web
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : HttpApplication
    {
        #region Events

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            // Initialize engine context
            EngineContext.CreateEngineInstance = () => new WebEngine(); // engine object used in Web app
            EngineContext.GetDependencyRegistrars = () => new IDependencyRegistrar[]
            {
                // dependency registrar
                new CoreDependencyRegistrar(),
                new DependencyRegistrar(),
                new BatchJobTestDependencyRegistrar()
            };
            EngineContext.Initialize();

            // Object Mapping
            AutoMapperConfiguration.ConfigMapping();

            // FluentValidation
            DataAnnotationsModelValidatorProvider.AddImplicitRequiredAttributeForValueTypes = false;
            FluentValidationModelValidatorProvider.Configure(GlobalConfiguration.Configuration,
                x => x.ValidatorFactory = new ModelValidatorFactory());
        }

        protected void Application_Error(object sender, EventArgs e)
        {
            if (HttpContext.Current.IsDebuggingEnabled) // for debug
                return;

            var error = Server.GetLastError();
            if (error != null)
            {
                var httpException = error as HttpException;
                if (httpException != null && httpException.GetHttpCode() == (int)HttpStatusCode.NotFound)
                    return;

                Server.ClearError();
                EngineContext.Current.Resolve<ILogger>()
                    .Error("Application Error: " + error.Message, error, LogSource.Web);
                Response.RedirectToRoute(new { controller = "Error", action = "Index" });
            }
        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {
            var request = HttpContext.Current.Request;
            var languageCoookie = request.Cookies["userlanguage"];
            if (languageCoookie != null)
            {
                SetThreadCulture(languageCoookie.Value);
            }
            else
            {
                // For now - use default language (ignore browser languages)
                //var browserLanguage = request.UserLanguages?.FirstOrDefault();
                //SetThreadCulture(!string.IsNullOrEmpty(browserLanguage)
                //    ? browserLanguage
                //    : EngineContext.Current.Resolve<IAppSettings>().DefaultAppCulture);
                SetThreadCulture(EngineContext.Current.Resolve<IAppSettings>().DefaultAppCulture);
            }
        }

        protected void Application_EndRequest(object sender, EventArgs e)
        {
            // we only want 302 redirects if they are for login purposes
            if (this.Response.StatusCode == 302 && this.Response.RedirectLocation.Contains("/login"))
            {
                if (IsIntranetRequest(Request.UserHostAddress))
                {
                    this.Response.StatusCode = 401;
                    // note that the following line is .NET 4.5 or later only
                    // otherwise you have to suppress the return URL etc manually!
                    this.Response.SuppressFormsAuthenticationRedirect = true;
                }
            }
        }

        #endregion

        #region Utility

        private bool IsIntranetRequest(string userHostAddress)
        {
            // IPv6
            if (userHostAddress.Contains(":"))
            {
                var hostAddresses = Dns.GetHostAddresses(userHostAddress);
                return hostAddresses != null && hostAddresses.Length > 0 && hostAddresses[0].IsIPv6LinkLocal;
            }

            // IPv4
            var userIpAddress = IPAddress.Parse(userHostAddress);
            const string ipCacheKey = "Ipv4PrivateAddresses";
            var ipv4PrivateAddresses = HttpContext.Current.Cache.Get(ipCacheKey) as List<Tuple<IPAddress, IPAddress>>;
            if (ipv4PrivateAddresses == null)
            {
                ipv4PrivateAddresses = CommonHelper.GetIpv4PrivateAddresses();
                HttpContext.Current.Cache.Add(ipCacheKey, ipv4PrivateAddresses, null, Cache.NoAbsoluteExpiration, TimeSpan.FromMinutes(30), CacheItemPriority.Normal, null);
            }
            return ipv4PrivateAddresses.Any(ip => userIpAddress.GreaterThanOrEqual(ip.Item1) && userIpAddress.LessThanOrEqual(ip.Item2));
        }

        private void SetThreadCulture(string culture)
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo(culture);
            Thread.CurrentThread.CurrentUICulture = Thread.CurrentThread.CurrentCulture;
        }

        #endregion
    }
}

