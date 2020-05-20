using System;

namespace StoreApp.Model.View
{
    public class ItemDetail {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string ImageName { get; set; }
        public double UnitPrice { get; set; }
        public bool NotFound { get; set; } = false;
        public int Stock { get; set; }
        public int NumInOrder { get; set; }

        public static ItemDetail ItemNotFound()
        {
            var model = new ItemDetail();
            model.NotFound = true;
            return model;
        }

        public int MaxCanOrder() {
            return NumInOrder == 0 ? Stock : Stock - NumInOrder;
        }
    }
}