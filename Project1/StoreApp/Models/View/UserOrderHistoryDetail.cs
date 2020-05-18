using System;
using System.Collections.Generic;

namespace StoreApp.Model.View
{
    public class UserOrderHistoryLine
    {
        public string Name { get; set; }
        public double? AmountCharged { get; set; }
        public int Quantity { get; set; }
        public string ImageName { get; set; }
    }

    public class UserOrderHistoryDetail {
        public Guid OrderId { get; set; }
        public DateTime? TimeSubmitted { get; set; }
        public double? AmountPaid { get; set; }
        public string StoreName { get; set; }
        public List<UserOrderHistoryLine> LineItems { get; set; } = new List<UserOrderHistoryLine>();

        public UserOrderHistoryDetail(Entity.Order order)
        {
            this.OrderId = order.OrderId;
            this.TimeSubmitted = order.TimeSubmitted;
            this.AmountPaid = order.AmountPaid;
            this.StoreName = order.Location.Name;
        }

        public void AddLineItem(Entity.OrderLineItem lineItem)
        {
            var historyLine = new UserOrderHistoryLine();
            historyLine.Name = lineItem.Product.Name;
            historyLine.AmountCharged = lineItem.AmountCharged;
            historyLine.Quantity = lineItem.Quantity;
            historyLine.ImageName = lineItem.Product.ImageName;

            this.LineItems.Add(historyLine);
        }
    }
}