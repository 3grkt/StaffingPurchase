using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Hosting;
using System.Web.Http;
using StaffingPurchase.Core;
using StaffingPurchase.Core.Domain;
using StaffingPurchase.Core.SearchCriteria;
using StaffingPurchase.Services.Awards;
using StaffingPurchase.Services.ImportExport;
using StaffingPurchase.Services.Localization;
using StaffingPurchase.Services.Logging;
using StaffingPurchase.Web.Extensions;
using StaffingPurchase.Web.Framework.Filters;
using StaffingPurchase.Web.Models.Awards;
using StaffingPurchase.Web.Models.Common;

namespace StaffingPurchase.Web.Api
{
    [JsonCamelCaseConfig]
    [RoutePrefix("api/award")]
    [PermissionAuthorize(UserPermission.CreateAward, UserPermission.UploadAwardList)]
    public class AwardController : ApiControllerBase
    {
        private readonly IAwardService _awardService;
        private readonly IImportManager _importManager;
        private readonly IWorkContext _workContext;
        private readonly IAppSettings _appSettings;

        public AwardController(
            IAwardService awardService,
            ILogger logger,
            IImportManager importManager,
            IWorkContext workContext,
            IResourceManager resourceManager,
            IAppSettings appSettings)
            : base(logger, resourceManager)
        {
            _awardService = awardService;
            _importManager = importManager;
            _workContext = workContext;
            _appSettings = appSettings;
        }

        public AwardModel Get(int id)
        {
            var award = _awardService.GetById(id);
            if (award == null)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(
                    HttpStatusCode.NotFound,
                    new HttpError(_resourceManager.GetString("Common.DataNotFound"))));
            }

            return award.ToModel<AwardModel>();
        }

        public JsonList<AwardModel> GetAll(
            [FromUri] AwardSearchCriteria searchCriteria,
            [FromUri] PaginationOptions options)
        {
            options = GetPaginationOptions(options);

            var model = new JsonList<AwardModel>();
            if (searchCriteria.NoPaging)
            {
                var allAwards = _awardService.GetAll();
                model.Data = allAwards.ToModelList<Award, AwardModel>();
                model.TotalItems = allAwards.Count;
            }
            else
            {
                var searchData = _awardService.Search(searchCriteria, options, _workContext.User);
                model.Data = searchData.ToModelList<Award, AwardModel>();
                model.TotalItems = searchData.TotalCount;
            }
            return model;
        }

        [HttpPost]
        public HttpResponseMessage Post(AwardModel awardModel)
        {
            if (!ModelState.IsValid)
            {
                return Request.CreateErrorResponse(
                    HttpStatusCode.BadRequest,
                    new HttpError(ModelState.FirstError()));
            }

            var award = awardModel.ToEntity<Award>();
            return SaveChanges(award, x => _awardService.Insert(x), _resourceManager.GetString("Award.FailedToCreate"));
        }

        [HttpPut]
        public HttpResponseMessage Put(AwardModel awardModel)
        {
            if (!ModelState.IsValid)
            {
                return Request.CreateErrorResponse(
                    HttpStatusCode.BadRequest,
                    new HttpError(ModelState.FirstError()));
            }

            var award = _awardService.GetById(awardModel.Id);
            if (award == null)
            {
                return Request.CreateErrorResponse(
                    HttpStatusCode.NotFound,
                    new HttpError(_resourceManager.GetString("Common.DataNotFound")));
            }

            award = awardModel.ToEntity(award);
            return SaveChanges(award, x => _awardService.Update(x), _resourceManager.GetString("Award.FailedToUpdate"));
        }

        [HttpDelete]
        public HttpResponseMessage Delete(int id)
        {
            var savedAward = _awardService.GetById(id);
            if (savedAward == null)
            {
                return Request.CreateErrorResponse(
                    HttpStatusCode.NotFound,
                    new HttpError(_resourceManager.GetString("Common.DataNotFound")));
            }

            return SaveChanges(savedAward, x => _awardService.Delete(x), _resourceManager.GetString("Award.FailedToDelete"));
        }

        [HttpPost]
        public async Task<HttpResponseMessage> Upload(string sheetName, int awardId)
        {
            // Check if the request contains multipart/form-data.
            if (!Request.Content.IsMimeMultipartContent())
            {
                throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
            }

            var root = HostingEnvironment.MapPath("~/App_Data");
            var provider = new MultipartFormDataStreamProvider(root);
            string uploadedFilePath = null;
            try
            {
                // Read the form data.
                await Request.Content.ReadAsMultipartAsync(provider);

                // TODO: find solution to read data from stream
                uploadedFilePath = provider.FileData.First().LocalFileName;
                using (var stream = new FileStream(uploadedFilePath, FileMode.Open))
                {
                    var userList = _importManager.ImportAwardList(stream, sheetName);
                    _awardService.ImportAwardedUsers(awardId, userList);
                }

                return Request.CreateResponse(HttpStatusCode.OK);
            }
            catch (Exception e)
            {
                _logger.Error("Failed to upload award", e);
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, e);
            }
            finally
            {
                // Delete file
                if (File.Exists(uploadedFilePath))
                {
                    File.Delete(uploadedFilePath);
                }
            }
        }

        [HttpPost]
        [Route("download-template")]
        public IHttpActionResult DownloadTemplate()
        {
            return DownloadFile(HostingEnvironment.MapPath(_appSettings.AwardUploadSampleTemplate));
        }

        #region Utility

        private HttpResponseMessage SaveChanges(Award award, Action<Award> action, string failedMessage)
        {
            try
            {
                action(award);
            }
            catch (Exception ex)
            {
                _logger.WriteLog("Failed to create or update award.", ex);
                return Request.CreateErrorResponse(
                    HttpStatusCode.BadRequest,
                    new HttpError(failedMessage));
            }

            return Request.CreateResponse();
        }

        #endregion
    }
}
