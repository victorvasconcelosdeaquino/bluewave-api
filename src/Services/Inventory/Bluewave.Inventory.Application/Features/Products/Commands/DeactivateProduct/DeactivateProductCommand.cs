using MediatR;

namespace Bluewave.Inventory.Application.Features.Products.Commands.DeactivateProduct;

public record DeactivateProductCommand(Guid Id) : IRequest<bool>;