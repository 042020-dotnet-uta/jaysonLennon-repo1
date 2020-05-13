using Microsoft.EntityFrameworkCore;
using Microsoft.Data.Sqlite;

using StoreApp.Data;

namespace TestStoreApp
{
    public class TestUtil
    {
        public static DbContextOptions<StoreContext> GetMemDbOptions(string dbName)
        {
            var sqlite = new SqliteConnection("Filename=:memory:");
            sqlite.Open();
            var options = new DbContextOptionsBuilder<StoreContext>()
                       //.UseInMemoryDatabase(databaseName: dbName)
                       .UseSqlite(sqlite)
                       .Options;

            using (var db = new StoreContext(options))
            {
                db.Database.Migrate();
            }

            return options;
        }
    }

}
