namespace Bluewave.Core.Messages.IntegrationEvents;

public class OrderCreatedEvent
{
    public Guid OrderId { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public List<OrderItemEventDto> Items { get; set; } = new();
}

public class OrderItemEventDto
{
    public Guid ProductId { get; set; }
    public decimal Quantity { get; set; }
}