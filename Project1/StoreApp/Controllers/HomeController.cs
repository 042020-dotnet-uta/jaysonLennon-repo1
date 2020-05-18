using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using StoreApp.Models;

namespace StoreApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        [Route("/")]
        [Route("Index/")]
        [ServiceFilter(typeof(PageHeader.PopulateHeader))]
        public IActionResult Index()
        {
            return RedirectToAction("Index", "Storefront");
        }

        [Route("/Privacy")]
        [ServiceFilter(typeof(PageHeader.PopulateHeader))]
        public IActionResult Privacy()
        {
            return View();
        }
    }
}
