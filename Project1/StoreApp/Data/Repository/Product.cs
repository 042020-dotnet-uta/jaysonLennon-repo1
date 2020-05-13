using System;
using System.Collections.Generic;
using StoreApp.Data;
using StoreApp.Entity;

namespace StoreApp.Repository
{
    public interface IProduct
    {
        Product GetProductById(Guid id);
        IEnumerable<LocationInventory> GetProductsAvailable(Location location);
        IEnumerable<Product> GetProducts(Location location);
    }

    public class ProductRepository : IProduct
    {
        private StoreContext _context;

        public ProductRepository(StoreContext context)
        {
            this._context = context;
        }

        Product IProduct.GetProductById(Guid id)
        {
            throw new NotImplementedException();
        }

        IEnumerable<Product> IProduct.GetProducts(Location location)
        {
            throw new NotImplementedException();
        }

        IEnumerable<LocationInventory> IProduct.GetProductsAvailable(Location location)
        {
            throw new NotImplementedException();
        }
    }
}