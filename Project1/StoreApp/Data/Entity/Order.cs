using System;
using System.Collections.Generic;

namespace StoreApp.Entity
{
    /// <summary>
    /// Represents and order created by the customer.
    /// An <c>Order</c> goes through a lifecycle of: creation, submission, and fulfillment.
    /// <c>Products</c> on the <c>Order</c> are added via adding an <c>OrderLineObject</c> through the
    /// <c>AddLineItem()</c> method.
    /// </summary>
    public class Order
    {
        /// <summary>
        /// ID of the <c>Order</c>.
        /// </summary>
        public Guid OrderId { get; set; }

        /// <summary>
        /// <c>Customer</c> that placed the <c>Order</c>.
        /// </summary>
        public virtual User Customer { get; set; }

        /// <summary>
        /// <c>Location</c> in which the <c>Order</c> was placed.
        /// </summary>
        public virtual Location Location { get; set; }

        /// <summary>
        /// The time this <c>Order</c> was initially created.
        /// </summary>
        public virtual Nullable<DateTime> TimeCreated { get; set; }

        /// <summary>
        /// The time this <c>Order</c> was submitted for fulfillment.
        /// </summary>
        public virtual Nullable<DateTime> TimeSubmitted { get; set; }

        /// <summary>
        /// The time this <c>order</c> was sent out from the store.
        /// </summary>
        public virtual Nullable<DateTime> TimeFulfilled { get; set; }
        
        /// <summary>
        /// The amount the customer paid for this <c>Order</c>.
        /// </summary>
        public Nullable<double> AmountPaid { get; set; }

        /// <summary>
        /// The items that this <c>Order</c> consists of.
        /// </summary>
        public virtual List<OrderLineItem> OrderLineItems { get; set; } = new List<OrderLineItem>();
        
        /// <summary>
        /// Creates a new <c>Order</c> object, setting the creation time to the current time.
        /// </summary>
        public Order(){
            this.OrderId = Guid.NewGuid();
            this.TimeCreated = DateTime.Now;
        }

        /// <summary>
        /// Creates a new <c>Order</c> object.
        /// </summary>
        /// <param name="customer">The <c>Customer</c> that created this Order.</param>
        /// <param name="location">The <c>Location</c> where this <c>Order</c> was placed.</param>
        public Order(User customer, Location location)
        {
            this.OrderId = Guid.NewGuid();
            this.TimeCreated = DateTime.Now;
            this.Customer = customer;
            this.Location = location;
        }

        /// <summary>
        /// Adds a new line item to an <c>Order</c>.
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
    /// <c>Orders</c> are composed of any number of <c>OrderLineItems</c>.
    /// </summary>
    public class OrderLineItem
    {
        /// <summary>
        /// The ID for this <c>OrderLineItem</c>.
        /// </summary>
        public Guid OrderLineItemId { get; set; }

        /// <summary>
        /// The <c>Order</c> this <c>OrderLineItem</c> belongs to.
        /// </summary>
        public virtual Order Order { get; set; }

        /// <summary>
        /// The <c>Product</c> that this <c>OrderLineItem</c> is referring to.
        /// </summary>
        public virtual Product Product { get; set; }

        /// <summary>
        /// The amount charged to the <c>Customer</c> for this <c>OrderLineItem</c>.
        /// </summary>
        public virtual Nullable<double> AmountCharged { get; set; }

        /// <summary>
        /// The total quantity of <c>Product</c> that this line item represents.
        /// </summary>
        public int Quantity { get; set; }

        /// <summary>
        /// Needed for EF.
        /// </summary>
        public OrderLineItem(){}

        /// <summary>
        /// Creates a new <c>OrderLineItem</c>.
        /// </summary>
        /// <param name="order">The <c>Order</c> that this <c>OrderLineItem</c> is associated with.</param>
        /// <param name="product">The <c>Product</c> that this <c>OrderLineItem</c> represents.</param>
        public OrderLineItem(Order order, Product product)
        {
            this.OrderLineItemId = Guid.NewGuid();
            this.Order = order;
            this.Product = product;
        }
    }
}