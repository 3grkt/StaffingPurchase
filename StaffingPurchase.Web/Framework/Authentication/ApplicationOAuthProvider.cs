using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OAuth;
using Newtonsoft.Json;
using StaffingPurchase.Core;
using StaffingPurchase.Web.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;

namespace StaffingPurchase.Web.Framework.Authentication
{
    public class ApplicationOAuthProvider : OAuthAuthorizationServerProvider
    {
        private readonly string _publicClientId;

        public ApplicationOAuthProvider(string publicClientId)
        {
            if (publicClientId == null)
            {
                throw new ArgumentNullException("publicClientId");
            }

            _publicClientId = publicClientId;
        }

        public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {
            var userManager = context.OwinContext.GetUserManager<ApplicationUserManager>();

            ApplicationUser user = await userManager.FindAsync(context.UserName, context.Password);
            // Uncomment if need to get user by username only
            // ApplicationUser user = await userManager.FindByNameAsync(context.UserName);

            if (user == null)
            {
                context.SetError("invalid_grant", "The user name or password is incorrect.");
                return;
            }

            ClaimsIdentity oAuthIdentity = await user.GenerateUserIdentityAsync(userManager, OAuthDefaults.AuthenticationType);
            SetCustomDataForClaimIdentity(oAuthIdentity, user);

            ClaimsIdentity cookiesIdentity = await user.GenerateUserIdentityAsync(userManager, CookieAuthenticationDefaults.AuthenticationType);
            SetCustomDataForClaimIdentity(cookiesIdentity, user);

            AuthenticationProperties properties = CreateProperties(user.UserName);
            AuthenticationTicket ticket = new AuthenticationTicket(oAuthIdentity, properties);
            context.Validated(ticket);
            context.Request.Context.Authentication.SignIn(cookiesIdentity);
        }

        public override Task TokenEndpoint(OAuthTokenEndpointContext context)
        {
            foreach (KeyValuePair<string, string> property in context.Properties.Dictionary)
            {
                context.AdditionalResponseParameters.Add(property.Key, property.Value);
            }

            // Populate user info
            context.AdditionalResponseParameters.Add("permissions", JsonConvert.SerializeObject(context.Identity.GetUserPermissions()));
            context.AdditionalResponseParameters.Add("userlanguage", context.Identity.GetUserLanguage());

            return Task.FromResult<object>(null);
        }

        public override Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            // Resource owner password credentials does not provide a client ID.
            if (context.ClientId == null)
            {
                context.Validated();
            }

            return Task.FromResult<object>(null);
        }

        public override Task ValidateClientRedirectUri(OAuthValidateClientRedirectUriContext context)
        {
            if (context.ClientId == _publicClientId)
            {
                Uri expectedRootUri = new Uri(context.Request.Uri, "/");

                if (expectedRootUri.AbsoluteUri == context.RedirectUri)
                {
                    context.Validated();
                }
            }

            return Task.FromResult<object>(null);
        }

        public static AuthenticationProperties CreateProperties(string userName)
        {
            IDictionary<string, string> data = new Dictionary<string, string>
            {
                { "userName", userName }
            };
            return new AuthenticationProperties(data);
        }

        internal static void SetCustomDataForClaimIdentity(ClaimsIdentity identity, ApplicationUser user)
        {
            identity.AddClaim(new Claim(ClaimTypes.Role, user.RoleId.ToString()));
            identity.AddClaim(new Claim(CustomClaimTypes.FullName, user.FullName));
            identity.AddClaim(new Claim(CustomClaimTypes.Permissions, string.Join(",", user.Permissions)));
            identity.AddClaim(new Claim(CustomClaimTypes.LocationId, user.LocationId?.ToString() ?? string.Empty));
            identity.AddClaim(new Claim(CustomClaimTypes.DepartmentId, user.DepartmentId?.ToString() ?? string.Empty));
            identity.AddClaim(new Claim(CustomClaimTypes.Language, user.Language));
        }
    }
}
