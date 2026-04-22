using Bluewave.Inventory.Application.Common.Interfaces;
using Bluewave.Inventory.Domain.Entities;
using MediatR;

namespace Bluewave.Inventory.Application.Features.Suppliers.Commands;

// DTOs
public record CreateSupplierCommand(string CompanyName, string? TaxId, string? ContactName, string? Email, string? Phone, string? City) : IRequest<Guid>;
public record UpdateSupplierCommand(Guid Id, string CompanyName, string? TaxId, string? ContactName, string? Email, string? Phone, string? City) : IRequest<bool>;
public record DeactivateSupplierCommand(Guid Id) : IRequest<bool>;

// HANDLERS
public class SupplierCommandHandlers :
    IRequestHandler<CreateSupplierCommand, Guid>,
    IRequestHandler<UpdateSupplierCommand, bool>,
    IRequestHandler<DeactivateSupplierCommand, bool>
{
    private readonly IInventoryDbContext _context;

    public SupplierCommandHandlers(IInventoryDbContext context)
    {
        _context = context;
    }

    public async Task<Guid> Handle(CreateSupplierCommand request, CancellationToken cancellationToken)
    {
        var entity = new Supplier(request.CompanyName, request.TaxId, request.ContactName, request.Email, request.Phone, request.City);

        _context.Suppliers.Add(entity);
        await _context.SaveChangesAsync(cancellationToken);

        return entity.Id;
    }

    public async Task<bool> Handle(UpdateSupplierCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.Suppliers.FindAsync(new object[] { request.Id }, cancellationToken);
        if (entity == null) return false;

        entity.Update(request.CompanyName, request.TaxId, request.ContactName, request.Email, request.Phone, request.City);
        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }

    public async Task<bool> Handle(DeactivateSupplierCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.Suppliers.FindAsync(new object[] { request.Id }, cancellationToken);
        if (entity == null) return false;

        entity.Deactivate(); // Use the logical Soft Delete instead of _context.Suppliers.Remove()
        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }
}