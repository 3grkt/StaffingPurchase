using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using StaffingPurchase.Core;
using StaffingPurchase.Core.Domain;
using StaffingPurchase.Core.SearchCriteria;
using StaffingPurchase.Services.Localization;
using StaffingPurchase.Services.Logging;
using StaffingPurchase.Services.Orders;
using StaffingPurchase.Web.Extensions;
using StaffingPurchase.Web.Framework.Filters;
using StaffingPurchase.Web.Models.Common;
using StaffingPurchase.Web.Models.Order;
using System.Collections.Generic;

namespace StaffingPurchase.Web.Api
{
    [RoleAuthorize(UserRole.HRAdmin, UserRole.HRManager)]
    [JsonCamelCaseConfig] // TODO: remove once enable config globally (in WebApiConfig.cs)
    [RoutePrefix("api/orderbatch")]
    public class OrderBatchController : ApiControllerBase
    {
        #region Fields

        private const string ApproveAction = "approve";
        private const string RejectAction = "reject";

        private readonly IOrderBatchService _orderBatchService;
        private readonly IWorkContext _workContext;

        #endregion Fields

        #region Ctor.

        public OrderBatchController(
            IOrderBatchService orderBatchService,
            IResourceManager resourceManager,
            ILogger logger,
            IWorkContext workContext)
            : base(logger, resourceManager)
        {
            _orderBatchService = orderBatchService;
            _workContext = workContext;
        }

        #endregion Ctor.

        #region Apis

        [Route("")]
        public OrderBatchModel Get(
            [FromUri] OrderBatchSearchCriteria searchCriteria,
            [FromUri] PaginationOptions options)
        {
            options = GetPaginationOptions(options);

            var orderBatch = _orderBatchService.GetByLocation(searchCriteria.LocationId, searchCriteria.OrderType, _workContext.User);
            if (orderBatch == null)
            {
                return new OrderBatchModel();
            }

            OrderBatchModel model = orderBatch.ToModel<OrderBatchModel>();

            var orders = _orderBatchService.SearchOrders(orderBatch.Id, searchCriteria.DepartmentId, searchCriteria.UserId, searchCriteria.UserName, options);
            model.Orders = orders.ToModelList<Order, OrderModel>();
            model.TotalOrders = orders.TotalCount;
            model.TotalValueWithFilter = model.Orders.Sum(x => x.Value);

            return model;
        }

        [Route("getall")]
        public JsonList<OrderModel> GetAll(
            [FromUri] OrderBatchSearchCriteria searchCriteria,
            [FromUri] PaginationOptions options)
        {
            var sessionEndDate = _orderBatchService.GetNearestSessionEndDate();
            var orders = _orderBatchService.GetAllOrdersInSubmittedBatches(sessionEndDate, searchCriteria.DepartmentId, searchCriteria.UserId, searchCriteria.UserName, searchCriteria.OrderType, options, _workContext.User);

            var model = new JsonList<OrderModel>();
            model.Data = orders.ToModelList<Order, OrderModel>();
            model.TotalItems = orders.TotalCount;
            model.Metadata = new
            {
                OrderSession = _orderBatchService.GetNearestOrderSession(sessionEndDate),
                TotalValueWithFilter = model.Data.Sum(x => x.Value)
            };

            return model;
        }

        [Route("")]
        public HttpResponseMessage Put([FromBody]OrderBatchUpdateRequest request)
        {
            var orderBatch = _orderBatchService.GetById(request.Id);
            if (orderBatch == null)
            {
                return NotFound(_resourceManager.GetString("Common.DataNotFound"));
            }

            try
            {
                if (request.Type == ApproveAction)
                {
                    _orderBatchService.Approve(_workContext.User, orderBatch);
                }
                else if (request.Type == RejectAction)
                {
                    _orderBatchService.Reject(_workContext.User, orderBatch, request.Reason);
                }
            }
            catch (Exception ex)
            {
                _logger.WriteLog("Failed to update order.", ex);
                return Request.CreateErrorResponse(
                    HttpStatusCode.BadRequest,
                    new HttpError(_resourceManager.GetString("OrderBatch.FailedToUpdate")));
            }

            return Request.CreateResponse();
        }

        [Route("putall")]
        public HttpResponseMessage PutAll([FromBody]OrderBatchUpdateRequest request)
        {
            IList<string> failedLocations = null;
            try
            {
                if (request.Type == ApproveAction)
                {
                    _orderBatchService.ApproveAll(_workContext.User, request.OrderType, out failedLocations);
                }
                else if (request.Type == RejectAction)
                {
                    _orderBatchService.RejectAll(_workContext.User, request.OrderType, request.Reason, out failedLocations);
                }
            }
            catch (Exception ex)
            {
                _logger.WriteLog("Failed to update order.", ex);
                return Request.CreateErrorResponse(
                    HttpStatusCode.BadRequest,
                    new HttpError(_resourceManager.GetString("OrderBatch.FailedToUpdate")));
            }

            return Request.CreateResponse(failedLocations);
        }

        #endregion Apis

        #region Nested classes

        public class OrderBatchUpdateRequest
        {
            public int Id { get; set; }
            public string Reason { get; set; }
            public string Type { get; set; }
            public OrderType OrderType { get; set; }
        }

        #endregion Nested classes
    }
}
