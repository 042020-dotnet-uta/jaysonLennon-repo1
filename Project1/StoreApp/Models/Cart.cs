using System;
using System.Linq;
using System.Collections.Generic;

namespace StoreApp.Models
{
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
        public double TotalCost() => Items.Sum(item => item.TotalPrice());
    }
}