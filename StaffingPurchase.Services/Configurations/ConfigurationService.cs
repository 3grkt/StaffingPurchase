using System;
using System.Collections.Generic;
using System.Linq;
using StaffingPurchase.Core;
using StaffingPurchase.Core.Data;
using StaffingPurchase.Core.Domain;
using StaffingPurchase.Services.Caching;

namespace StaffingPurchase.Services.Configurations
{
    public class ConfigurationService : ServiceBase, IConfigurationService
    {
        private readonly IRepository<Configuration> _configurationRepo;
        private readonly ICacheService _cacheService;

        public ConfigurationService(IRepository<Configuration> configurationRepo, ICacheService cacheService)
        {
            _configurationRepo = configurationRepo;
            _cacheService = cacheService;
        }

        public Configuration Get(string name)
        {
            return _configurationRepo.Table.FirstOrDefault(x => x.Name == name);
        }

        public T Get<T>(string name)
            where T : IConvertible
        {
            var config = Get(name);
            if (config != null)
            {
                return (T)Convert.ChangeType(config.Value, typeof(T));
            }
            return default(T);
        }

        public IList<Configuration> GetAll(bool noCache = false)
        {
            var allConfigs = _cacheService.Get<IList<Configuration>>(CacheNames.Configurations);
            if (noCache || allConfigs == null)
            {
                allConfigs = _configurationRepo.TableNoTracking.ToList();
                _cacheService.Set(CacheNames.Configurations, allConfigs);
            }
            return allConfigs;
        }

        public void Update(Configuration config, bool inTransaction = false)
        {
            _configurationRepo.Update(config, !inTransaction);
            _cacheService.Remove(CacheNames.Configurations); // clear cache
        }

        public void Update(IEnumerable<Configuration> configurations)
        {
            var allConfigs = _configurationRepo.Table.ToList();
            foreach (var updatedConfig in configurations)
            {
                var currentConfig = allConfigs.FirstOrDefault(x => x.Name.Equals(updatedConfig.Name, StringComparison.OrdinalIgnoreCase));
                if (currentConfig != null && currentConfig.Value != updatedConfig.Value)
                {
                    currentConfig.Value = updatedConfig.Value;
                    currentConfig.ModifiedDate = DateTime.Now;
                    _configurationRepo.Update(updatedConfig, false);
                }
            }
            _configurationRepo.SaveChanges();
            _cacheService.Remove(CacheNames.Configurations); // clear cache
        }

        public string GetWithCache(string name)
        {
            var config = GetAll().FirstOrDefault(x => x.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
            return config == null ? string.Empty : config.Value;
        }
    }
}
