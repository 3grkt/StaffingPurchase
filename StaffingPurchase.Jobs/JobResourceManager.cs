using System.Collections.Generic;
using System.Resources;
using StaffingPurchase.Core;
using StaffingPurchase.Core.Infrastructure;
using StaffingPurchase.Jobs.Properties;
using StaffingPurchase.Services.Localization;

namespace StaffingPurchase.Jobs
{
    public class JobResourceManager : IResourceManager
    {
        private static ResourceManager _resourceManager;

        public static ResourceManager ResourceManager
        {
            get { return _resourceManager ?? (_resourceManager = new ResourceManager(typeof (Resources))); }
        }

        public string GetString(string resourceKey)
        {
            var value = ResourceManager.GetString(resourceKey, EngineContext.Current.Resolve<IWorkContext>().WorkingCulture);
            return string.IsNullOrEmpty(value) ? resourceKey : value;
        }

        public IDictionary<string, string> GetAllTexts(string locale)
        {
            return new Dictionary<string, string>(); // return empty dictionary
        }
    }
}
