using System;
using System.Collections.Generic;

namespace StoreApp.Entity
{
    /// <summary>
    /// Represents and order created by the user.
    /// An Order goes through a lifecycle of: creation, submission, and fulfillment.
    /// Products on the Order are added via adding an OrderLineObject through the
    /// AddLineItem() method.
    /// </summary>
    public class Order
    {
        /// <summary>
        /// ID of the Order.
        /// </summary>
        public Guid OrderId { get; set; }

        /// <summary>
        /// User that placed the Order.
        /// </summary>
        public virtual User User { get; set; }

        /// <summary>
        /// Location in which the Order was placed.
        /// </summary>
        public virtual Location Location { get; set; }

        /// <summary>
        /// The time this Order was initially created.
        /// </summary>
        public virtual Nullable<DateTime> TimeCreated { get; set; }

        /// <summary>
        /// The time this Order was submitted for fulfillment.
        /// </summary>
        public virtual Nullable<DateTime> TimeSubmitted { get; set; }

        /// <summary>
        /// The time this order was sent out from the store.
        /// </summary>
        public virtual Nullable<DateTime> TimeFulfilled { get; set; }
        
        /// <summary>
        /// The amount the user paid for this Order.
        /// </summary>
        public Nullable<double> AmountPaid { get; set; }

        /// <summary>
        /// The items that this Order consists of.
        /// </summary>
        public virtual List<OrderLineItem> OrderLineItems { get; set; } = new List<OrderLineItem>();
        
        /// <summary>
        /// Creates a new Order object, setting the creation time to the current time.
        /// </summary>
        public Order(){
            this.OrderId = Guid.NewGuid();
            this.TimeCreated = DateTime.Now;
        }

        /// <summary>
        /// Creates a new Order object.
        /// </summary>
        /// <param name="user">The User that created this Order.</param>
        /// <param name="location">The Location where this Order was placed.</param>
        public Order(User user, Location location)
        {
            this.OrderId = Guid.NewGuid();
            this.TimeCreated = DateTime.Now;
            this.User = user;
            this.Location = location;
        }

        /// <summary>
        /// Adds a new line item to an Order.
        /// </summary>
        /// <param name="lineItem">The line item to be added.</param>
        public void AddLineItem(OrderLineItem lineItem)
        {
            bool updated = false;
            foreach(var li in this.OrderLineItems)
            {
                if (li.Product == lineItem.Product)
                {
                    li.Quantity += lineItem.Quantity;
                    updated = true;
                    break;
                }
            }

            if (!updated) this.OrderLineItems.Add(lineItem);
        }
    }

    /// <summary>
    /// Represents a single entry in an order.
    /// Orders are composed of any number of OrderLineItems.
    /// </summary>
    public class OrderLineItem
    {
        /// <summary>
        /// The ID for this OrderLineItem.
        /// </summary>
        public Guid OrderLineItemId { get; set; }

        /// <summary>
        /// The Order this OrderLineItem belongs to.
        /// </summary>
        public virtual Order Order { get; set; }

        /// <summary>
        /// The Product that this OrderLineItem is referring to.
        /// </summary>
        public virtual Product Product { get; set; }

        /// <summary>
        /// The amount charged to the User for this OrderLineItem.
        /// </summary>
        public virtual Nullable<double> AmountCharged { get; set; }

        /// <summary>
        /// The total quantity of Product that this line item represents.
        /// </summary>
        public int Quantity { get; set; }

        /// <summary>
        /// Needed for EF.
        /// </summary>
        public OrderLineItem(){}

        /// <summary>
        /// Creates a new OrderLineItem.
        /// </summary>
        /// <param name="order">The Order that this OrderLineItem is associated with.</param>
        /// <param name="product">The Product that this OrderLineItem represents.</param>
        public OrderLineItem(Order order, Product product)
        {
            this.OrderLineItemId = Guid.NewGuid();
            this.Order = order;
            this.Product = product;
        }
    }
}