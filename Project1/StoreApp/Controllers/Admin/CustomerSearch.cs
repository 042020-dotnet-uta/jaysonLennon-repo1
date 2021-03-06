using System;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using System.Linq;
using Microsoft.AspNetCore.Authorization;

using StoreApp.Data;

namespace StoreApp.AdminControllers
{
    /// <summary>
    /// Controller for admin customer searching.
    /// </summary>
    public class AdminCustomerSearch : Controller
    {
        private StoreContext _context;
        private ILogger<AdminCustomerSearch> _logger;
        private IServiceProvider _services;

        /// <summary>
        /// Standard constructor.
        /// </summary>
        public AdminCustomerSearch(
            StoreContext context,
            ILogger<AdminCustomerSearch> logger,
            IServiceProvider services
            )
        {
            this._context = context;
            this._logger = logger;
            this._services = services;
        }

        /// <summary>
        /// Index page for customer search.
        /// </summary>
        [Route("Admin/CustomerSearch")]
        [Authorize(Roles = Auth.Role.Administrator)]
        [ServiceFilter(typeof(FlashMessage.FlashMessageFilter))]
        [ServiceFilter(typeof(PageHeader.PopulateHeader))]
        public IActionResult Index()
        {
            return View("/Views/Admin/CustomerSearch/Index.cshtml");
        }

        /// <summary>
        /// API endpoint for displaying the results of a customer.
        /// </summary>
        /// <param name="nameQuery">The name to query.</param>
        /// <returns>JSON object containing the results.</returns>
        [Route("Admin/CustomerSearch/api/search")]
        [Authorize(Roles = Auth.Role.Administrator)]
        [ServiceFilter(typeof(FlashMessage.FlashMessageFilter))]
        public IActionResult ApiSearch(string nameQuery)
        {
            if (String.IsNullOrEmpty(nameQuery)) return Json(new object());

            var userRepo = (Repository.IUser)this._services.GetService(typeof(Repository.IUser));
            var results = userRepo.FindUserQueryIncludeRevenue(nameQuery);
            var response = new AdminModel.CustomerSearchResult();
            response.QueryTerm1 = results.QueryTerm1;
            response.QueryTerm2 = results.QueryTerm2;
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