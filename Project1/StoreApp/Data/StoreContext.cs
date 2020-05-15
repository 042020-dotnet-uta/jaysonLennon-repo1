using Microsoft.EntityFrameworkCore;

namespace StoreApp.Data
{
    /// <summary>
    /// Context used to access the database.
    /// </summary>
    public class StoreContext : DbContext
    {
        /// <summary>The Products table.</summary>
        public DbSet<Entity.Product> Products { get; set; }

        /// <summary>The Customers table.</summary>
        public DbSet<Entity.Customer> Customers { get; set; }

        /// <summary>The Locations table.</summary>
        public DbSet<Entity.Location> Locations { get; set; }

        /// <summary>The LocationInventories table.</summary>
        public DbSet<Entity.LocationInventory> LocationInventories { get; set; }
        /// <summary>The Orders table.</summary>

        public DbSet<Entity.Order> Orders { get; set; }
        /// <summary>The OrderLineItems table.</summary>

        public DbSet<Entity.OrderLineItem> OrderLineItems { get; set; }
        /// <summary>The Addresses table.</summary>

        public DbSet<Entity.Address> Addresses { get; set; }
        public DbSet<Entity.AddressLine1> AddressLine1s { get; set; }
        public DbSet<Entity.AddressLine2> AddressLine2s { get; set; }
        public DbSet<Entity.ZipCode> ZipCodes { get; set; }
        public DbSet<Entity.State> States { get; set; }
        public DbSet<Entity.City> Cities { get; set; }

        /// <summary>Needed for EF.</summary>
        public StoreContext() { }

        /// <summary>
        /// Create a new database context.
        /// </summary>
        /// <param name="options">The options for configuring the context.</param>
        /// <returns>A new database context.</returns>
        public StoreContext(DbContextOptions<StoreContext> options) : base(options) { }

        /// <summary>
        /// Applies a default configuration it one has not yet been configured.
        /// </summary>
        /// <param name="options">Provided by EF.</param>
        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            if (!options.IsConfigured)
                options
                    .UseSqlite("Data Source=store.sqlite");
        }

        /// <summary>
        /// Table configuration when creating the model.
        /// </summary>
        /// <param name="modelBuilder">Provided by EF.</param>
        protected override void OnModelCreating(ModelBuilder modelBuilder) 
        {
            modelBuilder.Entity<Entity.Order>().Property(t => t.TimeCreated).IsRequired(false);
        }
    }
}