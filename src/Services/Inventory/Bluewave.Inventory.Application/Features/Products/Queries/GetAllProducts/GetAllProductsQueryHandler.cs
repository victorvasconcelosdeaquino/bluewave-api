using Bluewave.Inventory.Application.Common.Interfaces;
using Bluewave.Inventory.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Bluewave.Inventory.Application.Features.Products.Queries.GetAllProducts;

public class GetAllProductsQueryHandler(IInventoryDbContext context)
    : IRequestHandler<GetAllProductsQuery, List<Product>>
{
    public async Task<List<Product>> Handle(GetAllProductsQuery request, CancellationToken cancellationToken)
    {
        var query = context.Products
            .AsNoTracking()
            .Include(p => p.Category)
            .Include(p => p.Uom)
            .Include(p => p.PreferredSupplier)
            .AsQueryable();

        if (request.OnlyActive)
            query = query.Where(p => p.IsActive);

        return await query.OrderBy(x => x.Name).ToListAsync(cancellationToken);
    }
}