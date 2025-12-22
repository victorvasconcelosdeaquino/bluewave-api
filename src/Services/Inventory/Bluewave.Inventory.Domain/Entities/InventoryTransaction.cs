using Bluewave.Core.Domain;
using Bluewave.Inventory.Domain.Enums;

namespace Bluewave.Inventory.Domain.Entities;

public class InventoryTransaction : BaseEntity
{
    public Guid ProductId { get; set; }
    public virtual Product? Product { get; set; }

    public Guid WarehouseId { get; set; }
    public virtual Warehouse? Warehouse { get; set; }

    public TransactionType TransactionType { get; set; }

    public decimal Quantity { get; set; }

    public string? BatchNumber { get; set; }
    public DateOnly? ExpiryDate { get; set; }

    public decimal? UnitCost { get; set; }

    public string? ReferenceDocument { get; set; }
    public string? Notes { get; set; }
    public Guid? PerformedByUserId { get; set; }
}