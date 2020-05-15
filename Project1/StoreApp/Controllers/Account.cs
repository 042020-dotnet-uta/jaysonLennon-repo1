using System;
using Microsoft.AspNetCore.Mvc.Rendering;
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
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using StoreApp.Util;

namespace StoreApp.Controllers
{
    public class Account : Controller
    {
        private StoreContext _context;
        private ILogger<Account> _logger;
        private IServiceProvider _services;

        public Account(
            StoreContext context,
            ILogger<Account> logger,
            IServiceProvider services
            )
        {
            this._context = context;
            this._logger = logger;
            this._services = services;
        }

        [Route("Account/Manage")]
        [Authorize(Roles = Auth.Role.Customer)]
        public async Task<IActionResult> Manage()
        {
            var customerId = Guid.Parse(HttpContext.User.FindFirst(claim => claim.Type == Auth.Claim.UserId).Value);
            var customerRepo = (Repository.ICustomer)this._services.GetService(typeof(Repository.ICustomer));
            var locationRepo = (Repository.ILocation)this._services.GetService(typeof(Repository.ILocation));

            var defaultLocation = await customerRepo.GetDefaultLocation(customerId);
            var allLocations = locationRepo.GetLocations();

            var model = new Models.AccountManagement();
            model.StorePicked = defaultLocation.LocationId.ToString();

            foreach(var loc in allLocations)
            {
                model.Stores.Add( new SelectListItem { Value = loc.LocationId.ToString(), Text = loc.Name });
            }

            var customer = await customerRepo.GetCustomerById(customerId);
            model.FirstName = customer.FirstName;
            model.LastName = customer.LastName;

            var userAddress = await customerRepo.GetAddressByCustomerId(customerId);
            if (userAddress != null)
            {
                model.AddressLine1 = userAddress.Line1 != null ? userAddress.Line1.Data : null;
                model.AddressLine2 = userAddress.Line2 != null ? userAddress.Line2.Data : null;
                model.City = userAddress.City != null ? userAddress.City.Name : null;
                model.StatePicked = userAddress.State != null ? userAddress.State.Name : null;
                model.Zip = userAddress.Zip != null ? userAddress.Zip.Zip : null;
            }

            model.OkMessage = this.GetFlashInfo();
            model.ErrorMessage = this.GetFlashError();

            return View("Manage", model);
        }

        [HttpPost]
        [Route("Account/Update")]
        public async Task<IActionResult> UpdateAccountInfo(Models.AccountManagement model)
        {
            if (!ModelState.IsValid)
            {
                this.SetFlashError("There was an error submitting your information. Please try again.");
                return RedirectToAction("Manage");
            }

            Guid defaultLocationId;
            if (!Guid.TryParse(model.StorePicked, out defaultLocationId))
            {
                this.SetFlashError("Unable to find the selected store. Please choose another store.");
                return RedirectToAction("Manage");
            }

            var customerId = Guid.Parse(HttpContext.User.FindFirst(claim => claim.Type == Auth.Claim.UserId).Value);
            var customerRepo = (Repository.ICustomer)this._services.GetService(typeof(Repository.ICustomer));
            var locationRepo = (Repository.ILocation)this._services.GetService(typeof(Repository.ILocation));

            var customer = await customerRepo.GetCustomerById(customerId);
            var location = await locationRepo.GetById(defaultLocationId);
            customerRepo.SetDefaultLocation(customer, location);

            var updateOk = await customerRepo.UpdateCustomerInfo(customer.CustomerId, model);
            if (!updateOk)
            {
                this.SetFlashError("There was an error saving your information. Please try again.");
                return RedirectToAction("Manage");
            }

            var allLocations = locationRepo.GetLocations();

            foreach(var loc in allLocations)
            {
                model.Stores.Add( new SelectListItem { Value = loc.LocationId.ToString(), Text = loc.Name });
            }

            this.SetFlashInfo("Your information has been successfully updated.");
            return RedirectToAction("Manage");
        }

        [HttpGet]
        [Route("Account/Update")]
        public async Task<IActionResult> RedirectUpdateAccountInfo(Models.AccountManagement model)
        {
            return RedirectToAction("Manage");
        }

        [Route("Account/OrderHistory")]
        [Authorize(Roles = Auth.Role.Customer)]
        public async Task<IActionResult> OrderHistory()
        {
            var customerId = Guid.Parse(HttpContext.User.FindFirst(claim => claim.Type == Auth.Claim.UserId).Value);
            var orderRepo = (Repository.IOrder)this._services.GetService(typeof(Repository.IOrder));
            var orders = orderRepo.GetSubmittedOrders(customerId);
            _logger.LogTrace($"num orders={orders.Count()}");

            var model = new Models.CustomerOrderHistory();

            foreach(var o in orders)
            {
                model.AddHistoryItem(o.Item1, o.Item2);
            }

            return View("OrderHistory", model);
        }

        [Route("Account/OrderHistoryDetail")]
        [Authorize(Roles = Auth.Role.Customer)]
        public async Task<IActionResult> OrderHistoryDetail(Guid orderId)
        {
            var customerId = Guid.Parse(HttpContext.User.FindFirst(claim => claim.Type == Auth.Claim.UserId).Value);
            var customerRepo = (Repository.ICustomer)this._services.GetService(typeof(Repository.ICustomer));
            var orderRepo = (Repository.IOrder)this._services.GetService(typeof(Repository.IOrder));
            var orderLines = orderRepo.GetOrderLines(customerId, orderId);
            var order = await orderRepo.GetOrderById(orderId);

            var model = new Models.CustomerOrderHistoryDetail(order);

            foreach(var line in orderLines)
            {
                model.AddLineItem(line);
            }

            return View("OrderHistoryDetail", model);
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
                var customerRepo = (Repository.ICustomer)this._services.GetService(typeof(Repository.ICustomer));
                var loginExists = await customerRepo.LoginExists(model.UserName);
                if (loginExists)
                {
                    model.ErrorMessage = "That user name is unavailable.";
                    return View("CreateAccount", model);
                }

                var customer = new Entity.Customer();
                customer.Login = model.UserName;
                customer.Password = model.Password;
                await customerRepo.Add(customer);

                var loginOk = await DoLogin(customer.CustomerId);
                _logger.LogTrace($"loginok={loginOk}");

                return Redirect("/Storefront");
            }
        }

        private async Task<bool> DoLogin(Guid customerId)
        {
            var customerRepo = (Repository.ICustomer)this._services.GetService(typeof(Repository.ICustomer));
            var customer = await customerRepo.GetCustomerById(customerId);
            if (customer == null)
            {
                return false;
            }

            var claims = new List<Claim>
            {
                new Claim(Auth.Claim.UserName, customer.Login),
                new Claim(ClaimTypes.Role, Auth.Role.Customer),
                new Claim(Auth.Claim.UserId, customerId.ToString()),
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

            return true;
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

            var customerRepo = (Repository.ICustomer)this._services.GetService(typeof(Repository.ICustomer));
            var customer = await customerRepo.VerifyCredentials(model.UserName, model.Password);
            // Not finding a customer means their credentials could not be verified.
            if (customer == null) 
            {
                var loginRedirect = new Models.LoginRedirect();
                loginRedirect.ErrorMessage = "Invalid login credentials";
                loginRedirect.ReturnUrl = model.ReturnUrl;
                return RedirectToAction("LoginIndex", loginRedirect);
            }

            await DoLogin(customer.CustomerId);

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
            var customerRepo = (Repository.ICustomer)this._services.GetService(typeof(Repository.ICustomer));
            var verified = await customerRepo.VerifyUserLogin(username);
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
