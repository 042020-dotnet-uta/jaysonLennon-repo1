using System.IO;
using System.Threading;
using System.Net;
using System;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Text.Encodings.Web;
using Microsoft.Extensions.Logging;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;

using StoreApp.Data;

namespace StoreApp.Controllers
{
    [Authorize(Roles = Auth.Role.Customer)]
    public class CustomerHome : Controller
    {
        private StoreContext _context;
        private ILogger<HelloWorldController> _logger;

        public CustomerHome(
            StoreContext context,
            ILogger<HelloWorldController> logger
            )
        {
            this._context = context;
            this._logger = logger;
        }

        [Route("Customer/Home")]
        [ServiceFilter(typeof(FlashMessage.FlashMessageFilter))]
        [ServiceFilter(typeof(CartHeader.CartHeaderFilter))]
        public async Task<IActionResult> Home()
        {
            return View("Index");
        }
    }
}
