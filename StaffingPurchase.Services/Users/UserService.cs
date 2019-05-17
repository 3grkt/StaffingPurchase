using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using OfficeOpenXml.FormulaParsing.ExpressionGraph;
using StaffingPurchase.Core;
using StaffingPurchase.Core.Data;
using StaffingPurchase.Core.Domain;
using StaffingPurchase.Core.SearchCriteria;
using StaffingPurchase.Data;
using StaffingPurchase.Services.Caching;
using StaffingPurchase.Services.CadenaIntegration;
using StaffingPurchase.Services.Configurations;
using StaffingPurchase.Services.Departments;
using StaffingPurchase.Services.LevelGroups;
using StaffingPurchase.Services.Localization;
using StaffingPurchase.Services.Locations;
using StaffingPurchase.Services.Logging;
using StaffingPurchase.Services.PV;

namespace StaffingPurchase.Services.Users
{
    public partial class UserService : ServiceBase, IUserService
    {
        #region Ctor.

        public UserService(
            IRepository<User> userRepository,
            IRepository<Role> roleRepository,
            ILocationService locationService,
            IDepartmentService departmentService,
            IPvLogService pvLogService,
            ICacheService cacheService,
            ICadenaIntegrationService cadenaIntegrationService,
            ILogger logger,
            IAppSettings appSettings,
            IAppPolicy appPolicy,
            IResourceManager resourceManager)
            : base(appSettings, appPolicy)
        {
            _userRepository = userRepository;
            _roleRepository = roleRepository;
            _locationService = locationService;
            _departmentService = departmentService;
            _pvLogService = pvLogService;
            _cacheService = cacheService;
            _cadenaIntegrationService = cadenaIntegrationService;
            _logger = logger;
            _resourceManager = resourceManager;
        }

        #endregion Ctor.

        #region Utility

        private string GetUserRoleResourceText(UserRole role)
        {
            return _resourceManager.GetString(string.Format("Enums.{0}.{1}", typeof(UserRole).Name, role));
        }

        #endregion Utility

        #region Fields

        private readonly IRepository<Role> _roleRepository;
        private readonly IRepository<User> _userRepository;
        private readonly ILocationService _locationService;
        private readonly IDepartmentService _departmentService;
        private readonly IPvLogService _pvLogService;
        private readonly ICacheService _cacheService;
        private readonly ICadenaIntegrationService _cadenaIntegrationService;
        private readonly ILogger _logger;
        private readonly IResourceManager _resourceManager;

        #endregion Fields

        #region Methods

        public IPagedList<User> GetAllUsers(int pageIndex, int pageSize)
        {
            var query = _userRepository.Table;
            query = query.OrderBy(u => u.UserName);
            return new PagedList<User>(query, pageIndex, pageSize);
        }

        public UserRole[] GetRolesByUserName(string username)
        {
            var query = _userRepository.TableNoTracking
                .Where(u => u.UserName == username);
            return query.Select(u => (UserRole)u.RoleId).ToArray();
        }

        public User GetUserById(int userId)
        {
            return _userRepository.GetById(userId);
        }

        public User GetUserByUsername(string userName)
        {
            var query = _userRepository.TableNoTracking
                .IncludeTable(x => x.Role)
                .IncludeTable(x => x.Role.Permissions)
                .IncludeTable(x => x.Location);
            return query.FirstOrDefault(x => x.UserName == userName);
        }

        public void InsertUser(User user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            if (GetUserByUsername(user.UserName) != null)
            {
                throw new StaffingPurchaseException("User.Validation.UserNameExisted");
            }
            if (string.IsNullOrEmpty(user.Language))
            {
                user.Language = _appSettings.DefaultAppCulture;
            }
            user.PasswordHash = CommonHelper.HashString(user.UserName, _appSettings.PasswordHashAlgorithm);
            _userRepository.Insert(user);
        }

        public void UpdateUser(User user)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));
            var updatingUser = _userRepository.GetById(user.Id);
            updatingUser.DepartmentId = user.DepartmentId;
            updatingUser.LocationId = user.LocationId;
            updatingUser.RoleId = user.RoleId;
            updatingUser.FullName = user.FullName;
            if (updatingUser.UserName != user.UserName)
            {
                if (GetUserByUsername(user.UserName) == null)
                {
                    throw new StaffingPurchaseException("User.Validation.UserNameExisted");
                }

                updatingUser.UserName = user.UserName;
            }

            _userRepository.Update(updatingUser);
        }

        public void ReserUserPassword(int userId, string password, bool useUserName = true)
        {
            var user = _userRepository.GetById(userId);
            if (user == null)
            {
                throw new ArgumentNullException(nameof(userId));
            }

            if (useUserName == true)
            {
                user.PasswordHash = CommonHelper.HashString(user.UserName);
            }
            else
            {
                if (!string.IsNullOrEmpty(password))
                {
                    user.PasswordHash = CommonHelper.HashString(password);
                }
                else
                {
                    throw new ArgumentNullException(nameof(password));
                }
            }

            _userRepository.Update(user);
        }

        public IPagedList<User> GetAllUsers(PaginationOptions pagingOptions, UserSearchCriteria filter, WorkingUser workingUser, bool includeReferences = false)
        {
            var users = _userRepository.TableNoTracking;

            if (includeReferences)
            {
                users = users
                    .IncludeTable(c => c.Department)
                    .IncludeTable(c => c.Location)
                    .IncludeTable(c => c.Role);
            }

            users = users.Where(FilterUsers(filter));

            if (filter.RestrictedByRole && !workingUser.IsInRole(UserRole.HRManager, UserRole.HRAdmin))
            {
                users = users.Where(x => x.LocationId == workingUser.LocationId);
            }

            users = !string.IsNullOrEmpty(pagingOptions.Sort)
                ? users.SortBy(pagingOptions.SortExpression)
                : users.OrderBy(c => c.Id);
            return new PagedList<User>(users, pagingOptions.PageIndex, pagingOptions.PageSize);
        }

        private Expression<Func<User, bool>> FilterUsers(UserSearchCriteria filterParams)
        {
            Expression<Func<User, bool>> filter = c => (string.IsNullOrEmpty(filterParams.UserName) || c.UserName.Trim().ToLower().Contains(filterParams.UserName.Trim().ToLower())) &&
                        (!filterParams.DepartmentId.HasValue || c.DepartmentId == filterParams.DepartmentId.Value) &&
                        (!filterParams.LocationId.HasValue || c.LocationId == filterParams.LocationId) &&
                        (!filterParams.RoleId.HasValue || c.RoleId == filterParams.RoleId);
            return filter;
        }


        public void DeleteUser(int id)
        {
            var user = _userRepository.TableNoTracking.FirstOrDefault(x => x.Id == id);
            if (user == null)
            {
                throw new ArgumentNullException(nameof(id));
            }

            if (user.RoleId == (int)UserRole.IT)
            {
                if (_userRepository.TableNoTracking.Count(c => c.RoleId == (int)UserRole.IT) == 1)
                {
                    throw new StaffingPurchaseException(_resourceManager.GetString("User.CannotDeleteOnlyIT"));
                }
            }

            _userRepository.Delete(_userRepository.GetById(id));
        }

        public void DeleteUser(User entity)
        {
            _userRepository.Delete(entity);
        }

        public IDictionary<short, string> GetAllRoles()
        {
            var allRoles = _cacheService.Get<IDictionary<short, string>>(CacheNames.Roles);
            if (allRoles == null)
            {
                allRoles = _roleRepository.TableNoTracking.ToDictionary(x => x.Id, x => x.Name);
                _cacheService.Set(CacheNames.Roles, allRoles);
            }
            return allRoles;
        }

        public IList<User> GetAllUsers(int? locationId = null, int? departmentId = null)
        {
            var query =
                _userRepository.TableNoTracking.Where(
                    c =>
                        (locationId == null || c.LocationId == locationId) &&
                        (departmentId == null || c.DepartmentId == departmentId));
            return query.ToList();
        }

        public IPagedList<User> GetUsers(int pageIndex, int pageSize, int? locationId = null, int userId = 0)
        {
            var query = _userRepository.TableNoTracking.IncludeTable(x => x.Role).IncludeTable(x => x.Location);

            if (userId != 0)
                query = query.Where(t => t.Id == userId);

            if (locationId != null)
                query = query.Where(t => t.LocationId == locationId);
            query = query.OrderBy(u => u.Id);
            return new PagedList<User>(query, pageIndex, pageSize);
        }

        #endregion Methods
    }
}
