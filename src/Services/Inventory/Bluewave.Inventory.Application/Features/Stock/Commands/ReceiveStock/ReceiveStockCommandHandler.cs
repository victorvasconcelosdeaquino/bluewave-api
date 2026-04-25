using MediatR;
using Microsoft.EntityFrameworkCore;
using Bluewave.Inventory.Domain.Entities;
using Bluewave.Inventory.Domain.Enums;
using Bluewave.Inventory.Application.Common.Interfaces;

namespace Bluewave.Inventory.Application.Features.Stock.Commands.ReceiveStock;

public class ReceiveStockCommandHandler : IRequestHandler<ReceiveStockCommand, Guid>
{
    private readonly IInventoryDbContext _dbContext;

    public ReceiveStockCommandHandler(IInventoryDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Guid> Handle(ReceiveStockCommand request, CancellationToken cancellationToken)
    {
        var productExists = await _dbContext.Products.AnyAsync(p => p.Id == request.ProductId, cancellationToken);
        if (!productExists)
            throw new Exception($"Product with ID {request.ProductId} not found.");

        var warehouseExists = await _dbContext.Warehouses.AnyAsync(w => w.Id == request.WarehouseId, cancellationToken);
        if (!warehouseExists)
            throw new Exception($"Warehouse with ID {request.WarehouseId} not found.");

        var transaction = new InventoryTransaction(
            request.ProductId,
            request.WarehouseId,
            TransactionType.InboundPurchase,
            request.Quantity,
            null,
            null,
            request.ReferenceDocument,
            request.Notes
        );

        // 3. Salva no banco de dados
        _dbContext.InventoryTransactions.Add(transaction);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return transaction.Id;
    }
}