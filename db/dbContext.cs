using Microsoft.EntityFrameworkCore;
using settings;
namespace db_namespace;


public class dbContext: DbContext
{
    public DbSet<User> Users { get; set; }

    public dbContext(): base()
    {
    }

    public dbContext(DbContextOptions<dbContext> options) : base(options)
    {
            
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        Seeders.Seed(modelBuilder);
        base.OnModelCreating(modelBuilder);
    }
}
