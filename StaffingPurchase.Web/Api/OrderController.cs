using StaffingPurchase.Core;
using StaffingPurchase.Core.Domain;
using StaffingPurchase.Services.Localization;
using StaffingPurchase.Services.Logging;
using StaffingPurchase.Services.Orders;
using StaffingPurchase.Web.Extensions;
using StaffingPurchase.Web.Models.Common;
using StaffingPurchase.Web.Models.Order;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace StaffingPurchase.Web.Api
{
    [RoutePrefix("api/order")]
    public class OrderController : ApiControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly IWorkContext _workContext;

        public OrderController(
            IOrderService orderService,
            IResourceManager resourceManager,
            ILogger logger,
            IWorkContext workContext)
            : base(logger, resourceManager)
        {
            _orderService = orderService;
            _workContext = workContext;
        }

        [HttpPost]
        [Route("addorderdetail")]
        public HttpResponseMessage AddOrderDetails(OrderDetailModel orderDetailM)
        {
            if (!ModelState.IsValid)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, new HttpError(ModelState.FirstError()));
            }

            if (!orderDetailM.IsPrice && !_orderService.IsEnoughPV(orderDetailM.ProductId, orderDetailM.Volume, GetCurrentUserId()))
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, _resourceManager.GetString("OrderDetail.Validation.NotEnoughPV"));
            }

            if (!_orderService.IsEligibleUser(_workContext.User.Id))
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, _resourceManager.GetString("Order.Validation.NotEligibleUser"));
            }

            try
            {
                var orderDetail = new OrderDetail
                {
                    ProductId = orderDetailM.ProductId,
                    Volume = orderDetailM.Volume
                };

                int currentUserId = GetCurrentUserId();
                _orderService.AddOrderDetail(orderDetail, currentUserId, orderDetailM.IsPrice ? OrderType.Cash : OrderType.PV);
                return Request.CreateResponse();
            }
            catch (StaffingPurchaseException ex)
            {
                _logger.Info(ex.Message);
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, new HttpError(ex.Message));
            }
            catch (Exception ex)
            {
                _logger.WriteLog("Error when adding order detail", ex);
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, new HttpError(_resourceManager.GetString(ExceptionResources.GeneralException)));
            }
        }

        [HttpPost]
        [Route("delete-order/{isPrice}")]
        public HttpResponseMessage DeleteOrder(bool isPrice)
        {
            try
            {
                var order = _orderService.GetOrder(GetCurrentUserId(), new[] { OrderStatus.Draft }, isPrice ? OrderType.Cash : OrderType.PV);
                if (order == null)
                {
                    throw new ArgumentNullException(nameof(isPrice));
                }

                _orderService.RemoveOrder(order);
            }
            catch (StaffingPurchaseException ex)
            {
                _logger.Info(_resourceManager.GetString(_resourceManager.GetString(ex.Message)));
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, new HttpError(_resourceManager.GetString(ex.Message)));
            }
            catch (Exception ex)
            {
                _logger.Error("Error when deleting order: " + DateTime.Now, ex);
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, new HttpError(_resourceManager.GetString(ExceptionResources.GeneralException)));
            }

            return Request.CreateResponse();
        }

        [HttpPost]
        [Route("delete-orderdetail")]
        public HttpResponseMessage DeleteOrderDetails(IEnumerable<OrderDetailGridModel> orderDetails)
        {
            try
            {
                if (!_orderService.IsLocked(DateTime.Now))
                {
                    _orderService.RemoveOrderDetails(orderDetails.Select(c => c.Id).ToArray());
                }
            }
            catch (StaffingPurchaseException ex)
            {
                _logger.Info(ex.Message);
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex.Message);
            }
            catch (Exception ex)
            {
                _logger.WriteLog("Error when deleting order details: " + DateTime.Now, ex);
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, _resourceManager.GetString(ExceptionResources.GeneralException));
            }

            return Request.CreateResponse();
        }

        [HttpGet]
        [Route("get-eligible-discount")]
        public object GetEligibleDiscout()
        {
            bool result = _orderService.IsEligibleUser(GetCurrentUserId());
            return new { IsEligible = result };
        }

        [HttpGet]
        [Route("orderdetail/{isPrice}")]
        public JsonList<OrderDetailGridModel> GetOrderDetails(bool isPrice = true)
        {
            var order = _orderService.GetOrder(
                GetCurrentUserId(),
                new[] { OrderStatus.Draft },
                isPrice ? OrderType.Cash : OrderType.PV);
            if (order == null || _orderService.IsLocked(DateTime.Now) || !_orderService.IsEligibleUser(GetCurrentUserId()))
            {
                return new JsonList<OrderDetailGridModel>()
                {
                    TotalItems = 0,
                    Data = null
                };
            }

            var orderDetails = _orderService.GetOrderDetails(order.Id).ToList().ToModelList<OrderDetail, OrderDetailGridModel>();

            return new JsonList<OrderDetailGridModel>()
            {
                TotalItems = orderDetails.Count,
                Data = orderDetails
            };
        }

        [HttpGet]
        [Route("check-locked")]
        public bool IsLookedOrder()
        {
            return _orderService.IsLocked(DateTime.Now) || !_orderService.IsEligibleUser(GetCurrentUserId());
        }

        #region Utils

        private int GetCurrentUserId()
        {
            return _workContext.User.Id;
        }

        #endregion Utils
    }
}
