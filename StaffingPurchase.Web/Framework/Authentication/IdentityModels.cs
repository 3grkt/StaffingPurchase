using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using StaffingPurchase.Core.Domain;
using StaffingPurchase.Services.Users;

namespace StaffingPurchase.Web.Framework.Authentication
{
    public class ApplicationUser : IUser<string>
    {
        private readonly User _user;

        public ApplicationUser(User user)
        {
            _user = user;
        }

        public string Id => _user.Id.ToString();

        public string UserName
        {
            get { return _user.UserName; }

            set { _user.UserName = value; }
        }

        public string FullName => _user.FullName;

        public string PasswordHash => _user.PasswordHash;

        public short RoleId => _user.Role.Id;

        public short[] Permissions
        {
            get { return _user.Role.Permissions.Select(x => x.Id).ToArray(); }
        }

        public int? LocationId => _user.LocationId;

        public int? DepartmentId => _user.DepartmentId;

        public string Language => _user.Language;

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager,
            string authenticationType)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, authenticationType);
            // Add custom user claims here
            return userIdentity;
        }
    }

    public class CustomUserStore : IUserStore<ApplicationUser>, IUserPasswordStore<ApplicationUser>
    {
        private readonly IUserService _userService;

        public CustomUserStore()
        {
        }

        public CustomUserStore(IUserService userService)
        {
            _userService = userService;
        }

        public Task CreateAsync(ApplicationUser user)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(ApplicationUser user)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
        }

        public Task<ApplicationUser> FindByIdAsync(string userId)
        {
            ApplicationUser applicationUser = null;
            var user = _userService.GetUserById(Convert.ToInt32(userId));
            if (user != null)
            {
                applicationUser = new ApplicationUser(user);
            }
            return Task.FromResult(applicationUser);
        }

        public Task<ApplicationUser> FindByNameAsync(string userName)
        {
            ApplicationUser applicationUser = null;
            var user = _userService.GetUserByUsername(userName);
            if (user != null)
            {
                applicationUser = new ApplicationUser(user);
            }
            return Task.FromResult(applicationUser);
        }

        public Task<string> GetPasswordHashAsync(ApplicationUser user)
        {
            return Task.FromResult(user.PasswordHash);
        }

        public Task<bool> HasPasswordAsync(ApplicationUser user)
        {
            return Task.FromResult(true);
        }

        public Task SetPasswordHashAsync(ApplicationUser user, string passwordHash)
        {
            throw new NotImplementedException();
        }

        public Task UpdateAsync(ApplicationUser user)
        {
            throw new NotImplementedException();
        }
    }
}
