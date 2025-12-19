using Bluewave.Inventory.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Bluewave.Inventory.Application.Common.Interfaces;

public interface IInventoryDbContext
{
    DbSet<Product> Products { get; }
    DbSet<InventoryTransaction> Transactions { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}