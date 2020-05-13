using System;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Text.Encodings.Web;
using Microsoft.Extensions.Logging;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using System.Collections.Generic;
using System.Security.Claims;

using StoreApp.Data;

namespace StoreApp.Controllers
{
    public class Cart : Controller
    {
        private StoreContext _context;
        private ILogger<Cart> _logger;
        private IServiceProvider _services;

        public Cart(
            StoreContext context,
            ILogger<Cart> logger,
            IServiceProvider services
            )
        {
            this._context = context;
            this._logger = logger;
            this._services = services;
        }

        [Route("Cart/View")]
        [Authorize(Roles = Auth.Role.Customer)]
        public async Task<IActionResult> Index()
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

            var model = new Models.Cart();
            model.Items.Add(cartItem);
            model.Items.Add(cartItem2);

            return View("Cart", model);
        }

        [Route("Cart/Add")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = Auth.Role.Customer)]
        public async Task<IActionResult> AddToCart(Models.CartAdd model)
        {
            var locationRepo = (Repository.ILocation)this._services.GetService(typeof(Repository.ILocation));
            var customerRepo = (Repository.ICustomer)this._services.GetService(typeof(Repository.ICustomer));
            var orderRepo = (Repository.IOrder)this._services.GetService(typeof(Repository.IOrder));
            var productRepo = (Repository.IProduct)this._services.GetService(typeof(Repository.IProduct));

            var customerId = Guid.Parse(HttpContext.User.FindFirst(claim => claim.Type == Auth.Claim.UserId).Value);

            var customer = await customerRepo.GetCustomerById(customerId);
            var location = await customerRepo.GetDefaultLocation(customer);
            var product = await productRepo.GetProductById(model.ItemId);

            var currentOrder = await customerRepo.GetOpenOrder(customer, location);
            var added = await orderRepo.AddLineItem(currentOrder, product, model.ItemQuantity);
            if (!added)
            {
                return View("CartAddError");
            }

            _logger.LogTrace($" call add to cart with id {model.ItemId} and quantity {model.ItemQuantity}");
            var okModel = new Models.CartAddOk();
            okModel.ItemId = model.ItemId;
            okModel.ItemQuantity = model.ItemQuantity;
            return View("CartAddOk", okModel);
        }

        [Route("Cart/Update")]
        [HttpGet]
        [Authorize(Roles = Auth.Role.Customer)]
        public IActionResult UpdateRedirect(Models.Cart model)
        {
            return Redirect("/Cart/View");
        }

        [Route("Cart/Update")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = Auth.Role.Customer)]
        public IActionResult Update(Models.Cart model)
        {
            _logger.LogTrace($"update cart");
            if (ModelState.IsValid)
            {
                _logger.LogTrace($"action = {model.Action}");
                _logger.LogTrace($"item count = {model.Items.Count}");

                var removeIndex = model.RemoveIndex();
                if (removeIndex != null)
                {
                    // TODO: Implement 'remove item' functionality.
                    _logger.LogTrace($"remove item at index {removeIndex}");
                }
                else
                {
                    // TODO: Implement 'update item quantities' functionality.
                    _logger.LogTrace($"do quantity update");
                    foreach (var i in model.Items)
                    {
                        _logger.LogTrace($"new item info={i.Id}::{i.Quantity}");
                    }
                }
            }
            else
            {
                // TODO: display error on broken model
                _logger.LogTrace($"broken model");
            }
            return Redirect("/Cart/View");
        }

        [Route("Cart/RemoveItem")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = Auth.Role.Customer)]
        public IActionResult RemoveItem(Guid itemId)
        {
            // TODO: Implement 'update cart' functionality.
            _logger.LogTrace($"remove item {itemId}");
            return Redirect("/Cart/View");
        }
    }
}
