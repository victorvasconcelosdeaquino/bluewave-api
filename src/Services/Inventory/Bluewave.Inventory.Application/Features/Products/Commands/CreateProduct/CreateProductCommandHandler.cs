// File: CreateProductCommandHandler.cs
using Bluewave.Inventory.Application.Common.Interfaces;
using Bluewave.Inventory.Domain.Entities;
using MediatR;

namespace Bluewave.Inventory.Application.Features.Products.Commands.CreateProduct;

public class CreateProductCommandHandler(IInventoryDbContext context)
    : IRequestHandler<CreateProductCommand, Guid>
{
    public async Task<Guid> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        var entity = new Product(
            request.Sku, request.Name, request.Description, request.CategoryId, request.UomId,
            request.PreferredSupplierId, request.MinStockLevel, request.MaxStockLevel, request.IsPerishable,
            request.RequiresColdChain, request.ActiveIngredient, request.Concentration, request.StandardCost);

        context.Products.Add(entity);
        await context.SaveChangesAsync(cancellationToken);

        return entity.Id;
    }
}