using System;

namespace StoreApp.Model.Input
{
    public class CartAdd {
        public Guid ItemId { get; set; }
        public int ItemQuantity { get; set; }
    }
}