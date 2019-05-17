using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using StaffingPurchase.Core;
using StaffingPurchase.Services.Orders;
using StaffingPurchase.Web.Framework.Filters;

namespace StaffingPurchase.Web.Api
{
    [RoleAuthorize(UserRole.IT)]
    [RoutePrefix("api/it")]
    public class SpecialTaskController: ApiControllerBase
    {
        private readonly IOrderWarehouseService _warehouseService;
        private readonly IOrderService _orderService;

        public SpecialTaskController(IOrderWarehouseService warehouseService, IOrderService orderService)
        {
            _warehouseService = warehouseService;
            _orderService = orderService;
        }

        [HttpPut]
        [Route("update/order-area")]
        public HttpResponseMessage UpdateOrderArea()
        {
            _warehouseService.UpdateOrderAreaBasedOnPackageLog();
            _orderService.UpdateMissingAreaOrders();

            return Request.CreateResponse(HttpStatusCode.OK);
        }
    }
}