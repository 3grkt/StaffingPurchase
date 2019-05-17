using StaffingPurchase.Core.Infrastructure;

namespace StaffingPurchase.Tests
{
    public abstract class TestSuiteBase
    {
        //private object AssetService;
        public static ContainerManager ContainerManager
        {
            get { return EngineContext.Current.ContainerManager; }
        }

        protected string DbContextName => TestEngine.DbContextName;

        static TestSuiteBase()
        {
            //RegisterDependencies();
            EngineContext.CreateEngineInstance = () => new TestEngine();
            EngineContext.Initialize();
        }


        //private static void RegisterDependencies()
        //{
        //    var builder = new ContainerBuilder();
        //    var container = builder.Build();

        //    builder = new ContainerBuilder();
        //    //builder.RegisterType<AMSDataContext>().As<IDbContext>().InstancePerLifetimeScope();
        //    //builder.RegisterType<AMSDataContext>().Named<IDbContext>("SqlServerContext").InstancePerLifetimeScope();
        //    builder.RegisterType<AMSDataContext>().Named<IDbContext>("SqlServerContext").SingleInstance();

        //    // Repository
        //    builder.RegisterGeneric(typeof(EfRepository<>)).Named("SqlServerRepository", typeof(IRepository<>))
        //        .WithParameter(
        //            (pi, ctx) => pi.ParameterType == typeof(IDbContext),
        //            (pi, ctx) => ctx.ResolveKeyed<IDbContext>("SqlServerContext"))
        //        //.WithParameter(new TypedParameter(typeof(IDbContext), new AMSDataContext()))
        //        .InstancePerLifetimeScope();

        //    // Service
        //    builder.RegisterType<AssetService>().Named<IAssetService>("SqlServerService").InstancePerLifetimeScope();

        //    builder.Update(container);

        //    ContainerManager = new ContainerManager(container, new TestContextManager());
        //}

        #region Methods

        // Put common test methods here

        #endregion
    }
}
