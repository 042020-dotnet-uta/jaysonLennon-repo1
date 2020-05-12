using System;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Text.Encodings.Web;
using Microsoft.Extensions.Logging;
using System.Linq;
using Microsoft.EntityFrameworkCore;

using StoreApp.Data;
using System.Collections.Generic;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;

namespace StoreApp.Controllers
{
    public class AccountManagement : Controller
    {
        private StoreContext _context;
        private ILogger<Models.Login> _logger;
        private Repository.ICustomer _customerRepository;

        public AccountManagement(
            StoreContext context,
            ILogger<Models.Login> logger,
            Repository.ICustomer customerRepository
            )
        {
            this._context = context;
            this._logger = logger;
            this._customerRepository = customerRepository;
        }

        [Route("CreateAccount")]
        public async Task<IActionResult> CreateAccountIndex()
        {
            var model = new Models.CreateAccount();
            return View("CreateAccount", model);
        }

        [Route("Login")]
        public async Task<IActionResult> LoginIndex()
        {
            var model = new Models.Login();
            return View("Login", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("CreateAccount/TryCreate")]
        public async Task<IActionResult> TryCreate(Models.CreateAccount model)
        {
            if (!ModelState.IsValid)
            {
                return View("CreateAccount", model);
            }
            else
            {
                var customer = new Entity.Customer();
                customer.Login = model.UserName;
                customer.Password = model.Password;
                await this._customerRepository.Add(customer);
                // TODO: redirect to customer page
                return View("TODO", model);
            }
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
            this._logger.LogDebug("We are letting anyone sign in atm for testing");
            this._logger.LogTrace($"model username={model.UserName}");

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, model.UserName),
                new Claim(ClaimTypes.Role, "Customer"),
                // TODO: get database last user update time to reauth
                new Claim("LastChanged", "2000-01-01"),
            };

            var claimsIdentity = new ClaimsIdentity(
                claims, CookieAuthenticationDefaults.AuthenticationScheme);

            var authProperties = new AuthenticationProperties
            {
                AllowRefresh = true,
                RedirectUri = "/Customer/Home"
            };

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties);

            return Redirect("/Customer/Home");
        }

        [Route("Login/AccessDenied")]
        [HttpGet]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AccessDenied()
        {
            this._logger.LogTrace($"access denied");
            return View("AccessDenied");
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
