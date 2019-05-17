using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Http.Controllers;
using System.Web.Security;
using System.Security.Claims;
using Microsoft.Owin.Security.Cookies;
using StaffingPurchase.Web.Framework.Authentication;
using StaffingPurchase.Core.Infrastructure;
using StaffingPurchase.Services.Users;
using System.Security.Principal;
using System.Text.RegularExpressions;

namespace StaffingPurchase.Web.Framework.Filters
{
    public class ClaimsAuthorize : AuthorizeAttribute
    {
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            var httpContext = filterContext.HttpContext;
            if (httpContext.User is WindowsPrincipal)
            {
                var userName = Regex.Replace(httpContext.User.Identity.Name, ".*\\\\(.*)", "$1", RegexOptions.None);
                var user = EngineContext.Current.Resolve<IUserService>().GetUserByUsername(userName);
                if (user != null)
                {
                    var authManager = filterContext.HttpContext.GetOwinContext().Authentication;
                    ClaimsIdentity cookiesIdentity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationType);
                    cookiesIdentity.AddClaim(new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()));
                    cookiesIdentity.AddClaim(new Claim(ClaimTypes.Name, userName));

                    ApplicationUser applicationUser = new ApplicationUser(user);
                    ApplicationOAuthProvider.SetCustomDataForClaimIdentity(cookiesIdentity, applicationUser);
                    authManager.SignIn(cookiesIdentity);

                    // Change from WindowsIdentity to ClaimsPrincipal
                    filterContext.HttpContext.User = new ClaimsPrincipal(cookiesIdentity);
                }
            }
            base.OnAuthorization(filterContext);
        }
    }
}
