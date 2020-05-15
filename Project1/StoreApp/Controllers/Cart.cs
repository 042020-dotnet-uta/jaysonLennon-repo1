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
            var orderLines = orderRepo.GetOrderLines(customerId, currentOrder.OrderId);
            _logger.LogDebug($"order lines obj={orderLines}");

            var model = new Models.Cart();

            var allStock = locationRepo.GetStock(currentOrder.OrderId);

            foreach(var item in orderLines)
            {
                _logger.LogDebug($"iterate through item {item.Product.ProductId}");
                var stock = allStock
                    .Where(i => i.Item1 == item.Product.ProductId)
                    .Select(s => s.Item2)
                    .FirstOrDefault();

                var cartItem = new Models.CartItem();
                cartItem.Id = item.Product.ProductId;
                cartItem.Name = item.Product.Name;
                cartItem.UnitPrice = item.Product.Price;
                cartItem.Quantity = item.Quantity;
                cartItem.ImageName = item.Product.ImageName;
                cartItem.Stock = stock;
                model.Items.Add(cartItem);
            }

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
            var added = await orderRepo.AddLineItem(customerId, currentOrder, product, model.ItemQuantity);
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

        [Route("Cart/Add")]
        [HttpGet]
        [Authorize(Roles = Auth.Role.Customer)]
        public IActionResult RedirectCartAdd(Models.Cart model)
        {
            return Redirect("/Cart/View");
        }

        [Route("Cart/Update")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = Auth.Role.Customer)]
        public async Task<IActionResult> Update(Models.Cart model)
        {
            var orderRepo = (Repository.IOrder)this._services.GetService(typeof(Repository.IOrder));
            var customerRepo = (Repository.ICustomer)this._services.GetService(typeof(Repository.ICustomer));

            var customerId = Guid.Parse(HttpContext.User.FindFirst(claim => claim.Type == Auth.Claim.UserId).Value);
            var customer = await customerRepo.GetCustomerById(customerId);
            var location = await customerRepo.GetDefaultLocation(customer);

            var order = await customerRepo.GetOpenOrder(customer, location);

            _logger.LogTrace($"update cart");
            if (ModelState.IsValid)
            {
                _logger.LogTrace($"action = {model.Action}");
                _logger.LogTrace($"item count = {model.Items.Count}");

                var removeIndex = model.RemoveIndex();
                if (removeIndex != null)
                {
                    if ((int)removeIndex < model.Items.Count)
                    {
                        _logger.LogTrace($"remove item at index {removeIndex}");
                        var removed = await orderRepo.DeleteLineItem(customerId, order, model.Items[(int)removeIndex].Id);
                        if (!removed)
                        {
                            model.ErrorMessage = "There was an error removing an item from your order. Please try again.";
                            return View("Cart", model);
                        }
                    }
                }
                else
                {
                    _logger.LogTrace($"do quantity update");
                    foreach (var i in model.Items)
                    {
                        _logger.LogTrace($"new item info={i.Id}::{i.Quantity}");
                        var updated = await orderRepo.SetLineItemQuantity(customerId, order, i.Id, i.Quantity);
                        if (!updated)
                        {
                            model.ErrorMessage = "There was an error updating the items quantities in your order. Please try again.";
                            return View("Cart", model);
                        }
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

        [Route("Cart/Update")]
        [HttpGet]
        [Authorize(Roles = Auth.Role.Customer)]
        public IActionResult RedirectCartUpdated(Models.Cart model)
        {
            return Redirect("/Cart/View");
        }
    }
}
