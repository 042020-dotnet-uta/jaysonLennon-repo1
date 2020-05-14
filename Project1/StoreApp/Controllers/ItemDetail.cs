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
        private Repository.IProduct _productRepository;

        public ItemDetail(
            ILogger<HomeController> logger,
            Repository.IProduct productRepository)
        {
            this._logger = logger;
            this._productRepository = productRepository;
        }

        [Route("ItemDetail")]
        [Route("ItemDetail/Index")]
        public IActionResult Index()
        {
            return Redirect("/Storefront");
        }

        [Route("ItemDetail/View")]
        public async Task<IActionResult> RedirectShowDetail(Guid id)
        {
            return Redirect("/Storefront");
        }

        [Route("ItemDetail/View/{id}")]
        [Authorize(Roles = Auth.Role.Customer)]
        public async Task<IActionResult> ShowDetail(Guid id)
        {
            if (!ModelState.IsValid) return View("ItemDetail", Models.ItemDetail.ItemNotFound());

            var product = await _productRepository.GetProductById(id);
            if (product == null)
            {
                return View("ItemDetail", Models.ItemDetail.ItemNotFound());
            }

            var model = new Models.ItemDetail();
            model.Id = product.ProductId;
            model.Name = product.Name;
            model.ImageName = product.ImageName;
            model.UnitPrice = product.Price;

            return View("ItemDetail", model);
        }
    }
}
