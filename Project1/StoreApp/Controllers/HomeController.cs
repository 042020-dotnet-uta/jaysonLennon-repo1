using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using StoreApp.Models;

namespace StoreApp.Controllers
{
    /// <summary>
    /// Controller for misc pages.
    /// </summary>
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        /// <summary>
        /// Standard constructor.
        /// </summary>
        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Route when '/' is visited, redirects to the store front.
        /// </summary>
        [Route("/")]
        [Route("Index/")]
        [ServiceFilter(typeof(PageHeader.PopulateHeader))]
        public IActionResult Index()
        {
            return RedirectToAction("Index", "Storefront");
        }

        /// <summary>
        /// Display privacy page.
        /// </summary>
        [Route("/Privacy")]
        [ServiceFilter(typeof(PageHeader.PopulateHeader))]
        public IActionResult Privacy()
        {
            return View();
        }
    }
}
