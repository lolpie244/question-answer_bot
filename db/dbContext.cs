using helping;
using Microsoft.EntityFrameworkCore;
using settings;
namespace db_namespace;


public class dbContext : DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<UserData> UsersData { get; set; }
    public DbSet<Chat> Chats { get; set; }
    public DbSet<Archive> Archive { get; set; }
    public static string ConnectionString { get; set; }
    public dbContext() : base()
    {
    }
    public dbContext(DbContextOptions<dbContext> options) : base(options)
    {
    }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql(ConnectionString);
        base.OnConfiguring(optionsBuilder);
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        Seeders.Seed(modelBuilder);
        base.OnModelCreating(modelBuilder);
    }
    
}
