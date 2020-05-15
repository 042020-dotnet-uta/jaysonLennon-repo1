using System.Linq;
using System.Collections.Generic;

namespace StoreApp.Model.View
{
    public class Checkout
    {
        /// <summary>
        /// The products that are in the current order.
        /// </summary>
        public List<Model.Input.CartItem> Items { get; set; } = new List<Model.Input.CartItem>();
        public double TotalCost() => Items.Sum(item => item.TotalPrice());
    }
}