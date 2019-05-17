using AutoMapper;
using StaffingPurchase.Core;
using StaffingPurchase.Core.Domain;
using StaffingPurchase.Services.Localization;
using StaffingPurchase.Services.Logging;
using StaffingPurchase.Services.Orders;
using StaffingPurchase.Services.Products;
using StaffingPurchase.Web.Extensions;
using StaffingPurchase.Web.Framework.Filters;
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
    [RoleAuthorize(UserRole.HRAdmin)]
    [RoutePrefix("api/orderupdate")]
    public class OrderUpdateController : ApiControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly IProductService _productService;

        public OrderUpdateController(IOrderService orderService, IProductService productService, IResourceManager resourceManager, ILogger logger)
            :base(logger, resourceManager)
        {
            _orderService = orderService;
            _productService = productService;
        }

        [HttpGet]
        [Route("{userId}/{orderId}")]
        public JsonList<OrderUpdateModel> Get(int userId, int orderId)
        {
            IList<OrderUpdateModel> result = new List<OrderUpdateModel>();
            if (orderId > 0)
            {
                var order = _orderService.GetById(orderId, true, true, true);
                if (order == null)
                {
                    throw new HttpResponseException(HttpStatusCode.NotFound);
                }

                var model = order.ToModel<OrderUpdateModel>();
                result.Add(model);
            }
            else
            {
                var query = GetDbOrders(userId);
                result = query.ToList();
            }

            return new JsonList<OrderUpdateModel>
            {
                TotalItems = result.Count,
                Data = result
            };
        }

        [HttpPut]
        public HttpResponseMessage Put(OrderUpdateModel order)
        {
            var updateOrder = Mapper.Map<Order>(order);

            try
            {
                _orderService.UpdateOrder(updateOrder);
            }
            catch (StaffingPurchaseException ex)
            {
                _logger.Info(ex.Message);
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex.Message);
            }
            catch (Exception ex)
            {
                _logger.Error("Error when updating order", ex);
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, _resourceManager.GetString(ExceptionResources.GeneralException));
            }

            return Request.CreateResponse();
        }

        private IEnumerable<OrderUpdateModel> GetDbOrders(int userId)
        {
            var products = _productService.GetAllProducts().ToList();
            var query = _orderService
                                .GetOrderInNearestOrderSessionDate(userId, new OrderStatus[] {})
                                .Select(c => GetOrderUpdateModel(c, products));
            return query;
        }

        private static OrderUpdateModel GetOrderUpdateModel(Order c, IEnumerable<Product> products)
        {
            return new OrderUpdateModel
            {
                Id = c.Id,
                Type = (OrderType)c.TypeId,
                UserId = c.UserId,
                Status = (OrderStatus)c.StatusId,
                OrderDetails = c.OrderDetails.Join(products, k => k.ProductId, p => p.Id, (k, p) => new OrderDetailGridModel
                {
                    OrderDetailId = k.Id,
                    Price = p.Price ?? 0,
                    PV = p.PV ?? 0,
                    ProductId = p.Id,
                    ProductName = p.Name,
                    Volume = k.Volume,
                    ProductSku = p.Sku
                })
            };
        }
    }
}
