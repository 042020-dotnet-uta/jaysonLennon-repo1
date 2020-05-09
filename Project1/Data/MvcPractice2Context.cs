using Microsoft.EntityFrameworkCore;

namespace MvcPractice2.Data
{
    public class MvcPractice2Context : DbContext
    {
        public MvcPractice2Context (DbContextOptions<MvcPractice2Context> options)
            : base(options)
        {
        }

        public DbSet<Models.HelloWorld> Movie { get; set; }
    }
}