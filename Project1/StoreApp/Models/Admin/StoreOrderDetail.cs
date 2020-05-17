using System;
using System.Collections.Generic;

namespace StoreApp.AdminModel
{
    public class StoreOrderDetailItem
    {
        public string Name { get; set; }
        public int Quantity { get; set; }
        public double? Price { get; set; }
    }

    public class StoreOrderDetail {
        public double TotalPrice { get; set; }
        public int TotalItems { get; set; }
        public List<StoreOrderDetailItem> Items { get; set; }

        public void AddDetailItem(Entity.OrderLineItem orderLine)
        {
            if (this.Items == null) this.Items = new List<StoreOrderDetailItem>();

            var detailItem = new StoreOrderDetailItem();
            detailItem.Name = orderLine.Product.Name;
            detailItem.Quantity = orderLine.Quantity;
            detailItem.Price = orderLine.AmountCharged;

            this.TotalPrice += orderLine.AmountCharged ?? 0.0;
            this.TotalItems += orderLine.Quantity;
            this.Items.Add(detailItem);
        }
    }
}