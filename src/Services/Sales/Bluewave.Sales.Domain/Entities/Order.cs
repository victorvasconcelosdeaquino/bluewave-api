namespace Bluewave.Sales.Domain.Entities;

public class Order
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string CustomerName { get; set; } = string.Empty;
    public DateTime OrderDate { get; set; } = DateTime.UtcNow;
    public decimal TotalAmount { get; set; }

    // TODO: Consider using an enum for Status
    public string Status { get; set; } = "Created"; 

    public List<OrderItem> Items { get; set; } = new();
}