using System;
using System.Collections.Generic;
using StoreApp.Data;
using StoreApp.Entity;

namespace StoreApp.Repository
{
    public interface IProductRepository
    {
        Product GetProductById(Guid id);
        IEnumerable<LocationInventory> GetProductsAvailable(Location location);
    }

    public class ProductRepository : IProductRepository
    {
        private StoreContext _context;

        public ProductRepository(StoreContext context)
        {
            this._context = context;
        }

        Product IProductRepository.GetProductById(Guid id)
        {
            throw new NotImplementedException();
        }

        IEnumerable<LocationInventory> IProductRepository.GetProductsAvailable(Location location)
        {
            throw new NotImplementedException();
        }
    }
}