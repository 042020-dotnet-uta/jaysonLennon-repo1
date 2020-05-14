using System;
using System.Linq;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Mvc;

namespace StoreApp.Util
{
    public static class Flash
    {
        public const string FlashInfoKey = "FlashInfo";
        public const string FlashOkKey = "FlashOk";
        public const string FlashErrorKey = "FlashError";

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

    public static class Hash
    {
        /// <summary>
        /// Performs a SHA256 hash on the given input.
        /// </summary>
        /// <param name="input"><c>String</c> to hash.</param>
        /// <returns>A hashed <c>String</c>.</returns>
        public static String Sha256(string input)
        {
            using (var sha256 = SHA256.Create())
            {
                var bytes = input.ToCharArray().Select(c => (byte)c).ToArray();
                var hash = sha256.ComputeHash(bytes);
                var encoded = Convert.ToBase64String(hash);
                return encoded;
            }
        }
    }

}