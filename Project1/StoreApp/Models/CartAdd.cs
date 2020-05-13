using System;

namespace StoreApp.Models
{
    public class CartAdd {
        public Guid ItemId { get; set; }
        public int ItemQuantity { get; set; }
    }
}