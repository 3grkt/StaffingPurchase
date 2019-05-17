using Autofac;
using StaffingPurchase.Core.Infrastructure;

namespace StaffingPurchase.Jobs.Infrastructure
{
    public class JobContextManager : IContextManager
    {
        private readonly IContainer _currentContainer;
        private ILifetimeScope _currentScope;

        public JobContextManager(IContainer container)
        {
            _currentContainer = container;
        }

        protected ILifetimeScope CurrentScope
        {
            get
            {
                if (_currentScope == null)
                    _currentScope = _currentContainer.BeginLifetimeScope("JobContextManager");
                return _currentScope;
            }
        }

        #region IContextManager Members

        public ILifetimeScope GetContextLifetimeScope()
        {
            return CurrentScope;
        }

        #endregion
    }
}
