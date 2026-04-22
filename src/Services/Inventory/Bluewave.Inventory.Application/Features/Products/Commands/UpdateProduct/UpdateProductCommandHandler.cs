using Bluewave.Inventory.Application.Common.Interfaces;
using MediatR;

namespace Bluewave.Inventory.Application.Features.Products.Commands.UpdateProduct;

public class UpdateProductCommandHandler(IInventoryDbContext context)
    : IRequestHandler<UpdateProductCommand, bool>
{
    public async Task<bool> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
    {
        var entity = await context.Products.FindAsync(new object[] { request.Id }, cancellationToken);
        if (entity == null) return false;

        entity.Update(
            request.Name, request.Description, request.CategoryId, request.UomId, request.PreferredSupplierId,
            request.MinStockLevel, request.MaxStockLevel, request.IsPerishable, request.RequiresColdChain,
            request.ActiveIngredient, request.Concentration, request.StandardCost);

        await context.SaveChangesAsync(cancellationToken);
        return true;
    }
}