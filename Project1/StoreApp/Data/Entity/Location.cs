using System;

namespace StoreApp.Entity
{
    /// <summary>
    /// Represents a store location and consists of the name and address of the location.
    /// </summary>
    public class Location
    {
        /// <summary>
        /// The ID for this <c>Location</c> object.
        /// </summary>
        public Guid LocationId { get; set; }

        /// <summary>
        /// The name of the <c>Location</c>.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The address for this <c>Location</c>.
        /// </summary>
        public virtual Address Address { get; set; }

        /// <summary>
        /// Needed for EF.
        /// </summary>
        public Location() { }

        /// <summary>
        /// Create a new <c>Location</c> object.
        /// </summary>
        /// <param name="name">The name of the <c>Location</c>.</param>
        public Location(string name)
        {
            this.LocationId = Guid.NewGuid();
            this.Name = name;
        }
    }

    /// <summary>
    /// Represents the current inventory for a <c>Product</c> at a specific <c>Location</c>.
    /// </summary>
    public class LocationInventory
    {
        /// <summary>
        /// The ID for this <c>LocationInventory</c> object.
        /// </summary>
        public Guid LocationInventoryId { get; set; }

        /// <summary>
        /// The <c>Product</c> associated with this <c>LocationInventory</c> object.
        /// </summary>
        public virtual Product Product { get; set; }

        /// <summary>
        /// The <c>Location</c> where this <c>LocationInventory</c> resides.
        /// </summary>
        public virtual Location Location { get; set; }

        /// <summary>
        /// The Quantity of <c>Product</c> available at this <c>Location</c>.
        /// </summary>
        public int Quantity { get; set; }

        /// <summary>
        /// Needed for EF.
        /// </summary>
        public LocationInventory() { }

        /// <summary>
        /// Create a new <c>LocationInventory</c> object
        /// </summary>
        /// <param name="product">The <c>Product</c> associated with this <c>LocationInventory</c> object.</param>
        /// <param name="location">The <c>Location</c> associated with thie <c>LocationInventory</c> object.</param>
        /// <param name="quantity">The Quantity of <c>Product</c> represented by the <c>LocationInventory</c> object.</param>
        public LocationInventory(Product product, Location location, int quantity)
        {
            this.LocationInventoryId = Guid.NewGuid();
            this.Product = product;
            this.Location = location;
            this.Quantity = quantity;
        }

        /// <summary>
        /// Attempts to adjust the quantity of this <c>LocationInventory</c> object.
        /// If the function returns a value, then the inventory adjustment has taken place.
        /// If a null is returned, then no adjustment has taken place.
        /// <param name="amount">The adjustment to make.</param>
        /// <returns>A value indicating the amount of <c>Product</c> remaining in the inventory
        /// for this <c>Location</c>.
        /// A null value indicates that <c>Location</c> does not have enough items in stock to 
        /// make the adjustment.</returns>
        public Nullable<int> TryAdjustQuantity(int amount)
        {
            if (this.Quantity + amount < 0)
            {
                return null;
            }
            else
            {
                this.Quantity += amount;
                return this.Quantity;
            }
        }
    }
}