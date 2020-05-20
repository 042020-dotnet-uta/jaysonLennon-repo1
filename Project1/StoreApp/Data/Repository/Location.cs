using System.Globalization;
using System;
using System.Collections.Generic;
using StoreApp.Entity;
using StoreApp.Data;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace StoreApp.Repository
{
    /// <summary>
    /// Interface for the location repository.
    /// </summary>
    public interface ILocation
    {
        /// <summary>
        /// Retrieves a Location based on id.
        /// </summary>
        /// <param name="id">The location id</param>
        /// <returns>A Location</returns>
        Task<Location> GetById(Guid id);
        /// <summary>
        /// Retrieves all locations
        /// </summary>
        /// <returns>IEnumerable of Location</returns>
        IEnumerable<Location> GetLocations();
        /// <summary>
        /// Retrieves the Location that has the highest stock of items.
        /// </summary>
        /// <returns>The Location with the highest stock of items.</returns>
        Task<Location> GetMostStocked();
        /// <summary>
        /// Get all products from a specific location that are currently in stock.
        /// </summary>
        /// <param name="location">The location to query.</param>
        /// <returns>IEnumerable of Tuple containing the Product and the total in stock.</returns>
        IEnumerable<Tuple<Product, int>> GetProductsAvailable(Location location);
        /// <summary>
        /// Gets all products from a specific location, regardless of stock.
        /// </summary>
        /// <param name="location">The location to query.</param>
        /// <returns>IEnumerable of Tuple containing the Product and the total in stock.</returns>
        IEnumerable<Tuple<Product, int>> GetAllProducts(Location location);
        /// <summary>
        /// Gets the stock of a specific product from a location.
        /// </summary>
        /// <param name="location">The location to query.</param>
        /// <param name="product">The product to check the stock of.</param>
        /// <returns>Number of stock available for the specific product.</returns>
        Task<int> GetStock(Location location, Product product);
        /// <summary>
        /// Gets the stock from a location based on an order.
        /// <remarks>
        /// This may be used to determine which items have enough stock available for fulfillling an order.
        /// </remarks>
        /// </summary>
        /// <param name="orderId">The order id to query.</param>
        /// <returns>IEnumerable of Tuple containing the product ID and how many are in stock at the location
        /// where the order is placed.</returns>
        IEnumerable<Tuple<Guid, int>> GetStock(Guid orderId);
        /// <summary>
        /// Gets all orders from the specified location.
        /// </summary>
        /// <param name="locationId">Location to query</param>
        /// <returns>IEnumerable of Tuple containing the Order and count of number of items within the order.</returns>
        IEnumerable<Tuple<Order, int>> GetOrders(Guid locationId);
        /// <summary>
        /// Gets all the order line items for a specific order.
        /// </summary>
        /// <param name="orderId">The order to query.</param>
        /// <returns>IEnumerable of order line items.</returns>
        IEnumerable<OrderLineItem> GetOrderLineItems(Guid orderId);
    }

    /// <summary>
    /// Implementation of ILocation.
    /// </summary>
    public class LocationRepository : ILocation
    {
        private StoreContext _context;

        /// <summary>
        /// Standard constructor.
        /// </summary>
        public LocationRepository(StoreContext context)
        {
            this._context = context;
        }

        /// <summary>
        /// Retrieves a Location based on id.
        /// </summary>
        /// <param name="id">The location id</param>
        /// <returns>A Location</returns>
        async public Task<Location> GetById(Guid id)
        {
            return await _context.Locations
                           .Where(loc => loc.LocationId == id)
                           .Select(loc => loc)
                           .SingleOrDefaultAsync();
        }

        /// <summary>
        /// Retrieves all locations
        /// </summary>
        /// <returns>IEnumerable of Location</returns>
        IEnumerable<Location> ILocation.GetLocations()
        {
            return (from loc in _context.Locations
                   select loc)
                   .AsEnumerable();
        }

        /// <summary>
        /// Retrieves the Location that has the highest stock of items.
        /// </summary>
        /// <returns>The Location with the highest stock of items.</returns>
        async Task<Location> ILocation.GetMostStocked()
        {
            var locationWithMostProducts = _context.LocationInventories
                .GroupBy(li => li.Location.LocationId)
                .Select( gr => new {
                    LocationId = gr.Key,
                    Count = gr.Sum(li => li.Quantity)
                })
                .OrderByDescending(gr => gr.Count)
                .FirstOrDefault();

            return await _context.Locations
                       .Where(l => l.LocationId == locationWithMostProducts.LocationId)
                       .FirstOrDefaultAsync();
        }

        /// <summary>
        /// Gets all products from a specific location, regardless of stock.
        /// </summary>
        /// <param name="location">The location to query.</param>
        /// <returns>IEnumerable of Tuple containing the Product and the total in stock.</returns>
        IEnumerable<Tuple<Product, int>> ILocation.GetAllProducts(Location location)
        {
            return from product in _context.Products
                   join li in _context.LocationInventories on product.ProductId equals li.Product.ProductId
                   where li.Location.LocationId == location.LocationId
                   select new Tuple<Product, int>(product, li.Quantity);
        }

        /// <summary>
        /// Get all products from a specific location that are currently in stock.
        /// </summary>
        /// <param name="location">The location to query.</param>
        /// <returns>IEnumerable of Tuple containing the Product and the total in stock.</returns>
        IEnumerable<Tuple<Product, int>> ILocation.GetProductsAvailable(Location location)
        {
            return from product in _context.Products
                   join li in _context.LocationInventories on product.ProductId equals li.Product.ProductId
                   where li.Quantity > 0
                   where li.Location.LocationId == location.LocationId
                   select new Tuple<Product, int>(product, li.Quantity);
        }

        /// <summary>
        /// Gets the stock of a specific product from a location.
        /// </summary>
        /// <param name="location">The location to query.</param>
        /// <param name="product">The product to check the stock of.</param>
        /// <returns>Number of stock available for the specific product.</returns>
        async Task<int> ILocation.GetStock(Location location, Product product)
        {
            return await _context.LocationInventories
                .Where(li => li.Product.ProductId == product.ProductId)
                .Where(li => li.Location.LocationId == location.LocationId)
                .SumAsync(li => li.Quantity);
        }

        /// <summary>
        /// Gets the stock from a location based on an order.
        /// <remarks>
        /// This may be used to determine which items have enough stock available for fulfillling an order.
        /// </remarks>
        /// </summary>
        /// <param name="orderId">The order id to query.</param>
        /// <returns>IEnumerable of Tuple containing the product ID and how many are in stock at the location
        /// where the order is placed.</returns>
        IEnumerable<Tuple<Guid, int>> ILocation.GetStock(Guid orderId)
        {
            var order = _context.Orders
                .Where(o => o.OrderId == orderId)
                .Select(o => o)
                .FirstOrDefault();
            if (order == null) return null;

            return from li in _context.LocationInventories
            join ol in _context.OrderLineItems on li.Product.ProductId equals ol.Product.ProductId
            where li.Location.LocationId == order.Location.LocationId
            select new Tuple<Guid, int>(li.Product.ProductId, li.Quantity);
        }

        /// <summary>
        /// Gets all orders from the specified location.
        /// </summary>
        /// <param name="locationId">Location to query</param>
        /// <returns>IEnumerable of Tuple containing the Order and count of number of items within the order.</returns>
        IEnumerable<Tuple<Order, int>> ILocation.GetOrders(Guid locationId)
        {
            return _context.Orders
                .Where(o => o.Location.LocationId == locationId)
                .Where(o => o.TimeSubmitted != null)
                .Select(o =>
                    new Tuple<Order, int>(
                        o,
                        _context.OrderLineItems
                            .Where(ol => ol.Order.OrderId == o.OrderId)
                            .Sum(ol => ol.Quantity)
                    )
                )
                .AsEnumerable();
        }

        /// <summary>
        /// Gets all the order line items for a specific order.
        /// </summary>
        /// <param name="orderId">The order to query.</param>
        /// <returns>IEnumerable of order line items.</returns>
        IEnumerable<OrderLineItem> ILocation.GetOrderLineItems(Guid orderId)
        {
            return _context.OrderLineItems
                .Include(ol => ol.Product)
                .Where(ol => ol.Order.OrderId == orderId)
                .Select(ol => ol);
        }
    }
}