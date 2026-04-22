// File: CreateProductCommand.cs
using MediatR;

namespace Bluewave.Inventory.Application.Features.Products.Commands.CreateProduct;

public record CreateProductCommand(
    string Sku,
    string Name,
    string? Description,
    Guid CategoryId,
    Guid UomId,
    Guid? PreferredSupplierId,
    decimal MinStockLevel,
    decimal? MaxStockLevel,
    bool IsPerishable,
    bool RequiresColdChain,
    string? ActiveIngredient,
    decimal? Concentration,
    decimal StandardCost
) : IRequest<Guid>;