using Bluewave.Core.Domain;
using Bluewave.Inventory.Domain.Enums;

namespace Bluewave.Inventory.Domain.Entities;

public class InventoryTransaction : BaseEntity
{
    public Guid ProductId { get; private set; }
    public Guid WarehouseId { get; private set; }
    public TransactionType TransactionType { get; private set; }
    public decimal Quantity { get; private set; }
    public string? BatchNumber { get; private set; }
    public decimal? UnitCost { get; private set; }
    public string? ReferenceDocument { get; private set; }
    public string? Notes { get; private set; }

    // Navigation properties for EF Core
    public Product? Product { get; private set; }
    public Warehouse? Warehouse { get; private set; }

    protected InventoryTransaction() { }

    public InventoryTransaction(Guid productId, Guid warehouseId, TransactionType transactionType,
        decimal quantity, string? batchNumber, decimal? unitCost, string? referenceDocument, string? notes)
    {
        if (quantity <= 0) throw new ArgumentException("The transaction quantity must be greater than zero.", nameof(quantity));

        ProductId = productId;
        WarehouseId = warehouseId;
        TransactionType = transactionType;
        Quantity = quantity;
        BatchNumber = batchNumber;
        UnitCost = unitCost;
        ReferenceDocument = referenceDocument;
        Notes = notes;
    }
}