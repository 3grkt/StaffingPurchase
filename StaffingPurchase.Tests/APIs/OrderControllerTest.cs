using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NSubstitute;
using NUnit.Framework;
using StaffingPurchase.Core;
using StaffingPurchase.Services.Localization;
using StaffingPurchase.Services.Logging;
using StaffingPurchase.Services.Orders;
using StaffingPurchase.Web.Api;

namespace StaffingPurchase.Tests.APIs
{
    [TestFixture]
    public class OrderControllerTest
    {
        private OrderController _orderController;
        private readonly IOrderService _mockOrderService = Substitute.For<IOrderService>();
        private readonly IWorkContext _mockWorkContext = Substitute.For<IWorkContext>();
        private readonly IResourceManager _mockResourceManager = Substitute.For<IResourceManager>();
        private readonly ILogger _mockLogger = Substitute.For<ILogger>();

        [SetUp]
        public void SetUp()
        {
            _orderController = new OrderController(_mockOrderService, _mockResourceManager, _mockLogger, _mockWorkContext);
        }

        [Test]
        public void IsLockedToOrderIfUserIsNotEligible()
        {
            _mockOrderService.IsEligibleUser(1).ReturnsForAnyArgs(false);
            _mockOrderService.IsLocked(DateTime.Now).ReturnsForAnyArgs(false);
            _mockWorkContext.User.Returns(new WorkingUser()
            {
                Id = 1
            });

            var isLocked = _orderController.IsLookedOrder();
            Assert.IsTrue(isLocked);
        }

        [Test]
        public void IsLockedToOrderIfDateIsNotValid()
        {
            _mockOrderService.IsLocked(DateTime.Now).ReturnsForAnyArgs(true);
            var isLocked = _orderController.IsLookedOrder();
            Assert.IsTrue(isLocked);
        }
    }
}
