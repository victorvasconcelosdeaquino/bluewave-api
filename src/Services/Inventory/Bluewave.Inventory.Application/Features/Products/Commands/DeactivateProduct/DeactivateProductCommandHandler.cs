using Bluewave.Inventory.Application.Common.Interfaces;
using MediatR;

namespace Bluewave.Inventory.Application.Features.Products.Commands.DeactivateProduct;

public class DeactivateProductCommandHandler(IInventoryDbContext context)
    : IRequestHandler<DeactivateProductCommand, bool>
{
    public async Task<bool> Handle(DeactivateProductCommand request, CancellationToken cancellationToken)
    {
        var entity = await context.Products.FindAsync(new object[] { request.Id }, cancellationToken);
        if (entity == null) return false;

        entity.Deactivate();
        await context.SaveChangesAsync(cancellationToken);

        return true;
    }
}