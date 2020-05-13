using System;

namespace StoreApp.Models
{
    public class ItemDetail {
        public string Name { get; set; }
        public string ImageName { get; set; }
        public double Price { get; set; }
        public Guid Id { get; set; }
    }
}