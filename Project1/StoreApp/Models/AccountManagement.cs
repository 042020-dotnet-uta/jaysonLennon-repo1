using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace StoreApp.Models
{
    // TODO: Add constraints
    public class AccountManagement
    {
        [BindNever]
        public string ErrorMessage { get; set; }

        [BindNever]
        public string OkMessage { get; set; }

        [Required]
        [Display(Name = "Default Store", Description = "Store to order items from.")]
        public string DefaultStore { get; set; }

        public List<SelectListItem> Stores { get; } = new List<SelectListItem>();
    }
}