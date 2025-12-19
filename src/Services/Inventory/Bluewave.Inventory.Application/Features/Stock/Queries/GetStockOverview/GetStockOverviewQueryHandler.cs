// Nota: Em CQRS purista, a Application define a Interface do DbContext.
// Para simplificar aqui, vamos assumir que o Handler pode ler as Entidades.

// Vamos precisar injetar o DbContext. Como o DbContext está na Infra, 
// normalmente usamos uma interface IApplicationDbContext na camada Application.
// Mas para avançarmos rápido sem criar interfaces extras agora, vamos usar uma abordagem pragmática:
// O Handler vai projetar direto das Entidades.

using Bluewave.Inventory.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Bluewave.Inventory.Application.Features.Stock.Queries.GetStockOverview;

public class GetStockOverviewQueryHandler(IInventoryDbContext context)
    : IRequestHandler<GetStockOverviewQuery, List<StockOverviewDto>>
{
    public async Task<List<StockOverviewDto>> Handle(GetStockOverviewQuery request, CancellationToken cancellationToken)
    {
        // 1. Pegamos todos os produtos
        // 2. Fazemos um Left Join com as transações para somar o estoque

        var stockData = await context.Products
            .AsNoTracking() // Performance: Não precisamos rastrear mudanças em leitura
            .Include(p => p.Category)
            .Include(p => p.Uom)
            .Select(p => new StockOverviewDto(
                p.Id,
                p.Sku,
                p.Name,
                p.Category!.Name,
                // Subquery para somar o estoque atual deste produto
                context.Transactions
                    .Where(t => t.ProductId == p.Id)
                    .Sum(t => t.Quantity),
                p.Uom!.Code
            ))
            .ToListAsync(cancellationToken);

        return stockData;
    }
}