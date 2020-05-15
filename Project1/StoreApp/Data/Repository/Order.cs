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
    }
    public interface IOrder
    {
        Task<PlaceOrderResult> PlaceOrder(Guid customerId, Guid orderId);
        IEnumerable<Tuple<Order, int>> GetSubmittedOrders(Guid customerId);
        IEnumerable<OrderLineItem> GetOrderLines(Guid customerId, Guid orderId);
        Task<bool> DeleteLineItem(Guid customerId, Order order, Guid productId);
        Task<bool> SetLineItemQuantity(Guid customerId, Order order, Guid productId, int newQuantity);
        Task<bool> AddLineItem(Guid customerId, Order order, Product product, int quantity);
        Task<Order> GetOrderById(Guid customerId, Guid orderId);
    }

    public class OrderRepository : IOrder
    {
        private StoreContext _context;

        public OrderRepository(StoreContext context)
        {
            this._context = context;
        }

        async Task<bool> IOrder.AddLineItem(Guid customerId, Order order, Product product, int quantity)
        {
            if (order == null || product == null) return false;

            // Check if this product is already part of the order.
            var currentOrderLine = await _context.OrderLineItems
                .Where(li => li.Product.ProductId == product.ProductId)
                .Where(li => li.Order.OrderId == order.OrderId)
                .Where(li => li.Order.Customer.CustomerId == customerId)
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

        async Task<bool> IOrder.DeleteLineItem(Guid customerId, Order order, Guid productId)
        {
            var lineItem = await _context.OrderLineItems
                                   .Where(li => li.Order.OrderId == order.OrderId)
                                   .Where(li => li.Order.Customer.CustomerId == customerId)
                                   .Where(li => li.Product.ProductId == productId)
                                   .Select(li => li)
                                   .SingleOrDefaultAsync();

            if (lineItem == null) return false;

            _context.Remove(lineItem);
            await _context.SaveChangesAsync();

            return true;
        }

        async Task<Order> IOrder.GetOrderById(Guid customerId, Guid orderId)
        {
            return await _context.Orders
                .Include(o => o.Location)
                .Where(o => o.OrderId == orderId)
                .Where(o => o.Customer.CustomerId == customerId)
                .SingleOrDefaultAsync();
        }

        IEnumerable<Tuple<Order, int>> IOrder.GetSubmittedOrders(Guid customerId)
        {
            Console.WriteLine($"getting orders for {customerId}");
            return _context.Orders
                .Where(o => o.Customer.CustomerId == customerId)
                .Where(o => o.TimeSubmitted != null)
                .Select(o =>
                    new Tuple<Order, int>(
                        o,
                        o.OrderLineItems
                            .Where(li => li.Order.OrderId == o.OrderId)
                            .Sum(li => li.Quantity)
                    )
                )
                .AsEnumerable();
        }

        IEnumerable<OrderLineItem> IOrder.GetOrderLines(Guid customerId, Guid orderId)
        {
            return _context.OrderLineItems
                .Include(ol => ol.Product)
                .Where(ol => ol.Order.OrderId == orderId)
                .Where(ol => ol.Order.Customer.CustomerId == customerId)
                .Select(ol => ol)
                .OrderBy(ol => ol.Product.Name)
                .AsEnumerable();
        }

        async Task<PlaceOrderResult> IOrder.PlaceOrder(Guid customerId, Guid orderId)
        {
            var order = await _context.Orders
                .Include(o => o.Location)
                .Include(o => o.OrderLineItems)
                    .ThenInclude(li => li.Product)
                .Where(o => o.OrderId == orderId)
                .Where(o => o.Customer.CustomerId == customerId)
                .SingleOrDefaultAsync();

            if (order == null) return PlaceOrderResult.OrderNotFound;
            if (order.OrderLineItems.Count() == 0) return PlaceOrderResult.NoLineItems;

            var totalOrderPrice = 0.0;
            var totalItemQuantity = 0;

            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                foreach (var lineItem in order.OrderLineItems)
                {
                    var locationInventory = await _context.LocationInventories
                        .Where(i => i.Product.ProductId == lineItem.Product.ProductId)
                        .Where(i => i.Location.LocationId == order.Location.LocationId)
                        .SingleOrDefaultAsync();

                    if (locationInventory == null) return PlaceOrderResult.OutOfStock;
                    var newStockQuantity = locationInventory.TryAdjustQuantity(-lineItem.Quantity);
                    if (newStockQuantity == null)
                    {
                        transaction.Rollback();
                        return PlaceOrderResult.OutOfStock;
                    }

                    var lineItemPrice = lineItem.Quantity * lineItem.Product.Price;
                    totalOrderPrice += lineItemPrice;
                    lineItem.AmountCharged = lineItemPrice;
                    totalItemQuantity += lineItem.Quantity;
                    await _context.SaveChangesAsync();
                }

                order.AmountPaid = totalOrderPrice;
                order.TimeSubmitted = DateTime.Now;
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            return PlaceOrderResult.Ok;
        }

        async Task<bool> IOrder.SetLineItemQuantity(Guid customerId, Order order, Guid productId, int newQuantity)
        {
            var lineItem = await _context.OrderLineItems
                .Where(li => li.Product.ProductId == productId)
                .Where(li => li.Order.OrderId == order.OrderId)
                .Where(li => li.Order.Customer.CustomerId == customerId)
                .Select(li => li)
                .SingleOrDefaultAsync();

            if (lineItem == null) return false;

            if (newQuantity <= 0)
            {
                _context.Remove(lineItem);
            }
            else
            {
                lineItem.Quantity = newQuantity;

            }
            await _context.SaveChangesAsync();

            return true;
        }

    }
}