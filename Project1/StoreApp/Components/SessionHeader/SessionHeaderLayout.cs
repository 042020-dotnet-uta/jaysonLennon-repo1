using System.Security.Claims;
using System.Net.Security;
using System;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Text.Encodings.Web;
using Microsoft.Extensions.Logging;
using System.Linq;
using Microsoft.EntityFrameworkCore;

using StoreApp.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;

namespace StoreApp.SessionLayout
{
    /// <summary>
    /// Constants used as Keys for the ViewData dictionary in the view.
    /// </summary>
    public static class K
    {
        public const string UserName = "_SESSION_HEADER__USER_NAME";
        public const string NumItemsInCart = "_SESSION_HEADER__NUM_ITEMS_IN_CART";
        public const string UseSessionLayout = "_LAYOUT_USE_SESSION_IN_HEADER";
    }

    /// <summary>
    /// Populates the appropriate fields to transform the layout to include
    /// session information in the header.
    /// </summary>
    public class SessionLayoutFilter : IAsyncActionFilter
    {
        private IServiceProvider _services;

        public SessionLayoutFilter(IServiceProvider services)
        {
            this._services = services;
        }

        async Task IAsyncActionFilter.OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var customerRepository = (Repository.ICustomer)_services.GetService(typeof(Repository.ICustomer));

            var customerId = context.HttpContext.User.FindFirst(claim => claim.Type == Auth.Claim.UserId);

            var controller = (Controller)context.Controller;
            controller.ViewData[K.UseSessionLayout] = true;

            if (customerId != null)
            {
                var customerIdAsGuid = Guid.Parse(customerId.Value);
                var username = context.HttpContext.User.FindFirst(claim => claim.Type == Auth.Claim.UserName).Value;
                var numProductsInCart = await customerRepository.CountProductsInCart(customerIdAsGuid);

                controller.ViewData[K.UserName] = username;
                controller.ViewData[K.NumItemsInCart] = numProductsInCart;
            }

            await next();
        }
    }
}
