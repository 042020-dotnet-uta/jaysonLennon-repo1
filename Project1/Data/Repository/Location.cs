using System;
using System.Collections.Generic;
using StoreApp.Data;
using StoreApp.Data.Entity;

namespace StoreApp.Repository
{
    public interface ILocationRepository
    {
        IEnumerable<Location> GetLocations();
        IEnumerable<LocationInventory> GetAllInventory(Location location);
        IEnumerable<LocationInventory> GetInventoryAvailable(Location location);
    }

    public class LocationRepository : ILocationRepository
    {
        private StoreContext _context;

        public LocationRepository(StoreContext context)
        {
            this._context = context;
        }

        IEnumerable<LocationInventory> ILocationRepository.GetAllInventory(Location location)
        {
            throw new NotImplementedException();
        }

        IEnumerable<LocationInventory> ILocationRepository.GetInventoryAvailable(Location location)
        {
            throw new NotImplementedException();
        }

        IEnumerable<Location> ILocationRepository.GetLocations()
        {
            throw new NotImplementedException();
        }
    }
}