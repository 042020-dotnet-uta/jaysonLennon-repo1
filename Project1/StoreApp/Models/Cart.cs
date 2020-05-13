using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

namespace StoreApp.Models
{
    // TODO: constraints
    public class CartItem
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string ImageName { get; set; }
        public double UnitPrice { get; set; }
        public int Quantity { get; set; }
        public double TotalPrice() => UnitPrice * (double)Quantity;
        
    }

    public class Cart {
        public List<CartItem> Items { get; set; } = new List<CartItem>();
        public string Action { get; set; }
        public double TotalCost() => Items.Sum(item => item.TotalPrice());
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