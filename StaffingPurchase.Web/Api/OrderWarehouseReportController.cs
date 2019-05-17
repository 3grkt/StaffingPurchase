using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Http;
using StaffingPurchase.Core;
using StaffingPurchase.Core.SearchCriteria;
using StaffingPurchase.Services.Localization;
using StaffingPurchase.Services.Logging;
using StaffingPurchase.Services.Orders;
using StaffingPurchase.Web.Extensions;
using StaffingPurchase.Web.Framework.Filters;

namespace StaffingPurchase.Web.Api
{
    [RoleAuthorize(UserRole.Warehouse)]
    [RoutePrefix("api/order/warehouse-report")]
    public class OrderWarehouseReportController: ApiControllerBase
    {
        private readonly IWorkContext _workContext;
        private readonly IAppSettings _appSettings;
        private readonly IOrderReportService _orderReportService;

        public OrderWarehouseReportController(IWorkContext workContext, IAppSettings appSettings, IOrderReportService orderReportService, ILogger logger, IResourceManager resourceManager):base(logger, resourceManager)
        {
            _workContext = workContext;
            _appSettings = appSettings;
            _orderReportService = orderReportService;
        }

        [HttpGet]
        [Route("download/package-report")]
        public IHttpActionResult DownloadWarehousePackageReport([FromUri] OrderAdminSearchCriteria searchCriteria, [FromUri]OrderType orderType)
        {
            try
            {
                var template = orderType == OrderType.Cash
                    ? _appSettings.WarehousePackageDiscountOrderTemplate
                    : _appSettings.WarehousePackagePVOrderTemplate;
                var path = System.Web.Hosting.HostingEnvironment.MapPath(template);
                byte[] content = { };
                using (var file = File.Open(path, FileMode.Open))
                {
                    searchCriteria.OrderStatus = OrderStatus.Packaged;
                    switch (orderType)
                    {
                        case OrderType.PV:
                            content = _orderReportService.GetWarehousePackagePVOrderReport(_workContext.User, file,
                                searchCriteria);
                            break;
                        case OrderType.Cash:
                            content = _orderReportService.GetWarehousePackageDiscountOrderReport(_workContext.User, file,
                                searchCriteria);
                            break;
                        default:
                            throw new ArgumentOutOfRangeException(nameof(orderType), orderType, null);
                    }
                }
                var stream = new MemoryStream(content);
                var result = new HttpExtension.FileActionResult(stream, $"WarehousePackageOrderReport_{orderType}_{DateTime.Now.ToString("dd-MM-yyyy")}.xlsx");
                return result;
            }
            catch (StaffingPurchaseException ex)
            {
                return new HttpExtension.StaffPurchaseExceptionActionResult(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.Error("Failed to download Warehouse Package Report", ex);
                return new HttpExtension.StaffPurchaseExceptionActionResult(_resourceManager.GetString("OrderReport.FailedToReport"));
            }
        }

        [HttpGet]
        [Route("download/preview-report")]
        public IHttpActionResult DownloadWarehousePreviewReport([FromUri] OrderAdminSearchCriteria searchCriteria, [FromUri] OrderType orderType)
        {
            try
            {
                var template = orderType == OrderType.Cash ? _appSettings.WarehousePackageDiscountOrderTemplate : _appSettings.WarehousePackagePVOrderTemplate;
                var path = System.Web.Hosting.HostingEnvironment.MapPath(template);
                byte[] content = {};
                using (var file = File.Open(path, FileMode.Open))
                {
                    searchCriteria.OrderStatus = OrderStatus.Approved;
                    switch (orderType)
                    {
                        case OrderType.PV:
                            content = _orderReportService.GetWarehousePackagePVOrderReport(_workContext.User, file, searchCriteria, true);
                            break;
                        case OrderType.Cash:
                            content = _orderReportService.GetWarehousePackageDiscountOrderReport(_workContext.User, file, searchCriteria, true);
                            break;
                        default:
                            content = _orderReportService.GetWarehousePackageReport(_workContext.User, file, searchCriteria);
                            break;
                    }
                }
                var stream = new MemoryStream(content);
                var result = new HttpExtension.FileActionResult(stream, $"WarehouseOrderPreviewReport_{orderType}_{DateTime.Now.ToString("dd-MM-yyyy")}.xlsx");
                return result;
            }
            catch (StaffingPurchaseException ex)
            {
                return new HttpExtension.StaffPurchaseExceptionActionResult(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.Error("Failed to download Warehouse Preview Report", ex);
                return new HttpExtension.StaffPurchaseExceptionActionResult(_resourceManager.GetString("OrderReport.FailedToReport"));
            }
        }
    }
}