using System;
using System.Collections.Generic;
using StoreApp.Entity;
using StoreApp.Data;
using System.Linq;
using Microsoft.EntityFrameworkCore;


namespace StoreApp.Repository
{
    public interface ILocation
    {
        IEnumerable<Location> GetLocations();
        IEnumerable<LocationInventory> GetAllInventory(Location location);
        IEnumerable<LocationInventory> GetInventoryAvailable(Location location);
        Location GetMostStocked();
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

        Location ILocation.GetMostStocked()
        {
            var locationIds = this._context.LocationInventories
                .Include(loc => loc.Location)
                .Select( li => new {
                    LocationId = li.Location.LocationId
                });

            var locationWithMostProducts = locationIds
                .GroupBy(l => l.LocationId)
                .Select( gr => new {
                    LocationId = gr.Key,
                    Count = gr.Count()
                })
                .OrderByDescending(gr => gr.Count)
                .FirstOrDefault();

            return this._context.Locations
                       .Where(l => l.LocationId == locationWithMostProducts.LocationId)
                       .FirstOrDefault();
        }
    }
}