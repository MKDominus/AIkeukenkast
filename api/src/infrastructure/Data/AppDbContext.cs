using api.Domain;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace api.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Product> Products => Set<Product>();
    public DbSet<Scan> Scans => Set<Scan>();
    public DbSet<DetectedProduct> DetectedProducts => Set<DetectedProduct>();
    public DbSet<Ingredient> Ingredients => Set<Ingredient>();
    public DbSet<Municipality> Municipalities => Set<Municipality>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}
