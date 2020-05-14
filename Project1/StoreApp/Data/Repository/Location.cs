using System;
using System.Collections.Generic;
using StoreApp.Entity;
using StoreApp.Data;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace StoreApp.Repository
{
    public interface ILocation
    {
        Task<Location> GetById(Guid id);
        IEnumerable<Location> GetLocations();
        Location GetMostStocked();
        IEnumerable<Tuple<Product, int>> GetProductsAvailable(Location location);
        IEnumerable<Tuple<Product, int>> GetAllProducts(Location location);
        Task<int> GetStock(Location location, Product product);
    }

    public class LocationRepository : ILocation
    {
        private StoreContext _context;

        public LocationRepository(StoreContext context)
        {
            this._context = context;
        }

        async public Task<Location> GetById(Guid id)
        {
            return await _context.Locations
                           .Where(loc => loc.LocationId == id)
                           .Select(loc => loc)
                           .SingleOrDefaultAsync();
        }

        IEnumerable<Location> ILocation.GetLocations()
        {
            return (from loc in _context.Locations
                   select loc)
                   .AsEnumerable();
        }

        Location ILocation.GetMostStocked()
        {
            var locationIds = _context.LocationInventories
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

            return _context.Locations
                       .Where(l => l.LocationId == locationWithMostProducts.LocationId)
                       .FirstOrDefault();
        }
        IEnumerable<Tuple<Product, int>> ILocation.GetAllProducts(Location location)
        {
            return from product in _context.Products
                   join li in _context.LocationInventories on product.ProductId equals li.Product.ProductId
                   where li.Location.LocationId == location.LocationId
                   select new Tuple<Product, int>(product, li.Quantity);
        }

        IEnumerable<Tuple<Product, int>> ILocation.GetProductsAvailable(Location location)
        {
            return from product in _context.Products
                   join li in _context.LocationInventories on product.ProductId equals li.Product.ProductId
                   where li.Quantity > 0
                   where li.Location.LocationId == location.LocationId
                   select new Tuple<Product, int>(product, li.Quantity);
        }

        async Task<Location> ILocation.GetById(Guid id)
        {
            return await (from loc in _context.Locations
                         where loc.LocationId == id
                         select loc)
                         .FirstOrDefaultAsync();
        }

        async Task<int> ILocation.GetStock(Location location, Product product)
        {
            return await _context.LocationInventories
                .Where(li => li.Product.ProductId == product.ProductId)
                .Where(li => li.Location.LocationId == location.LocationId)
                .SumAsync(li => li.Quantity);
        }
    }
}