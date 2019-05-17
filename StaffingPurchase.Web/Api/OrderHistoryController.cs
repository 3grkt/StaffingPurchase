using StaffingPurchase.Core;
using StaffingPurchase.Core.Domain;
using StaffingPurchase.Core.SearchCriteria;
using StaffingPurchase.Services.Orders;
using StaffingPurchase.Web.Extensions;
using StaffingPurchase.Web.Models.Common;
using StaffingPurchase.Web.Models.Order;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using Microsoft.Ajax.Utilities;
using StaffingPurchase.Web.Framework.Filters;

namespace StaffingPurchase.Web.Api
{
    [RoutePrefix("api/orderhistory")]
    [PermissionAuthorize(UserPermission.ViewOrderHistory)]
    public class OrderHistoryController : ApiControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly IWorkContext _workContext;

        public OrderHistoryController(IOrderService orderService, IWorkContext workContext)
        {
            _orderService = orderService;
            _workContext = workContext;
        }

        public JsonList<OrderHistory> Get([FromUri] OrderHistorySearchCriteria filter, [FromUri]PaginationOptions pagingOptions)
        {
            var pagingOrders = _orderService.GetOrders(pagingOptions, _workContext.User.Id, filter.StartDate, filter.EndDate, filter.Status);
            var orders = pagingOrders.ToList().ToModelList<Order, OrderHistory>();
            if (!string.IsNullOrEmpty(pagingOptions.Sort))
            {
                if (orders.Count > 0)
                {
                    orders = orders.OrderBy(pagingOptions.SortExpression).ToList();
                }
            }

            return new JsonList<OrderHistory>()
            {
                Data = orders,
                TotalItems = pagingOrders.TotalCount
            };
        }

        [HttpGet]
        [Route("get/order/details/{orderId}")]
        public JsonList<OrderDetailGridModel> GetOrderDetails(int orderId)
        {
            var orderDetails = _orderService.GetOrderDetails(orderId).ToList().ToModelList<OrderDetail, OrderDetailGridModel>();
            return new JsonList<OrderDetailGridModel>()
            {
                Data = orderDetails,
                TotalItems = orderDetails.Count
            };
        }

        [HttpGet]
        [Route("get/order/view/{orderId}")]
        public OrderViewModel GetOrderView(int orderId)
        {
            var order = _orderService.GetById(orderId, includeOrderDetails: false, includeProducts: false, includeMetadata: true);
            OrderViewModel model = new OrderViewModel
            {
                Order = AutoMapper.Mapper.Map<Order, OrderHistory>(order),
                OrderDetails =
                    _orderService.GetOrderDetails(orderId).ToList().ToModelList<OrderDetail, OrderDetailGridModel>()
            };

            return model;
        }
    }
}