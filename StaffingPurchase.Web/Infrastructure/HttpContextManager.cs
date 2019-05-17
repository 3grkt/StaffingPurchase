using System.Web;
using Autofac.Integration.Mvc;
using StaffingPurchase.Core.Infrastructure;

namespace StaffingPurchase.Web.Infrastructure
{
    public class HttpContextManager : IContextManager
    {
        #region IContextManager Members

        public Autofac.ILifetimeScope GetContextLifetimeScope()
        {
            if (HttpContext.Current != null)
                return AutofacDependencyResolver.Current.RequestLifetimeScope;
            return null;
        }

        #endregion
    }
}