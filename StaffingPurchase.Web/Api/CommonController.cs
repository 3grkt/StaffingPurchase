using StaffingPurchase.Core;
using StaffingPurchase.Services.Departments;
using StaffingPurchase.Services.Locations;
using StaffingPurchase.Services.Orders;
using StaffingPurchase.Services.Products;
using StaffingPurchase.Services.Users;
using StaffingPurchase.Web.Models.Order;
using System;
using System.Collections.Generic;
using System.Linq;
using StaffingPurchase.Services;
using StaffingPurchase.Services.Configurations;

namespace StaffingPurchase.Web.Api
{
    public class CommonController : ApiControllerBase
    {
        private readonly IOrderBatchService _batchService;
        private readonly IDepartmentService _departmentService;
        private readonly ILocationService _locationService;
        private readonly IProductService _productService;
        private readonly IUserService _userService;
        private readonly IWorkContext _workContext;
        private readonly IOrderService _orderService;

        public CommonController(
            ILocationService locationService,
            IDepartmentService departmentService,
            IProductService productService,
            IUserService userService,
            IOrderBatchService batchService,
            IWorkContext workContext,
            IOrderService orderService)
        {
            _locationService = locationService;
            _departmentService = departmentService;
            _productService = productService;
            _userService = userService;
            _batchService = batchService;
            _workContext = workContext;
            _orderService = orderService;
        }

        public IDictionary<int, string> GetAllDepartments()
        {
            return _departmentService.GetAllDepartments();
        }

        public IDictionary<int, string> GetAllLocations()
        {
            return _locationService.GetAllLocations();
        }

        public IDictionary<int, string> GetAllProductCategories()
        {
            return _productService.GetAllProductCategories();
        }

        public IDictionary<short, string> GetAllRoles()
        {
            return _userService.GetAllRoles();
        }

        public IDictionary<int, string> GetAllOrderStatus()
        {
            return _orderService.AllOrderStatus;
        }

        public double GetCurrentUserPV()
        {
            var user = _userService.GetUserById(_workContext.User.Id);
            return user.CurrentPV;
        }

        public OrderBatchDate GetOrderBatchDate()
        {
            var batch = _batchService.GetByDate(_batchService.GetNearestSessionEndDate(DateTime.Now));
            if (batch == null)
            {
                return new OrderBatchDate();
            }

            return new OrderBatchDate
            {
                ActionDate = batch.ActionDate,
                EndDate = batch.EndDate,
                Id = batch.Id,
                StartDate = batch.StartDate
            };
        }

        public OrderBatchDate GetNearestOrderBatchDate()
        {
            return new OrderBatchDate
            {
                ActionDate = null,
                Id = 0,
                EndDate = _batchService.GetNearestSessionEndDate(),
                StartDate = _batchService.GetNearestSessionStartDate()
            };
        }

        public List<OrderBatchDate> GetAllOrderBatchDates()
        {
            var lst = _batchService.GetAllSessionPeriod();
            return lst.Select(c => new OrderBatchDate()
            {
                StartDate = c.Item1,
                EndDate = c.Item2
            }).ToList();
        }
    }
}