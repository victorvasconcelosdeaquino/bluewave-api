using Bluewave.Inventory.Application.Common.Interfaces;
using Bluewave.Inventory.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace Bluewave.Inventory.Infrastructure.Persistence;

public class InventoryDbContext(DbContextOptions<InventoryDbContext> options) : DbContext(options), IInventoryDbContext
{
    public DbSet<Product> Products { get; set; }
    public DbSet<ProductCategory> Categories { get; set; }
    public DbSet<InventoryTransaction> InventoryTransactions { get; set; }
    public DbSet<Warehouse> Warehouses { get; set; }
    public DbSet<Supplier> Suppliers { get; set; }
    public DbSet<MeasurementUnit> MeasurementUnits { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        modelBuilder.Entity<Warehouse>().HasQueryFilter(w => w.IsActive);
        base.OnModelCreating(modelBuilder);
    }
}