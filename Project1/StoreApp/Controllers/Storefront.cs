using System.Net;
using System;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Text.Encodings.Web;
using Microsoft.Extensions.Logging;
using System.Linq;
using Microsoft.EntityFrameworkCore;

using StoreApp.Data;
using Microsoft.AspNetCore.Authorization;

namespace StoreApp.Controllers
{
    public class Storefront : Controller
    {
        private StoreContext _context;
        private ILogger<HelloWorldController> _logger;
        private IServiceProvider _services;

        public Storefront(
            StoreContext context,
            ILogger<HelloWorldController> logger,
            IServiceProvider services
            )
        {
            this._context = context;
            this._logger = logger;
            this._services = services;

            this._logger.LogTrace("instantiate storefront");
        }

        [Route("Storefront")]
        [ServiceFilter(typeof(SessionLayout.UseLayout))] // Change the layout to include session info.
        public async Task<IActionResult> Index()
        {
            var locationRepo = (Repository.ILocation)this._services.GetService(typeof(Repository.ILocation));
            var model = new Models.Storefront();


            // Determine which location's inventory we should display.
            Entity.Location location = null;
            var username = HttpContext.User.FindFirst(claim => claim.Type == Auth.Claim.UserName);
            if (username != null)
            {
                var customerRepo = (Repository.ICustomer)this._services.GetService(typeof(Repository.ICustomer));
                var customer = await customerRepo.GetCustomerByLogin(username.Value);
                location = await customerRepo.GetDefaultLocation(customer);
            }

            if (location == null) location = locationRepo.GetMostStocked();

            model.products = locationRepo.GetProductsAvailable(location).ToList();

            return View("Index", model);
        }
    }
}
