using System;
using System.Collections.Generic;

namespace StoreApp.AdminModel
{
    /// <summary>
    /// A row in the order detail item list.
    /// </summary>
    public class StoreOrderDetailItem
    {
        /// <summary>The name of the item.</summary>
        public string Name { get; set; }

        /// <summary>The quantity purchased.</summary>
        public int Quantity { get; set; }

        /// <summary>The total price paid for this specific item.</summary>
        public double? Price { get; set; }
    }

    /// <summary>
    /// Detail page information for an order.
    /// </summary>
    public class StoreOrderDetail {
        /// <summary>Total price paid for the entire order.</summary>
        public double TotalPrice { get; set; }

        /// <summary>Total items in the order.</summary>
        public int TotalItems { get; set; }

        /// <summary>The time the order was submitted.</summary>
        public DateTime? TimeSubmitted { get; set; }

        /// <summary>List of items present in the order.</summary>
        public List<StoreOrderDetailItem> Items { get; set; }

        /// <summary>
        /// Adds a new item detail line.
        /// </summary>
        /// <param name="orderLine">The order line to be added.</param>
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