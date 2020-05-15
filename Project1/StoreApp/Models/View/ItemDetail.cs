using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace StoreApp.Model.View
{
    public class ItemDetail {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string ImageName { get; set; }
        public double UnitPrice { get; set; }
        public bool NotFound { get; set; } = false;
        public int Stock { get; set; }

        public static ItemDetail ItemNotFound()
        {
            var model = new ItemDetail();
            model.NotFound = true;
            return model;
        }
    }
}