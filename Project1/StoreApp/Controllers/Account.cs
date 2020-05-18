using System;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using System.Linq;

using StoreApp.Data;
using System.Collections.Generic;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using StoreApp.FlashMessageExtension;

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
        [ServiceFilter(typeof(FlashMessage.FlashMessageFilter))]
        [ServiceFilter(typeof(PageHeader.PopulateHeader))]
        public async Task<IActionResult> Manage()
        {
            var userId = Guid.Parse(HttpContext.User.FindFirst(claim => claim.Type == Auth.Claim.UserId).Value);
            var userRepo = (Repository.IUser)this._services.GetService(typeof(Repository.IUser));
            var locationRepo = (Repository.ILocation)this._services.GetService(typeof(Repository.ILocation));

            var defaultLocation = await userRepo.GetDefaultLocation(userId);
            var allLocations = locationRepo.GetLocations();

            var model = new Model.Input.AccountManagement();
            model.StorePicked = defaultLocation.LocationId.ToString();

            foreach(var loc in allLocations)
            {
                model.Stores.Add( new SelectListItem { Value = loc.LocationId.ToString(), Text = loc.Name });
            }

            var user = await userRepo.GetUserById(userId);
            model.FirstName = user.FirstName;
            model.LastName = user.LastName;

            var userAddress = await userRepo.GetAddressByuserId(userId);
            if (userAddress != null)
            {
                model.AddressLine1 = userAddress.Line1 != null ? userAddress.Line1.Data : null;
                model.AddressLine2 = userAddress.Line2 != null ? userAddress.Line2.Data : null;
                model.City = userAddress.City != null ? userAddress.City.Name : null;
                model.StatePicked = userAddress.State != null ? userAddress.State.Name : null;
                model.Zip = userAddress.Zip != null ? userAddress.Zip.Zip : null;
            }

            return View("Manage", model);
        }

        [HttpPost]
        [Authorize(Roles = Auth.Role.Customer)]
        [ValidateAntiForgeryToken]
        [Route("Account/Update")]
        public async Task<IActionResult> UpdateAccountInfo(Model.Input.AccountManagement model)
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

            var userId = Guid.Parse(HttpContext.User.FindFirst(claim => claim.Type == Auth.Claim.UserId).Value);
            var userRepo = (Repository.IUser)this._services.GetService(typeof(Repository.IUser));
            var locationRepo = (Repository.ILocation)this._services.GetService(typeof(Repository.ILocation));

            var user = await userRepo.GetUserById(userId);
            var location = await locationRepo.GetById(defaultLocationId);
            userRepo.SetDefaultLocation(user, location);

            var updateOk = await userRepo.UpdateUserInfo(user.UserId, model);
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
        public async Task<IActionResult> RedirectUpdateAccountInfo(Model.Input.AccountManagement model)
        {
            return RedirectToAction("Manage");
        }

        [Route("Account/OrderHistory")]
        [Authorize(Roles = Auth.Role.Customer)]
        [ServiceFilter(typeof(FlashMessage.FlashMessageFilter))]
        [ServiceFilter(typeof(PageHeader.PopulateHeader))]
        public async Task<IActionResult> OrderHistory()
        {
            var userId = Guid.Parse(HttpContext.User.FindFirst(claim => claim.Type == Auth.Claim.UserId).Value);
            var orderRepo = (Repository.IOrder)this._services.GetService(typeof(Repository.IOrder));
            var orders = orderRepo.GetSubmittedOrders(userId);
            _logger.LogTrace($"num orders={orders.Count()}");

            var model = new Model.View.UserOrderHistory();

            foreach(var o in orders)
            {
                model.AddHistoryItem(o.Item1, o.Item2);
            }

            return View("OrderHistory", model);
        }

        [Route("Account/OrderHistoryDetail")]
        [Authorize(Roles = Auth.Role.Customer)]
        [ServiceFilter(typeof(FlashMessage.FlashMessageFilter))]
        [ServiceFilter(typeof(PageHeader.PopulateHeader))]
        public async Task<IActionResult> OrderHistoryDetail(Guid orderId)
        {
            var userId = Guid.Parse(HttpContext.User.FindFirst(claim => claim.Type == Auth.Claim.UserId).Value);
            var userRepo = (Repository.IUser)this._services.GetService(typeof(Repository.IUser));
            var orderRepo = (Repository.IOrder)this._services.GetService(typeof(Repository.IOrder));
            var orderLines = orderRepo.GetOrderLines(userId, orderId);
            var order = await orderRepo.GetOrderById(userId, orderId);

            var model = new Model.View.UserOrderHistoryDetail(order);

            foreach(var line in orderLines)
            {
                model.AddLineItem(line);
            }

            return View("OrderHistoryDetail", model);
        }

        [Route("Account/Create")]
        [ServiceFilter(typeof(FlashMessage.FlashMessageFilter))]
        [ServiceFilter(typeof(PageHeader.PopulateHeader))]
        public async Task<IActionResult> CreateAccountIndex()
        {
            var model = new Model.Input.CreateAccount();
            return View("CreateAccount", model);
        }

        [Route("Account/Login")]
        [ServiceFilter(typeof(FlashMessage.FlashMessageFilter))]
        [ServiceFilter(typeof(PageHeader.PopulateHeader))]
        public async Task<IActionResult> LoginIndex(Model.Input.LoginRedirect loginRedirectModel)
        {
            // LoginRedirectModel is used for redirection requests.
            var loginUser = new Model.Input.LoginUser();
            loginUser.ReturnUrl = loginRedirectModel.ReturnUrl;
            this._logger.LogDebug($"return url={loginUser.ReturnUrl}");
            return View("Login", loginUser);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Account/Logout")]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            this.SetFlashOk("You are now logged out.");
            return RedirectToAction("LoginIndex");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Account/TryCreate")]
        public async Task<IActionResult> TryCreate(Model.Input.CreateAccount model)
        {
            if (!ModelState.IsValid)
            {
                return View("CreateAccount", model);
            }
            else
            {
                var userRepo = (Repository.IUser)this._services.GetService(typeof(Repository.IUser));
                var loginExists = await userRepo.LoginExists(model.UserName);
                if (loginExists)
                {
                    this.SetFlashError("That user name is unavailable.");
                    return View("CreateAccount", model);
                }

                var user = new Entity.User();
                user.Login = model.UserName;
                user.Password = model.Password;
                await userRepo.Add(user);

                var loginOk = await DoLogin(user.UserId);
                _logger.LogTrace($"loginok={loginOk}");

                return RedirectToAction("Index", "Storefront");
            }
        }

        private async Task<bool> DoLogin(Guid userId)
        {
            var userRepo = (Repository.IUser)this._services.GetService(typeof(Repository.IUser));
            var user = await userRepo.GetUserById(userId);
            if (user == null)
            {
                return false;
            }

            var claims = new List<Claim>
            {
                new Claim(Auth.Claim.UserName, user.Login),
                new Claim(ClaimTypes.Role, Auth.Role.Customer),
                new Claim(Auth.Claim.UserId, userId.ToString()),
                // TODO: Check user permissions regularly in case they get revoked.
            };

            if (user.Role == Entity.Role.Admin)
            {
                claims.Add(new Claim(ClaimTypes.Role, Auth.Role.Administrator));
            }

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
        public async Task<IActionResult> TryLogin(Model.Input.LoginUser model)
        {
            if (!ModelState.IsValid)
            {
                return View("Login", model);
            }

            this._logger.LogDebug("We are letting anyone sign in atm for testing");
            this._logger.LogTrace($"model username={model.UserName}");

            var userRepo = (Repository.IUser)this._services.GetService(typeof(Repository.IUser));
            var user = await userRepo.VerifyCredentials(model.UserName, model.Password);
            // Not finding a user means their credentials could not be verified.
            if (user == null) 
            {
                var loginRedirect = new Model.Input.LoginRedirect();
                this.SetFlashError("Invalid login credentials");
                loginRedirect.ReturnUrl = model.ReturnUrl;
                return RedirectToAction("LoginIndex", loginRedirect);
            }

            await DoLogin(user.UserId);

            this._logger.LogDebug($"return url={model.ReturnUrl}");

            if (!String.IsNullOrEmpty(model.ReturnUrl))
            {
                return Redirect(model.ReturnUrl);
            }
            else
            {
                return RedirectToAction("Index", "StoreFront");
            }
        }

        [Route("Account/AccessDenied")]
        [HttpGet]
        [ServiceFilter(typeof(FlashMessage.FlashMessageFilter))]
        [ServiceFilter(typeof(PageHeader.PopulateHeader))]
        public async Task<IActionResult> AccessDenied(string returnUrl)
        {
            this._logger.LogTrace($"access denied: return={returnUrl}");
            return View("AccessDenied");
        }

        [HttpGet]
        [Route("Account/VerifyUserName")]
        public async Task<IActionResult> VerifyUserName(string username)
        {
            var userRepo = (Repository.IUser)this._services.GetService(typeof(Repository.IUser));
            var verified = await userRepo.VerifyUserLogin(username);
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
