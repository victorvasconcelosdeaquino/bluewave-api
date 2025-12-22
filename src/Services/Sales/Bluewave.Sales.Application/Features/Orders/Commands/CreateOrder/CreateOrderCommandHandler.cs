using Bluewave.Core.Messages.IntegrationEvents; // Onde está o evento compartilhado
using Bluewave.Sales.Application.Interfaces;
using Bluewave.Sales.Domain.Entities;
using MassTransit;
using MediatR;

namespace Bluewave.Sales.Application.Features.Orders.Commands.CreateOrder;

public class CreateOrderCommandHandler(
    ISalesDbContext context,
    IPublishEndpoint publishEndpoint)
    : IRequestHandler<CreateOrderCommand, Guid>
{
    public async Task<Guid> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        // 1. Converter DTO para Entidade de Domínio
        var order = new Order
        {
            CustomerName = request.CustomerName,
            Status = "Created",
            TotalAmount = request.Items.Sum(i => i.Quantity * i.UnitPrice)
        };

        foreach (var item in request.Items)
        {
            order.Items.Add(new OrderItem
            {
                ProductId = item.ProductId,
                ProductName = item.ProductName,
                UnitPrice = item.UnitPrice,
                Quantity = item.Quantity
            });
        }

        // 2. Salvar no Banco de Vendas
        context.Orders.Add(order);
        await context.SaveChangesAsync(cancellationToken);

        // 3. Publicar Evento para o RabbitMQ (Inventário vai escutar isso!)
        var eventMessage = new OrderCreatedEvent
        {
            OrderId = order.Id,
            Items = request.Items.Select(i => new Bluewave.Core.Messages.IntegrationEvents.OrderItemDto
            {
                ProductId = i.ProductId,
                Quantity = i.Quantity
            }).ToList()
        };

        await publishEndpoint.Publish(eventMessage, cancellationToken);

        return order.Id;
    }
}