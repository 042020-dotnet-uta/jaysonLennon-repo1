using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using StoreApp.Repository;

namespace StoreApp.AdminModel
{
    /// <summary>
    /// List of stores used when picking which store to view orders from.
    /// </summary>
    public class StoreList {
        [Required]
        [Display(Name = "Store", Description = "Store to check orders for.")]
        public string StorePicked { get; set; }
        public List<SelectListItem> Stores { get; } = new List<SelectListItem>();
    }
}