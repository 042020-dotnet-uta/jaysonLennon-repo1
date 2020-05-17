using System;
using System.Collections.Generic;

namespace StoreApp.Model.View
{
    public class UserOrderHistoryItem {
        public Guid OrderId { get; set; }
        public DateTime? TimeSubmitted { get; set; }
        public double? AmountPaid { get; set; }
        public int ItemQuantity { get; set; }
    }

    public class UserOrderHistory {
        public List<UserOrderHistoryItem> orders { get; set; } = new List<UserOrderHistoryItem>();
        public void AddHistoryItem(Entity.Order order, int itemQuantity)
        {
            // TODO: handle cast error
            var historyItem = new UserOrderHistoryItem();
            historyItem.OrderId = order.OrderId;
            historyItem.TimeSubmitted = order.TimeSubmitted;
            historyItem.AmountPaid = order.AmountPaid;
            historyItem.ItemQuantity = itemQuantity;

            this.orders.Add(historyItem);
        }
    }
}