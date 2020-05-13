using System;
using System.Linq;
using System.Collections.Generic;
using StoreApp.Data;
using StoreApp.Entity;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace StoreApp.Repository
{
    public interface IProduct
    {
        Task<Product> GetProductById(Guid id);
    }

    public class ProductRepository : IProduct
    {
        private StoreContext _context;

        public ProductRepository(StoreContext context)
        {
            this._context = context;
        }

        async Task<Product> IProduct.GetProductById(Guid id)
        {
            return await _context.Products
                                 .Where(p => p.ProductId == id)
                                 .Select(p => p)
                                 .SingleOrDefaultAsync();
        }

    }
}