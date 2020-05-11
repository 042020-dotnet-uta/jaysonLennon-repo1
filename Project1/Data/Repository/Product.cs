using System;
using StoreApp.Data;

namespace StoreApp.Repository
{
    public interface IProductRepository
    {
        bool sup();
    }

    public class ProductRepository : IProductRepository
    {
        private StoreContext _context;

        public ProductRepository(StoreContext context)
        {
            this._context = context;
        }

        public bool sup()
        {
            return true;
        }
    }
}