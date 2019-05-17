using Autofac;
using Autofac.Core;
using StaffingPurchase.Core;
using StaffingPurchase.Core.Data;
using StaffingPurchase.Core.Infrastructure;
using StaffingPurchase.Data;
using StaffingPurchase.Jobs.Workers;
using StaffingPurchase.Services.Awards;
using StaffingPurchase.Services.Caching;
using StaffingPurchase.Services.CadenaIntegration;
using StaffingPurchase.Services.Configurations;
using StaffingPurchase.Services.Departments;
using StaffingPurchase.Services.LevelGroups;
using StaffingPurchase.Services.Localization;
using StaffingPurchase.Services.Locations;
using StaffingPurchase.Services.Logging;
using StaffingPurchase.Services.Orders;
using StaffingPurchase.Services.Products;
using StaffingPurchase.Services.PV;
using StaffingPurchase.Services.Users;

namespace StaffingPurchase.Jobs.Infrastructure
{
    public class DependencyRegistrar : IDependencyRegistrar
    {
        public void Register(ContainerBuilder containerBuilder)
        {
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
            containerBuilder.RegisterType<JobWorkContext>().As<IWorkContext>().InstancePerLifetimeScope();
            containerBuilder.RegisterType<JobResourceManager>().As<IResourceManager>().InstancePerLifetimeScope();
            //containerBuilder.RegisterType<FileLogger>().As<ILogger>().InstancePerLifetimeScope();
            containerBuilder.RegisterType<JobLogger>().As<ILogger>().InstancePerLifetimeScope();
            containerBuilder.RegisterType<UserService>().As<IUserService>().InstancePerLifetimeScope();
            containerBuilder.RegisterType<CacheService>().As<ICacheService>().InstancePerLifetimeScope();
            containerBuilder.RegisterType<LocationService>().As<ILocationService>().InstancePerLifetimeScope();
            containerBuilder.RegisterType<DepartmentService>().As<IDepartmentService>().InstancePerLifetimeScope();
            containerBuilder.RegisterType<LevelService>().As<ILevelService>().InstancePerLifetimeScope();
            containerBuilder.RegisterType<OrderBatchService>().As<IOrderBatchService>().InstancePerLifetimeScope();
            containerBuilder.RegisterType<ProductService>().As<IProductService>().InstancePerLifetimeScope();
            containerBuilder.RegisterType<OrderService>().As<IOrderService>().InstancePerLifetimeScope();
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

            // Workers
            containerBuilder.RegisterType<EmployeeInfoSyncWorker>().InstancePerLifetimeScope();
            containerBuilder.RegisterType<DataUpdateWorker>().InstancePerLifetimeScope();
        }

        public int Order
        {
            get { return 0; }
        }
    }
}
