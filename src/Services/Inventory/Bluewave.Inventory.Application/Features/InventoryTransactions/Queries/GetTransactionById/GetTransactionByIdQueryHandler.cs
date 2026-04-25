using Bluewave.Inventory.Application.Common.Interfaces;
using Bluewave.Inventory.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Bluewave.Inventory.Application.Features.InventoryTransactions.Queries.GetTransactionById;

public class GetTransactionByIdQueryHandler(IInventoryDbContext context)
    : IRequestHandler<GetTransactionByIdQuery, InventoryTransaction?>
{
    public async Task<InventoryTransaction?> Handle(GetTransactionByIdQuery request, CancellationToken cancellationToken)
    {
        return await context.InventoryTransactions
            .AsNoTracking()
            .Include(t => t.Product)
            .Include(t => t.Warehouse)
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
    }
}