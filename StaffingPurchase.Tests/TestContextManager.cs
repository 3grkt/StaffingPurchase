using StaffingPurchase.Core.Infrastructure;

namespace StaffingPurchase.Tests
{
    public class TestContextManager : IContextManager
    {
        #region IContextManager Members

        public Autofac.ILifetimeScope GetContextLifetimeScope()
        {
            return null;
        }

        #endregion
    }
}