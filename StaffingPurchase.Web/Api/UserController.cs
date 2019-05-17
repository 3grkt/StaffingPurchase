using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using AutoMapper;
using StaffingPurchase.Core;
using StaffingPurchase.Core.Domain;
using StaffingPurchase.Core.SearchCriteria;
using StaffingPurchase.Services.Localization;
using StaffingPurchase.Services.Logging;
using StaffingPurchase.Services.Users;
using StaffingPurchase.Web.Extensions;
using StaffingPurchase.Web.Framework.Filters;
using StaffingPurchase.Web.Models.Common;
using StaffingPurchase.Web.Models.User;

namespace StaffingPurchase.Web.Api
{
    public class UserController : ApiControllerBase
    {
        private readonly IUserService _userService;
        private readonly IWorkContext _workContext;

        public UserController(IResourceManager resourceManager, IUserService userService, ILogger logger, IWorkContext workContext)
            : base(logger, resourceManager)
        {
            _userService = userService;
            _workContext = workContext;
        }

        [PermissionAuthorize(UserPermission.MaintainUser)]
        public HttpResponseMessage Delete(int id)
        {
            try
            {
                _userService.DeleteUser(id);
            }
            catch (Exception ex)
            {
                return RecordException(ex, "Error when deleting user", _resourceManager.GetString(ExceptionResources.GeneralException));
            }

            return Request.CreateResponse();
        }

        [PermissionAuthorize(UserPermission.MaintainUser)]
        public UserModel Get(int id)
        {
            try
            {
                var user = _userService.GetUserById(id);
                return Mapper.Map<User, UserModel>(user);
            }
            catch (Exception)
            {
                return new UserModel();
            }
        }

        [PermissionAuthorize(UserPermission.MaintainUser)]
        public JsonList<UserGridModel> GetAll([FromUri] PaginationOptions pagingOptions,
            [FromUri] UserSearchCriteria searchCriteria)
        {
            var pagingUsers = _userService.GetAllUsers(pagingOptions, searchCriteria, _workContext.User, true);
            var users = pagingUsers.ToList().ToModelList<User, UserGridModel>();

            return new JsonList<UserGridModel>
            {
                Data = users,
                TotalItems = pagingUsers.TotalCount
            };
        }

        [HttpGet]
        [Route("api/user/search-user")]
        [PermissionAuthorize(UserPermission.MaintainUser, UserPermission.ViewUserPvLog)]
        public JsonList<UserModel> SearchUser(
            [FromUri] UserSearchCriteria searchCriteria,
            [FromUri] PaginationOptions options)
        {
            options = GetPaginationOptions(options);
            searchCriteria.RestrictedByRole = true;
            var users = _userService.GetAllUsers(options, searchCriteria, _workContext.User, false);
            return new JsonList<UserModel>
            {
                Data = users.ToModelList<User, UserModel>(),
                TotalItems = users.TotalCount
            };
        }

        [HttpGet]
        [Route("api/user/get-by-department/{locationId}/{departmentId}")]
        public JsonList<UserDropDownModel> GetUsersByDepartment(int? locationId, int? departmentId)
        {
            var users =
                _userService.GetAllUsers(locationId, departmentId)
                    .Select(c => new UserDropDownModel { UserId = c.Id, FullName = c.FullName, UserName = c.UserName });
            var userDropDownModels = users as IList<UserDropDownModel> ?? users.ToList();
            return new JsonList<UserDropDownModel>
            {
                TotalItems = userDropDownModels.Count,
                Data = userDropDownModels
            };
        }

        [HttpPost]
        [Route("api/user/reset-pwd")]
        [PermissionAuthorize(UserPermission.ResetUserPassword)]
        public HttpResponseMessage ReserUserPassword([FromBody]int userId)
        {
            try
            {
                _userService.ReserUserPassword(userId);
            }
            catch (Exception ex)
            {
                return RecordException(ex, "Error when reseting user's password", _resourceManager.GetString(ExceptionResources.GeneralException));
            }

            return Request.CreateResponse();
        }

        [PermissionAuthorize(UserPermission.MaintainUser)]
        public HttpResponseMessage Post(UserModel user)
        {
            if (!ModelState.IsValid)
            {
                return Request.CreateErrorResponse(
                    HttpStatusCode.BadRequest,
                    new HttpError(ModelState.FirstError()));
            }

            try
            {
                var userDomain = Mapper.Map<UserModel, User>(user);
                _userService.InsertUser(userDomain);
            }
            catch (StaffingPurchaseException ex)
            {
                return RecordException(ex, ex.Message, ex.Message, LogLevel.Info);
            }
            catch (Exception ex)
            {
                return RecordException(ex, "Error when creating user", ExceptionResources.GeneralException);
            }

            return Request.CreateResponse();
        }

        [PermissionAuthorize(UserPermission.MaintainUser)]
        public HttpResponseMessage Put(UserModel user)
        {
            if (!ModelState.IsValid)
            {
                return Request.CreateErrorResponse(
                    HttpStatusCode.BadRequest,
                    new HttpError(ModelState.FirstError()));
            }

            try
            {
                _userService.UpdateUser(Mapper.Map<UserModel, User>(user));
            }
            catch (StaffingPurchaseException ex)
            {
                return RecordException(ex, ex.Message, ex.Message, LogLevel.Info);
            }
            catch (Exception ex)
            {
                return RecordException(ex, "Error when updating user", ExceptionResources.GeneralException);
            }

            return Request.CreateResponse();
        }

        [HttpPost]
        [Route("api/user/update-language")]
        public HttpResponseMessage UpdateLanguage(string locale)
        {
            var currentUser = _userService.GetUserById(_workContext.User.Id);
            if (currentUser == null)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(
                    HttpStatusCode.NotFound,
                    new HttpError(_resourceManager.GetString("Common.DataNotFound"))));
            }

            currentUser.Language = locale;
            _userService.UpdateUser(currentUser);

            return Request.CreateResponse();
        }
    }
}
