using Microsoft.EntityFrameworkCore;
using OrderApi.Modules.Orders;

namespace OrderApi.Database;

public class OrderDbContext(IConfiguration configuration) : DbContext
{
    public DbSet<Order> Orders { get; set; }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql(configuration.GetConnectionString("workflow"));
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Order>(x =>
        {
            x.ToTable("orders");
        });
    }
}
