using Bluewave.Inventory.Application.Common.Interfaces;
using Bluewave.Inventory.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Bluewave.Inventory.Application.Features.Products.Queries.GetProductById;

public class GetProductByIdQueryHandler(IInventoryDbContext context)
    : IRequestHandler<GetProductByIdQuery, Product?>
{
    public async Task<Product?> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
    {
        return await context.Products
            .AsNoTracking()
            .Include(p => p.Category)
            .Include(p => p.Uom)
            .Include(p => p.PreferredSupplier)
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
    }
}