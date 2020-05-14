using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using StoreApp.Models;
using Microsoft.AspNetCore.Authorization;

namespace StoreApp.Controllers
{
    public class Checkout : Controller
    {
        private readonly ILogger<Checkout> _logger;
        private IServiceProvider _services;

        public Checkout(
            ILogger<Checkout> logger,
            IServiceProvider services
            )
        {
            this._logger = logger;
            this._services = services;
        }

        [Route("Checkout")]
        [Authorize(Roles = Auth.Role.Customer)]
        public async Task<IActionResult> Index()
        {
            var locationRepo = (Repository.ILocation)this._services.GetService(typeof(Repository.ILocation));
            var customerRepo = (Repository.ICustomer)this._services.GetService(typeof(Repository.ICustomer));
            var orderRepo = (Repository.IOrder)this._services.GetService(typeof(Repository.IOrder));
            var productRepo = (Repository.IProduct)this._services.GetService(typeof(Repository.IProduct));

            var customerId = Guid.Parse(HttpContext.User.FindFirst(claim => claim.Type == Auth.Claim.UserId).Value);
            _logger.LogDebug($"customer id={customerId}");

            var customer = await customerRepo.GetCustomerById(customerId);
            _logger.LogDebug($"customer obj={customer}");
            var location = await customerRepo.GetDefaultLocation(customer);
            _logger.LogDebug($"location obj={location}");

            var currentOrder = await customerRepo.GetOpenOrder(customer, location);
            _logger.LogDebug($"current order obj={currentOrder}");
            var orderLines = orderRepo.GetOrderLines(currentOrder);

            var model = new Models.Checkout();

            foreach(var item in orderLines)
            {
                _logger.LogDebug($"iterate through item {item.Product.ProductId}");
                var checkoutItem = new Models.CartItem();
                checkoutItem.Id = item.Product.ProductId;
                checkoutItem.Name = item.Product.Name;
                checkoutItem.UnitPrice = item.Product.Price;
                checkoutItem.Quantity = item.Quantity;
                checkoutItem.ImageName = item.Product.ImageName;
                model.Items.Add(checkoutItem);
            }

            return View("Checkout", model);
        }

        [Route("Checkout/PlaceOrder")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = Auth.Role.Customer)]
        public IActionResult PlaceOrder()
        {
            // TODO: place order
            // TODO: return order status page based on outcome
            _logger.LogTrace($"call placeorder");
            return View("PlaceOrderOk");
        }
    }
}
