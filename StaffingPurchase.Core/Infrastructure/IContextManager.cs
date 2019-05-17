using Autofac;

namespace StaffingPurchase.Core.Infrastructure
{
    public interface IContextManager
    {
        ILifetimeScope GetContextLifetimeScope();
    }
}