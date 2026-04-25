using Bluewave.Inventory.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Bluewave.Inventory.Application.Features.Stock.Queries.GetTransactionHistory;

public class GetTransactionHistoryQueryHandler : IRequestHandler<GetTransactionHistoryQuery, PagedList<TransactionDto>>
{
    private readonly IInventoryDbContext _context;

    public GetTransactionHistoryQueryHandler(IInventoryDbContext context)
    {
        _context = context;
    }

    public async Task<PagedList<TransactionDto>> Handle(GetTransactionHistoryQuery request, CancellationToken cancellationToken)
    {
        var query = _context.InventoryTransactions.AsNoTracking();

        if (request.ProductId.HasValue && request.ProductId != Guid.Empty)
        {
            query = query.Where(t => t.ProductId == request.ProductId.Value);
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(t => t.Id)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(t => new TransactionDto(
                t.Id,
                t.ProductId,
                t.TransactionType.ToString(),
                t.Quantity,
                t.CreatedAt,
                t.ReferenceDocument
            ))
            .ToListAsync(cancellationToken);

        return new PagedList<TransactionDto>(items, totalCount, request.Page, request.PageSize);
    }
}