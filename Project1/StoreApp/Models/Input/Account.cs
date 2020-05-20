using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace StoreApp.Model.Input
{
    /// <summary>
    /// Used for the "Create account" page.
    /// </summary>
    public class CreateAccount {

        /// <summary>The user's login name.</summary>
        [Required(ErrorMessage = "A user name is required.")]
        [Display(Name = "User Name", Description = "Unique name used to log in.")]
        [Remote(action: "VerifyUserName", controller: "Account")]
        public string UserName { get; set; }

        /// <summary>Password used to login.</summary>
        [Required(ErrorMessage = "A password is required.")]
        [MinLength(8, ErrorMessage = "The password must be at least 8 characters long.")]
        [Display(Name = "Password", Description = "Password used to log in.")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }

    /// <summary>
    /// For validating user information on the login page.
    /// </summary>
    public class LoginUser {

        /// <summary>The user's login name.</summary>
        [Required(ErrorMessage = "A user name is required.")]
        [Display(Name = "User Name", Description = "Unique name used to log in.")]
        public string UserName { get; set; }

        /// <summary>Password used to login.</summary>
        [Required(ErrorMessage = "A password is required.")]
        [MinLength(8, ErrorMessage = "The password must be at least 8 characters long.")]
        [Display(Name = "Password", Description = "Password used to log in.")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        /// <summary>The URL to return to, if this was a redirect.</summary>
        public string ReturnUrl { get; set; }
    }


    /// <summary>
    /// For handling a redirect to the login page from another page that requires
    /// authorization.
    /// <remarks>
    /// This type is for forwarding error messages and return urls to the LoginUser model.
    /// We need to do this so the LoginUser model doesn't immediately produce client-side
    /// verification errors.
    /// </remarks>
    /// </summary>
    public class LoginRedirect
    {
        /// <summary>The URL to return to, if this was a redirect.</summary>
        public string ReturnUrl { get; set; }
    }
}