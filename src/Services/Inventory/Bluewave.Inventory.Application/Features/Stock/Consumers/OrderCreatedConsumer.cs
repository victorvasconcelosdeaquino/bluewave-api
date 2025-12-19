using Bluewave.Core.Messages.IntegrationEvents;
using Bluewave.Inventory.Application.Common.Interfaces;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Bluewave.Inventory.Application.Features.Stock.Consumers;

public class OrderCreatedConsumer(
    IInventoryDbContext dbContext,
    ILogger<OrderCreatedConsumer> logger)
    : IConsumer<OrderCreatedEvent>
{
    public async Task Consume(ConsumeContext<OrderCreatedEvent> context)
    {
        var message = context.Message;
        logger.LogInformation("Recebido evento de Venda: Pedido {OrderId}", message.OrderId);

        foreach (var item in message.Items)
        {
            // Busca o produto
            var product = await dbContext.Products
                .FirstOrDefaultAsync(p => p.Id == item.ProductId);

            if (product == null)
            {
                logger.LogWarning("Produto {ProductId} não encontrado no estoque!", item.ProductId);
                continue;
            }

            // Cria a transação de saída (Baixa de Estoque)
            var transaction = new Domain.Entities.InventoryTransaction
            {
                ProductId = item.ProductId,
                WarehouseId = Guid.Empty, // Em um app real, viria do evento ou lógica de alocação
                TransactionType = Domain.Enums.TransactionType.OutboundSales,
                Quantity = -item.Quantity, // Negativo pois é saída
                ReferenceDocument = $"ORDER-{message.OrderId}",
                Notes = "Baixa automática via RabbitMQ"
            };

            // Hack rápido: Para este exemplo funcionar sem WarehouseId válido, 
            // no mundo real teríamos lógica para escolher o armazém.
            // Aqui vamos assumir que você pegará o ID do "Silo 01" fixo ou ajustará o banco depois.

            dbContext.Transactions.Add(transaction);
            logger.LogInformation("🔻 Estoque baixado: {Product} - {Qtd}", product.Name, item.Quantity);
        }

        await dbContext.SaveChangesAsync(context.CancellationToken);
    }
}