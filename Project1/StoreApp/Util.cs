using System;
using System.Linq;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Mvc;

namespace StoreApp.Util
{
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