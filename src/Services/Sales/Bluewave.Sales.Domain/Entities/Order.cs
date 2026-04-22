using Bluewave.Core.Domain;
using Bluewave.Sales.Domain.Enums;

namespace Bluewave.Sales.Domain.Entities;

public class Order : BaseEntity
{
    public string CustomerName { get; private set; } = string.Empty;
    public DateTime OrderDate { get; private set; }
    public OrderStatus Status { get; private set; }
    public decimal TotalAmount { get; private set; }

    // Blindagem da lista
    private readonly List<OrderItem> _items = new();
    public IReadOnlyCollection<OrderItem> Items => _items.AsReadOnly();

    protected Order() { }

    public Order(string customerName)
    {
        CustomerName = customerName;
        OrderDate = DateTime.UtcNow;
        Status = OrderStatus.Pending;
        TotalAmount = 0;
    }

    public void AddItem(Guid productId, string productName, decimal unitPrice, decimal quantity)
    {
        var item = new OrderItem(productId, productName, unitPrice, quantity);
        _items.Add(item);

        RecalculateTotal();
    }

    public void Approve()
    {
        if (Status != OrderStatus.Pending)
            throw new InvalidOperationException("Only pending orders can be approved.");

        Status = OrderStatus.Approved;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Cancel()
    {
        Status = OrderStatus.Canceled;
        UpdatedAt = DateTime.UtcNow;
    }

    private void RecalculateTotal()
    {
        TotalAmount = _items.Sum(i => i.TotalPrice);
    }
}