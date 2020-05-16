using System;
using System.Collections.Generic;

namespace StoreApp.AdminModel
{
    public class StoreOrderSummaryItem
    {
        public int ItemQuantity { get; set; }
        public double? Price { get; set; }
        public DateTime? TimeSubmitted { get; set; }
        public Guid OrderId { get; set; }
    }

    public class StoreOrderSummary {
        public string StoreName { get; set; }
        public Guid StoreId { get; set; }
        public List<StoreOrderSummaryItem> Items { get; set; } = new List<StoreOrderSummaryItem>();

        public void AddSummaryItem(Entity.Order order, int itemQuantity)
        {
            var summaryItem = new StoreOrderSummaryItem();
            summaryItem.ItemQuantity = itemQuantity;
            summaryItem.Price = order.AmountPaid;
            summaryItem.TimeSubmitted = order.TimeSubmitted;
            summaryItem.OrderId = order.OrderId;

            this.Items.Add(summaryItem);
        }
    }
}