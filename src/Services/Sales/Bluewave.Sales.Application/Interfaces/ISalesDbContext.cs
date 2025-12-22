using Bluewave.Sales.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Bluewave.Sales.Application.Interfaces;

public interface ISalesDbContext
{
    DbSet<Order> Orders { get; }
    DbSet<OrderItem> OrderItems { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}