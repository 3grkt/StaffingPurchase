using System.Linq;
using System.Reflection;
using StaffingPurchase.Core;
using StaffingPurchase.Core.Infrastructure;
using StaffingPurchase.Jobs.Infrastructure;
using StaffingPurchase.Jobs.Workers;
using StaffingPurchase.Services.Logging;

namespace StaffingPurchase.JobExecutor
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            try
            {
                // Initialize engine context
                EngineContext.CreateEngineInstance = () => new JobEngine(); // engine object used in Web app
                EngineContext.GetDependencyRegistrars = () => new IDependencyRegistrar[]
                {
                    // dependency registrar
                    new CoreDependencyRegistrar(),
                    new DependencyRegistrar()
                };
                EngineContext.Initialize();

                // Get registered IWorker types
                var batchJobAssemblyName = EngineContext.Current.Resolve<IAppSettings>().BatchJobAssemblyName;
                var workerType = typeof(IWorker);
                var foundTypes = Assembly.Load(batchJobAssemblyName).GetTypes()
                    .Where(t => workerType.IsAssignableFrom(t) && t.IsClass && !t.IsAbstract)
                    .ToList();

                // Create IWorker instance and add to list
                var workers = foundTypes.Select(type => EngineContext.Current.Resolve(type) as IWorker).ToList();

                // Sort and execute
                var sortedWorkers = workers.OrderBy(w => w.Order);
                foreach (var worker in sortedWorkers)
                {
                    if (worker.CanWork)
                    {
                        worker.DoWork();
                    }
                    else
                    {
                        EngineContext.Current.Resolve<ILogger>().Warn(
                            $"Job {worker.GetType().Name} is currently turned off");
                    }
                }
            }
            finally
            {
                // End engine context
                EngineContext.End();
            }
        }
    }
}
