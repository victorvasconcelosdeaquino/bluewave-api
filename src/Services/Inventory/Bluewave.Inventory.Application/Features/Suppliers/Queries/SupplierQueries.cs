using Bluewave.Inventory.Application.Common.Interfaces;
using Bluewave.Inventory.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Bluewave.Inventory.Application.Features.Suppliers.Queries;

// DTOs
public record GetSupplierByIdQuery(Guid Id) : IRequest<Supplier?>;
public record GetAllSuppliersQuery(bool OnlyActive = true) : IRequest<List<Supplier>>;

// HANDLERS
public class SupplierQueryHandlers :
    IRequestHandler<GetSupplierByIdQuery, Supplier?>,
    IRequestHandler<GetAllSuppliersQuery, List<Supplier>>
{
    private readonly IInventoryDbContext _context;

    public SupplierQueryHandlers(IInventoryDbContext context)
    {
        _context = context;
    }

    public async Task<Supplier?> Handle(GetSupplierByIdQuery request, CancellationToken cancellationToken)
    {
        return await _context.Suppliers
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
    }

    public async Task<List<Supplier>> Handle(GetAllSuppliersQuery request, CancellationToken cancellationToken)
    {
        var query = _context.Suppliers.AsNoTracking();

        if (request.OnlyActive)
        {
            query = query.Where(s => s.IsActive);
        }

        return await query.OrderBy(x => x.CompanyName).ToListAsync(cancellationToken);
    }
}