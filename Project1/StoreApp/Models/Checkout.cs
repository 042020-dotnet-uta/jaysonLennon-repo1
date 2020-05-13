using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace StoreApp.Models
{
    public class Checkout
    {
        /// <summary>
        /// The products that are in the current order.
        /// </summary>
        public List<CartItem> Items { get; set; } = new List<CartItem>();
        public double TotalCost() => Items.Sum(item => item.TotalPrice());
    }
}