using System;
using System.Collections.Generic;

namespace StoreApp.Models
{
    public class CustomerOrderHistoryItem {
        public Guid OrderId { get; set; }
        public DateTime? TimeSubmitted { get; set; }
        public double? AmountPaid { get; set; }
        public int ItemQuantity { get; set; }
    }

    public class CustomerOrderHistory {
        public List<CustomerOrderHistoryItem> orders { get; set; } = new List<CustomerOrderHistoryItem>();
        public void AddHistoryItem(Entity.Order order, int itemQuantity)
        {
            // TODO: handle cast error
            var historyItem = new CustomerOrderHistoryItem();
            historyItem.OrderId = order.OrderId;
            historyItem.TimeSubmitted = order.TimeSubmitted;
            historyItem.AmountPaid = order.AmountPaid;
            historyItem.ItemQuantity = itemQuantity;

            this.orders.Add(historyItem);
        }
    }
}