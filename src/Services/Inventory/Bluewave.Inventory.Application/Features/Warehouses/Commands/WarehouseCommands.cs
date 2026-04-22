using Bluewave.Inventory.Application.Common.Interfaces;
using Bluewave.Inventory.Domain.Entities;
using MediatR;

namespace Bluewave.Inventory.Application.Features.Warehouses.Commands;

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

        entity.Deactivate(); // Soft Delete lógico
        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }
}