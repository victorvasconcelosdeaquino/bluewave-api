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

        var defaultWarehouse = await dbContext.Warehouses
            .FirstOrDefaultAsync(w => w.Code == "SILO-01" || w.Code == "WH-MAIN", context.CancellationToken);

        if (defaultWarehouse == null)
        {
            defaultWarehouse = await dbContext.Warehouses.FirstOrDefaultAsync(context.CancellationToken);
        }

        if (defaultWarehouse == null)
        {
            logger.LogError("ERRO CRÍTICO: Nenhum armazém encontrado no banco para dar baixa no estoque!");
            return;
        }

        foreach (var item in message.Items)
        {
            var product = await dbContext.Products
                .FirstOrDefaultAsync(p => p.Id == item.ProductId, context.CancellationToken);

            if (product == null)
            {
                logger.LogWarning("Produto {ProductId} não encontrado no estoque!", item.ProductId);
                continue;
            }

            var transaction = new Domain.Entities.InventoryTransaction
            {
                ProductId = item.ProductId,

                WarehouseId = defaultWarehouse.Id,

                TransactionType = Domain.Enums.TransactionType.OutboundSales,
                Quantity = -item.Quantity,
                ReferenceDocument = $"ORDER-{message.OrderId}",
                Notes = "Baixa automática via RabbitMQ",
                UnitCost = product.StandardCost
            };

            dbContext.Transactions.Add(transaction);
            logger.LogInformation("Estoque baixado: {Product} - {Qtd} (Armazém: {Wh})", product.Name, item.Quantity, defaultWarehouse.Name);
        }

        await dbContext.SaveChangesAsync(context.CancellationToken);
        logger.LogInformation("Transações salvas com sucesso!");
    }
}