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
        private Repository.IProduct _productRepository;

        public Storefront(
            StoreContext context,
            ILogger<HelloWorldController> logger,
            IServiceProvider services,
            Repository.IProduct productRepository
            )
        {
            this._context = context;
            this._logger = logger;
            this._services = services;
            this._productRepository = productRepository;

            this._logger.LogTrace("instantiate storefront");
        }

        [Route("Storefront")]
        [ServiceFilter(typeof(SessionLayout.UseLayout))] // Change the layout to include session info.
        public async Task<IActionResult> Index()
        {
            var model = new Models.Storefront();
            return View("Index");
        }
    }
}
