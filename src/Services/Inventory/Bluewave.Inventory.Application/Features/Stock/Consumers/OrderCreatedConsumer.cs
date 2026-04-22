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
        logger.LogInformation("Sales event received: Order {OrderId}", message.OrderId);

        var defaultWarehouse = await dbContext.Warehouses
            .FirstOrDefaultAsync(w => w.Code == "SILO-01" || w.Code == "WH-MAIN", context.CancellationToken);

        if (defaultWarehouse == null)
        {
            defaultWarehouse = await dbContext.Warehouses.FirstOrDefaultAsync(context.CancellationToken);
        }

        if (defaultWarehouse == null)
        {
            logger.LogError("CRITICAL ERROR: No warehouse found in the database to decrease stock!");
            return;
        }

        foreach (var item in message.Items)
        {
            var product = await dbContext.Products
                .FirstOrDefaultAsync(p => p.Id == item.ProductId, context.CancellationToken);

            if (product == null)
            {
                logger.LogWarning("Product {ProductId} not found in stock!", item.ProductId);
                continue;
            }

            var transaction = new Domain.Entities.InventoryTransaction(
                productId: item.ProductId,
                warehouseId: defaultWarehouse.Id,
                transactionType: Domain.Enums.TransactionType.OutboundSales, 
                quantity: item.Quantity,
                batchNumber: null, // Pode ser null
                unitCost: product.StandardCost,
                referenceDocument: $"ORDER-{message.OrderId}",
                notes: "Automatic decrease via RabbitMQ"
            );

            dbContext.Transactions.Add(transaction);
            logger.LogInformation("Stock decreased: {Product} - {Qtd} (Warehouse: {Wh})", product.Name, item.Quantity, defaultWarehouse.Name);
        }

        await dbContext.SaveChangesAsync(context.CancellationToken);
        logger.LogInformation("Transactions saved successfully!");
    }
}