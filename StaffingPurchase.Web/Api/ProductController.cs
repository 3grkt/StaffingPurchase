using StaffingPurchase.Core;
using StaffingPurchase.Core.Domain;
using StaffingPurchase.Core.SearchCriteria;
using StaffingPurchase.Services.ImportExport;
using StaffingPurchase.Services.Localization;
using StaffingPurchase.Services.Logging;
using StaffingPurchase.Services.Products;
using StaffingPurchase.Web.Extensions;
using StaffingPurchase.Web.Framework.Filters;
using StaffingPurchase.Web.Models.Common;
using StaffingPurchase.Web.Models.Product;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Hosting;
using System.Web.Http;
using static StaffingPurchase.Web.Extensions.HttpExtension;

namespace StaffingPurchase.Web.Api
{
    public class ProductController : ApiControllerBase
    {
        private readonly IImportManager _importManager;
        private readonly IProductService _prodService;
        private readonly IAppSettings _appSettings;

        public ProductController(
            IProductService prodService,
            IImportManager importManager,
            ILogger logger,
            IResourceManager resourceManager,
            IAppSettings appSettings)
            : base(logger, resourceManager)
        {
            _prodService = prodService;
            _importManager = importManager;
            _appSettings = appSettings;
        }

        public JsonList<ProductModel> GetAll()
        {
            var products = _prodService.GetAllProducts(true).ToModelEnumerable<Product, ProductModel>();

            var productModels = products as IList<ProductModel> ?? products.ToList();
            return new JsonList<ProductModel>
            {
                Data = productModels,
                TotalItems = productModels.Count
            };
        }

        [HttpGet]
        [Route("api/product/get-by-category/{categoryId}")]
        public JsonList<ProductModel> GetByCategory(int categoryId)
        {
            var products = _prodService.GetAllProducts(true, categoryId).ToModelEnumerable<Product, ProductModel>();

            var productModels = products as IList<ProductModel> ?? products.ToList();
            return new JsonList<ProductModel>
            {
                Data = productModels,
                TotalItems = productModels.Count()
            };
        }

        [HttpGet]
        [Route("api/product/getbyid/{productId}")]
        public ProductModel GetById(int productId)
        {
            var product = _prodService.GetProductById(productId);
            if (product == null)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(
                    HttpStatusCode.NotFound,
                    new HttpError(_resourceManager.GetString("Common.DataNotFound"))));
            }

            return product.ToModel<ProductModel>();
        }

        [HttpGet]
        [Route("api/product/search")]
        public JsonList<ProductModel> Search(
            [FromUri]ProductListSearchCriteria searchCriteria,
            [FromUri]PaginationOptions options)
        {
            var products = _prodService.Search(searchCriteria, options);
            return new JsonList<ProductModel>()
            {
                Data = products.ToModelList<Product, ProductModel>(),
                TotalItems = products.TotalCount
            };
        }

        [HttpPut]
        [Route("api/product/put")]
        public HttpResponseMessage Put(ProductModel model)
        {
            if (!ModelState.IsValid)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, new HttpError(ModelState.FirstError()));
            }

            var entity = _prodService.GetProductById(model.Id);
            if (entity == null)
            {
                return NotFound(_resourceManager.GetString("Common.DataNotFound"));
            }

            entity = model.ToEntity<Product>(entity);
            try
            {
                _prodService.Update(entity);
            }
            catch (StaffingPurchaseException ex)
            {
                return Error(ex.Message);
            }
            catch (Exception ex)
            {
                return RecordException(ex, "Failed to update product.", responseStatus: HttpStatusCode.InternalServerError);
            }

            return Request.CreateResponse(HttpStatusCode.OK);
        }

        [HttpDelete]
        [Route("api/product/delete/{productId}")]
        public HttpResponseMessage Delete(int productId)
        {
            var entity = _prodService.GetProductById(productId);
            if (entity == null)
            {
                return NotFound(_resourceManager.GetString("Common.DataNotFound"));
            }

            try
            {
                _prodService.Delete(entity);
            }
            catch (StaffingPurchaseException ex)
            {
                return Error(ex.Message);
            }
            catch (Exception ex)
            {
                return RecordException(ex, "Failed to delete product.", "Error.DataException", responseStatus: HttpStatusCode.InternalServerError);
            }

            return Request.CreateResponse(HttpStatusCode.OK);
        }

        [HttpPost]
        public async Task<HttpResponseMessage> Upload(string sheetName)
        {
            // Check if the request contains multipart/form-data.
            if (!Request.Content.IsMimeMultipartContent())
            {
                throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
            }

            string root = HostingEnvironment.MapPath("~/App_Data");
            var provider = new MultipartFormDataStreamProvider(root);
            string uploadedFilePath = null;
            try
            {
                // Read the form data.
                await Request.Content.ReadAsMultipartAsync(provider);

                // TODO: find solution to read data from stream
                uploadedFilePath = provider.FileData.First().LocalFileName;
                using (FileStream stream = new FileStream(uploadedFilePath, FileMode.Open))
                {
                    var importData = _importManager.ImportProductList(stream, sheetName);
                    _prodService.ImportProducts(importData);
                }

                return Request.CreateResponse(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return RecordException(ex, "Failed to upload product list", "Error.DataException", responseStatus: HttpStatusCode.InternalServerError);
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
        [Route("api/product/download-template")]
        public IHttpActionResult DownloadTemplate()
        {
            return DownloadFile(HostingEnvironment.MapPath(_appSettings.ProductUploadSampleTemplate));
        }
    }
}
