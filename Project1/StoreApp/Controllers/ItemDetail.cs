using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;

namespace StoreApp.Controllers
{
    /// <summary>
    /// Controller to handle the item detail view.
    /// </summary>
    public class ItemDetail : Controller
    {
        private readonly ILogger<ItemDetail> _logger;
        private IServiceProvider _services;

        /// <summary>
        /// Standard constructor.
        /// </summary>
        public ItemDetail(
            ILogger<ItemDetail> logger,
            IServiceProvider services
            )
        {
            this._logger = logger;
            this._services = services;
        }

        /// <summary>
        /// Redirect to store front if this page is visited without an ID.
        /// </summary>
        [Route("ItemDetail")]
        [Route("ItemDetail/Index")]
        public IActionResult Index()
        {
            return Redirect("/Storefront");
        }

        /// <summary>
        /// Redirect to store front if this page is visited without an ID.
        /// </summary>
        [Route("ItemDetail/View")]
        public IActionResult RedirectShowDetail()
        {
            return Redirect("/Storefront");
        }

        /// <summary>
        /// View the detail of an item.
        /// </summary>
        /// <param name="id">Item detail to view</param>
        [Route("ItemDetail/View/{id}")]
        [Authorize(Roles = Auth.Role.Customer)]
        [ServiceFilter(typeof(FlashMessage.FlashMessageFilter))]
        [ServiceFilter(typeof(PageHeader.PopulateHeader))]
        public async Task<IActionResult> ShowDetail(Guid id)
        {
            if (!ModelState.IsValid) return View("ItemDetail", Model.View.ItemDetail.ItemNotFound());

            var productRepo = (Repository.IProduct)this._services.GetService(typeof(Repository.IProduct));

            var product = await productRepo.GetProductById(id);
            if (product == null)
            {
                this._logger.LogWarning($"Unable to locate item id '{id}' for detail display.");
                return View("ItemDetail", Model.View.ItemDetail.ItemNotFound());
            }

            var userRepo = (Repository.IUser)this._services.GetService(typeof(Repository.IUser));
            var locationRepo = (Repository.ILocation)this._services.GetService(typeof(Repository.ILocation));

            var userId = Guid.Parse(HttpContext.User.FindFirst(claim => claim.Type == Auth.Claim.UserId).Value);

            var location = await userRepo.GetDefaultLocation(userId);

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
