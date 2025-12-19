namespace Bluewave.Core.Messages.IntegrationEvents;

public record OrderCreatedEvent
{
    public Guid OrderId { get; init; }
    public List<OrderItemDto> Items { get; init; } = new();
}

public record OrderItemDto
{
    public Guid ProductId { get; init; }
    public decimal Quantity { get; init; }
}