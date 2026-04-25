using Bluewave.Inventory.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Bluewave.Inventory.Application.Common.Interfaces;

public interface IInventoryDbContext
{
    DbSet<Product> Products { get; }
    DbSet<InventoryTransaction> InventoryTransactions { get; }
    DbSet<Warehouse> Warehouses { get; }
    DbSet<MeasurementUnit> MeasurementUnits { get; }
    DbSet<ProductCategory> Categories { get; }
    DbSet<Supplier> Suppliers { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}