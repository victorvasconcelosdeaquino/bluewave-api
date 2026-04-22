using Bluewave.Inventory.Domain.Enums;
using MediatR;

namespace Bluewave.Inventory.Application.Features.InventoryTransactions.Commands.CreateTransaction;

public record CreateTransactionCommand(
    Guid ProductId,
    Guid WarehouseId,
    TransactionType TransactionType,
    decimal Quantity,
    string? BatchNumber,
    decimal? UnitCost,
    string? ReferenceDocument,
    string? Notes
) : IRequest<Guid>;