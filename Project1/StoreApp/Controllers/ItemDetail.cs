using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;

namespace StoreApp.Controllers
{
    public class ItemDetail : Controller
    {
        private readonly ILogger<ItemDetail> _logger;
        private IServiceProvider _services;

        public ItemDetail(
            ILogger<ItemDetail> logger,
            IServiceProvider services
            )
        {
            this._logger = logger;
            this._services = services;
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
        [ServiceFilter(typeof(FlashMessage.FlashMessageFilter))]
        [ServiceFilter(typeof(PageHeader.PopulateHeader))]
        public async Task<IActionResult> ShowDetail(Guid id)
        {
            if (!ModelState.IsValid) return View("ItemDetail", Model.View.ItemDetail.ItemNotFound());
            _logger.LogDebug("pass model validation");

            var productRepo = (Repository.IProduct)this._services.GetService(typeof(Repository.IProduct));

            _logger.LogDebug($"search id {id}");
            var product = await productRepo.GetProductById(id);
            if (product == null)
            {
                return View("ItemDetail", Model.View.ItemDetail.ItemNotFound());
            }
            _logger.LogDebug("pass product validation");

            var userRepo = (Repository.IUser)this._services.GetService(typeof(Repository.IUser));
            var locationRepo = (Repository.ILocation)this._services.GetService(typeof(Repository.ILocation));

            var userId = Guid.Parse(HttpContext.User.FindFirst(claim => claim.Type == Auth.Claim.UserId).Value);
            _logger.LogDebug($"user id={userId}");

            var location = await userRepo.GetDefaultLocation(userId);
            _logger.LogDebug($"location obj={location}");

            var quantityInStock = await locationRepo.GetStock(location, product);

            var model = new Model.View.ItemDetail();
            model.Id = product.ProductId;
            model.Name = product.Name;
            model.ImageName = product.ImageName;
            model.UnitPrice = product.Price;
            model.Stock = quantityInStock;

            return View("ItemDetail", model);
        }
    }
}
