using Bluewave.Inventory.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Bluewave.Inventory.Application.Common.Interfaces;

public interface IInventoryDbContext
{
    DbSet<Product> Products { get; }
    DbSet<InventoryTransaction> Transactions { get; }
    DbSet<Warehouse> Warehouses { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}