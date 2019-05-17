using System;
using System.Data;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Web.Http;
using System.Web.Razor.Generator;
using AutoMapper;
using StaffingPurchase.Core;
using StaffingPurchase.Core.SearchCriteria;
using StaffingPurchase.Services.Localization;
using StaffingPurchase.Services.Logging;
using StaffingPurchase.Services.Orders;
using StaffingPurchase.Web.Framework.Filters;
using StaffingPurchase.Web.Models.Report;
using static StaffingPurchase.Web.Extensions.HttpExtension;

namespace StaffingPurchase.Web.Api
{
    [RoutePrefix("api/order/admin-report")]
    [RoleAuthorize(UserRole.HRManager, UserRole.HRAdmin)]
    public class OrderReportController : ApiControllerBase
    {
        private readonly IOrderReportService _orderReportService;
        private readonly IAppSettings _appSettings;
        private readonly IWorkContext _workContext;

        public OrderReportController(IOrderReportService orderReportService, 
            IAppSettings appSettings,
            IWorkContext workContext, 
            ILogger logger, 
            IResourceManager resourceManager) : base(logger, resourceManager)
        {
            _orderReportService = orderReportService;
            _appSettings = appSettings;
            _workContext = workContext;
        }

        [HttpGet]
        [Route("search/internal-requisition-form")]
        public InternalRequisitionFormModel SearchInternalRequisitionFormModels(
            [FromUri] OrderAdminSearchCriteria searchCriteria, [FromUri] PaginationOptions paginationOptions)
        {
            var tbl = _orderReportService.SearchInternalRequisitionForm(_workContext.User, paginationOptions,
                searchCriteria);

            var dataRows = tbl.AsEnumerable().Skip((paginationOptions.PageIndex - 1) * paginationOptions.PageSize).Take(paginationOptions.PageSize);

            int summaryTotalAmount = 0;
            decimal summaryTotalPV = 0;
            decimal summaryTotalPrice = 0;

            var model = dataRows.Select(Mapper.Map<DataRow, InternalRequisitionRowModel>).ToList();
            if (model.Count > 0)
            {
                summaryTotalAmount = Convert.ToInt32(tbl.Compute("Sum(Quantity)", ""));
                summaryTotalPV = Convert.ToDecimal(tbl.Compute("Sum(TotalPV)", ""));
                summaryTotalPrice = Convert.ToDecimal(tbl.Compute("Sum(TotalPrice)", ""));
            }
            
            return new InternalRequisitionFormModel()
            {
                Data = model,
                TotalItems = tbl.Rows.Count,
                SummaryTotalAmount = summaryTotalAmount,
                SummaryTotalPV = summaryTotalPV,
                SummaryTotalPrice = summaryTotalPrice
            };
        }

        [HttpGet]
        [Route("search/order-by-individual-pv")]
        public SummaryOrderByIndividualPVModel SearchSummaryOrderByIndividualPVModels(
            [FromUri] OrderAdminSearchCriteria searchCriteria, [FromUri] PaginationOptions paginationOptions)
        {
            var tbl = _orderReportService.SearchSummaryOrderByIndividualPV(_workContext.User, paginationOptions,
                searchCriteria);

            var dataRows = tbl.AsEnumerable().Skip((paginationOptions.PageIndex - 1) * paginationOptions.PageSize).Take(paginationOptions.PageSize);

            var model = dataRows.Select(Mapper.Map<DataRow, SummaryOrderByIndividualPVRowModel>).ToList();

            var summaryTotalAmount = 0;
            decimal summaryTotalPV = 0;
            decimal summaryTotalPrice = 0;
            if (model.Count > 0)
            {
                summaryTotalAmount = Convert.ToInt32(tbl.Compute("Sum(Quantity)", ""));
                summaryTotalPV = Convert.ToDecimal(tbl.Compute("Sum(TotalPV)", ""));
                summaryTotalPrice = Convert.ToDecimal(tbl.Compute("Sum(TotalPrice)", ""));
            }
            
            
            return new SummaryOrderByIndividualPVModel()
            {
                Data = model,
                TotalItems = tbl.Rows.Count,
                SummaryTotalAmount = summaryTotalAmount,
                SummaryTotalPV = summaryTotalPV,
                SummaryTotalPrice = summaryTotalPrice
            };
        }

        [HttpGet]
        [Route("search/summary-discount-product")]
        public SummaryDiscountProductModel SearchSummaryDiscountProductModels(
            [FromUri] OrderAdminSearchCriteria searchCriteria, [FromUri] PaginationOptions paginationOptions)
        {
            var tbl = _orderReportService.SearchSummaryDiscountProduct(_workContext.User, paginationOptions,
                searchCriteria);

            var dataRows = tbl.AsEnumerable().Skip((paginationOptions.PageIndex - 1) * paginationOptions.PageSize).Take(paginationOptions.PageSize);

            var model = dataRows.Select(Mapper.Map<DataRow, SummaryDiscountProductRowModel>).ToList();


            int summaryTotalAmount = 0;
            decimal summaryTotalDiscountedPrice = 0;
            decimal summaryTotalPrice = 0;
            if (model.Count > 0)
            {
                summaryTotalAmount = Convert.ToInt32(tbl.Compute("Sum(Quantity)", ""));
                summaryTotalDiscountedPrice = Convert.ToDecimal(tbl.Compute("Sum(TotalDiscountedPrice)", ""));
                summaryTotalPrice = Convert.ToDecimal(tbl.Compute("Sum(TotalPrice)", ""));
            }
            

            return new SummaryDiscountProductModel()
            {
                Data = model,
                SummaryTotalPrice = summaryTotalPrice,
                SummaryTotalAmount = summaryTotalAmount,
                SummaryTotalDiscountedPrice = summaryTotalDiscountedPrice,
                TotalItems = tbl.Rows.Count
            };
        }

        [HttpGet]
        [Route("search/order-by-individual-discount")]
        public SummaryOrderByIndividualDiscountModel SearchSummaryOrderByIndividualDiscountModels(
            [FromUri] OrderAdminSearchCriteria searchCriteria, [FromUri] PaginationOptions paginationOptions)
        {
            var tbl = _orderReportService.SearchSummaryOrderByIndividualDiscount(_workContext.User, paginationOptions,
                searchCriteria);

            var dataRows = tbl.AsEnumerable().Skip((paginationOptions.PageIndex - 1) * paginationOptions.PageSize).Take(paginationOptions.PageSize);

            var model = dataRows.Select(Mapper.Map<DataRow, SummaryOrderByIndividualDiscountRowModel>).ToList();

            int summaryTotalAmount = 0;
            decimal summaryTotalDiscountedPrice = 0;
            decimal summaryTotalPrice = 0;
            if (model.Count > 0)
            {
                summaryTotalAmount = Convert.ToInt32(tbl.Compute("Sum(Quantity)", ""));
                summaryTotalDiscountedPrice = Convert.ToDecimal(tbl.Compute("Sum(TotalDiscountedPrice)", ""));
                summaryTotalPrice = Convert.ToDecimal(tbl.Compute("Sum(TotalPrice)", ""));
            }
            

            return new SummaryOrderByIndividualDiscountModel()
            {
                Data = model,
                SummaryTotalPrice = summaryTotalPrice,
                SummaryTotalAmount = summaryTotalAmount,
                SummaryTotalDiscountedPrice = summaryTotalDiscountedPrice,
                TotalItems = tbl.Rows.Count
            };
        }

        [HttpGet]
        [Route("download/internal-requisition-form")]
        public IHttpActionResult DownloadInternalRequisitionFormReport([FromUri] OrderAdminSearchCriteria searchCriteria)
        {
            try
            {
                var path = System.Web.Hosting.HostingEnvironment.MapPath(_appSettings.InternalRequisitionFormTemplate);
                byte[] content;
                using (FileStream file = File.Open(path, FileMode.Open))
                {
                    content = _orderReportService.GetInternalRequisitionFormReport(
                        _workContext.User,
                        file,
                        searchCriteria);
                }
                var stream = new MemoryStream(content);
                var result = new FileActionResult(stream,
                    $"InternalRequisitionForm_{searchCriteria.OrderStatus}_{DateTime.Now.ToString("dd-MM-yyyy")}.xlsx");
                return result;
            }
            catch (StaffingPurchaseException ex)
            {
                return new StaffPurchaseExceptionActionResult(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.Error("Failed to download Internal Requisition Form Report", ex);
                return new StaffPurchaseExceptionActionResult(_resourceManager.GetString("OrderReport.FailedToReport"));
            }
        }

        [HttpGet]
        [Route("download/order-by-individual-pv")]
        public IHttpActionResult DownloadSummaryOrderByIndividualPVReport([FromUri] OrderAdminSearchCriteria searchCriteria)
        {
            try
            {
                var path = System.Web.Hosting.HostingEnvironment.MapPath(_appSettings.OrderByIndividualPVTemplate);
                byte[] content;
                using (FileStream file = File.Open(path, FileMode.Open))
                {
                    content = _orderReportService.GetSummaryOrderByIndividualPVReport(
                        _workContext.User,
                        file,
                        searchCriteria);
                }
                var stream = new MemoryStream(content);
                var result = new FileActionResult(stream,
                    $"SummaryOrderByIndividualPV_{searchCriteria.OrderStatus}_{DateTime.Now.ToString("dd-MM-yyyy")}.xlsx");
                return result;
            }
            catch (StaffingPurchaseException ex)
            {
                return new StaffPurchaseExceptionActionResult(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.Error("Failed to download Summary Order By Individual PV Report", ex);
                return new StaffPurchaseExceptionActionResult(_resourceManager.GetString("OrderReport.FailedToReport"));
            }
        }

        [HttpGet]
        [Route("download/order-by-individual-discount")]
        public IHttpActionResult DownloadSummaryOrderByIndividualDiscountReport([FromUri] OrderAdminSearchCriteria searchCriteria)
        {
            try
            {
                var path = System.Web.Hosting.HostingEnvironment.MapPath(_appSettings.OrderByIndividualDiscountTemplate);
                byte[] content;
                using (FileStream file = File.Open(path, FileMode.Open))
                {
                    content = _orderReportService.GetSummaryOrderByIndividualDiscountReport(
                        _workContext.User,
                        file,
                        searchCriteria);
                }
                var stream = new MemoryStream(content);
                var result = new FileActionResult(stream,
                    $"SummaryOrderByIndividualDiscount_{searchCriteria.OrderStatus}_{DateTime.Now.ToString("dd-MM-yyyy")}.xlsx");
                return result;
            }
            catch (StaffingPurchaseException ex)
            {
                return new StaffPurchaseExceptionActionResult(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.Error("Failed to download Summary Order By Individual Discount Report", ex);
                return new StaffPurchaseExceptionActionResult(_resourceManager.GetString("OrderReport.FailedToReport"));
            }
        }

        [HttpGet]
        [Route("download/summary-discount-product")]
        public IHttpActionResult DownloadSummaryDiscountProductReport([FromUri] OrderAdminSearchCriteria searchCriteria)
        {
            try
            {
                var path = System.Web.Hosting.HostingEnvironment.MapPath(_appSettings.SummaryDiscountProductTemplate);
                byte[] content;
                var isWarehouse = _workContext.User.IsInRole(UserRole.Warehouse);
                using (FileStream file = File.Open(path, FileMode.Open))
                {
                    content = _orderReportService.GetSummaryDiscountProductReport(
                        _workContext.User,
                        file,
                        searchCriteria);
                }
                var stream = new MemoryStream(content);
                var result = new FileActionResult(stream,
                    $"SummaryDiscountProduct_{searchCriteria.OrderStatus}_{DateTime.Now.ToString("dd-MM-yyyy")}.xlsx");
                return result;
            }
            catch (StaffingPurchaseException ex)
            {
                return new StaffPurchaseExceptionActionResult(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.Error("Failed to download Summary Discount Product Report", ex);
                return new StaffPurchaseExceptionActionResult(_resourceManager.GetString("OrderReport.FailedToReport"));
            }
        }
    }
}