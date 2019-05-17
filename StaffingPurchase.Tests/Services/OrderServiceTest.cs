using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NSubstitute;
using NSubstitute.Core;
using NUnit.Framework;
using StaffingPurchase.Core;
using StaffingPurchase.Core.Data;
using StaffingPurchase.Core.Domain;
using StaffingPurchase.Data;
using StaffingPurchase.Services.Configurations;
using StaffingPurchase.Services.Localization;
using StaffingPurchase.Services.Logging;
using StaffingPurchase.Services.Orders;
using StaffingPurchase.Services.PV;

namespace StaffingPurchase.Tests.Services
{
    [TestFixture]
    public class OrderServiceTest
    {
        private static OrderService _orderService;
        readonly IAppPolicy _mockAppPolicy = Substitute.For<IAppPolicy>();
        readonly IRepository<Order> _mockOrderRepository = Substitute.For<IRepository<Order>>();


        public void InitOrderService()
        {
            var appSetting = Substitute.For<IAppSettings>();
            var orderDetailRepository = Substitute.For<IRepository<OrderDetail>>();
            var userRepository = Substitute.For<IRepository<User>>();
            var productRepository = Substitute.For<IRepository<Product>>();
            var orderBatchRepository = Substitute.For<IRepository<OrderBatch>>();
            var purchaseLimitRepository = Substitute.For<IRepository<PurchaseLitmit>>();
            var pvLogService = Substitute.For<IPvLogService>();
            var logger = Substitute.For<ILogger>();
            var resourceManager = Substitute.For<IResourceManager>();
            var dbContext = Substitute.For<IDbContext>();

            _orderService = new OrderService(appSetting, _mockAppPolicy, resourceManager, _mockOrderRepository, orderDetailRepository, userRepository, productRepository, orderBatchRepository, purchaseLimitRepository, pvLogService, dbContext, logger);
        }

        [SetUp]
        public void Setup()
        {
            InitOrderService();
        }

        [TestCase(6, 1)]
        [TestCase(7, 14)]
        [TestCase(7, 20)]
        public void ShouldNotLockOnValidDate(int month, int day)
        {
            _mockAppPolicy.OrderSessionEndDayOfMonth.ReturnsForAnyArgs((short)15);
            _mockAppPolicy.OrderSessionStartDayOfMonth.ReturnsForAnyArgs((short)20);
            var isLocked = _orderService.IsLocked(new DateTime(2017, month, day));

            Assert.IsFalse(isLocked);
        }

        [TestCase(7, 15)]
        [TestCase(7, 19)]
        public void ShouldLockOnValidDate(int month, int day)
        {
            _mockAppPolicy.OrderSessionEndDayOfMonth.ReturnsForAnyArgs((short)15);
            _mockAppPolicy.OrderSessionStartDayOfMonth.ReturnsForAnyArgs((short)20);
            var isLocked = _orderService.IsLocked(new DateTime(2017, month, day));

            Assert.True(isLocked);
        }

        [Test]
        public void ShouldReturnTrueIfOrderWasSubmitted()
        {
            int userId = 1;
            OrderType orderType = OrderType.Cash;
            IQueryable<Order> returnedOrders = new List<Order>()
            {
                new Order()
                {
                    UserId = 1, 
                    StatusId = (short) OrderStatus.Submitted
                }
            }.AsQueryable();
            _mockOrderRepository.TableNoTracking.Returns(returnedOrders);

            Assert.IsTrue(_orderService.IsOrderSubmitted(userId, orderType));
        }
    }
}
