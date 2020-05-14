using System;

namespace StoreApp.Models
{
    public class ItemDetail {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string ImageName { get; set; }
        public double UnitPrice { get; set; }
        public bool NotFound { get; set; } = false;

        public static ItemDetail ItemNotFound()
        {
            var model = new ItemDetail();
            model.NotFound = true;
            return model;
        }
    }
}