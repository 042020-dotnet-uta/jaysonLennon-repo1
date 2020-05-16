/*
using System;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using System.Linq;
using Microsoft.AspNetCore.Authorization;

using StoreApp.Data;

namespace StoreApp.AdminControllers
{
    public class CustomerSearch : Controller
    {
        private StoreContext _context;
        private ILogger<CustomerSearch> _logger;
        private IServiceProvider _services;

        public CustomerSearch(
            StoreContext context,
            ILogger<CustomerSearch> logger,
            IServiceProvider services
            )
        {
            this._context = context;
            this._logger = logger;
            this._services = services;

            this._logger.LogTrace("instantiate admin home");
        }

        [Route("Admin")]
        [Authorize(Roles = Auth.Role.Administrator)]
        [ServiceFilter(typeof(FlashMessage.FlashMessageFilter))]
        public async Task<IActionResult> Index()
        {
            var locationRepo = (Repository.ILocation)this._services.GetService(typeof(Repository.ILocation));

            return View("Index", model);
        }
    }
}
*/