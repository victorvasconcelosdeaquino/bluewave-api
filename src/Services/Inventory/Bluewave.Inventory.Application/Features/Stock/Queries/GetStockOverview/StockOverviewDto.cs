namespace Bluewave.Inventory.Application.Features.Stock.Queries.GetStockOverview;

public record StockOverviewDto(
    Guid ProductId,
    string Sku,
    string ProductName,
    string Category,
    decimal QuantityOnHand,
    string UomCode
);