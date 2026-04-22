using MediatR;

namespace Bluewave.Inventory.Application.Features.Products.Commands.UpdateProduct;

public record UpdateProductCommand(
    Guid Id,
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
) : IRequest<bool>;