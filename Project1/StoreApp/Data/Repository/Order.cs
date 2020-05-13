using System;
using System.Linq;
using System.Collections.Generic;
using StoreApp.Data;
using StoreApp.Entity;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

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
        Task<bool> AddLineItem(Order order, Product product, int quantity);
    }

    public class OrderRepository : IOrder
    {
        private StoreContext _context;

        public OrderRepository(StoreContext context)
        {
            this._context = context;
        }

        async Task<bool> IOrder.AddLineItem(Order order, Product product, int quantity)
        {
            if (order == null || product == null) return false;

            // Check if this product is already part of the order.
            var currentOrderLine = await _context.OrderLineItems
                .Where(li => li.Product.ProductId == product.ProductId)
                .Where(li => li.Order.OrderId == order.OrderId)
                .Select(li => li)
                .SingleOrDefaultAsync();

            if (currentOrderLine == null)
            {
                // When the product is not in the order, we will create
                // a new line item.
                var orderLine = new OrderLineItem(order, product);
                orderLine.Quantity = quantity;
                _context.Add(orderLine);
            }
            else
            {
                // When the product is already in the order, we will adjust
                // the product quantity.
                var orderLine = currentOrderLine.Quantity += quantity;
            }
            await _context.SaveChangesAsync();

            return true;
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