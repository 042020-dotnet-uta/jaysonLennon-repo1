using System;

namespace StoreApp.Models
{
    public class CartAddOk {
        public Guid ItemId { get; set; }
        public string ItemName { get; set; }
        public string ItemImageName { get; set; }
        public double ItemUnitPrice { get; set; }
        public int ItemQuantity { get; set; }
    }
}