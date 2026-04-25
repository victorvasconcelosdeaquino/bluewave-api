using Bluewave.Inventory.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Category = Bluewave.Inventory.Domain.Entities.ProductCategory;

namespace Bluewave.Inventory.Application.Features.ProductCategory.Commands.CreateProductCategory;

// DTOs
public record CreateCategoryCommand(string Name, string? Description, Guid? ParentId = null) : IRequest<Guid>;
public record UpdateCategoryCommand(Guid Id, string Name, string? Description, Guid? ParentId = null) : IRequest<bool>;
public record DeleteCategoryCommand(Guid Id) : IRequest<bool>;

// HANDLERS
public class CategoryCommandHandlers :
    IRequestHandler<CreateCategoryCommand, Guid>,
    IRequestHandler<UpdateCategoryCommand, bool>,
    IRequestHandler<DeleteCategoryCommand, bool>
{
    private readonly IInventoryDbContext _context;

    public CategoryCommandHandlers(IInventoryDbContext context)
    {
        _context = context;
    }

    //TODO: split handlers into separate classes for better separation of concerns
    public async Task<Guid> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
    {
        var entity = new Category(request.Name, request.Description, request.ParentId);

        _context.Categories.Add(entity);
        await _context.SaveChangesAsync(cancellationToken);

        return entity.Id;
    }

    public async Task<bool> Handle(UpdateCategoryCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.Categories.FindAsync(new object[] { request.Id }, cancellationToken);
        if (entity == null) return false;

        entity.Update(request.Name, request.Description, request.ParentId);
        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }

    public async Task<bool> Handle(DeleteCategoryCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.Categories.FindAsync(new object[] { request.Id }, cancellationToken);
        if (entity == null) return false;

        // Regra de negócio simples: não deletar se tiver subcategorias
        var hasChildren = await _context.Categories.AnyAsync(c => c.ParentId == request.Id, cancellationToken);
        if (hasChildren) throw new InvalidOperationException("Cannot delete a category that has subcategories.");

        _context.Categories.Remove(entity);
        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }
}