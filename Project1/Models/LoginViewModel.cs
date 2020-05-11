using System.ComponentModel.DataAnnotations;

namespace StoreApp.Models
{
    public class LoginViewModel {

        [Required(ErrorMessage = "A user name is required.")]
        [Display(Name = "User Name", Description = "Unique name used to log in.")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "A password is required.")]
        [MinLength(8, ErrorMessage = "The password must be at least 8 characters long.")]
        [Display(Name = "Password", Description = "Password used to log in.")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}