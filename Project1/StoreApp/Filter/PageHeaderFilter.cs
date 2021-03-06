using System;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Filters;

namespace StoreApp.PageHeader
{
    /// <summary>
    /// Constants used as Keys for the ViewData dictionary in the view.
    /// </summary>
    public static class K
    {
        /// <summary>
        /// Key to use when accessing the user name.
        /// </summary>
        public const string UserName = "_HEADER__USER_NAME";

        /// <summary>
        /// Key to use when accessing the number of items in the cart.
        /// </summary>
        public const string NumItemsInCart = "_HEADER__NUM_ITEMS_IN_CART";
    }

    /// <summary>
    /// Populates the appropriate fields to transform the layout to include
    /// session information in the header.
    /// </summary>
    public class PopulateHeader : IAsyncActionFilter
    {
        private IServiceProvider _services;

        /// <summary>
        /// Standard constructor.
        /// </summary>
        public PopulateHeader(IServiceProvider services)
        {
            this._services = services;
        }

        async Task IAsyncActionFilter.OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var userRepository = (Repository.IUser)_services.GetService(typeof(Repository.IUser));

            var userId = context.HttpContext.User.FindFirst(claim => claim.Type == Auth.Claim.UserId);

            var controller = (Controller)context.Controller;

            if (userId != null)
            {
                var userIdAsGuid = Guid.Parse(userId.Value);
                var username = context.HttpContext.User.FindFirst(claim => claim.Type == Auth.Claim.UserName).Value;
                var numProductsInCart = await userRepository.CountProductsInCart(userIdAsGuid);

                // Set info.
                controller.ViewData[K.UserName] = username;
                controller.ViewData[K.NumItemsInCart] = numProductsInCart;
            }

            await next();
        }
    }
}
