using Bluewave.Inventory.Application.Common.Interfaces;
using Bluewave.Inventory.Domain.Entities;
using MediatR;

namespace Bluewave.Inventory.Application.Features.MeasurementUnits.Commands.CreateMeasurementUnit;

// DTOs
public record CreateMeasurementUnitCommand(string Code, string Name, string? Description) : IRequest<Guid>;
public record UpdateMeasurementUnitCommand(Guid Id, string Name, string? Description) : IRequest<bool>;
public record DeleteMeasurementUnitCommand(Guid Id) : IRequest<bool>;

// HANDLERS
public class MeasurementUnitCommandHandlers :
    IRequestHandler<CreateMeasurementUnitCommand, Guid>,
    IRequestHandler<UpdateMeasurementUnitCommand, bool>,
    IRequestHandler<DeleteMeasurementUnitCommand, bool>
{
    private readonly IInventoryDbContext _context;

    public MeasurementUnitCommandHandlers(IInventoryDbContext context)
    {
        _context = context;
    }

    public async Task<Guid> Handle(CreateMeasurementUnitCommand request, CancellationToken cancellationToken)
    {
        var entity = new MeasurementUnit(request.Code, request.Name, request.Description);

        _context.MeasurementUnits.Add(entity);
        await _context.SaveChangesAsync(cancellationToken);

        return entity.Id;
    }

    public async Task<bool> Handle(UpdateMeasurementUnitCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.MeasurementUnits.FindAsync(new object[] { request.Id }, cancellationToken);
        if (entity == null) return false;

        entity.Update(request.Name, request.Description);
        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }

    public async Task<bool> Handle(DeleteMeasurementUnitCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.MeasurementUnits.FindAsync(new object[] { request.Id }, cancellationToken);
        if (entity == null) return false;

        // Ideally we would perform Soft Delete or check if there are products using this UOM
        _context.MeasurementUnits.Remove(entity);
        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }
}