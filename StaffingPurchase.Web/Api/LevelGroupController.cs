using AutoMapper;
using AutoMapper.QueryableExtensions;
using StaffingPurchase.Core;
using StaffingPurchase.Core.Domain;
using StaffingPurchase.Services.LevelGroups;
using StaffingPurchase.Services.Localization;
using StaffingPurchase.Services.Logging;
using StaffingPurchase.Web.Extensions;
using StaffingPurchase.Web.Models.Common;
using StaffingPurchase.Web.Models.LevelGroup;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using StaffingPurchase.Web.Framework.Filters;

namespace StaffingPurchase.Web.Api
{
    [PermissionAuthorize(UserPermission.MaintainLevelGroup)]
    public class LevelGroupController : ApiControllerBase
    {
        private readonly ILevelGroupService _service;

        public LevelGroupController(ILevelGroupService service, ILogger logger, IResourceManager resourceManager) : base(logger, resourceManager)
        {
            _service = service;
        }

        public HttpResponseMessage Delete(int id)
        {
            try
            {
                _service.DeleteLevelGroup(id);
                return Request.CreateResponse();
            }
            catch (Exception ex)
            {
                _logger.Error("Error when deleting Level Group", ex);
                return Request.CreateErrorResponse(
                   HttpStatusCode.InternalServerError,
                   new HttpError(_resourceManager.GetString(ExceptionResources.GeneralException)));
            }
            
        }

        public JsonList<LevelGroupModel> GetAll()
        {
            var levelGroups = _service.GetAllLevelGroups();
            var levelGroupModels = levelGroups.ToList().ToModelList<LevelGroup, LevelGroupModel>();

            return new JsonList<LevelGroupModel>()
            {
                TotalItems = levelGroupModels.Count,
                Data = levelGroupModels
            };
        }

        public HttpResponseMessage Post(LevelGroupModel levelGroup)
        {
            if (!ModelState.IsValid)
            {
                return Request.CreateErrorResponse(
                    HttpStatusCode.BadRequest,
                    new HttpError(ModelState.FirstError()));
            }

            try
            {
                _service.InsertLevelGroup(Mapper.Map<LevelGroup>(levelGroup));
                return Request.CreateResponse();
            }
            catch (Exception ex)
            {
                _logger.Error("Error when inserting new Level Group", ex);
                return Request.CreateErrorResponse(
                   HttpStatusCode.InternalServerError,
                   new HttpError(_resourceManager.GetString(ExceptionResources.GeneralException)));
            }
        }

        public HttpResponseMessage Put(LevelGroupModel levelGroup)
        {
            if (!ModelState.IsValid)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest,
                    new HttpError(ModelState.FirstError()));
            }

            try
            {
                _service.UpdateLevelGroup(Mapper.Map<LevelGroup>(levelGroup));
            }
            catch (Exception ex)
            {
                _logger.Error("Error when updating Level Group", ex);
                return Request.CreateErrorResponse(
                   HttpStatusCode.InternalServerError,
                   new HttpError(_resourceManager.GetString(ExceptionResources.GeneralException)));
            }

            return Request.CreateResponse();
        }

        [HttpPost]
        [ActionName("SaveAll")]
        [Route("api/levelgroup/saveall")]
        public HttpResponseMessage SaveAll(IEnumerable<LevelGroupModel> levelGroupModels)
        {
            try
            {
                _service.UpdateMultiLevelGroups(levelGroupModels.AsQueryable().Project().To<LevelGroup>().AsEnumerable());
                return Request.CreateResponse();
            }
            catch (StaffingPurchaseException ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, new HttpError(ex.Message));
            }
            catch (Exception ex)
            {
                _logger.Error("Error when saving multiple Level Groups", ex);
                return Request.CreateErrorResponse(
                   HttpStatusCode.InternalServerError,
                   new HttpError(_resourceManager.GetString(ExceptionResources.GeneralException)));
            }
        }
    }
}