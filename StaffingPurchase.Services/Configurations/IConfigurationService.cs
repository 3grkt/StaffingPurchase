using System;
using System.Collections.Generic;
using StaffingPurchase.Core.Domain;

namespace StaffingPurchase.Services.Configurations
{
    public interface IConfigurationService
    {
        /// <summary>
        /// Gets value of configration.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <returns></returns>
        T Get<T>(string name) where T : IConvertible;
        
        /// <summary>
        /// Gets value with cache.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        string GetWithCache(string name);

        /// <summary>
        /// Gets configuration object.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        Configuration Get(string name);

        /// <summary>
        /// Updates configuration.
        /// </summary>
        /// <param name="config"></param>
        /// <param name="inTransaction"></param>
        void Update(Configuration config, bool inTransaction = false);

        /// <summary>
        /// Updates multiple configurations at once.
        /// </summary>
        /// <param name="configurations"></param>
        void Update(IEnumerable<Configuration> configurations);

        /// <summary>
        /// Gets all configurations.
        /// </summary>
        /// <param name="noCache"></param>
        /// <returns></returns>
        IList<Configuration> GetAll(bool noCache = false);
    }
}
