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
            var customerRepo = (Repository.ICustomer)this._services.GetService(typeof(Repository.ICustomer));
            var orderRepo = (Repository.IOrder)this._services.GetService(typeof(Repository.IOrder));

            var customerId = Guid.Parse(HttpContext.User.FindFirst(claim => claim.Type == Auth.Claim.UserId).Value);
            _logger.LogDebug($"customer id={customerId}");

            var customer = await customerRepo.GetCustomerById(customerId);
            _logger.LogDebug($"customer obj={customer}");
            var location = await customerRepo.GetDefaultLocation(customer);
            _logger.LogDebug($"location obj={location}");

            var currentOrder = await customerRepo.GetOpenOrder(customer, location);
            _logger.LogDebug($"current order obj={currentOrder}");
            var orderLines = orderRepo.GetOrderLines(customerId, currentOrder.OrderId);

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
        public async Task<IActionResult> PlaceOrder()
        {
            _logger.LogTrace($"call placeorder");
            var customerRepo = (Repository.ICustomer)this._services.GetService(typeof(Repository.ICustomer));
            var orderRepo = (Repository.IOrder)this._services.GetService(typeof(Repository.IOrder));

            var customerId = Guid.Parse(HttpContext.User.FindFirst(claim => claim.Type == Auth.Claim.UserId).Value);
            _logger.LogDebug($"customer id={customerId}");

            var customer = await customerRepo.GetCustomerById(customerId);
            _logger.LogDebug($"customer obj={customer}");
            var location = await customerRepo.GetDefaultLocation(customer);
            _logger.LogDebug($"location obj={location}");

            var currentOrder = await customerRepo.GetOpenOrder(customer, location);
            var orderPlaced = await orderRepo.PlaceOrder(customerId, currentOrder.OrderId);
            switch (orderPlaced)
            {
                case StoreApp.Repository.PlaceOrderResult.Ok:
                {
                    return View("PlaceOrderOk");
                }
                case StoreApp.Repository.PlaceOrderResult.OutOfStock:
                {
                    return View("PlaceOrderFailOutOfStock");
                }
                case StoreApp.Repository.PlaceOrderResult.NoLineItems:
                {
                    return View("PlaceOrderFailNoLineItems");
                }
                case StoreApp.Repository.PlaceOrderResult.OrderNotFound:
                {
                    return View("PlaceOrderFailOrderNotFound");
                }
                default:
                {
                    return View("PlaceOrderFail");
                }

            }
        }

        [Route("Checkout/PlaceOrder")]
        [HttpGet]
        public IActionResult RedirectPlaceOrder()
        {
            return Redirect("/Checkout");
        }
    }
}