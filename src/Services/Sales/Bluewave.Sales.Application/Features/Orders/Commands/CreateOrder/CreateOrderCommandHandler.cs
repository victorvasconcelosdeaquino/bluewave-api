using Bluewave.Core.Messages.IntegrationEvents;
using Bluewave.Sales.Application.Interfaces;
using Bluewave.Sales.Domain.Entities;
using MassTransit;
using MediatR;

namespace Bluewave.Sales.Application.Features.Orders.Commands.CreateOrder;

public class CreateOrderCommandHandler(ISalesDbContext context, IPublishEndpoint publishEndpoint)
    : IRequestHandler<CreateOrderCommand, Guid>
{
    public async Task<Guid> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        // Create the Aggregate Root (The Order)
        var order = new Order(request.CustomerName);

        // Add Items using the rich domain behavior
        foreach (var item in request.Items)
        {
            order.AddItem(item.ProductId, item.ProductName, item.UnitPrice, item.Quantity);
        }

        // Persist the Order in the Sales Database
        context.Orders.Add(order);
        await context.SaveChangesAsync(cancellationToken);

        // Publish the event to RabbitMQ for Inventory to reduce stock
        var orderCreatedEvent = new OrderCreatedEvent
        {
            OrderId = order.Id,
            CustomerName = order.CustomerName,
            Items = order.Items.Select(i => new OrderItemEventDto
            {
                ProductId = i.ProductId,
                Quantity = i.Quantity
            }).ToList()
        };

        await publishEndpoint.Publish(orderCreatedEvent, cancellationToken);

        return order.Id;
    }
}