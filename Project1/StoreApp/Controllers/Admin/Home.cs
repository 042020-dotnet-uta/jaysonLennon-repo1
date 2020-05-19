using System;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;
using System.Linq;

using StoreApp.Data;

namespace StoreApp.AdminControllers
{
    /// <summary>
    /// Administrator home page controller.
    /// </summary>
    public class AdminHome : Controller
    {
        private StoreContext _context;
        private ILogger<AdminHome> _logger;
        private IServiceProvider _services;

        /// <summary>
        /// Standard constructor.
        /// </summary>
        public AdminHome(
            StoreContext context,
            ILogger<AdminHome> logger,
            IServiceProvider services
            )
        {
            this._context = context;
            this._logger = logger;
            this._services = services;
        }

        /// <summary>
        /// Route for displaying the home page.
        /// </summary>
        [Route("Admin")]
        [Authorize(Roles = Auth.Role.Administrator)]
        [ServiceFilter(typeof(FlashMessage.FlashMessageFilter))]
        [ServiceFilter(typeof(PageHeader.PopulateHeader))]
        public async Task<IActionResult> Index()
        {
            var userName = HttpContext.User.FindFirst(claim => claim.Type == Auth.Claim.UserName).Value;
            this._logger.LogInformation($"'{userName}' accessed the administration page.");
            return View("/Views/Admin/Home/Index.cshtml");
        }
    }
}
