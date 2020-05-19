using System;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using StoreApp.Models;
using StoreApp.FlashMessageExtension;

namespace StoreApp.Controllers
{
    public class Error : Controller
    {
        private readonly ILogger<Error> _logger;

        public Error(ILogger<Error> logger)
        {
            _logger = logger;
        }

        [Route("/Error")]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Index()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [Route("/TestFlashMsg")]
        [HttpGet]
        [ServiceFilter(typeof(FlashMessage.FlashMessageFilter))]
        public IActionResult FlashMsg()
        {
            this.SetFlashError("Flash error message");
            this.SetFlashInfo("Flash info message that is long");
            this.SetFlashOk("Flash ok message that is way longer than the other messages");
            return View("Teststuff");
        }
    }
}
