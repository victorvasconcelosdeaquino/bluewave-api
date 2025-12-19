using MediatR;
using Bluewave.Inventory.Application.Common.Interfaces;
using Bluewave.Inventory.Domain.Entities;

namespace Bluewave.Inventory.Application.Features.Products.Commands.CreateProduct;

public class CreateProductCommandHandler(IInventoryDbContext context)
    : IRequestHandler<CreateProductCommand, Guid>
{
    public async Task<Guid> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        // 1. Criar a Entidade
        var entity = new Product
        {
            Sku = request.Sku,
            Name = request.Name,
            Description = request.Description,
            CategoryId = request.CategoryId,
            UomId = request.UomId,
            MinStockLevel = request.MinStockLevel,
            StandardCost = request.StandardCost,
            IsActive = true
            // Outros campos como IsPerishable poderiam vir no command também
        };

        // 2. Adicionar ao Contexto
        context.Products.Add(entity);

        // 3. Persistir no Banco
        await context.SaveChangesAsync(cancellationToken);

        // 4. Retornar o ID gerado
        return entity.Id;
    }
}