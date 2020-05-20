using System;
using System.Collections.Generic;

namespace StoreApp.AdminModel
{
    /// <summary>
    /// A row in the order summary table.
    /// </summary>
    public class StoreOrderSummaryItem
    {
        /// <summary>The quantity of items in the entire order.</summary>
        public int ItemQuantity { get; set; }

        /// <summary>The total price paid for the order.</summary>
        public double? Price { get; set; }

        /// <summary>The time the order was submitted.</summary>
        public DateTime? TimeSubmitted { get; set; }

        /// <summary>The other ID.</summary>
        public Guid OrderId { get; set; }
    }

    public class StoreOrderSummary {
        /// <summary>The name of the store.</summary>
        public string StoreName { get; set; }

        /// <summary>The store ID.</summary>
        public Guid StoreId { get; set; }

        /// <summary>List of orders to be displayed.</summary>
        public List<StoreOrderSummaryItem> Items { get; set; } = new List<StoreOrderSummaryItem>();

        /// <summary>
        /// Adds a new item to the order list.
        /// </summary>
        /// <param name="order">The order to be added.</param>
        /// <param name="itemQuantity">The quantity of items in the order.</param>
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