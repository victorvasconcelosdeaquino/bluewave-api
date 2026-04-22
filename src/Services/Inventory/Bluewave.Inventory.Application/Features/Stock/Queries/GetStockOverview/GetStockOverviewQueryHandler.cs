// Note: In pure CQRS, the Application defines the DbContext interface.
// To simplify here, let's assume the Handler can read the Entities.

// We'll need to inject the DbContext. Since the DbContext is in the Infra,
// we usually use an IApplicationDbContext interface in the Application layer.
// But to move quickly without creating extra interfaces now, we'll use a pragmatic approach:
// The Handler will project directly from the Entities.

using Bluewave.Inventory.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Bluewave.Inventory.Application.Features.Stock.Queries.GetStockOverview;

public class GetStockOverviewQueryHandler(IInventoryDbContext context)
    : IRequestHandler<GetStockOverviewQuery, List<StockOverviewDto>>
{
    public async Task<List<StockOverviewDto>> Handle(GetStockOverviewQuery request, CancellationToken cancellationToken)
    {
        // 1. Get all products
        // 2. Perform a Left Join with transactions to sum the stock

        var stockData = await context.Products
            .AsNoTracking() // Performance: We don't need to track changes on read
            .Include(p => p.Category)
            .Include(p => p.Uom)
            .Select(p => new StockOverviewDto(
                p.Id,
                p.Sku,
                p.Name,
                p.Category!.Name,
                // Subquery to sum the current stock of this product
                context.Transactions
                    .Where(t => t.ProductId == p.Id)
                    .Sum(t => t.Quantity),
                p.Uom!.Code
            ))
            .ToListAsync(cancellationToken);

        return stockData;
    }
}