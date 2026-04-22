using Bluewave.Inventory.Application.Common.Interfaces;
using Bluewave.Inventory.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Bluewave.Inventory.Application.Features.InventoryTransactions.Queries.GetAllTransactions;

public class GetAllTransactionsQueryHandler(IInventoryDbContext context)
    : IRequestHandler<GetAllTransactionsQuery, List<InventoryTransaction>>
{
    public async Task<List<InventoryTransaction>> Handle(GetAllTransactionsQuery request, CancellationToken cancellationToken)
    {
        var query = context.Transactions
            .AsNoTracking()
            .Include(t => t.Product)
            .Include(t => t.Warehouse)
            .AsQueryable();

        // Optional filter: bring only transactions from a specific product
        if (request.ProductId.HasValue)
        {
            query = query.Where(t => t.ProductId == request.ProductId.Value);
        }

        return await query.OrderByDescending(x => x.CreatedAt).ToListAsync(cancellationToken);
    }
}