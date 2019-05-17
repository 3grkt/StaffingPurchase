using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Hosting;
using System.Web.Http;
using StaffingPurchase.Core;
using StaffingPurchase.Core.Domain;
using StaffingPurchase.Services.Configurations;
using StaffingPurchase.Services.Localization;
using StaffingPurchase.Services.Logging;
using StaffingPurchase.Web.Extensions;
using StaffingPurchase.Web.Models.Configurations;
using StaffingPurchase.Web.Framework.Filters;

namespace StaffingPurchase.Web.Api
{
    [RoutePrefix("api/configuration")]
    public class ConfigurationController : ApiControllerBase
    {
        private const string PolicyDocumentKey = "PolicyDocumentFile";
        private readonly IConfigurationService _configurationService;
        private readonly IAppPolicy _appPolicy;
        private readonly IAppSettings _appSettings;

        public ConfigurationController(
            IResourceManager resourceManager,
            ILogger logger,
            IConfigurationService configurationService,
            IAppPolicy appPolicy,
            IAppSettings appSettings)
            : base(logger, resourceManager)
        {
            _configurationService = configurationService;
            _appPolicy = appPolicy;
            _appSettings = appSettings;
        }

        [Route("getall")]
        public PolicyConfigurationModel GetAll()
        {
            var model = new PolicyConfigurationModel();
            var allConfigs = _configurationService.GetAll().ToModelList<Configuration, ConfigurationModel>();
            var policyDocument = allConfigs.FirstOrDefault(x => x.Name.Equals(PolicyDocumentKey, StringComparison.OrdinalIgnoreCase));
            if (policyDocument != null)
            {
                allConfigs.Remove(policyDocument);
                model.PolicyDocument = policyDocument;
            }
            model.AllConfigurations = allConfigs;
            return model;
        }

        [Route("gethighvalueproductprice")]
        public decimal GetHighValueProductPrice()
        {
            return _appPolicy.HighValueProductPrice;
        }

        [HttpPut]
        [Route("put")]
        [PermissionAuthorize(UserPermission.MaintainPolicy)]
        public HttpResponseMessage Put(IList<ConfigurationModel> configrations)
        {
            if (!ModelState.IsValid)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, new HttpError(ModelState.FirstError()));
            }

            try
            {
                _configurationService.Update(configrations.ToEntityList<ConfigurationModel, Configuration>());
            }
            catch (StaffingPurchaseException ex)
            {
                return Error(ex.Message);
            }
            catch (Exception ex)
            {
                return RecordException(ex, "Failed to update configurations.", responseStatus: HttpStatusCode.InternalServerError);
            }

            return Request.CreateResponse();
        }

        [HttpPost]
        [Route("download-policy-file")]
        public IHttpActionResult DownloadPolicyFile()
        {
            return DownloadFile(HostingEnvironment.MapPath(_appSettings.PolicyDocumentFolder + _appPolicy.PolicyDocumentFile));
        }

        [HttpPost]
        [Route("upload-policy-file")]
        [PermissionAuthorize(UserPermission.MaintainPolicy)]
        public async Task<HttpResponseMessage> UploadPolicyFile()
        {
            // Check if the request contains multipart/form-data.
            if (!Request.Content.IsMimeMultipartContent())
            {
                throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
            }

            string root = HostingEnvironment.MapPath(_appSettings.PolicyDocumentFolder);
            var provider = new MultipartFormDataStreamProvider(root);
            try
            {
                // Read the form data.
                await Request.Content.ReadAsMultipartAsync(provider);

                // Rename uploaded file
                var fileData = provider.FileData.First();
                string newFileName = fileData.Headers.ContentDisposition.FileName.Replace("\"", "");
                RenameUploadedFile(root, fileData.LocalFileName, newFileName);

                // Update policy configuration
                UpdatePolicyConfiguration(newFileName);

                return Request.CreateResponse(HttpStatusCode.OK, new { newFile = newFileName });
            }
            catch (Exception ex)
            {
                return RecordException(ex, "Failed to upload policy file", "Error.DataException", responseStatus: HttpStatusCode.InternalServerError);
            }
        }

        private void UpdatePolicyConfiguration(string newFileName)
        {
            var policyFileConfig = _configurationService.Get(PolicyDocumentKey);
            if (policyFileConfig == null)
            {
                throw new StaffingPurchaseException("Policy Document not exist in configuration table.");
            }
            policyFileConfig.Value = newFileName;
            _configurationService.Update(policyFileConfig);
        }

        private static void RenameUploadedFile(string root, string uploadedFileName, string newFileName)
        {
            string newPath = Path.Combine(root, newFileName);
            try
            {
                if (File.Exists(newPath))
                {
                    File.Delete(newPath);
                }

                File.Move(uploadedFileName, newPath);
            }
            catch (Exception ex)
            {
                throw new StaffingPurchaseException("Failed to rename uploaded file.", ex);
            }
        }
    }
}
