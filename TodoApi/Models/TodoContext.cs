using Microsoft.EntityFrameworkCore;

namespace TodoApi.Data
{
    public class TodoContext : DbContext
    {
        public TodoContext(DbContextOptions<TodoContext> options)
            : base(options)
        {
        }

        public DbSet<TodoItem> TodoItems { get; set; }
        public DbSet<DoNotDoItem> DoNotDoItems { get; set; }
    }
}