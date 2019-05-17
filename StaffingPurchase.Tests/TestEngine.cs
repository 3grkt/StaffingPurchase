using System;
using Autofac;
using StaffingPurchase.Core.Configuration;
using StaffingPurchase.Core.Data;
using StaffingPurchase.Core.Infrastructure;
using StaffingPurchase.Data;
using StaffingPurchase.Core;

namespace StaffingPurchase.Tests
{
    public class TestEngine : IEngine
    {
        private ContainerManager _containerManager;

        public const string DbContextName = "SqlServerContext";
        //public const string DbRepositoryName = "SqlServerRepository";

        #region Utilities

        private void RegisterDependencies(StaffingPurhcaseConfig config,
            params IDependencyRegistrar[] dependencyRegistars)
        {
            var builder = new ContainerBuilder();
            var container = builder.Build();

            builder = new ContainerBuilder();
            builder.RegisterType<StaffingPurchaseDataContext>().Named<IDbContext>(DbContextName).SingleInstance();

            // Repository
            builder.RegisterGeneric(typeof(EfRepository<>)).Named(DbContextName, typeof(IRepository<>))
                .WithParameter(
                    (pi, ctx) => pi.ParameterType == typeof(IDbContext),
                    (pi, ctx) => ctx.ResolveKeyed<IDbContext>(DbContextName))
                .InstancePerLifetimeScope();

            // Data Provider
            builder.RegisterType<SqlServerDataProvider>().Named<IDataProvider>(DbContextName).SingleInstance();
            builder.RegisterType<DataHelper>().Named<IDataHelper>(DbContextName).SingleInstance();

            // Service
            //builder.RegisterType<AssetService>().Named<IAssetService>("SqlServerService").InstancePerLifetimeScope();

            builder.Update(container);

            // Set container manager
            _containerManager = new ContainerManager(container, new TestContextManager());
            //DependencyResolver.SetResolver(new AutofacDependencyResolver(container));
        }

        #endregion

        #region IEngine Members

        public ContainerManager ContainerManager
        {
            get { return _containerManager; }
        }

        public void Initialize(StaffingPurhcaseConfig config, params IDependencyRegistrar[] dependencyRegistars)
        {
            RegisterDependencies(config, dependencyRegistars);
        }

        public T Resolve<T>(string key = null) where T : class
        {
            if (!string.IsNullOrEmpty(key))
                return ContainerManager.Resolve<T>(key);

            return ContainerManager.Resolve<T>();
        }

        public object Resolve(Type type)
        {
            return ContainerManager.Resolve(type);
        }

        public T[] ResolveAll<T>()
        {
            return ContainerManager.ResolveAll<T>();
        }

        #endregion
    }
}
