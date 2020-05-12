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
        public const string IsLoggedIn = "_SESSION_HEADER__USER_NAME";
        public const string UserName = "_SESSION_HEADER__USER_NAME";
        public const string NumItemsInCart = "_SESSION_HEADER__NUM_ITEMS_IN_CART";
        public const string UseSessionLayout = "_LAYOUT_USE_SESSION_IN_HEADER";
    }

    /// <summary>
    /// Populates the appropriate fields to transform the layout to include
    /// session information in the header.
    /// </summary>
    public class UseLayout : IAsyncActionFilter
    {
        private IServiceProvider _services;

        public UseLayout(IServiceProvider services)
        {
            this._services = services;
        }

        async Task IAsyncActionFilter.OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            //var customerRepository = (Repository.ICustomer)_services.GetService(typeof(Repository.ICustomer));

            // Use claims system to get username for display.
            // Can also query the database via customer context for additional info.
            var username = context.HttpContext.User.FindFirst(claim => claim.Type == Auth.Claim.UserName);

            var controller = (Controller)context.Controller;

            if (username != null)
            {
                controller.ViewData[K.UserName] = username.Value;
                // Change the layout.
                controller.ViewData[K.UseSessionLayout] = true;
            }
            else
            {
                controller.ViewData[K.UserName] = "no user name";
            }

            await next();
        }
    }
}
