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
    /// <summary>
    /// Administrator store order summary controller.
    /// </summary>
    public class AdminStoreOrderSummary : Controller
    {
        private StoreContext _context;
        private ILogger<AdminStoreOrderSummary> _logger;
        private IServiceProvider _services;

        /// <summary>
        /// Standard constructor
        /// </summary>
        public AdminStoreOrderSummary(
            StoreContext context,
            ILogger<AdminStoreOrderSummary> logger,
            IServiceProvider services
            )
        {
            this._context = context;
            this._logger = logger;
            this._services = services;
        }

        /// <summary>
        /// Route for the index page.
        /// </summary>
        [Route("Admin/StoreOrderSummary")]
        [Authorize(Roles = Auth.Role.Administrator)]
        [ServiceFilter(typeof(FlashMessage.FlashMessageFilter))]
        [ServiceFilter(typeof(PageHeader.PopulateHeader))]
        public IActionResult Index()
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

        /// <summary>
        /// Route for the store order summary page.
        /// </summary>
        /// <param name="storeId">The store ID to retrieve a summary for.</param>
        [Route("Admin/StoreOrderSummary/Summary")]
        [Authorize(Roles = Auth.Role.Administrator)]
        [ServiceFilter(typeof(FlashMessage.FlashMessageFilter))]
        [ServiceFilter(typeof(PageHeader.PopulateHeader))]
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

            foreach(var order in orders.OrderByDescending(t => t.Item1.TimeSubmitted))
            {
                summary.AddSummaryItem(order.Item1, order.Item2);
            }

            summary.StoreId = storeId;

            return View("/Views/Admin/StoreOrderSummary/Summary.cshtml", summary);
        }

        /// <summary>
        /// Route to display the details of an order from a store.
        /// </summary>
        /// <param name="orderId">The order id to display.</param>
        /// <param name="storeId">The store id this order belongs to.</param>
        /// <returns></returns>
        [Route("Admin/StoreOrderSummary/Detail")]
        [Authorize(Roles = Auth.Role.Administrator)]
        [ServiceFilter(typeof(FlashMessage.FlashMessageFilter))]
        [ServiceFilter(typeof(PageHeader.PopulateHeader))]
        public async Task<IActionResult> Detail(Guid orderId, Guid storeId)
        {
            var locationRepo = (Repository.ILocation)this._services.GetService(typeof(Repository.ILocation));

            var orderRepo = (Repository.IOrder)this._services.GetService(typeof(Repository.IOrder));

            var order = await orderRepo.AdminGetOrderById(orderId);
            if (order == null)
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
            model.TimeSubmitted = order.TimeSubmitted;

            return View("/Views/Admin/StoreOrderSummary/Detail.cshtml", model);
        }
    }
}
