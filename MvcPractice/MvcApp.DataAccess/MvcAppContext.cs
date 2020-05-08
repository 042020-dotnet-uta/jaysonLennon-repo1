using Microsoft.EntityFrameworkCore;
using MvcApp.Domain;

namespace MvcApp.DataAccess
{
    public class MvcAppContext : DbContext
    {
        public MvcAppContext(DbContextOptions<MvcAppContext> options)
            : base(options)
        {
        }
        public DbSet<Department> Departments { get; set; }
        public DbSet<Employee> Employees { get; set; }
    }
}