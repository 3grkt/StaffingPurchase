using System;
using System.Runtime.Caching;
using StaffingPurchase.Core;

namespace StaffingPurchase.Services.Caching
{
    public class CacheService : ICacheService
    {
        private readonly IAppSettings _appSettings;

        private static readonly MemoryCache _cache = MemoryCache.Default;

        public CacheService(IAppSettings appSettings)
        {
            _appSettings = appSettings;
        }

        #region ICacheService Members

        public T Get<T>(string key)
            where T : class
        {
            try
            {
                return (T) _cache[key];
            }
            catch
            {
                return null;
            }
        }

        public void Set<T>(string key, T value, int? cacheDuration = null, bool absoluteExpiration = false)
        {
            double cacheMinutes = (double) (cacheDuration ?? _appSettings.DefaultCacheDuration);
            if (absoluteExpiration)
            {
                _cache.Add(key, value, DateTimeOffset.Now.AddMinutes(cacheMinutes));
            }
            else
            {
                _cache.Add(key, value, new CacheItemPolicy {SlidingExpiration = TimeSpan.FromMinutes(cacheMinutes)});
            }
        }

        public void Remove(string key)
        {
            _cache.Remove(key);
        }
        #endregion
    }
}
