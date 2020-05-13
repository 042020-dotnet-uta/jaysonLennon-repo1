using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using StoreApp.Models;
using Microsoft.AspNetCore.Authorization;

namespace StoreApp.Controllers
{
    public class ItemDetail : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public ItemDetail(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        [Route("ItemDetail")]
        [Route("ItemDetail/Index")]
        public IActionResult Index()
        {
            return Redirect("/Storefront");
        }

        [Route("ItemDetail/View/{id?}")]
        [Authorize(Roles = Auth.Role.Customer)]
        public IActionResult ShowDetail(Guid id)
        {
            var model = new Models.ItemDetail();
            model.Id = id;
            return View("ItemDetail", model);
        }
    }
}
