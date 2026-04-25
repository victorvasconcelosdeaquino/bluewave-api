using Bluewave.Inventory.Application.Common.Interfaces;
using Bluewave.Inventory.Domain.Entities;
using MediatR;

namespace Bluewave.Inventory.Application.Features.InventoryTransactions.Commands.CreateTransaction;

public class CreateTransactionCommandHandler(IInventoryDbContext context)
    : IRequestHandler<CreateTransactionCommand, Guid>
{
    public async Task<Guid> Handle(CreateTransactionCommand request, CancellationToken cancellationToken)
    {
        var entity = new InventoryTransaction(
            request.ProductId, request.WarehouseId, request.TransactionType, request.Quantity,
            request.BatchNumber, request.UnitCost, request.ReferenceDocument, request.Notes);

        context.InventoryTransactions.Add(entity);
        await context.SaveChangesAsync(cancellationToken);

        // TODO: Maybe here is where we could publish an event to RabbitMQ
        // informing that the stock of a product has changed!

        return entity.Id;
    }
}