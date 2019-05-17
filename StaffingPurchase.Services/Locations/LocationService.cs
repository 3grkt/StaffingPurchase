using StaffingPurchase.Core;
using StaffingPurchase.Core.Data;
using StaffingPurchase.Core.Domain;
using StaffingPurchase.Services.Caching;
using System.Collections.Generic;
using System.Linq;

namespace StaffingPurchase.Services.Locations
{
    public class LocationService : ServiceBase, ILocationService
    {
        #region Fields

        private readonly ICacheService _cacheService;
        private readonly IRepository<Location> _locationRepository;

        #endregion Fields

        public LocationService(IRepository<Location> locationRepository, ICacheService cacheService)
        {
            _locationRepository = locationRepository;
            _cacheService = cacheService;
        }

        public IDictionary<int, string> GetAllLocations()
        {
            var locations = _cacheService.Get<IDictionary<int, string>>(CacheNames.Locations);
            if (locations == null)
            {
                locations = _locationRepository.TableNoTracking
                    .Select(x => new { x.Id, x.Name })
                    .ToDictionary(t => t.Id, t => t.Name.Trim());
                _cacheService.Set(CacheNames.Locations, locations);
            }
            return locations;
        }

        public Location GetByName(string locationName)
        {
            var query = _locationRepository.TableNoTracking;
            return query.FirstOrDefault(x => x.Name == locationName); // TODO: consider adding LocationCode to database
        }

        public string GetLocationName(int locationId)
        {
            var location = _locationRepository.TableNoTracking.FirstOrDefault(c => c.Id == locationId);
            if (location != null)
            {
                return location.Name;
            }

            return string.Empty;
        }

        public void Add(Location location, bool inTransaction = false)
        {
            _locationRepository.Insert(location, !inTransaction);
        }
    }
}
