using Autofac;

namespace StaffingPurchase.Core.Infrastructure
{
    public class CoreDependencyRegistrar : IDependencyRegistrar
    {
        #region IDependencyRegistrar Members

        public void Register(ContainerBuilder builder)
        {
            builder.RegisterType<AppSettings>().As<IAppSettings>().SingleInstance();
        }

        public int Order
        {
            get { return 0; }
        }

        #endregion
    }
}
