using System;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Text.Encodings.Web;
using Microsoft.Extensions.Logging;
using System.Linq;
using Microsoft.EntityFrameworkCore;

using StoreApp.Data;
using System.Collections.Generic;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;

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
        public async Task<IActionResult> ViewCart()
        {
            return View("Cart");
        }
    }
}
