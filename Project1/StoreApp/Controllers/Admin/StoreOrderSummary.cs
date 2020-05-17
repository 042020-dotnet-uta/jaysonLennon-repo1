using System;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using System.Linq;
using Microsoft.AspNetCore.Authorization;

using StoreApp.Data;
using StoreApp.FlashMessageExtension;

namespace StoreApp.AdminControllers
{
    public class AdminStoreOrderSummary : Controller
    {
        private StoreContext _context;
        private ILogger<AdminStoreOrderSummary> _logger;
        private IServiceProvider _services;

        public AdminStoreOrderSummary(
            StoreContext context,
            ILogger<AdminStoreOrderSummary> logger,
            IServiceProvider services
            )
        {
            this._context = context;
            this._logger = logger;
            this._services = services;

            this._logger.LogTrace("instantiate admin home");
        }

        [Route("Admin/StoreOrderSummary")]
        [Authorize(Roles = Auth.Role.Administrator)]
        [ServiceFilter(typeof(FlashMessage.FlashMessageFilter))]
        public async Task<IActionResult> Index()
        {
            var locationRepo = (Repository.ILocation)this._services.GetService(typeof(Repository.ILocation));
            var allLocations = locationRepo.GetLocations();

            var model = new AdminModel.StoreList();

            foreach(var loc in allLocations)
            {
                model.Stores.Add( new SelectListItem { Value = loc.LocationId.ToString(), Text = loc.Name });
            }

            return View("/Views/Admin/StoreOrderSummary/Index.cshtml", model);
        }

        [Route("Admin/StoreOrderSummary/Summary")]
        [Authorize(Roles = Auth.Role.Administrator)]
        [ServiceFilter(typeof(FlashMessage.FlashMessageFilter))]
        public async Task<IActionResult> Summary(Guid storeId)
        {
            var locationRepo = (Repository.ILocation)this._services.GetService(typeof(Repository.ILocation));
            var location = await locationRepo.GetById(storeId);
            if (location == null)
            {
                this.SetFlashError("Unable to locate orders for the selected store. Please try again.");
                return RedirectToAction("Index");
            }

            var summary = new AdminModel.StoreOrderSummary();
            summary.StoreName = location.Name;
            var orders = locationRepo.GetOrders(storeId);
            _logger.LogTrace($"order quantity = {orders.Count()}");

            foreach(var order in orders.OrderByDescending(t => t.Item1.TimeSubmitted))
            {
                summary.AddSummaryItem(order.Item1, order.Item2);
            }

            summary.StoreId = storeId;

            return View("/Views/Admin/StoreOrderSummary/Summary.cshtml", summary);
        }

        [Route("Admin/StoreOrderSummary/Detail")]
        [Authorize(Roles = Auth.Role.Administrator)]
        [ServiceFilter(typeof(FlashMessage.FlashMessageFilter))]
        public async Task<IActionResult> Detail(Guid orderId, Guid storeId)
        {
            var locationRepo = (Repository.ILocation)this._services.GetService(typeof(Repository.ILocation));

            var orderRepo = (Repository.IOrder)this._services.GetService(typeof(Repository.IOrder));

            var orderFound = await orderRepo.Exists(orderId);
            if (!orderFound)
            {
                this.SetFlashError("Unable to locate the selected order.");
                return Redirect($"/Admin/StoreOrderSummary/Summary?storeId={storeId}");
            }

            var lineItems = locationRepo.GetOrderLineItems(orderId);
            var model = new AdminModel.StoreOrderDetail();
            foreach(var lineItem in lineItems)
            {
                model.AddDetailItem(lineItem);
            }

            return View("/Views/Admin/StoreOrderSummary/Detail.cshtml", model);
        }

    }
}
