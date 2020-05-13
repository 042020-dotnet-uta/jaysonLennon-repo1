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
    public class Account : Controller
    {
        private StoreContext _context;
        private ILogger<Account> _logger;
        private Repository.ICustomer _customerRepository;

        public Account(
            StoreContext context,
            ILogger<Account> logger,
            Repository.ICustomer customerRepository
            )
        {
            this._context = context;
            this._logger = logger;
            this._customerRepository = customerRepository;
        }

        [Route("Account/Manage")]
        public async Task<IActionResult> Manage()
        {
            return View("Manage");
        }

        [Route("Account/Create")]
        public async Task<IActionResult> CreateAccountIndex()
        {
            var model = new Models.CreateAccount();
            return View("CreateAccount", model);
        }

        [Route("Account/Login")]
        public async Task<IActionResult> LoginIndex(Models.LoginRedirect loginRedirectModel)
        {
            // LoginRedirectModel is used to forward error messages and redirection requests.
            // It should always be mapped to a LoginUser before returning the Login view.
            var loginUser = new Models.LoginUser();
            loginUser.ReturnUrl = loginRedirectModel.ReturnUrl;
            loginUser.ErrorMessage = loginRedirectModel.ErrorMessage;
            this._logger.LogDebug($"return url={loginUser.ReturnUrl}");
            return View("Login", loginUser);
        }

        [HttpPost]
        [Route("Account/Logout")]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return Redirect("LogoutOk");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Account/TryCreate")]
        public async Task<IActionResult> TryCreate(Models.CreateAccount model)
        {
            if (!ModelState.IsValid)
            {
                return View("CreateAccount", model);
            }
            else
            {
                var loginExists = await this._customerRepository.LoginExists(model.UserName);
                if (loginExists)
                {
                    model.ErrorMessage = "That user name is unavailable.";
                    return View("CreateAccount", model);
                }

                var customer = new Entity.Customer();
                customer.Login = model.UserName;
                customer.Password = model.Password;
                await this._customerRepository.Add(customer);
                return Redirect("/Storefront");
            }
        }

        [Route("Account/TryLogin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> TryLogin(Models.LoginUser model)
        {
            if (!ModelState.IsValid)
            {
                return View("Login", model);
            }

            this._logger.LogDebug("We are letting anyone sign in atm for testing");
            this._logger.LogTrace($"model username={model.UserName}");

            var customer = await this._customerRepository.VerifyCredentials(model.UserName, model.Password);
            // Not finding a customer means their credentials could not be verified.
            if (customer == null) 
            {
                var loginRedirect = new Models.LoginRedirect();
                loginRedirect.ErrorMessage = "Invalid login credentials";
                loginRedirect.ReturnUrl = model.ReturnUrl;
                return RedirectToAction("LoginIndex", loginRedirect);
            }

            var claims = new List<Claim>
            {
                new Claim(Auth.Claim.UserName, model.UserName),
                new Claim(ClaimTypes.Role, Auth.Role.Customer),
                new Claim(Auth.Claim.UserId, customer.CustomerId.ToString()),
                // TODO: Check user permissions regularly in case they get revoked.
            };

            var claimsIdentity = new ClaimsIdentity(
                claims, CookieAuthenticationDefaults.AuthenticationScheme);

            var authProperties = new AuthenticationProperties
            {
                AllowRefresh = true,
                RedirectUri = "/Storefront"
            };

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties);

            this._logger.LogDebug($"return url={model.ReturnUrl}");

            if (!String.IsNullOrEmpty(model.ReturnUrl))
            {
                return Redirect(model.ReturnUrl);
            }
            else
            {
                return Redirect("/Storefront");
            }
        }

        [Route("Account/AccessDenied")]
        [HttpGet]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AccessDenied(string returnUrl)
        {
            this._logger.LogTrace($"access denied: return={returnUrl}");
            return View("AccessDenied");
        }

        [HttpGet]
        [Route("Account/VerifyUserName")]
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
