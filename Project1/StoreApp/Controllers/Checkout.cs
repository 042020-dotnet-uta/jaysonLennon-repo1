using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;

using StoreApp.FlashMessageExtension;

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
        [ServiceFilter(typeof(FlashMessage.FlashMessageFilter))]
        [ServiceFilter(typeof(PageHeader.PopulateHeader))]
        public async Task<IActionResult> Index()
        {
            var userRepo = (Repository.IUser)this._services.GetService(typeof(Repository.IUser));
            var orderRepo = (Repository.IOrder)this._services.GetService(typeof(Repository.IOrder));

            var userId = Guid.Parse(HttpContext.User.FindFirst(claim => claim.Type == Auth.Claim.UserId).Value);
            _logger.LogDebug($"user id={userId}");

            var user = await userRepo.GetUserById(userId);
            _logger.LogDebug($"user obj={user}");
            var location = await userRepo.GetDefaultLocation(user);
            _logger.LogDebug($"location obj={location}");

            var currentOrder = await userRepo.GetOpenOrder(user, location);
            _logger.LogDebug($"current order obj={currentOrder}");
            var orderLines = orderRepo.GetOrderLines(userId, currentOrder.OrderId);

            var model = new Model.View.Checkout();

            foreach(var item in orderLines)
            {
                _logger.LogDebug($"iterate through item {item.Product.ProductId}");
                var checkoutItem = new Model.Input.CartItem();
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
            var userRepo = (Repository.IUser)this._services.GetService(typeof(Repository.IUser));
            var orderRepo = (Repository.IOrder)this._services.GetService(typeof(Repository.IOrder));

            var userId = Guid.Parse(HttpContext.User.FindFirst(claim => claim.Type == Auth.Claim.UserId).Value);
            _logger.LogDebug($"user id={userId}");

            var user = await userRepo.GetUserById(userId);
            _logger.LogDebug($"user obj={user}");
            var location = await userRepo.GetDefaultLocation(user);
            _logger.LogDebug($"location obj={location}");

            var currentOrder = await userRepo.GetOpenOrder(user, location);
            var orderPlaced = await orderRepo.PlaceOrder(userId, currentOrder.OrderId);
            switch (orderPlaced)
            {
                case StoreApp.Repository.PlaceOrderResult.Ok:
                {
                    return RedirectToAction("PlaceOrderOk", "Checkout");
                }
                case StoreApp.Repository.PlaceOrderResult.OutOfStock:
                {
                    this.SetFlashError("Unable to place order: Some items are out of stock.");
                    return RedirectToAction("PlaceOrderError", "Checkout");
                }
                case StoreApp.Repository.PlaceOrderResult.NoLineItems:
                {
                    this.SetFlashError("Unable to place order: There are no items in your order.");
                    return RedirectToAction("PlaceOrderError", "Checkout");
                }
                case StoreApp.Repository.PlaceOrderResult.OrderNotFound:
                {
                    this.SetFlashError("Unable to place order: No order was found.");
                    return RedirectToAction("PlaceOrderError", "Checkout");
                }
                default:
                {
                    this.SetFlashError("Unable to place order. Please try again.");
                    return RedirectToAction("PlaceOrderError", "Checkout");
                }

            }
        }

        [Route("Checkout/PlaceOrder")]
        [HttpGet]
        public IActionResult RedirectPlaceOrder()
        {
            return Redirect("/Checkout");
        }

        [Route("Checkout/PlaceOrderOk")]
        [HttpGet]
        [ServiceFilter(typeof(FlashMessage.FlashMessageFilter))]
        [ServiceFilter(typeof(PageHeader.PopulateHeader))]
        public IActionResult PlaceOrderOk()
        {
            return View("PlaceOrderOk");
        }

        [Route("Checkout/PlaceOrderError")]
        [HttpGet]
        [ServiceFilter(typeof(FlashMessage.FlashMessageFilter))]
        [ServiceFilter(typeof(PageHeader.PopulateHeader))]
        public IActionResult PlaceOrderError()
        {
            return View("PlaceOrderError");
        }
    }
}