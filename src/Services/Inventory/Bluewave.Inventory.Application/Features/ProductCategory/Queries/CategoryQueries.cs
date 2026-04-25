using Bluewave.Inventory.Application.Common.Interfaces;
using Category = Bluewave.Inventory.Domain.Entities.ProductCategory;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Bluewave.Inventory.Application.Features.Categories.Queries;

// DTOs
public record GetCategoryByIdQuery(Guid Id) : IRequest<Category?>;
public record GetAllCategoriesQuery() : IRequest<List<Category>>;

// HANDLERS
public class CategoryQueryHandlers :
    IRequestHandler<GetCategoryByIdQuery, Category?>,
    IRequestHandler<GetAllCategoriesQuery, List<Category>>
{
    private readonly IInventoryDbContext _context;

    public CategoryQueryHandlers(IInventoryDbContext context)
    {
        _context = context;
    }

    public async Task<Category?> Handle(GetCategoryByIdQuery request, CancellationToken cancellationToken)
    {
        return await _context.Categories
            .AsNoTracking()
            .Include(c => c.SubCategories) // Traz as filhas junto se existirem
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
    }

    public async Task<List<Category>> Handle(GetAllCategoriesQuery request, CancellationToken cancellationToken)
    {
        return await _context.Categories
            .AsNoTracking()
            .OrderBy(x => x.Name)
            .ToListAsync(cancellationToken);
    }
}