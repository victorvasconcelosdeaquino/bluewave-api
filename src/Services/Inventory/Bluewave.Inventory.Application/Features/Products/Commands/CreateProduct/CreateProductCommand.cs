using MediatR;

namespace Bluewave.Inventory.Application.Features.Products.Commands.CreateProduct;

// O comando retorna o Guid do produto criado
public record CreateProductCommand(
    string Sku,
    string Name,
    string Description,
    Guid CategoryId,
    Guid UomId,
    decimal MinStockLevel,
    decimal StandardCost
) : IRequest<Guid>;