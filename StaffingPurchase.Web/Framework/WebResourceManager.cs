using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Resources;
using System.Threading;
using StaffingPurchase.Core;
using StaffingPurchase.Core.Infrastructure;
using StaffingPurchase.Services.Caching;
using StaffingPurchase.Services.Localization;

namespace StaffingPurchase.Web.Framework
{
    public class WebResourceManager : IResourceManager
    {
        private readonly ICacheService _cacheService;

        private static ResourceManager _resourceManager;
        public static ResourceManager ResourceManager
        {
            get
            {
                return _resourceManager ??
                    (_resourceManager = new ResourceManager("StaffingPurchase.Web.Properties.Resources", Assembly.GetCallingAssembly()));
            }
        }

        public WebResourceManager(ICacheService cacheService)
        {
            _cacheService = cacheService;
        }

        public string GetString(string resourceKey)
        {
            var value = ResourceManager.GetString(resourceKey,
                EngineContext.Current.Resolve<IWorkContext>().WorkingCulture);
            return string.IsNullOrEmpty(value) ? resourceKey : value;
        }

        public IDictionary<string, string> GetAllTexts(string locale)
        {
            locale = locale ?? EngineContext.Current.Resolve<IWorkContext>().WorkingCulture.TwoLetterISOLanguageName; // e.g. en, vi
            var cacheName = $"{CacheNames.WebResources}-{locale}";
            var dictResource = _cacheService.Get<IDictionary<string, string>>(cacheName);
            if (dictResource == null)
            {
                dictResource = new Dictionary<string, string>();
                var resourceSet = ResourceManager.GetResourceSet(CultureInfo.GetCultureInfo(locale), true, true);
                if (resourceSet != null)
                {
                    foreach (DictionaryEntry entry in resourceSet)
                    {
                        dictResource[entry.Key as string] = entry.Value as string;
                    }
                }
                _cacheService.Set(cacheName, dictResource);
            }
            return dictResource;
        }
    }
}
