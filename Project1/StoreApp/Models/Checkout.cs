using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace StoreApp.Models
{
    public class CheckoutItem
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string ImageName { get; set; }
        public double UnitPrice { get; set; }
        public int Quantity { get; set; }
        public double TotalPrice() => UnitPrice * (double)Quantity;
        
    }

    public class Checkout
    {
        /// <summary>
        /// The products that are in the current order.
        /// </summary>
        public List<CheckoutItem> Items { get; set; } = new List<CheckoutItem>();
        public double TotalCost() => Items.Sum(item => item.TotalPrice());
    }
}