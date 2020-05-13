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
        private ILogger<Models.Login> _logger;
        private Repository.IOrder _orderRepository;

        public Cart(
            StoreContext context,
            ILogger<Models.Login> logger,
            Repository.IOrder orderRepository
            )
        {
            this._context = context;
            this._logger = logger;
            this._orderRepository = orderRepository;
        }

        [Route("Cart/View")]
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

            var model = new Models.Checkout();
            model.Items.Add(cartItem);
            model.Items.Add(cartItem2);

            return View("Checkout", model);
            return View("Cart");
        }

        [Route("Cart/Add")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = Auth.Role.Customer)]
        public IActionResult AddToCart(Models.CartAdd model)
        {
            // TODO: Implement 'add to cart' functionality.
            _logger.LogTrace($" call add to cart with id {model.ItemId} and quantity {model.ItemQuantity}");
            var okModel = new Models.CartAddOk();
            okModel.ItemId = model.ItemId;
            okModel.ItemQuantity = model.ItemQuantity;
            return View("CartAddOk", okModel);
        }
    }
}
