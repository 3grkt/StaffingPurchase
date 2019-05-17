using System.Security.Claims;
using System.Web.Http;
using System.Web.Http.Controllers;
using StaffingPurchase.Core;

namespace StaffingPurchase.Web.Framework.Filters
{
    public class RoleAuthorizeAttribute : AuthorizeAttribute
    {
        public UserRole[] UserRoles { get; set; }

        public RoleAuthorizeAttribute(params UserRole[] userRoles)
        {
            UserRoles = userRoles;
        }

        /// <summary>
        /// Always "false" - use global if not configured at controlerl; otherwise, use controller's one.
        /// </summary>
        public override bool AllowMultiple
        {
            get
            {
                return false;
            }
        }

        protected override bool IsAuthorized(HttpActionContext actionContext)
        {
            var principal = actionContext.RequestContext.Principal as ClaimsPrincipal;
            if (principal == null || principal.Identity == null || !principal.Identity.IsAuthenticated)
            {
                return false;
            }

            // Leverage parent's logic if not user role passed in
            if (UserRoles == null || UserRoles.Length == 0)
            {
                return base.IsAuthorized(actionContext);
            }

            foreach (var role in UserRoles)
            {
                if (principal.IsInRole(role.ToString("D")))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
