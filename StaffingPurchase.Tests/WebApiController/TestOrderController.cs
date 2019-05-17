using Autofac;
using NUnit.Framework;
using StaffingPurchase.Core;
using StaffingPurchase.Core.Data;
using StaffingPurchase.Core.Infrastructure;
using StaffingPurchase.Data;
using StaffingPurchase.Services.Localization;
using StaffingPurchase.Services.Orders;
using StaffingPurchase.Services.Users;
using StaffingPurchase.Web.Api;
using StaffingPurchase.Web.Framework;
using StaffingPurchase.Web.Infrastructure;
using StaffingPurchase.Web.Models.Order;
using System.Linq;
using System.Net.Http;

namespace StaffingPurchase.Tests.WebApiController
{
    internal class TestOrderController : TestSuiteBase
    {
        [Test]
        public void AddOrderDetails_ShouldAddSuccessfully()
        {
            int userId = 10;
            var orderService = ContainerManager.Resolve<IOrderService>();
            var userService = ContainerManager.Resolve<IUserService>();
            var orderController = new OrderController(orderService, null, null, null); // TODO: leave null to build
            orderController.Request = new HttpRequestMessage();
            orderController.Configuration = new System.Web.Http.HttpConfiguration();
            var orderDetailM = new OrderDetailModel
            {
                ProductId = 24,
                Volume = 3,
                IsPrice = true
            };

            orderController.AddOrderDetails(orderDetailM);
            var result = orderController.GetOrderDetails();
            var order = orderService.GetOrder(userId, OrderStatus.Draft, OrderType.Cash);
            Assert.AreEqual(1, result.TotalItems);
            Assert.AreEqual((int)24, result.Data.ElementAt(0).ProductId);
            Assert.AreEqual(3, result.Data.ElementAt(0).Volume);

            orderService.RemoveOrder(order);
        }

        [Test]
        public void GetOrderDetails_ShouldReturnEmpty()
        {
            var orderService = ContainerManager.Resolve<IOrderService>();
            var userService = ContainerManager.Resolve<IUserService>();
            var orderController = new OrderController(orderService, null, null, null);
            var result = orderController.GetOrderDetails();
            Assert.AreEqual(0, result.TotalItems);
            Assert.IsNull(result.Data);
        }

        [SetUp]
        public void Setup()
        {
            var builder = new ContainerBuilder();
            AutoMapperConfiguration.ConfigMapping();
            builder.RegisterType<SqlServerDataProvider>().As<IDataProvider>().SingleInstance();
            builder.RegisterType<StaffingPurchaseDataContext>().As<IDbContext>().InstancePerLifetimeScope();
            builder.RegisterType<OrderService>().As<IOrderService>().InstancePerLifetimeScope();
            builder.RegisterType<UserService>().As<IUserService>().InstancePerLifetimeScope();
            builder.RegisterType<WebResourceManager>().As<IResourceManager>().InstancePerLifetimeScope();
            builder.RegisterGeneric(typeof(EfRepository<>)).As(typeof(IRepository<>)).InstancePerLifetimeScope();
            builder.Update(ContainerManager.Container);
        }
    }
}
