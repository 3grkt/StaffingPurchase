using StaffingPurchase.Core;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Security.Claims;
using System.Web;

namespace StaffingPurchase.Web.Extensions
{
    public static class IdentityExtensions
    {
        public static IList<short> GetUserPermissions(this ClaimsIdentity identity)
        {
            var permissionClaim = identity.FindFirst(CustomClaimTypes.Permissions);
            if (permissionClaim != null)
            {
                var permissions = permissionClaim.Value.Split(',');
                return permissions.Select(short.Parse).ToList();
            }

            return new List<short>();
        }

        public static string GetUserLanguage(this ClaimsIdentity identity)
        {
            return identity.FindFirst(CustomClaimTypes.Language)?.Value;
        }
    }
}
