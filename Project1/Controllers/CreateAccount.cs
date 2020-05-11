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
    public class CreateAccount : Controller
    {
        private StoreContext _context;
        private ILogger<LoginController> _logger;
        private Repository.ICustomerRepository _customerRepository;

        public CreateAccount(
            StoreContext context,
            ILogger<LoginController> logger,
            Repository.ICustomerRepository customerRepository
            )
        {
            this._context = context;
            this._logger = logger;
            this._customerRepository = customerRepository;
        }

        [Route("CreateAccount")]
        public async Task<IActionResult> Index()
        {
            var model = new Models.CreateAccount();
            return View("Index", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("CreateAccount/TryCreate")]
        public async Task<IActionResult> TryCreate(Models.CreateAccount model)
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

        [HttpGet]
        [Route("CreateAccount/VerifyUserName")]
        public async Task<IActionResult> VerifyUserName(string username)
        {
            return Json($"That user name is unavailable.");
        }
    }
}
