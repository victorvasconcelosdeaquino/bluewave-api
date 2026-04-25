using Bluewave.Core.Messages.IntegrationEvents;
using Bluewave.Inventory.Application.Common.Interfaces;
using Bluewave.Inventory.Domain.Entities;
using Bluewave.Inventory.Domain.Enums;
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

        // Standart warehouse lookup logic
        var defaultWarehouse = await dbContext.Warehouses
            .FirstOrDefaultAsync(w => w.Code == "SILO-01" || w.Code == "WH-MAIN", context.CancellationToken);

        if (defaultWarehouse == null)
        {
            defaultWarehouse = await dbContext.Warehouses.FirstOrDefaultAsync(context.CancellationToken);
        }

        if (defaultWarehouse == null)
        {
            logger.LogError("CRITICAL ERROR: No warehouse found in the database!");
            return;
        }

        // Process each order item
        foreach (var item in message.Items)
        {
            // This check ensures that if the same OrderCreatedEvent is received multiple times (e.g., due to retries or duplicates),
            var referenceDoc = $"ORDER-{message.OrderId}";

            var alreadyProcessed = await dbContext.InventoryTransactions
                .AnyAsync(t => t.ProductId == item.ProductId &&
                               t.ReferenceDocument == referenceDoc,
                          context.CancellationToken);

            if (alreadyProcessed)
            {
                logger.LogWarning("Order {OrderId} for Product {ProductId} already processed. Skipping to avoid double decrease.",
                    message.OrderId, item.ProductId);
                continue; 
            }

            // Calculate current stock for the specific product of the item
            var currentStock = await dbContext.InventoryTransactions
                .Where(t => t.ProductId == item.ProductId)
                .SumAsync(t =>
                    (t.TransactionType == TransactionType.InboundPurchase ||
                     t.TransactionType == TransactionType.TransferIn ||
                     t.TransactionType == TransactionType.Return) ? t.Quantity :
                    (t.TransactionType == TransactionType.OutboundSales ||
                     t.TransactionType == TransactionType.OutboundProduction) ? -t.Quantity : 0,
                context.CancellationToken);

            // Verifies if there is enough stock
            if (currentStock < item.Quantity)
            {
                logger.LogError("Insufficient stock for product {ProductId}. Stock: {Current}, Requested: {Requested}",
                    item.ProductId, currentStock, item.Quantity);

                // Throws an exception here causes MassTransit to move the message to the _error queue
                throw new InvalidOperationException($"Insufficient stock for product {item.ProductId} in Order {message.OrderId}");
            }

            var product = await dbContext.Products
                .FirstOrDefaultAsync(p => p.Id == item.ProductId, context.CancellationToken);

            if (product == null)
            {
                logger.LogWarning("Product {ProductId} not found in database!", item.ProductId);
                continue;
            }

            var transaction = new Domain.Entities.InventoryTransaction(
                productId: item.ProductId,
                warehouseId: defaultWarehouse.Id,
                transactionType: Domain.Enums.TransactionType.OutboundSales,
                quantity: item.Quantity,
                batchNumber: null,
                unitCost: product.StandardCost,
                referenceDocument: $"ORDER-{message.OrderId}",
                notes: "Automatic decrease via RabbitMQ"
            );

            dbContext.InventoryTransactions.Add(transaction);
            logger.LogInformation("Stock decreased: {Product} - {Qtd} (Warehouse: {Wh})", product.Name, item.Quantity, defaultWarehouse.Name);
        }

        await dbContext.SaveChangesAsync(context.CancellationToken);
        logger.LogInformation("Transactions saved successfully for Order {OrderId}!", message.OrderId);
    }
}