using Bluewave.Inventory.Application.Common.Interfaces;
using Bluewave.Inventory.Domain.Entities;
using Bluewave.Inventory.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Bluewave.Inventory.Application.Features.Warehouses.Commands.CreateWarehouse;

// DTOs
public record CreateWarehouseCommand(string Name, string? Code, string? Address, bool IsVirtual) : IRequest<Guid>;
public record UpdateWarehouseCommand(Guid Id, string Name, string? Code, string? Address, bool IsVirtual) : IRequest<bool>;
public record DeactivateWarehouseCommand(Guid Id) : IRequest<bool>;

// HANDLERS
public class WarehouseCommandHandlers :
    IRequestHandler<CreateWarehouseCommand, Guid>,
    IRequestHandler<UpdateWarehouseCommand, bool>,
    IRequestHandler<DeactivateWarehouseCommand, bool>
{
    private readonly IInventoryDbContext _context;

    public WarehouseCommandHandlers(IInventoryDbContext context)
    {
        _context = context;
    }

    public async Task<Guid> Handle(CreateWarehouseCommand request, CancellationToken cancellationToken)
    {
        var entity = new Warehouse(request.Name, request.Code, request.Address, request.IsVirtual);

        _context.Warehouses.Add(entity);
        await _context.SaveChangesAsync(cancellationToken);

        return entity.Id;
    }

    public async Task<bool> Handle(UpdateWarehouseCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.Warehouses.FindAsync(new object[] { request.Id }, cancellationToken);
        if (entity == null) return false;

        entity.Update(request.Name, request.Code, request.Address, request.IsVirtual);
        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }

    public async Task<bool> Handle(DeactivateWarehouseCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.Warehouses.FindAsync(new object[] { request.Id }, cancellationToken);
        if (entity == null) return false;

        var currentBalance = await _context.InventoryTransactions
            .Where(t => t.WarehouseId == request.Id)
            .SumAsync(t =>
                (t.TransactionType == TransactionType.InboundPurchase) ? t.Quantity :
                (t.TransactionType == TransactionType.OutboundSales) ? -t.Quantity : 0,
                cancellationToken);

        if (currentBalance > 0)
        {
            throw new InvalidOperationException($"Cannot deactivate warehouse '{entity.Name}' because it still has {currentBalance} items in stock. Transfer the stock first.");
        }

        entity.Deactivate();
        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }
}