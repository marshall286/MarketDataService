using MarketDataService.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace MarketDataService.Infrastructure.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<Asset> Assets { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Asset>()
            .HasIndex(a => a.Symbol)
            .IsUnique();
    }
}