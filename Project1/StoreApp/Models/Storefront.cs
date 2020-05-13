using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace StoreApp.Models
{
    public class Storefront
    {
        public List<Tuple<Entity.Product, int>> products;
    }
}