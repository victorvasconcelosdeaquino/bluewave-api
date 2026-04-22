using Bluewave.Inventory.Domain.Entities;
using MediatR;

namespace Bluewave.Inventory.Application.Features.InventoryTransactions.Queries.GetAllTransactions;

public record GetAllTransactionsQuery(Guid? ProductId = null) : IRequest<List<InventoryTransaction>>;