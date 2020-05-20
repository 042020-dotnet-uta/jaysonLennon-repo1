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
    /// The return value of PlaceOrder()
    /// </summary>
    public enum PlaceOrderResult
    {
        /// <summary>The order was placed successfully.</summary>
        Ok,

        /// <summary>Order rejected because the Location is does not have
        /// enough Product in stock for at least one of the items in the Order.
        /// </summary>
        OutOfStock,

        /// <summary>Order rejected because the order is empty.</summary>
        NoLineItems,

        /// <summary>Order rejected because the order ID does not exist.</summary>
        OrderNotFound,
    }

    /// <summary>
    /// The return value when adding a line item.
    /// </summary>
    public enum AddLineItemResult
    {
        /// <summary>The Line Item was added successfully.</summary>
        Ok,
        /// <summary>There is not enough stock to add the line item.</summary>
        ExceedsStock,
        /// <summary>The line item does not reference a product.</summary>
        ProductMissing,
        /// <summary>Unable to find the order.</summary>
        OrderMissing
    }

    /// <summary>
    /// The return value when setting the line item quantity.
    /// </summary>
    public enum SetLineItemQuantityResult
    {
        /// <summary>The line item quantity was successfully set.</summary>
        Ok,
        /// <summary>There are not enough items in stock to update the quantity.</summary>
        ExceedsStock,
        /// <summary>The product to adjust was not found.</summary>
        ProductMissing,
        /// <summary>Unable to find the order.</summary>
        OrderMissing,
    }

    /// <summary>
    /// Interface for orders.
    /// </summary>
    public interface IOrder
    {
        /// <summary>
        /// Place a new order for a user, given the existing order id.
        /// </summary>
        /// <param name="userId">User ID for the order.</param>
        /// <param name="orderId">The order ID to place.</param>
        /// <returns>PlaceOrderResult detailing the status of the operation.</returns>
        Task<PlaceOrderResult> PlaceOrder(Guid userId, Guid orderId);
        /// <summary>
        /// Retrieve all the submitted orders made by a customer.
        /// <remarks>
        /// The orders returned will only be ones that have been submitted for processing.
        /// Open orders will not be returned.
        /// </remarks>
        /// </summary>
        /// <param name="userId">The user id to query.</param>
        /// <returns>IEnumerable of Tuple containing the Order and quantity of items within the order.</returns>
        IEnumerable<Tuple<Order, int>> GetSubmittedOrders(Guid userId);
        /// <summary>
        /// Retrieve all the other lines within an order.
        /// </summary>
        /// <param name="userId">The user id of the order.</param>
        /// <param name="orderId">The order id to query.</param>
        /// <returns>IEnumerable of OrderLineItem.</returns>
        IEnumerable<OrderLineItem> GetOrderLines(Guid userId, Guid orderId);
        /// <summary>
        /// Removes a line item from an order.
        /// </summary>
        /// <param name="userId">User id of the order.</param>
        /// <param name="order">The order to edit.</param>
        /// <param name="productId">The product ID that should be removed from the order.</param>
        /// <returns>Whether the operation was successful.</returns>
        Task<bool> DeleteLineItem(Guid userId, Order order, Guid productId);
        /// <summary>
        /// Sets the quantity for a line item in an order.
        /// </summary>
        /// <param name="userId">The user ID of the order.</param>
        /// <param name="order">The order ID to be edited.</param>
        /// <param name="productId">The product ID within the order to be adjusted.</param>
        /// <param name="newQuantity">The quantity that the order line should be set to.</param>
        /// <returns>SetLineItemQuantityResult detailing the status of the operation.</returns>
        Task<SetLineItemQuantityResult> SetLineItemQuantity(Guid userId, Order order, Guid productId, int newQuantity);
        /// <summary>
        /// Adds a new line item to an order.
        /// </summary>
        /// <param name="userId">User ID of the order.</param>
        /// <param name="order">Order to edit.</param>
        /// <param name="product">Product to add to the order.</param>
        /// <param name="quantity">Number of product that should be added.</param>
        /// <returns>AddLineItemResult detailing the status of the operation.</returns>
        Task<AddLineItemResult> AddLineItem(Guid userId, Order order, Product product, int quantity);
        /// <summary>
        /// Retrieves an order based on the order ID.
        /// </summary>
        /// <param name="userId">User id of the order.</param>
        /// <param name="orderId">Order ID of the order to be retrieved.</param>
        /// <returns>The Order, if found.</returns>
        Task<Order> GetOrderById(Guid userId, Guid orderId);
        /// <summary>
        /// Gets an order by ID (Admin use only).
        /// </summary>
        /// <param name="orderId">The order id to retrieve.</param>
        /// <returns>The order, if found.</returns>
        Task<Order> AdminGetOrderById(Guid orderId);
        /// <summary>
        /// Determines whether an order ID exists.
        /// </summary>
        /// <param name="orderId">The order ID to check.</param>
        /// <returns>Whether the order exists or not.</returns>
        Task<bool> Exists(Guid orderId);
    }

    public class OrderRepository : IOrder
    {
        private StoreContext _context;

        public OrderRepository(StoreContext context)
        {
            this._context = context;
        }

        /// <summary>
        /// Adds a new line item to an order.
        /// </summary>
        /// <param name="userId">User ID of the order.</param>
        /// <param name="order">Order to edit.</param>
        /// <param name="product">Product to add to the order.</param>
        /// <param name="quantity">Number of product that should be added.</param>
        /// <returns>AddLineItemResult detailing the status of the operation.</returns>
        async Task<AddLineItemResult> IOrder.AddLineItem(Guid userId, Order order, Product product, int quantity)
        {
            if (order == null) return AddLineItemResult.OrderMissing;
            if (product == null) return AddLineItemResult.ProductMissing;

            var inStock = await _context.LocationInventories
                .Where(li => li.Product.ProductId == product.ProductId)
                .Where(li => li.Location.LocationId == order.Location.LocationId)
                .SumAsync(li => li.Quantity);
            
            if (quantity > inStock) return AddLineItemResult.ExceedsStock;

            // Check if this product is already part of the order.
            var currentOrderLine = await _context.OrderLineItems
                .Where(li => li.Product.ProductId == product.ProductId)
                .Where(li => li.Order.OrderId == order.OrderId)
                .Where(li => li.Order.User.UserId == userId)
                .Select(li => li)
                .SingleOrDefaultAsync();

            if (currentOrderLine == null)
            {
                // When the product is not in the order, we will create
                // a new line item.
                var orderLine = new OrderLineItem(order, product);
                orderLine.Quantity = quantity > inStock ? inStock : quantity;
                _context.Add(orderLine);
            }
            else
            {
                // When the product is already in the order, we will adjust
                // the product quantity.
                currentOrderLine.Quantity += quantity;
                if (currentOrderLine.Quantity > inStock)
                {
                    return AddLineItemResult.ExceedsStock;
                }
            }

            await _context.SaveChangesAsync();
            return AddLineItemResult.Ok;
        }

        /// <summary>
        /// Removes a line item from an order.
        /// </summary>
        /// <param name="userId">User id of the order.</param>
        /// <param name="order">The order to edit.</param>
        /// <param name="productId">The product ID that should be removed from the order.</param>
        /// <returns>Whether the operation was successful.</returns>
        async Task<bool> IOrder.DeleteLineItem(Guid userId, Order order, Guid productId)
        {
            if (order == null) return false;
            var lineItem = await _context.OrderLineItems
                                   .Where(li => li.Order.OrderId == order.OrderId)
                                   .Where(li => li.Order.User.UserId == userId)
                                   .Where(li => li.Product.ProductId == productId)
                                   .Select(li => li)
                                   .SingleOrDefaultAsync();

            if (lineItem == null) return false;

            _context.Remove(lineItem);
            await _context.SaveChangesAsync();

            return true;
        }

        /// <summary>
        /// Retrieves an order based on the order ID.
        /// </summary>
        /// <param name="userId">User id of the order.</param>
        /// <param name="orderId">Order ID of the order to be retrieved.</param>
        /// <returns>The Order, if found.</returns>
        async Task<Order> IOrder.GetOrderById(Guid userId, Guid orderId)
        {
            return await _context.Orders
                .Include(o => o.Location)
                .Where(o => o.OrderId == orderId)
                .Where(o => o.User.UserId == userId)
                .SingleOrDefaultAsync();
        }

        /// <summary>
        /// Retrieve all the submitted orders made by a customer.
        /// <remarks>
        /// The orders returned will only be ones that have been submitted for processing.
        /// Open orders will not be returned.
        /// </remarks>
        /// </summary>
        /// <param name="userId">The user id to query.</param>
        /// <returns>IEnumerable of Tuple containing the Order and quantity of items within the order.</returns>
        IEnumerable<Tuple<Order, int>> IOrder.GetSubmittedOrders(Guid userId)
        {
            return _context.Orders
                .Where(o => o.User.UserId == userId)
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

        /// <summary>
        /// Retrieve all the other lines within an order.
        /// </summary>
        /// <param name="userId">The user id of the order.</param>
        /// <param name="orderId">The order id to query.</param>
        /// <returns>IEnumerable of OrderLineItem.</returns>
        IEnumerable<OrderLineItem> IOrder.GetOrderLines(Guid userId, Guid orderId)
        {
            return _context.OrderLineItems
                .Include(ol => ol.Product)
                .Where(ol => ol.Order.OrderId == orderId)
                .Where(ol => ol.Order.User.UserId == userId)
                .Select(ol => ol)
                .OrderBy(ol => ol.Product.Name)
                .AsEnumerable();
        }

        /// <summary>
        /// Place a new order for a user, given the existing order id.
        /// </summary>
        /// <param name="userId">User ID for the order.</param>
        /// <param name="orderId">The order ID to place.</param>
        /// <returns>PlaceOrderResult detailing the status of the operation.</returns>
        async Task<PlaceOrderResult> IOrder.PlaceOrder(Guid userId, Guid orderId)
        {
            var order = await _context.Orders
                .Include(o => o.Location)
                .Include(o => o.OrderLineItems)
                    .ThenInclude(li => li.Product)
                .Where(o => o.OrderId == orderId)
                .Where(o => o.User.UserId == userId)
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

        /// <summary>
        /// Sets the quantity for a line item in an order.
        /// </summary>
        /// <param name="userId">The user ID of the order.</param>
        /// <param name="order">The order ID to be edited.</param>
        /// <param name="productId">The product ID within the order to be adjusted.</param>
        /// <param name="newQuantity">The quantity that the order line should be set to.</param>
        /// <returns>SetLineItemQuantityResult detailing the status of the operation.</returns>
        async Task<SetLineItemQuantityResult> IOrder.SetLineItemQuantity(Guid userId, Order order, Guid productId, int newQuantity)
        {
            if (order == null) return SetLineItemQuantityResult.OrderMissing;

            var lineItem = await _context.OrderLineItems
                .Include(li => li.Product)
                .Where(li => li.Product.ProductId == productId)
                .Where(li => li.Order.OrderId == order.OrderId)
                .Where(li => li.Order.User.UserId == userId)
                .Select(li => li)
                .SingleOrDefaultAsync();
            
            if (lineItem == null) return SetLineItemQuantityResult.ProductMissing;

            int inStock;
            try
            {
                inStock = await _context.LocationInventories
                    .Where(li => li.Product.ProductId == lineItem.Product.ProductId)
                    .Where(li => li.Location.LocationId == order.Location.LocationId)
                    .SumAsync(li => li.Quantity);
            }
            catch (NullReferenceException)
            {
                return SetLineItemQuantityResult.ProductMissing;
            }

            if (lineItem == null) return SetLineItemQuantityResult.ProductMissing;

            if (newQuantity <= 0)
            {
                _context.Remove(lineItem);
            }
            else
            {
                lineItem.Quantity = newQuantity;
                if (lineItem.Quantity > inStock)
                {
                    return SetLineItemQuantityResult.ExceedsStock;
                }
            }

            await _context.SaveChangesAsync();
            return SetLineItemQuantityResult.Ok;
        }

        /// <summary>
        /// Determines whether an order ID exists.
        /// </summary>
        /// <param name="orderId">The order ID to check.</param>
        /// <returns>Whether the order exists or not.</returns>
        async Task<bool> IOrder.Exists(Guid orderId)
        {
            return await _context.Orders
                .Where(o => o.OrderId == orderId)
                .Select(o => o)
                .FirstOrDefaultAsync() != null;
        }

        /// <summary>
        /// Gets an order by ID (Admin use only).
        /// </summary>
        /// <param name="orderId">The order id to retrieve.</param>
        /// <returns>The order, if found.</returns>
        public async Task<Order> AdminGetOrderById(Guid orderId)
        {
            return await _context.Orders
                .Include(o => o.Location)
                .Where(o => o.OrderId == orderId)
                .SingleOrDefaultAsync();
        }
    }
}