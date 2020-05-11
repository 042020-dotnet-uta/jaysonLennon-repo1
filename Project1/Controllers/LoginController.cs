using System;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Text.Encodings.Web;
using Microsoft.Extensions.Logging;
using System.Linq;
using Microsoft.EntityFrameworkCore;

using StoreApp.Data;

namespace StoreApp.Controllers
{
    public class LoginController : Controller
    {
        private StoreContext _context;
        private ILogger<LoginController> _logger;
        private Repository.ICustomerRepository _customerRepository;

        public LoginController(
            StoreContext context,
            ILogger<LoginController> logger,
            Repository.ICustomerRepository customerRepository
            )
        {
            this._context = context;
            this._logger = logger;
            this._customerRepository = customerRepository;
        }

        [Route("Login")]
        public async Task<IActionResult> Index()
        {
            var model = new Models.LoginViewModel();
            return View("Index", model);
        }

        [Route("Login/TryLogin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> TryLogin([Bind]Models.LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                this._logger.LogCritical("invalid model");
            } else
            {
                this._logger.LogInformation("valid model");
            }
            this._logger.LogTrace($"model username={model.UserName}");
            return View("Index", model);
        }
    }
}
