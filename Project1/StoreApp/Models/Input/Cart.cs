using System;
using System.Linq;
using System.Collections.Generic;

namespace StoreApp.Model.Input
{
    // TODO: constraints
    /// <summary>
    /// Used on the cart view page; represents an item in the cart.
    /// </summary>
    public class CartItem
    {

        /// <summary>ID of the item.</summary>
        public Guid Id { get; set; }

        /// <summary>Name of the item.</summary>
        public string Name { get; set; }


        /// <summary>
        /// Image name of the item
        /// </summary>
        /// <value>Product images will be automatically served from /img/product.</value>
        public string ImageName { get; set; }

        /// <summary>Unit price for the item.</summary>
        public double UnitPrice { get; set; }

        /// <summary>Quantity of the item the user wishes to purchase.</summary>
        public int Quantity { get; set; }

        /// <summary>Total number of this item that are available in stock.</summary>
        public int Stock { get; set; }

        /// <summary>The total price to order Quantity of this item.</summary>
        public double TotalPrice() => UnitPrice * (double)Quantity;
        
    }

    /// <summary>
    /// Used on the "edit cart" page.
    /// </summary>
    public class Cart {

        /// <summary>The list of items in the cart.</summary>
        public List<CartItem> Items { get; set; } = new List<CartItem>();

        /// <summary>
        /// The update action that is being performed by the user when the form
        /// is submitted.
        /// </summary>
        /// <value>
        /// If this is "update", then the item quantities will be updated.
        /// If this is "RemoveItem.i", then the item at index 'i' in Items will be
        /// removed from the order.
        /// </value>
        public string Action { get; set; }

        /// <summary>The total cost for the other.</summary>
        public double TotalCost() => Items.Sum(item => item.TotalPrice());

        /// <summary>
        /// If Action is "RemoveItem.i", return 'i' as an int.
        /// <remarks>
        /// This is used when the user wants to remove an item and will return the item
        /// as an index into the 'Items' list.
        /// </remarks>
        /// </summary>
        /// <returns>Null if this is not a removal action, or an index if so.</returns>
        public Nullable<int> RemoveIndex()
        {
            if (!String.IsNullOrEmpty(Action))
            {
                var components = Action.Split('.', 2);
                if (components.Length == 2)
                {
                    var itemId = 0;
                    var parsed = Int32.TryParse(components[1], out itemId);
                    if (parsed) return itemId;
                }
            }
            return null;
        }
    }
}