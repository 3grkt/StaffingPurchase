using StaffingPurchase.Core;
using StaffingPurchase.Core.SearchCriteria;
using StaffingPurchase.Services.Localization;
using StaffingPurchase.Services.Locations;
using StaffingPurchase.Services.Logging;
using StaffingPurchase.Services.Orders;
using StaffingPurchase.Web.Extensions;
using StaffingPurchase.Web.Framework.Filters;
using StaffingPurchase.Web.Models.Common;
using StaffingPurchase.Web.Models.Order;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Results;
using AutoMapper;
using StaffingPurchase.Web.Models.Report;

namespace StaffingPurchase.Web.Api
{
    [RoleAuthorize(UserRole.Warehouse)]
    [RoutePrefix("api/orderwarehouse")]
    public class OrderWarehouseController : ApiControllerBase
    {
        private readonly ILocationService _locationService;
        private readonly IOrderReportService _orderReportService;
        private readonly IOrderWarehouseService _warehouseService;
        private readonly IWorkContext _workContext;

        public OrderWarehouseController(
            IOrderWarehouseService warehouseService,
            IWorkContext workContext,
            ILocationService locationService,
            ILogger logger,
            IResourceManager resourceManager,
            IOrderReportService orderReportService):base(logger, resourceManager)
        {
            _warehouseService = warehouseService;
            _workContext = workContext;
            _locationService = locationService;
            _orderReportService = orderReportService;
        }

        [HttpGet]
        [Route("order/pv")]
        public InternalRequisitionFormModel SearchPVOrders([FromUri] OrderAdminSearchCriteria searchCriteria, [FromUri] PaginationOptions paginationOptions)
        {
            if (_workContext.User?.LocationId == null)
            {
                return new InternalRequisitionFormModel();
            }

            searchCriteria.OrderStatus = OrderStatus.Approved;
            var internalRequisitionFormModel = GetPVOrders(searchCriteria, paginationOptions);
            return internalRequisitionFormModel;
        }

        [HttpGet]
        [Route("order/discount")]
        public SummaryDiscountProductModel SearchDiscountOrders([FromUri] OrderAdminSearchCriteria searchCriteria, [FromUri] PaginationOptions paginationOptions)
        {
            if (_workContext.User?.LocationId == null)
            {
                return new SummaryDiscountProductModel();
            }

            searchCriteria.OrderStatus = OrderStatus.Approved;
            var summaryDiscountProductModel = GetDiscountOrders(searchCriteria, paginationOptions);
            return summaryDiscountProductModel;
        }

        

        [HttpGet]
        [Route("warehouse-location")]
        public LocationModel GetWarehouseLocation()
        {
            var locationName = _locationService.GetLocationName(_workContext.User.LocationId.GetValueOrDefault(0));
            var location = new LocationModel
            {
                Id = _workContext.User.LocationId.GetValueOrDefault(0),
                Name = locationName
            };

            return location;
        }

        [HttpGet]
        [Route("existing-order/department")]
        public IDictionary<int, string> GetExistingOrderDepartments()
        {
            if (!_workContext.User.LocationId.HasValue)
            {
                return new Dictionary<int, string>();
            }

            var departments = _warehouseService.GetDepartmentsWithOrderStatus(_workContext.User.LocationId.Value, new[] { OrderStatus.Approved, OrderStatus.Packaged });
            return departments.ToDictionary(item => item.Id, item => item.Name);
        }

        [HttpGet]
        [Route("packaged-order/department")]
        public IDictionary<int, string> GetDepartmentWithPackagedOrders()
        {
            if (!_workContext.User.LocationId.HasValue)
            {
                return new Dictionary<int, string>();
            }   
            var departments = _warehouseService.GetDepartmentsWithOrderStatus(_workContext.User.LocationId.Value, new[] { OrderStatus.Packaged });
            return departments.ToDictionary(item => item.Id, item => item.Name);
        }

        [HttpPost]
        [Route("package/single")]
        public HttpResponseMessage PackageSingle([FromBody]OrderPackageRequest request)
        {
            try
            {
                _warehouseService.PackageOrder(_workContext.User, request.DepartmentId, request.OrderType, request.IsDeficient, request.Note);
                return Request.CreateResponse();
            }
            catch (Exception ex)
            {
                _logger.Error("Failed to pack order", ex);
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, _resourceManager.GetString(ExceptionResources.GeneralException));
            }
        }

        [HttpPost]
        [Route("package/all")]
        public HttpResponseMessage PackageAll([FromBody]OrderType orderType)
        {
            try
            {
                _warehouseService.PackageAllOrder(_workContext.User, orderType);
                return Request.CreateResponse();
            }
            catch (Exception ex)
            {
                _logger.Error("Failed to pack all orders", ex);
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, _resourceManager.GetString(ExceptionResources.GeneralException));
            }
        }

        [HttpGet]
        [Route("ispackaged")]
        public HttpResponseMessage IsPackaged([FromUri]PackagedCheckRequest request)
        {
            try
            {
                var isPackaged = _warehouseService.IsPackaged(_workContext.User, request.OrderType, request.DepartmentId);
                return Request.CreateResponse(HttpStatusCode.OK, new { IsPackaged = isPackaged });
            }
            catch (Exception ex)
            {
                _logger.Error("Failed to check whether orders have been packaged", ex);
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, _resourceManager.GetString(ExceptionResources.GeneralException));
            }
        }

        public class OrderPackageRequest
        {
            public int DepartmentId { get; set; }
            public bool IsDeficient { get; set; }
            public string Note { get; set; }
            public OrderType OrderType { get; set; }
        }

        public class PackagedCheckRequest
        {
            public OrderType OrderType { get; set; }
            public int? DepartmentId { get; set; }
        }

        #region Utils
        private SummaryDiscountProductModel GetDiscountOrders(OrderAdminSearchCriteria searchCriteria,
            PaginationOptions paginationOptions)
        {
            searchCriteria.OrderBatchStatus = OrderBatchStatus.Approved;
            var tbl = _orderReportService.SearchSummaryDiscountProduct(_workContext.User, paginationOptions,
                searchCriteria);

            var dataRows =
                tbl.AsEnumerable()
                    .Skip((paginationOptions.PageIndex - 1) * paginationOptions.PageSize)
                    .Take(paginationOptions.PageSize);

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


            var summaryDiscountProductModel = new SummaryDiscountProductModel()
            {
                Data = model,
                SummaryTotalPrice = summaryTotalPrice,
                SummaryTotalAmount = summaryTotalAmount,
                SummaryTotalDiscountedPrice = summaryTotalDiscountedPrice,
                TotalItems = tbl.Rows.Count
            };
            return summaryDiscountProductModel;
        }

        private InternalRequisitionFormModel GetPVOrders(OrderAdminSearchCriteria searchCriteria,
            PaginationOptions paginationOptions)
        {
            searchCriteria.OrderBatchStatus = OrderBatchStatus.Approved;
            var tbl = _orderReportService.SearchInternalRequisitionForm(_workContext.User, paginationOptions,
                searchCriteria);

            var dataRows =
                tbl.AsEnumerable()
                    .Skip((paginationOptions.PageIndex - 1) * paginationOptions.PageSize)
                    .Take(paginationOptions.PageSize);

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

            var internalRequisitionFormModel = new InternalRequisitionFormModel()
            {
                Data = model,
                TotalItems = tbl.Rows.Count,
                SummaryTotalAmount = summaryTotalAmount,
                SummaryTotalPV = summaryTotalPV,
                SummaryTotalPrice = summaryTotalPrice
            };
            return internalRequisitionFormModel;
        }
        #endregion
    }
}