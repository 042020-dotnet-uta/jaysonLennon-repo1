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
        public IActionResult Index()
        {
            return View();
        }

        [Route("/Privacy")]
        public IActionResult Privacy()
        {
            return View();
        }
    }
}
