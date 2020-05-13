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
        private readonly ILogger<HomeController> _logger;

        public Checkout(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        [Route("Checkout")]
        [Authorize(Roles = Auth.Role.Customer)]
        public IActionResult Index()
        {
            // TODO: Replace dummy data with order information.
            // TODO: Format items
            var cartItem = new Models.CartItem();
            cartItem.Id = Guid.NewGuid();
            cartItem.Name = "item name here (test item)";
            cartItem.UnitPrice = 12.34;
            cartItem.Quantity = 4;

            var cartItem2 = new Models.CartItem();
            cartItem2.Id = Guid.NewGuid();
            cartItem2.Name = "another item";
            cartItem2.UnitPrice = 3.0;
            cartItem2.Quantity = 3;

            var model = new Models.Checkout();
            model.Items.Add(cartItem);
            model.Items.Add(cartItem2);

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
