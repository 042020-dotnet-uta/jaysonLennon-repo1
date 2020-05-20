using System;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using StoreApp.Models;
using StoreApp.FlashMessageExtension;

namespace StoreApp.Controllers
{
    /// <summary>
    /// Controller for displaying an error page.
    /// </summary>
    public class Error : Controller
    {
        private readonly ILogger<Error> _logger;

        /// <summary>
        /// Standard constructor.
        /// </summary>
        public Error(ILogger<Error> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Route for displaying an error page when an exception occurs.
        /// </summary>
        [Route("/Error")]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Index()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        /// <summary>
        /// Route to generate flash messages for testing.
        /// </summary>
        /// <remarks>
        /// To use this, change the access modifier to public, visit the route, then visit any other link.
        /// The flash message will be present on the page that was navigated to from the FlashMsg page.
        /// </remarks>

        [Route("/TestFlashMsg")]
        [HttpGet]
        [ServiceFilter(typeof(FlashMessage.FlashMessageFilter))]
        private IActionResult FlashMsg()
        {
            this.SetFlashError("Flash error message");
            this.SetFlashInfo("Flash info message that is long");
            this.SetFlashOk("Flash ok message that is way longer than the other messages");
            return View("Teststuff");
        }
    }
}
