using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;


namespace MvcApp.Domain
{
    public class Department
    {
        [Required]
        public int Id { get; set; }
        [Required]
        [Display(Name="Department Name")]
        public string Name { get; set; }
        public ICollection<Employee> Employees { get; set; }
    }
}
