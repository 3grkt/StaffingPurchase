using Autofac;
using Autofac.Core;
using StaffingPurchase.Core.Infrastructure;
using StaffingPurchase.Jobs;
using StaffingPurchase.Jobs.Workers;
using StaffingPurchase.Services.Logging;
using StaffingPurchase.Services.Orders;
using StaffingPurchase.Services.Users;

namespace StaffingPurchase.Web.Infrastructure
{
    public class BatchJobTestDependencyRegistrar : IDependencyRegistrar
    {
        public int Order
        {
            get { return 0; }
        }

        public void Register(ContainerBuilder containerBuilder)
        {
            var jobLoggerParameter = new ResolvedParameter(
                    (pi, ctx) => pi.ParameterType == typeof(ILogger),
                    (pi, ctx) => ctx.ResolveNamed<ILogger>("JobLogger"));

            var userServiceParameter = new ResolvedParameter(
                (pi, ctx) => pi.ParameterType == typeof(IUserService),
                (pi, ctx) => ctx.ResolveNamed<IUserService>("WithJobLogger"));

            var orderBatchServiceParameter = new ResolvedParameter(
                (pi, ctx) => pi.ParameterType == typeof(IOrderBatchService),
                (pi, ctx) => ctx.ResolveNamed<IOrderBatchService>("WithJobLogger"));

            // Loggers
            containerBuilder.RegisterType<JobLogger>().Named<ILogger>("JobLogger").InstancePerLifetimeScope();

            // Services with JobLogger
            containerBuilder.RegisterType<UserService>().Named<IUserService>("WithJobLogger")
                .WithParameter(jobLoggerParameter).InstancePerLifetimeScope();

            containerBuilder.RegisterType<OrderBatchService>().Named<IOrderBatchService>("WithJobLogger")
                .WithParameter(jobLoggerParameter).InstancePerLifetimeScope();

            // Workers
            containerBuilder.RegisterType<EmployeeInfoSyncWorker>()
                .WithParameter(jobLoggerParameter)
                .WithParameter(userServiceParameter)
                .InstancePerLifetimeScope();

            containerBuilder.RegisterType<DataUpdateWorker>()
                .WithParameter(jobLoggerParameter)
                .WithParameter(userServiceParameter)
                .WithParameter(orderBatchServiceParameter)
                .InstancePerLifetimeScope();
        }
    }
}
