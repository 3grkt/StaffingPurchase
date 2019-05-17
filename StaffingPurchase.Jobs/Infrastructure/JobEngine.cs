using System;
using Autofac;
using StaffingPurchase.Core.Configuration;
using StaffingPurchase.Core.Infrastructure;
//using Autofac.Integration.Mvc;

//using System.Web.Mvc;

namespace StaffingPurchase.Jobs.Infrastructure
{
    public class JobEngine : IEngine, IDisposable
    {
        private ContainerManager _containerManager;

        #region Utilities

        private void RegisterDependencies(StaffingPurhcaseConfig config,
            params IDependencyRegistrar[] dependencyRegistars)
        {
            var builder = new ContainerBuilder();
            var container = builder.Build();

            // Infrastructure
            builder = new ContainerBuilder();
            builder.RegisterInstance(config).As<StaffingPurhcaseConfig>().SingleInstance();
            builder.RegisterInstance(this).As<IEngine>().SingleInstance();
            builder.Update(container);

            // Invoke other dependency registrars
            builder = new ContainerBuilder();
            foreach (var depRegistrar in dependencyRegistars)
                depRegistrar.Register(builder);
            builder.Update(container);

            // Set container manager
            _containerManager = new ContainerManager(container, new JobContextManager(container));
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
            return ContainerManager.Resolve<T>(key);
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

        #region IDisposable Members

        public void Dispose()
        {
            var scope = ContainerManager.ContextManager.GetContextLifetimeScope();
            if (scope != null)
                scope.Dispose();
        }

        #endregion
    }
}