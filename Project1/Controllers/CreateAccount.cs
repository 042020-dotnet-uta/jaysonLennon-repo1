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
        private ILogger<Login> _logger;
        private Repository.ICustomer _customerRepository;

        public CreateAccount(
            StoreContext context,
            ILogger<Login> logger,
            Repository.ICustomer customerRepository
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
                return View("Index", model);
            }
            else
            {
                var customer = new Entity.Customer();
                customer.Login = model.UserName;
                customer.Password = model.Password;
                await this._customerRepository.Add(customer);
                // TODO: redirect to customer page
                return View("Index", model);
            }
        }

        [HttpGet]
        [Route("CreateAccount/VerifyUserName")]
        public async Task<IActionResult> VerifyUserName(string username)
        {
            var verified = await _customerRepository.VerifyUserLogin(username);
            if (verified)
            {
                return Json(true);
            }
            else
            {

                return Json($"That user name is unavailable.");
            }
        }
    }
}
