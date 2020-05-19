using System;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;
using System.Linq;

using StoreApp.Data;

namespace StoreApp.AdminControllers
{
    public class AdminHome : Controller
    {
        private StoreContext _context;
        private ILogger<AdminHome> _logger;
        private IServiceProvider _services;

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

        [Route("Admin")]
        [Authorize(Roles = Auth.Role.Administrator)]
        [ServiceFilter(typeof(FlashMessage.FlashMessageFilter))]
        [ServiceFilter(typeof(PageHeader.PopulateHeader))]
        public async Task<IActionResult> Index()
        {
            return View("/Views/Admin/Home/Index.cshtml");
        }
    }
}
