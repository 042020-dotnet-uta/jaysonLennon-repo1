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
        [ServiceFilter(typeof(SessionLayout.SessionLayoutFilter))] // Change the layout to include session info.
        public async Task<IActionResult> Index()
        {
            var locationRepo = (Repository.ILocation)this._services.GetService(typeof(Repository.ILocation));
            var customerRepo = (Repository.ICustomer)this._services.GetService(typeof(Repository.ICustomer));
            var model = new Models.Storefront();

            // Determine which location's inventory we should display.
            Entity.Location location = null;
            var userId = HttpContext.User.FindFirst(claim => claim.Type == Auth.Claim.UserId);
            if (userId != null)
            {
                var userIdAsGuid = Guid.Parse(userId.Value);
                var customer = await customerRepo.GetCustomerById(userIdAsGuid);
                location = await customerRepo.GetDefaultLocation(customer);
                if (location == null)
                {
                    location = locationRepo.GetMostStocked();
                    customerRepo.SetDefaultLocation(customer, location);
                }
            }
            else
            {
                location = locationRepo.GetMostStocked();
            }

            model.products = locationRepo.GetProductsAvailable(location).ToList();
            model.StoreName = location.Name;

            return View("Index", model);
        }
    }
}
