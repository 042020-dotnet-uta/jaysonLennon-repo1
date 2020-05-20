using System;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Filters;

namespace StoreApp.FlashMessageExtension
{
    /// <summary>
    /// Excention methods for setting flash messages.
    /// </summary>
    public static class FlashMessageExtension
    {
        /// <summary>
        /// Key to be used when pulling "Info" data in a Razor template.
        /// </summary>
        public const string FlashInfoKey = "_FLASH_FlashInfo";

        /// <summary>
        /// Key to be used when pulling "OK" data in a Razor template.
        /// </summary>
        public const string FlashOkKey = "_FLASH_FlashOk";

        /// <summary>
        /// Key to be used when pulling "Error" data in a Razor template.
        /// </summary>
        public const string FlashErrorKey = "_FLASH_FlashError";

        /// <summary>
        /// Sets a flash message in the "Info" field.
        /// </summary>
        /// <param name="message">The message.</param>
        public static void SetFlashInfo(this Controller controller, string message)
        {
            controller.TempData[FlashInfoKey] = message;
        }

        /// <summary>
        /// Sets a flash message in the "OK" field.
        /// </summary>
        /// <param name="message">The message.</param>
        public static void SetFlashOk(this Controller controller, string message)
        {
            controller.TempData[FlashOkKey] = message;
        }

        /// <summary>
        /// Sets a flash message in the "Error" field.
        /// </summary>
        /// <param name="message">The message.</param>
        public static void SetFlashError(this Controller controller, string message)
        {
            controller.TempData[FlashErrorKey] = message;
        }

        /// <summary>
        /// Retrieves a flash message from the "Info" field.
        /// </summary>
        public static string GetFlashInfo(this Controller controller)
        {
            return controller.TempData[FlashInfoKey] as string;
        }

        /// <summary>
        /// Retrieves a flash message from the "Ok" field.
        /// </summary>
        public static string GetFlashOk(this Controller controller)
        {
            return controller.TempData[FlashOkKey] as string;
        }

        /// <summary>
        /// Retrieves a flash message from the "Error" field.
        /// </summary>
        public static string GetFlashError(this Controller controller)
        {
            return controller.TempData[FlashErrorKey] as string;
        }
    }
}

namespace StoreApp.FlashMessage
{
    using StoreApp.FlashMessageExtension;

    /// <summary>
    /// Constants used as Keys for the ViewData dictionary in the view.
    /// </summary>
    public static class K
    {
        /// <summary>
        /// Key to use for accessing flash "Info" field.
        /// </summary>
        public const string FlashInfo = FlashMessageExtension.FlashInfoKey;

        /// <summary>
        /// Key to use for accessing flash "Ok" field.
        /// </summary>
        public const string FlashOk = FlashMessageExtension.FlashOkKey;

        /// <summary>
        /// Key to use for accessing flash "Error" field.
        /// </summary>
        public const string FlashError = FlashMessageExtension.FlashErrorKey;
    }

    /// <summary>
    /// Populates the appropriate fields to transform the layout to include
    /// session information in the header.
    /// </summary>
    public class FlashMessageFilter : IAsyncActionFilter
    {
        private IServiceProvider _services;

        /// <summary>
        /// Standard constructor.
        /// </summary>
        public FlashMessageFilter(IServiceProvider services)
        {
            this._services = services;
        }

        async Task IAsyncActionFilter.OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var controller = (Controller)context.Controller;

            // Set the data.
            controller.ViewData[K.FlashInfo] = controller.GetFlashInfo();
            controller.ViewData[K.FlashOk] = controller.GetFlashOk();
            controller.ViewData[K.FlashError] = controller.GetFlashError();

            await next();
        }
    }
}
