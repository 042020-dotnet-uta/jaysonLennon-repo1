using System;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using System.Linq;
using Microsoft.AspNetCore.Authorization;

using StoreApp.Data;

namespace StoreApp.AdminControllers
{
    public class AdminCustomerSearch : Controller
    {
        private StoreContext _context;
        private ILogger<AdminCustomerSearch> _logger;
        private IServiceProvider _services;

        public AdminCustomerSearch(
            StoreContext context,
            ILogger<AdminCustomerSearch> logger,
            IServiceProvider services
            )
        {
            this._context = context;
            this._logger = logger;
            this._services = services;

            this._logger.LogTrace("instantiate admin customer search");
        }

        [Route("Admin/CustomerSearch")]
        [Authorize(Roles = Auth.Role.Administrator)]
        [ServiceFilter(typeof(FlashMessage.FlashMessageFilter))]
        public async Task<IActionResult> Index()
        {
            var customerRepo = (Repository.ILocation)this._services.GetService(typeof(Repository.ILocation));

            return View("/Views/Admin/CustomerSearch/Index.cshtml");
        }

        [Route("Admin/CustomerSearch/api/search")]
        [Authorize(Roles = Auth.Role.Administrator)]
        [ServiceFilter(typeof(FlashMessage.FlashMessageFilter))]
        public async Task<IActionResult> ApiSearch(string nameQuery)
        {
            if (String.IsNullOrEmpty(nameQuery)) return Json(new object());

            var userRepo = (Repository.IUser)this._services.GetService(typeof(Repository.IUser));
            var results = userRepo.FindUserQueryIncludeRevenue(nameQuery);
            var response = new AdminModel.CustomerSearchResult();
            response.QueryItem1 = results.QueryItem1;
            response.QueryItem2 = results.QueryItem2;
            response.IsOmniQuery = results.IsOmniQuery;

            var entries = results.Users
                .Select(r => new AdminModel.CustomerSearchResultEntry {
                    UserId = r.Item1.UserId,
                    FirstName = r.Item1.FirstName,
                    LastName = r.Item1.LastName,
                    Revenue = r.Item2,
                })
                .OrderBy(u => u.LastName)
                .Take(30)
                .ToList();

            response.Users = entries;

            return Json(response);
        }
    }
}