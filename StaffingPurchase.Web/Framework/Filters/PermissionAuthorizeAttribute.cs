using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;
using StaffingPurchase.Core;

namespace StaffingPurchase.Web.Framework.Filters
{
    public class PermissionAuthorizeAttribute: AuthorizeAttribute
    {
        public UserPermission[] UserPermissions { get; set; }

        public PermissionAuthorizeAttribute(params UserPermission[] userPermissions)
        {
            UserPermissions = userPermissions;
        }

        /// <summary>
        /// Always "false" - use global if not configured at controlerl; otherwise, use controller's one.
        /// </summary>
        public override bool AllowMultiple => false;

        protected override bool IsAuthorized(HttpActionContext actionContext)
        {
            var principal = actionContext.RequestContext.Principal as ClaimsPrincipal;
            if (principal?.Identity == null || !principal.Identity.IsAuthenticated)
            {
                return false;
            }

            // Leverage parent's logic if not user role passed in
            if (UserPermissions == null || UserPermissions.Length == 0)
            {
                return base.IsAuthorized(actionContext);
            }

            var claimPermissions = principal.FindFirst(c => c.Type == "Permissions");
            if (claimPermissions != null)
            {
                var claimPermissionArr = claimPermissions.Value.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var userPermission in UserPermissions)
                {
                    if (claimPermissionArr.Any(c => c.Equals(userPermission.ToString("D"))))
                    {
                        return true;
                    }
                }
            }

            return false;
        }
    }
}