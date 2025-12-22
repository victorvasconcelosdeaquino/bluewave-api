using MediatR;

namespace Bluewave.Sales.Application.Features.Orders.Commands.CreateOrder;

public record CreateOrderCommand : IRequest<Guid>
{
    public string CustomerName { get; init; } = string.Empty;
    public List<OrderItemDto> Items { get; init; } = new();
}

public record OrderItemDto
{
    public Guid ProductId { get; init; }
    public string ProductName { get; init; } = string.Empty;
    public decimal UnitPrice { get; init; }
    public decimal Quantity { get; init; }
}