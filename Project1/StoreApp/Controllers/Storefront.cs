using System;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using System.Linq;

using StoreApp.Data;
using Microsoft.AspNetCore.Authorization;

namespace StoreApp.Controllers
{
    /// <summary>
    /// Controller for managing the store front page.
    /// </summary>
    public class Storefront : Controller
    {
        private StoreContext _context;
        private ILogger<Storefront> _logger;
        private IServiceProvider _services;

        /// <summary>
        /// Standard constructor.
        /// </summary>
        public Storefront(
            StoreContext context,
            ILogger<Storefront> logger,
            IServiceProvider services
            )
        {
            this._context = context;
            this._logger = logger;
            this._services = services;
        }

        /// <summary>
        /// Main page for the store.
        /// </summary>
        [Route("Storefront")]
        [ServiceFilter(typeof(FlashMessage.FlashMessageFilter))]
        [ServiceFilter(typeof(PageHeader.PopulateHeader))]
        public async Task<IActionResult> Index()
        {
            var locationRepo = (Repository.ILocation)this._services.GetService(typeof(Repository.ILocation));
            var userRepo = (Repository.IUser)this._services.GetService(typeof(Repository.IUser));
            var model = new Model.View.Storefront();

            // Determine which location's inventory we should display.
            Entity.Location location = null;
            var userId = HttpContext.User.FindFirst(claim => claim.Type == Auth.Claim.UserId);
            if (userId != null)
            {
                var userIdAsGuid = Guid.Parse(userId.Value);
                var user = await userRepo.GetUserById(userIdAsGuid);
                location = await userRepo.GetDefaultLocation(user);
                if (location == null)
                {
                    location = await locationRepo.GetMostStocked();
                    userRepo.SetDefaultLocation(user, location);
                }
            }
            else
            {
                location = await locationRepo.GetMostStocked();
            }

            model.products = locationRepo.GetProductsAvailable(location).ToList();
            model.StoreName = location.Name;

            return View("Index", model);
        }
    }
}
