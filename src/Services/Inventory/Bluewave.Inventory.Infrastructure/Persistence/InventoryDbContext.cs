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

        // Avoidance of duplicate SKUs: No two products can have the same SKU code
        modelBuilder.Entity<Product>()
            .HasIndex(p => p.Sku)
            .IsUnique();

        // Concurrency control: The Version property is used to implement optimistic concurrency control, preventing data conflicts during updates.
        modelBuilder.Entity<Product>()
            .Property(p => p.Version)
            .IsRowVersion();

        // Each warehouse must have a unique code to prevent confusion and ensure accurate inventory tracking.
        modelBuilder.Entity<Warehouse>()
            .HasIndex(w => w.Code)
            .IsUnique();

        // Each supplier must have a unique TaxId (CNPJ) to prevent duplicate entries and ensure accurate supplier management.
        modelBuilder.Entity<Supplier>()
            .HasIndex(s => s.TaxId)
            .IsUnique();

        // Each measurement unit must have a unique code to prevent confusion and ensure accurate inventory tracking.
        modelBuilder.Entity<MeasurementUnit>()
            .HasIndex(m => m.Code)
            .IsUnique();

        base.OnModelCreating(modelBuilder);
    }
}