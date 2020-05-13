using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace StoreApp.Models
{
    public class Storefront
    {
        /// <summary>
        /// The products that should be displayed on the storefront page.
        /// </summary>
        public List<Tuple<Entity.Product, int>> products;
    }
}