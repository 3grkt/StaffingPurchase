using System;
using AutoMapper.QueryableExtensions;
using StaffingPurchase.Core.Domain;
using StaffingPurchase.Core.Infrastructure;
using StaffingPurchase.Services.LevelGroups;
using StaffingPurchase.Web.Extensions;
using StaffingPurchase.Web.Models.Common;
using StaffingPurchase.Web.Models.LevelGroup;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using StaffingPurchase.Core;
using StaffingPurchase.Services.Localization;
using StaffingPurchase.Services.Logging;
using StaffingPurchase.Web.Framework.Filters;

namespace StaffingPurchase.Web.Api
{
    [PermissionAuthorize(UserPermission.MaintainLevelGroup)]
    public class LevelController : ApiControllerBase
    {
        private readonly ILevelService _levelService;

        public LevelController(ILevelService levelService, ILogger logger, IResourceManager resourceManager) : base(logger, resourceManager)
        {
            _levelService = levelService;
        }

        public JsonList<LevelModel> GetAll()
        {
            var levels = EngineContext.Current.Resolve<ILevelService>().GetAllLevels();
            var levelModels = levels.ToList().ToModelList<Level, LevelModel>();

            return new JsonList<LevelModel>()
            {
                TotalItems = levelModels.Count,
                Data = levelModels
            };
        }

        [HttpPost]
        [Route("api/level/saveall")]
        public HttpResponseMessage SaveAll(IEnumerable<LevelModel> levelModels)
        {
            try
            {
                _levelService.UpdateMultiLevels(levelModels.AsQueryable().Project().To<Level>().AsEnumerable());

                return Request.CreateResponse();
            }
            catch (StaffingPurchaseException ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, new HttpError(ex.Message));
            }
            catch (Exception ex)
            {
                _logger.Error("Error when saving multiple Levels", ex);
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, new HttpError(_resourceManager.GetString(ExceptionResources.GeneralException)));
            }
        }
    }
}