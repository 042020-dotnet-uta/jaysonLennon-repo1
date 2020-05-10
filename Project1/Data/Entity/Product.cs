using System;

namespace StoreApp.Data.Entity
{
    /// <summary>
    /// Contains various product information such as name and price.
    /// </summary>
    public class Product
    {
        /// <summary>
        /// The Product's ID.
        /// </summary>
        public Guid ProductId { get; set; }

        /// <summary>
        /// The Product's price.
        /// </summary>
        public double Price { get; set; }

        /// <summary>
        /// The product's Name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Needed for EF.
        /// </summary>
        public Product(){}

        /// <summary>
        /// Create a new <c>Product</c> with a given name and price.
        /// </summary>
        /// <param name="name">Name of the <c>Product</c>.</param>
        /// <param name="price">Price of the <c>Product</c>.</param>
        public Product(string name, double price) {
            this.ProductId = Guid.NewGuid();
            this.Name = name;
            this.Price = price;
        }
    }
}