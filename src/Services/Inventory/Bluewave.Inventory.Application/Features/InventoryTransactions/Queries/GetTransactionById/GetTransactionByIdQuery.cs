using Bluewave.Inventory.Domain.Entities;
using MediatR;

namespace Bluewave.Inventory.Application.Features.InventoryTransactions.Queries.GetTransactionById;

public record GetTransactionByIdQuery(Guid Id) : IRequest<InventoryTransaction?>;