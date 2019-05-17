using StaffingPurchase.Core.Domain;
using System.Collections.Generic;

namespace StaffingPurchase.Services.Locations
{
    public interface ILocationService
    {
        IDictionary<int, string> GetAllLocations();

        string GetLocationName(int locationId);
        Location GetByName(string locationName);
        void Add(Location location, bool inTransaction = false);
    }
}
