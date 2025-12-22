using Bluewave.Sales.Application.Interfaces;
using Bluewave.Sales.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Bluewave.Sales.Infrastructure.Persistence;

public class SalesDbContext : DbContext, ISalesDbContext
{
    public SalesDbContext(DbContextOptions<SalesDbContext> options) : base(options) { }

    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderItem> OrderItems => Set<OrderItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(SalesDbContext).Assembly);
    }
}