using Bluewave.Inventory.Application.Common.Interfaces;
using Bluewave.Inventory.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Bluewave.Inventory.Application.Features.Warehouses.Queries;

// DTOs
public record GetWarehouseByIdQuery(Guid Id) : IRequest<Warehouse?>;
public record GetAllWarehousesQuery(bool OnlyActive = true) : IRequest<List<Warehouse>>;

// HANDLERS
public class WarehouseQueryHandlers :
    IRequestHandler<GetWarehouseByIdQuery, Warehouse?>,
    IRequestHandler<GetAllWarehousesQuery, List<Warehouse>>
{
    private readonly IInventoryDbContext _context;

    public WarehouseQueryHandlers(IInventoryDbContext context)
    {
        _context = context;
    }

    public async Task<Warehouse?> Handle(GetWarehouseByIdQuery request, CancellationToken cancellationToken)
    {
        return await _context.Warehouses
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
    }

    public async Task<List<Warehouse>> Handle(GetAllWarehousesQuery request, CancellationToken cancellationToken)
    {
        var query = _context.Warehouses.AsNoTracking();

        if (request.OnlyActive)
        {
            query = query.Where(w => w.IsActive);
        }

        return await query.OrderBy(x => x.Name).ToListAsync(cancellationToken);
    }
}