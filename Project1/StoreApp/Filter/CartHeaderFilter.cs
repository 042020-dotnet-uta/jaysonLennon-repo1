using System;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Filters;

namespace StoreApp.CartHeader
{
    /// <summary>
    /// Constants used as Keys for the ViewData dictionary in the view.
    /// </summary>
    public static class K
    {
        public const string UserName = "_CART_HEADER__USER_NAME";
        public const string NumItemsInCart = "_CART_HEADER__NUM_ITEMS_IN_CART";
        public const string UseCartHeader = "_CART_HEADER__USE_CART_HEADER";
    }

    /// <summary>
    /// Populates the appropriate fields to transform the layout to include
    /// session information in the header.
    /// </summary>
    public class CartHeaderFilter : IAsyncActionFilter
    {
        private IServiceProvider _services;

        public CartHeaderFilter(IServiceProvider services)
        {
            this._services = services;
        }

        async Task IAsyncActionFilter.OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var userRepository = (Repository.IUser)_services.GetService(typeof(Repository.IUser));

            var userId = context.HttpContext.User.FindFirst(claim => claim.Type == Auth.Claim.UserId);

            var controller = (Controller)context.Controller;
            controller.ViewData[K.UseCartHeader] = true;

            if (userId != null)
            {
                var userIdAsGuid = Guid.Parse(userId.Value);
                var username = context.HttpContext.User.FindFirst(claim => claim.Type == Auth.Claim.UserName).Value;
                var numProductsInCart = await userRepository.CountProductsInCart(userIdAsGuid);

                controller.ViewData[K.UserName] = username;
                controller.ViewData[K.NumItemsInCart] = numProductsInCart;
            }

            await next();
        }
    }
}
