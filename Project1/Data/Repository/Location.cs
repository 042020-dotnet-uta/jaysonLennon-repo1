using System;
using System.Collections.Generic;
using StoreApp.Entity;
using StoreApp.Data;

namespace StoreApp.Repository
{
    public interface ILocation
    {
        IEnumerable<Location> GetLocations();
        IEnumerable<LocationInventory> GetAllInventory(Location location);
        IEnumerable<LocationInventory> GetInventoryAvailable(Location location);
    }

    public class LocationRepository : ILocation
    {
        private StoreContext _context;

        public LocationRepository(StoreContext context)
        {
            this._context = context;
        }

        IEnumerable<LocationInventory> ILocation.GetAllInventory(Location location)
        {
            throw new NotImplementedException();
        }

        IEnumerable<LocationInventory> ILocation.GetInventoryAvailable(Location location)
        {
            throw new NotImplementedException();
        }

        IEnumerable<Location> ILocation.GetLocations()
        {
            throw new NotImplementedException();
        }
    }
}