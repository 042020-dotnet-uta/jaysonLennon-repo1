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
    /// <summary>
    /// User Account administration controller.
    /// </summary>
    public class Account : Controller
    {
        private StoreContext _context;
        private ILogger<Account> _logger;
        private IServiceProvider _services;

        /// <summary>
        /// Standard constructor
        /// </summary>
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

        /// <summary>
        /// Route to the account management page.
        /// </summary>
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

            var userAddress = await userRepo.GetAddressByUserId(userId);
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

        /// <summary>
        /// Route to make an update to user information.
        /// </summary>
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

            var businessRules = (Business.IBusinessRules)this._services.GetService(typeof(Business.IBusinessRules));

            if (!String.IsNullOrEmpty(model.FirstName)) {
                if (!businessRules.ValidateUserName(model.FirstName.Trim())) {
                    this.SetFlashError("Names may not contain numbers or special characters.");
                    return RedirectToAction("Manage");
                }
            }

            if (!String.IsNullOrEmpty(model.LastName)) {
                if (!businessRules.ValidateUserName(model.LastName.Trim())) {
                    this.SetFlashError("Names may not contain numbers or special characters.");
                    return RedirectToAction("Manage");
                }
            }

            var updateOk = await userRepo.UpdateUserPersonalInfo(user.UserId, model);
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

        /// <summary>
        /// Route to redirect to the user management page if they visit it with a GET request.
        /// </summary>
        [HttpGet]
        [Route("Account/Update")]
        public IActionResult RedirectUpdateAccountInfo(Model.Input.AccountManagement model)
        {
            return RedirectToAction("Manage");
        }

        /// <summary>
        /// Route to the order history page.
        /// </summary>
        [Route("Account/OrderHistory")]
        [Authorize(Roles = Auth.Role.Customer)]
        [ServiceFilter(typeof(FlashMessage.FlashMessageFilter))]
        [ServiceFilter(typeof(PageHeader.PopulateHeader))]
        public IActionResult OrderHistory()
        {
            var userId = Guid.Parse(HttpContext.User.FindFirst(claim => claim.Type == Auth.Claim.UserId).Value);
            var orderRepo = (Repository.IOrder)this._services.GetService(typeof(Repository.IOrder));
            var orders = orderRepo.GetSubmittedOrders(userId);

            var model = new Model.View.UserOrderHistory();

            foreach(var o in orders)
            {
                model.AddHistoryItem(o.Item1, o.Item2);
            }

            return View("OrderHistory", model);
        }

        /// <summary>
        /// Route to the order history detail page.
        /// </summary>
        /// <param name="orderId">The order id to display details for.</param>
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

        /// <summary>
        /// Route to the create new account page.
        /// </summary>
        [Route("Account/Create")]
        [ServiceFilter(typeof(FlashMessage.FlashMessageFilter))]
        [ServiceFilter(typeof(PageHeader.PopulateHeader))]
        public IActionResult CreateAccountIndex()
        {
            var model = new Model.Input.CreateAccount();
            return View("CreateAccount", model);
        }

        /// <summary>
        /// Route to the login page.
        /// </summary>
        [Route("Account/Login")]
        [ServiceFilter(typeof(FlashMessage.FlashMessageFilter))]
        [ServiceFilter(typeof(PageHeader.PopulateHeader))]
        public IActionResult LoginIndex(Model.Input.LoginRedirect loginRedirectModel)
        {
            // LoginRedirectModel is used for redirection requests.
            var loginUser = new Model.Input.LoginUser();
            loginUser.ReturnUrl = loginRedirectModel.ReturnUrl;
            return View("Login", loginUser);
        }

        /// <summary>
        /// Route to log the user out of their session.
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Account/Logout")]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            this.SetFlashOk("You are now logged out.");
            return RedirectToAction("LoginIndex");
        }

        /// <summary>
        /// Route to try and create a new user account.
        /// </summary>
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
                    this._logger.LogWarning($"An attempt to create duplicate login name was made for '{model.UserName}'.");
                    return View("CreateAccount", model);
                }

                var user = new Entity.User();
                user.Login = model.UserName;
                user.Password = model.Password;
                await userRepo.Add(user);

                var loginOk = await DoLogin(user.UserId);

                return RedirectToAction("Index", "Storefront");
            }
        }

        /// <summary>
        /// Logic to perform a user login.
        /// </summary>
        /// <remarks>
        /// It is assumed that the user credentials have already been previously verified.
        /// </remarks>
        /// <param name="userId">The user id that should be logged in.</param>
        /// <returns>Whether or not the login was successful. This will only fail if the user ID does not exist.</returns>
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

            this._logger.LogInformation($"User '{userId}' logged in.");
            return true;
        }

        /// <summary>
        /// Route to attempt to log the user into the application.
        /// </summary>
        [Route("Account/TryLogin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> TryLogin(Model.Input.LoginUser model)
        {
            if (!ModelState.IsValid)
            {
                return View("Login", model);
            }

            var userRepo = (Repository.IUser)this._services.GetService(typeof(Repository.IUser));
            var user = await userRepo.VerifyCredentials(model.UserName, model.Password);
            // Not finding a user means their credentials could not be verified.
            if (user == null) 
            {
                var loginRedirect = new Model.Input.LoginRedirect();
                this.SetFlashError("Invalid login credentials");
                loginRedirect.ReturnUrl = model.ReturnUrl;
                this._logger.LogWarning($"Invalid login attempt with login name '{model.UserName}'");
                return RedirectToAction("LoginIndex", loginRedirect);
            }

            await DoLogin(user.UserId);

            if (!String.IsNullOrEmpty(model.ReturnUrl))
            {
                return Redirect(model.ReturnUrl);
            }
            else
            {
                return RedirectToAction("Index", "StoreFront");
            }
        }

        /// <summary>
        /// Route when the user attempts to access a page that they don't have the authorization to view.
        /// </summary>
        [Route("Account/AccessDenied")]
        [HttpGet]
        [ServiceFilter(typeof(FlashMessage.FlashMessageFilter))]
        [ServiceFilter(typeof(PageHeader.PopulateHeader))]
        public IActionResult AccessDenied(string returnUrl)
        {
            var userId = Guid.Parse(HttpContext.User.FindFirst(claim => claim.Type == Auth.Claim.UserId).Value);
            this._logger.LogWarning($"User {userId} attempted to access a restricted page ({returnUrl})");
            return View("AccessDenied");
        }

        /// <summary>
        /// API endpoint to check if a user name exists.
        /// </summary>
        /// <param name="username">User name to check.</param>
        /// <returns>JSON object indicating 'true' if the username is valid, or an error message if the username is not valid/available.</returns>
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
