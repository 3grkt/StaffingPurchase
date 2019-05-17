using Autofac;

namespace StaffingPurchase.Core.Infrastructure
{
    public interface IDependencyRegistrar
    {
        void Register(ContainerBuilder builder);
        int Order { get; }
    }
}