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

    // Positivo entra, Negativo sai
    public decimal Quantity { get; set; }

    // Rastreabilidade
    public string? BatchNumber { get; set; }
    public DateOnly? ExpiryDate { get; set; } // .NET type otimizado para datas sem hora

    public decimal? UnitCost { get; set; }

    public string? ReferenceDocument { get; set; } // NF, Ordem Produção
    public string? Notes { get; set; }
    public Guid? PerformedByUserId { get; set; }
}