using Bluewave.Inventory.Application.Common.Interfaces;
using Bluewave.Inventory.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Bluewave.Inventory.Application.Features.Categories.Queries;

// DTOs
public record GetCategoryByIdQuery(Guid Id) : IRequest<ProductCategory?>;
public record GetAllCategoriesQuery() : IRequest<List<ProductCategory>>;

// HANDLERS
public class CategoryQueryHandlers :
    IRequestHandler<GetCategoryByIdQuery, ProductCategory?>,
    IRequestHandler<GetAllCategoriesQuery, List<ProductCategory>>
{
    private readonly IInventoryDbContext _context;

    public CategoryQueryHandlers(IInventoryDbContext context)
    {
        _context = context;
    }

    public async Task<ProductCategory?> Handle(GetCategoryByIdQuery request, CancellationToken cancellationToken)
    {
        return await _context.Categories
            .AsNoTracking()
            .Include(c => c.SubCategories) // Traz as filhas junto se existirem
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
    }

    public async Task<List<ProductCategory>> Handle(GetAllCategoriesQuery request, CancellationToken cancellationToken)
    {
        return await _context.Categories
            .AsNoTracking()
            .OrderBy(x => x.Name)
            .ToListAsync(cancellationToken);
    }
}