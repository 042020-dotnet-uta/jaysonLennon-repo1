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
    public class Login : Controller
    {
        private StoreContext _context;
        private ILogger<Login> _logger;
        private Repository.ICustomer _customerRepository;

        public Login(
            StoreContext context,
            ILogger<Login> logger,
            Repository.ICustomer customerRepository
            )
        {
            this._context = context;
            this._logger = logger;
            this._customerRepository = customerRepository;
        }

        [Route("Login")]
        public async Task<IActionResult> Index()
        {
            var model = new Models.Login();
            return View("Index", model);
        }

        [Route("Login/TryLogin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> TryLogin(Models.Login model)
        {
            // TODO: Implement login
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
