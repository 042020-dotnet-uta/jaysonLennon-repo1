using System;
using System.Linq;
using StoreApp.Data;
using StoreApp.Entity;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace StoreApp.Repository
{
    /// <summary>
    /// Interface for the Product repository.
    /// </summary>
    public interface IProduct
    {
        /// <summary>
        /// Retrieve a product based on its ID.
        /// </summary>
        /// <param name="id">The ID of the product to retrieve.</param>
        /// <returns>The Product, if found.</returns>
        Task<Product> GetProductById(Guid id);
    }

    /// <summary>
    /// Implementation of the IProduct interface.
    /// </summary>
    public class ProductRepository : IProduct
    {
        private StoreContext _context;

        /// <summary>
        /// Standard constructor.
        /// </summary>
        public ProductRepository(StoreContext context)
        {
            this._context = context;
        }

        /// <summary>
        /// Retrieve a product based on its ID.
        /// </summary>
        /// <param name="id">The ID of the product to retrieve.</param>
        /// <returns>The Product, if found.</returns>
        async Task<Product> IProduct.GetProductById(Guid id)
        {
            return await _context.Products
                                 .Where(p => p.ProductId == id)
                                 .Select(p => p)
                                 .SingleOrDefaultAsync();
        }

    }
}