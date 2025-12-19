using MediatR;

namespace Bluewave.Inventory.Application.Features.Stock.Queries.GetStockOverview;

public record GetStockOverviewQuery : IRequest<List<StockOverviewDto>>;