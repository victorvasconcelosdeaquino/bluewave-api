using MediatR;

namespace Bluewave.Inventory.Application.Features.Stock.Commands.ReceiveStock;

public record ReceiveStockCommand(
    Guid ProductId,
    Guid WarehouseId,
    decimal Quantity,
    string ReferenceDocument,
    string? Notes
) : IRequest<Guid>; // Retorna o ID da nova transação