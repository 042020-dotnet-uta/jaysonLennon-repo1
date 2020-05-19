using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;

using StoreApp.FlashMessageExtension;

namespace StoreApp.Controllers
{
    /// <summary>
    /// Controller to handle checkout operations.
    /// </summary>
    public class Checkout : Controller
    {
        private readonly ILogger<Checkout> _logger;
        private IServiceProvider _services;

        /// <summary>
        /// Standard constructor.
        /// </summary>
        public Checkout(
            ILogger<Checkout> logger,
            IServiceProvider services
            )
        {
            this._logger = logger;
            this._services = services;
        }

        /// <summary>
        /// Route to display the checkout page.
        /// </summary>
        [Route("Checkout")]
        [Authorize(Roles = Auth.Role.Customer)]
        [ServiceFilter(typeof(FlashMessage.FlashMessageFilter))]
        [ServiceFilter(typeof(PageHeader.PopulateHeader))]
        public async Task<IActionResult> Index()
        {
            var userRepo = (Repository.IUser)this._services.GetService(typeof(Repository.IUser));
            var orderRepo = (Repository.IOrder)this._services.GetService(typeof(Repository.IOrder));

            var userId = Guid.Parse(HttpContext.User.FindFirst(claim => claim.Type == Auth.Claim.UserId).Value);

            var user = await userRepo.GetUserById(userId);
            var location = await userRepo.GetDefaultLocation(user);

            var currentOrder = await userRepo.GetOpenOrder(user, location);
            var orderLines = orderRepo.GetOrderLines(userId, currentOrder.OrderId);

            var model = new Model.View.Checkout();

            foreach(var item in orderLines)
            {
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

        /// <summary>
        /// Route to place an order for the items in the cart.
        /// </summary>
        [Route("Checkout/PlaceOrder")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = Auth.Role.Customer)]
        public async Task<IActionResult> PlaceOrder()
        {
            var userRepo = (Repository.IUser)this._services.GetService(typeof(Repository.IUser));
            var orderRepo = (Repository.IOrder)this._services.GetService(typeof(Repository.IOrder));

            var userId = Guid.Parse(HttpContext.User.FindFirst(claim => claim.Type == Auth.Claim.UserId).Value);

            var user = await userRepo.GetUserById(userId);
            var location = await userRepo.GetDefaultLocation(user);

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
                    this._logger.LogWarning($"An order failed to be submitted due to insufficient stock. order id: '{currentOrder.OrderId}'");
                    this.SetFlashError("Unable to place order: Some items are out of stock.");
                    return RedirectToAction("PlaceOrderError", "Checkout");
                }
                case StoreApp.Repository.PlaceOrderResult.NoLineItems:
                {
                    this._logger.LogWarning($"An order failed to be submitted due to not having any line items. order id: '{currentOrder.OrderId}'");
                    this.SetFlashError("Unable to place order: There are no items in your order.");
                    return RedirectToAction("PlaceOrderError", "Checkout");
                }
                case StoreApp.Repository.PlaceOrderResult.OrderNotFound:
                {
                    this._logger.LogCritical($"An order failed to be submitted due to missing order id.");
                    this.SetFlashError("Unable to place order: No order was found.");
                    return RedirectToAction("PlaceOrderError", "Checkout");
                }
                default:
                {
                    this._logger.LogWarning($"An unknown error occurred when attempting to submit order number '{currentOrder.OrderId}'.");
                    this.SetFlashError("Unable to place order. Please try again.");
                    return RedirectToAction("PlaceOrderError", "Checkout");
                }

            }
        }

        /// <summary>
        /// Route to redirect to the checkout page if the place order page is accessed via a GET request.
        /// </summary>
        [Route("Checkout/PlaceOrder")]
        [HttpGet]
        public IActionResult RedirectPlaceOrder()
        {
            return Redirect("/Checkout");
        }

        /// <summary>
        /// Route to display the 'place order ok' page after successfully placing an order.
        /// </summary>
        [Route("Checkout/PlaceOrderOk")]
        [HttpGet]
        [ServiceFilter(typeof(FlashMessage.FlashMessageFilter))]
        [ServiceFilter(typeof(PageHeader.PopulateHeader))]
        public IActionResult PlaceOrderOk()
        {
            return View("PlaceOrderOk");
        }

        /// <summary>
        /// Route to display the 'order error' page after a failure occurred when placing an order.
        /// </summary>
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