using System;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using System.Linq;
using Microsoft.AspNetCore.Authorization;

using StoreApp.Data;
using StoreApp.FlashMessageExtension;

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

        [Route("Cart")]
        [Authorize(Roles = Auth.Role.Customer)]
        [ServiceFilter(typeof(FlashMessage.FlashMessageFilter))]
        [ServiceFilter(typeof(CartHeader.CartHeaderFilter))]
        public async Task<IActionResult> Index()
        {
            var locationRepo = (Repository.ILocation)this._services.GetService(typeof(Repository.ILocation));
            var customerRepo = (Repository.ICustomer)this._services.GetService(typeof(Repository.ICustomer));
            var orderRepo = (Repository.IOrder)this._services.GetService(typeof(Repository.IOrder));
            var productRepo = (Repository.IProduct)this._services.GetService(typeof(Repository.IProduct));

            var userId = Guid.Parse(HttpContext.User.FindFirst(claim => claim.Type == Auth.Claim.UserId).Value);
            _logger.LogDebug($"customer id={userId}");

            var customer = await customerRepo.GetCustomerById(userId);
            _logger.LogDebug($"customer obj={customer}");
            var location = await customerRepo.GetDefaultLocation(customer);
            _logger.LogDebug($"location obj={location}");

            var currentOrder = await customerRepo.GetOpenOrder(customer, location);
            _logger.LogDebug($"current order obj={currentOrder}");
            var orderLines = orderRepo.GetOrderLines(userId, currentOrder.OrderId);
            _logger.LogDebug($"order lines obj={orderLines}");

            var model = new Model.Input.Cart();

            var allStock = locationRepo.GetStock(currentOrder.OrderId);

            foreach(var item in orderLines)
            {
                _logger.LogDebug($"iterate through item {item.Product.ProductId}");
                var stock = allStock
                    .Where(i => i.Item1 == item.Product.ProductId)
                    .Select(s => s.Item2)
                    .FirstOrDefault();

                var cartItem = new Model.Input.CartItem();
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
        public async Task<IActionResult> AddToCart(Model.Input.CartAdd model)
        {
            var locationRepo = (Repository.ILocation)this._services.GetService(typeof(Repository.ILocation));
            var customerRepo = (Repository.ICustomer)this._services.GetService(typeof(Repository.ICustomer));
            var orderRepo = (Repository.IOrder)this._services.GetService(typeof(Repository.IOrder));
            var productRepo = (Repository.IProduct)this._services.GetService(typeof(Repository.IProduct));

            var userId = Guid.Parse(HttpContext.User.FindFirst(claim => claim.Type == Auth.Claim.UserId).Value);

            var customer = await customerRepo.GetCustomerById(userId);
            var location = await customerRepo.GetDefaultLocation(customer);
            var product = await productRepo.GetProductById(model.ItemId);

            var currentOrder = await customerRepo.GetOpenOrder(customer, location);
            var addStatus = await orderRepo.AddLineItem(userId, currentOrder, product, model.ItemQuantity);
            switch (addStatus)
            {
                case Repository.AddLineItemResult.Ok:
                {
                    _logger.LogTrace($" call add to cart with id {model.ItemId} and quantity {model.ItemQuantity}");
                    var okModel = new Model.View.CartAddOk();
                    okModel.ItemId = model.ItemId;
                    okModel.ItemQuantity = model.ItemQuantity;

                    return RedirectToAction("CartAddOk", "Cart", okModel);
                }
                case Repository.AddLineItemResult.ExceedsStock:
                {
                    this.SetFlashError("Unable to add the item to your order: The amount requested exceeds the amount available in stock.");
                    return Redirect($"/ItemDetail/View/{model.ItemId}");
                }
                case Repository.AddLineItemResult.OrderMissing:
                {
                    this.SetFlashError("There was a problem adding this item to your cart. Please try again.");
                    return Redirect($"/ItemDetail/View/{model.ItemId}");
                }
                case Repository.AddLineItemResult.ProductMissing:
                {
                    this.SetFlashError("There was a problem adding this item to your cart. Please try again.");
                    return Redirect($"/ItemDetail/View/{model.ItemId}");
                }
                default:
                {
                    this.SetFlashError("There was a problem adding this item to your cart. Please try again.");
                    return Redirect($"/ItemDetail/View/{model.ItemId}");
                }
            }

        }

        [Route("Cart/AddOk")]
        [HttpGet]
        [Authorize(Roles = Auth.Role.Customer)]
        [ServiceFilter(typeof(FlashMessage.FlashMessageFilter))]
        [ServiceFilter(typeof(CartHeader.CartHeaderFilter))]
        public IActionResult CartAddOk(Model.View.CartAddOk model)
        {
            return View("CartAddOk", model);
        }

        [Route("Cart/Add")]
        [HttpGet]
        [Authorize(Roles = Auth.Role.Customer)]
        public IActionResult RedirectCartAdd(Model.Input.Cart model)
        {
            return RedirectToAction("Index", "Cart");
        }

        [Route("Cart/Update")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = Auth.Role.Customer)]
        public async Task<IActionResult> Update(Model.Input.Cart model)
        {
            var orderRepo = (Repository.IOrder)this._services.GetService(typeof(Repository.IOrder));
            var customerRepo = (Repository.ICustomer)this._services.GetService(typeof(Repository.ICustomer));

            var userId = Guid.Parse(HttpContext.User.FindFirst(claim => claim.Type == Auth.Claim.UserId).Value);
            var customer = await customerRepo.GetCustomerById(userId);
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
                        var removed = await orderRepo.DeleteLineItem(userId, order, model.Items[(int)removeIndex].Id);
                        if (!removed)
                        {
                            this.SetFlashError("There was an error removing an item from your order. Please try again.");
                            return RedirectToAction("Index", "Cart");
                        }
                    }
                }
                else
                {
                    _logger.LogTrace($"do quantity update");
                    foreach (var i in model.Items)
                    {
                        _logger.LogTrace($"new item info={i.Id}::{i.Quantity}");
                        var updateStatus = await orderRepo.SetLineItemQuantity(userId, order, i.Id, i.Quantity);
                        switch (updateStatus)
                        {
                            case Repository.SetLineItemQuantityResult.ExceedsStock:
                            {
                                this.SetFlashError("Unable to update the quantities in your order: The amount requested exceeds the amount available in stock.");
                                return RedirectToAction("Index", "Cart");
                            }
                            case Repository.SetLineItemQuantityResult.ProductMissing:
                            {
                                this.SetFlashError("There was an error updating the item quantities in your order. Please try again.");
                                return RedirectToAction("Index", "Cart");
                            }
                        }
                    }
                }
            }
            else
            {
                this.SetFlashError("There was an error updating the item quantities in your order. Please try again.");
                return RedirectToAction("Index", "Cart");
            }

            this.SetFlashOk("Items quantities updated successfully.");
            return RedirectToAction("Index", "Cart");
        }

        [Route("Cart/Update")]
        [HttpGet]
        [Authorize(Roles = Auth.Role.Customer)]
        public IActionResult RedirectCartUpdated(Model.Input.Cart model)
        {
            return RedirectToAction("Index", "Cart");
        }
    }
}
