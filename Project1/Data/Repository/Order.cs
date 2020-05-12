using System;
using System.Collections.Generic;
using StoreApp.Data;
using StoreApp.Entity;

namespace StoreApp.Repository
{
    /// <summary>
    /// The return value of <c>PlaceOrder()</c>
    /// </summary>
    public enum PlaceOrderResult
    {
        /// <summary>The order was placed successfully.</summary>
        Ok,

        /// <summary>Order rejected because the <c>Location</c> is does not have
        /// enough <c>Product</c> in stock for at least one of the items in the <c>Order</c>.
        /// </summary>
        OutOfStock,

        /// <summary>Order rejected because the order is empty.</summary>
        NoLineItems,

        /// <summary>Order rejected because the order ID does not exist.</summary>
        OrderNotFound,

        /// <summary>Order rejected because the quantity of <c>Product</c> is too high.</summary>
        HighQuantityRejection
    }
    public interface IOrder
    {
        PlaceOrderResult PlaceOrder(Product product, Location location, int orderQuantity, int maxAllowed);
        IEnumerable<Order> GetOrderHistory(Customer customer);
        IEnumerable<OrderLineItem> GetOrderLines(Order order);
        bool DeleteLineItem(OrderLineItem orderLine);
        bool SetLineItemQuantity(OrderLineItem orderLine, int newQuantity);
    }

    public class OrderRepository : IOrder
    {
        private StoreContext _context;

        public OrderRepository(StoreContext context)
        {
            this._context = context;
        }

        bool IOrder.DeleteLineItem(OrderLineItem orderLine)
        {
            throw new NotImplementedException();
        }

        IEnumerable<Order> IOrder.GetOrderHistory(Customer customer)
        {
            throw new NotImplementedException();
        }

        IEnumerable<OrderLineItem> IOrder.GetOrderLines(Order order)
        {
            throw new NotImplementedException();
        }

        PlaceOrderResult IOrder.PlaceOrder(Product product, Location location, int orderQuantity, int maxAllowed)
        {
            throw new NotImplementedException();
        }

        bool IOrder.SetLineItemQuantity(OrderLineItem orderLine, int newQuantity)
        {
            throw new NotImplementedException();
        }
    }
}