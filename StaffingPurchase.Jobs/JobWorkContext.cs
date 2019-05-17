using System.Globalization;
using StaffingPurchase.Core;
using StaffingPurchase.Core.Infrastructure;

namespace StaffingPurchase.Jobs
{
    public class JobWorkContext : IWorkContext
    {
        public WorkingUser User { get; set; }

        public CultureInfo WorkingCulture { get; set; }

        public JobWorkContext()
        {
            WorkingCulture = new CultureInfo(EngineContext.Current.Resolve<IAppSettings>().DefaultAppCulture);
        }
    }
}
