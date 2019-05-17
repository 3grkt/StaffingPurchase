using Autofac;
using NUnit.Framework;
using StaffingPurchase.Core.Data;
using StaffingPurchase.Core.Domain;
using StaffingPurchase.Data;
using StaffingPurchase.Services.Products;
using StaffingPurchase.Web.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StaffingPurchase.Tests.WebApiController
{
    public class TestProductController : TestSuiteBase
    {
        [Test]
        public void GetAll_ShouldReturnAllProducts()
        {
            var prodService = ContainerManager.Resolve<IProductService>();
            var products = prodService.GetAllProducts();
            Assert.AreEqual(71, products.Count());
        }

        [SetUp]
        public void Setup()
        {
            var builder = new ContainerBuilder();
            AutoMapperConfiguration.ConfigMapping();
            builder.RegisterType<SqlServerDataProvider>().As<IDataProvider>().SingleInstance();
            builder.RegisterType<StaffingPurchaseDataContext>().As<IDbContext>().InstancePerLifetimeScope();
            builder.RegisterType<ProductService>().As<IProductService>().InstancePerLifetimeScope();
            builder.RegisterGeneric(typeof(EfRepository<>)).As(typeof(IRepository<>)).InstancePerLifetimeScope();
            builder.Update(ContainerManager.Container);
        }
    }
}