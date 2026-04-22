using Bluewave.Core.Domain;
using System;

namespace Bluewave.Sales.Domain.Entities;

public class OrderItem : BaseEntity
{
    public Guid OrderId { get; private set; }
    public Guid ProductId { get; private set; }
    public string ProductName { get; private set; } = string.Empty;
    public decimal UnitPrice { get; private set; }
    public decimal Quantity { get; private set; }

    // This is a calculated property (no need to set it manually)
    public decimal TotalPrice => UnitPrice * Quantity;

    // Navigation property for EF Core
    public Order? Order { get; private set; }

    protected OrderItem() { }

    internal OrderItem(Guid productId, string productName, decimal unitPrice, decimal quantity)
    {
        if (quantity <= 0) throw new ArgumentException("Quantity must be greater than zero.", nameof(quantity));
        if (unitPrice < 0) throw new ArgumentException("Unit price cannot be negative.", nameof(unitPrice));

        ProductId = productId;
        ProductName = productName;
        UnitPrice = unitPrice;
        Quantity = quantity;
    }
}