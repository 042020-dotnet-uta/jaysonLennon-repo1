using System;

namespace StoreApp.Entity
{
    /// <summary>
    /// Represents a store location and consists of the name and address of the location.
    /// </summary>
    public class Location
    {
        /// <summary>
        /// The ID for this Location object.
        /// </summary>
        public Guid LocationId { get; set; }

        /// <summary>
        /// The name of the Location.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The address for this Location.
        /// </summary>
        public virtual Address Address { get; set; }

        /// <summary>
        /// Needed for EF.
        /// </summary>
        public Location() { }

        /// <summary>
        /// Create a new Location object.
        /// </summary>
        /// <param name="name">The name of the Location.</param>
        public Location(string name)
        {
            this.LocationId = Guid.NewGuid();
            this.Name = name;
        }
    }

    /// <summary>
    /// Represents the current inventory for a Product at a specific Location.
    /// </summary>
    public class LocationInventory
    {
        /// <summary>
        /// The ID for this LocationInventory object.
        /// </summary>
        public Guid LocationInventoryId { get; set; }

        /// <summary>
        /// The Product associated with this LocationInventory object.
        /// </summary>
        public virtual Product Product { get; set; }

        /// <summary>
        /// The Location where this LocationInventory resides.
        /// </summary>
        public virtual Location Location { get; set; }

        /// <summary>
        /// The Quantity of Product available at this Location.
        /// </summary>
        public int Quantity { get; set; }

        /// <summary>
        /// Needed for EF.
        /// </summary>
        public LocationInventory() { }

        /// <summary>
        /// Create a new LocationInventory object
        /// </summary>
        /// <param name="product">The Product associated with this LocationInventory object.</param>
        /// <param name="location">The Location associated with thie LocationInventory object.</param>
        /// <param name="quantity">The Quantity of Product represented by the LocationInventory object.</param>
        public LocationInventory(Product product, Location location, int quantity)
        {
            this.LocationInventoryId = Guid.NewGuid();
            this.Product = product;
            this.Location = location;
            this.Quantity = quantity;
        }

        /// <summary>
        /// Attempts to adjust the quantity of this LocationInventory object.
        /// If the function returns a value, then the inventory adjustment has taken place.
        /// If a null is returned, then no adjustment has taken place.
        /// <param name="amount">The adjustment to make.</param>
        /// <returns>A value indicating the amount of Product remaining in the inventory
        /// for this Location.
        /// A null value indicates that Location does not have enough items in stock to 
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