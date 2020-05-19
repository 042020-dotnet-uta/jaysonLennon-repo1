using System;

namespace StoreApp.Model.Input
{
    /// <summary>
    /// Data needed to add an item to the cart.
    /// </summary>
    public class CartAdd {
        /// <summary>The item ID to add.</summary>
        public Guid ItemId { get; set; }

        /// <summary>The quantity of items to add.</summary>
        public int ItemQuantity { get; set; }
    }
}