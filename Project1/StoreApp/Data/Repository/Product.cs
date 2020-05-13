using System;
using System.Linq;
using System.Collections.Generic;
using StoreApp.Data;
using StoreApp.Entity;

namespace StoreApp.Repository
{
    public interface IProduct
    {
        Product GetProductById(Guid id);
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

    }
}