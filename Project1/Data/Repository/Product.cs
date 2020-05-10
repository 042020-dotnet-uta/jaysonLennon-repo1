using System;
using StoreApp.Data;

namespace StoreApp.Repository
{
    public interface IProduct
    {
        bool sup();
    }

    public class Product : IProduct
    {
        private StoreContext _context;

        public Product(StoreContext context)
        {
            this._context = context;
        }

        public bool sup()
        {
            return true;
        }
    }
}