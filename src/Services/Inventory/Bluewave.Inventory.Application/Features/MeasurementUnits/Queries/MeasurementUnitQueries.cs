using Bluewave.Inventory.Application.Common.Interfaces;
using Bluewave.Inventory.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Bluewave.Inventory.Application.Features.MeasurementUnits.Queries;

// DTOs
public record GetMeasurementUnitByIdQuery(Guid Id) : IRequest<MeasurementUnit?>;
public record GetAllMeasurementUnitsQuery() : IRequest<List<MeasurementUnit>>;

// HANDLERS
public class MeasurementUnitQueryHandlers :
    IRequestHandler<GetMeasurementUnitByIdQuery, MeasurementUnit?>,
    IRequestHandler<GetAllMeasurementUnitsQuery, List<MeasurementUnit>>
{
    private readonly IInventoryDbContext _context;

    public MeasurementUnitQueryHandlers(IInventoryDbContext context)
    {
        _context = context;
    }

    public async Task<MeasurementUnit?> Handle(GetMeasurementUnitByIdQuery request, CancellationToken cancellationToken)
    {
        return await _context.MeasurementUnits
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
    }

    public async Task<List<MeasurementUnit>> Handle(GetAllMeasurementUnitsQuery request, CancellationToken cancellationToken)
    {
        return await _context.MeasurementUnits
            .AsNoTracking()
            .OrderBy(x => x.Code)
            .ToListAsync(cancellationToken);
    }
}