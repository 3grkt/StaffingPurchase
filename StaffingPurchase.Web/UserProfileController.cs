using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.UI.WebControls;
using StaffingPurchase.Core;
using StaffingPurchase.Services.Users;
using StaffingPurchase.Web.Api;
using StaffingPurchase.Web.Extensions;
using StaffingPurchase.Web.Models.User;

namespace StaffingPurchase.Web
{
    public class UserProfileController : ApiControllerBase
    {
        private readonly IUserService _userService;
        private readonly IWorkContext _workContext;

        public UserProfileController(IUserService userService,
            IWorkContext workContext)
        {
            _userService = userService;
            _workContext = workContext;
        }

        [HttpPost]
        [Route("api/user-profile/change-password")]
        public HttpResponseMessage ChangePassword(UserPasswordModel password)
        {
            if (!ModelState.IsValid)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, new HttpError(ModelState.FirstError()));

            }

            try
            {
                _userService.ReserUserPassword(_workContext.User.Id, password.NewPassword, false);
            }
            catch (StaffingPurchaseException ex)
            {
                return RecordException(ex, ex.Message, ex.Message, LogLevel.Info);
            }
            catch (Exception ex)
            {
                return RecordException(ex, "Error when changing user password", ExceptionResources.GeneralException);
            }

            return Request.CreateResponse();
        }
    }
}