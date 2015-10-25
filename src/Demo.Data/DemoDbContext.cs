using System.Data.Entity;

namespace Demo.Data
{
    public class DemoDbContext : DbContext
    {
        public DemoDbContext(string connString) : base(connString) { }

        public DbSet<Property> Properties { get; set; }
    }
}
