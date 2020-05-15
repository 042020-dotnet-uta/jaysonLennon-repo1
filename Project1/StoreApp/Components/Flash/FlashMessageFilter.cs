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

namespace StoreApp.FlashMessageExtention
{
    public static class FlashMessageExtention
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
    using StoreApp.FlashMessageExtention;

    /// <summary>
    /// Constants used as Keys for the ViewData dictionary in the view.
    /// </summary>
    public static class K
    {
        public const string FlashInfo = FlashMessageExtention.FlashOkKey;
        public const string FlashOk = FlashMessageExtention.FlashOkKey;
        public const string FlashError = FlashMessageExtention.FlashErrorKey;
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

            controller.ViewData[K.FlashInfo] = controller.GetFlashInfo();
            controller.ViewData[K.FlashOk] = controller.GetFlashOk();
            controller.ViewData[K.FlashError] = controller.GetFlashError();

            await next();
        }
    }
}
