using System.Reflection;
using Autofac;
using Autofac.Core;
using Autofac.Integration.Mvc;
using Autofac.Integration.WebApi;
using StaffingPurchase.Core;
using StaffingPurchase.Core.Data;
using StaffingPurchase.Core.Infrastructure;
using StaffingPurchase.Data;
using StaffingPurchase.Services.Awards;
using StaffingPurchase.Services.Caching;
using StaffingPurchase.Services.CadenaIntegration;
using StaffingPurchase.Services.Configurations;
using StaffingPurchase.Services.Departments;
using StaffingPurchase.Services.Email;
using StaffingPurchase.Services.ImportExport;
using StaffingPurchase.Services.LevelGroups;
using StaffingPurchase.Services.Localization;
using StaffingPurchase.Services.Locations;
using StaffingPurchase.Services.Logging;
using StaffingPurchase.Services.Orders;
using StaffingPurchase.Services.Products;
using StaffingPurchase.Services.PV;
using StaffingPurchase.Services.Users;
using StaffingPurchase.Web.Framework;
using StaffingPurchase.Web.Helpers;

namespace StaffingPurchase.Web.Infrastructure
{
    public class DependencyRegistrar : IDependencyRegistrar
    {
        public int Order
        {
            get { return 0; }
        }

        public void Register(ContainerBuilder containerBuilder)
        {
            // Controllers
            containerBuilder.RegisterControllers(Assembly.GetExecutingAssembly());

            // WebApi
            containerBuilder.RegisterApiControllers(Assembly.GetExecutingAssembly());

            // DbContext
            containerBuilder.RegisterType<StaffingPurchaseDataContext>().As<IDbContext>().InstancePerLifetimeScope();
            containerBuilder.RegisterType<CadenaDataContext>().Named<IDbContext>("Cadena").InstancePerLifetimeScope();

            // Repository
            containerBuilder.RegisterGeneric(typeof(EfRepository<>))
                .As(typeof(IRepository<>))
                .InstancePerLifetimeScope();

            // Data Provider
            containerBuilder.RegisterType<SqlServerDataProvider>().As<IDataProvider>().InstancePerLifetimeScope();
            containerBuilder.RegisterType<DataHelper>().As<IDataHelper>().InstancePerLifetimeScope();

            // Services
            containerBuilder.RegisterType<WebResourceManager>().As<IResourceManager>().InstancePerLifetimeScope();
            containerBuilder.RegisterType<ElmahLogger>().As<ILogger>().InstancePerLifetimeScope();
            containerBuilder.RegisterType<ElmahLogger>().As<IQueriableLogger>().InstancePerLifetimeScope();
            containerBuilder.RegisterType<UserService>().As<IUserService>().InstancePerLifetimeScope();
            containerBuilder.RegisterType<LocationService>().As<ILocationService>().InstancePerLifetimeScope();
            containerBuilder.RegisterType<DepartmentService>().As<IDepartmentService>().InstancePerLifetimeScope();
            containerBuilder.RegisterType<ExcelParamManager>().As<IExcelParamManager>().InstancePerLifetimeScope();
            containerBuilder.RegisterType<ExportManager>().As<IExportManager>().InstancePerLifetimeScope();
            containerBuilder.RegisterType<ImportManager>().As<IImportManager>().InstancePerLifetimeScope();
            containerBuilder.RegisterType<CacheService>().As<ICacheService>().InstancePerLifetimeScope();

            containerBuilder.RegisterType<LevelGroupService>().As<ILevelGroupService>().InstancePerLifetimeScope();
            containerBuilder.RegisterType<LevelService>().As<ILevelService>().InstancePerLifetimeScope();
            containerBuilder.RegisterType<OrderBatchService>().As<IOrderBatchService>().InstancePerLifetimeScope();
            containerBuilder.RegisterType<OrderReportService>().As<IOrderReportService>().InstancePerLifetimeScope();
            containerBuilder.RegisterType<ProductService>().As<IProductService>().InstancePerLifetimeScope();
            containerBuilder.RegisterType<OrderService>().As<IOrderService>().InstancePerLifetimeScope();
            containerBuilder.RegisterType<EmailDelivery>().As<IEmailDelivery>().InstancePerLifetimeScope();
            containerBuilder.RegisterType<OrderWarehouseService>().As<IOrderWarehouseService>().InstancePerLifetimeScope();
            containerBuilder.RegisterType<PvLogService>().As<IPvLogService>().InstancePerLifetimeScope();
            containerBuilder.RegisterType<AwardService>().As<IAwardService>().InstancePerLifetimeScope();
            containerBuilder.RegisterType<ConfigurationService>().As<IConfigurationService>().InstancePerLifetimeScope();
            containerBuilder.RegisterType<AppPolicy>().As<IAppPolicy>().InstancePerLifetimeScope();

            containerBuilder.RegisterType<CadenaIntegrationService>().As<ICadenaIntegrationService>()
                .WithParameter(new ResolvedParameter(
                    (pi, ctx) => pi.ParameterType == typeof(IDbContext),
                    (pi, ctx) => ctx.ResolveNamed<IDbContext>("Cadena")))
                .InstancePerLifetimeScope();

            // WebHelper
            containerBuilder.RegisterType<WebHelper>().As<IWebHelper>().SingleInstance();

            // WorkContext
            containerBuilder.RegisterType<WorkContext>().As<IWorkContext>().InstancePerLifetimeScope();
        }
    }
}
