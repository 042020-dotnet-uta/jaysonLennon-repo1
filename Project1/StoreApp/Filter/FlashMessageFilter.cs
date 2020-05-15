using System.Runtime.CompilerServices;
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

namespace StoreApp.FlashMessageExtension
{
    public static class FlashMessageExtension
    {
        public const string FlashInfoKey = "_FLASH_FlashInfo";
        public const string FlashOkKey = "_FLASH_FlashOk";
        public const string FlashErrorKey = "_FLASH_FlashError";

        public static void SetFlashInfo(this Controller controller, string message)
        {
            controller.TempData[FlashInfoKey] = message;
        }

        public static void SetFlashOk(this Controller controller, string message)
        {
            controller.TempData[FlashOkKey] = message;
        }

        public static void SetFlashError(this Controller controller, string message)
        {
            Console.WriteLine($"set flash error to {message}");
            controller.TempData[FlashErrorKey] = message;
        }

        public static string GetFlashInfo(this Controller controller)
        {
            return controller.TempData[FlashInfoKey] as string;
        }

        public static string GetFlashOk(this Controller controller)
        {
            return controller.TempData[FlashOkKey] as string;
        }

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
        public const string FlashInfo = FlashMessageExtension.FlashOkKey;
        public const string FlashOk = FlashMessageExtension.FlashOkKey;
        public const string FlashError = FlashMessageExtension.FlashErrorKey;
    }

    /// <summary>
    /// Populates the appropriate fields to transform the layout to include
    /// session information in the header.
    /// </summary>
    public class FlashMessageFilter : IAsyncActionFilter
    {
        private IServiceProvider _services;

        public FlashMessageFilter(IServiceProvider services)
        {
            this._services = services;
        }

        async Task IAsyncActionFilter.OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var controller = (Controller)context.Controller;
            Console.WriteLine("building flash info in filter...");

            controller.ViewData[K.FlashInfo] = controller.GetFlashInfo();
            controller.ViewData[K.FlashOk] = controller.GetFlashOk();
            controller.ViewData[K.FlashError] = controller.GetFlashError();

            Console.WriteLine($"flashinfo = {controller.ViewData[K.FlashInfo]}");
            Console.WriteLine($"flashok = {controller.ViewData[K.FlashOk]}");
            Console.WriteLine($"flasherror = {controller.ViewData[K.FlashError]}");

            await next();
        }
    }
}
