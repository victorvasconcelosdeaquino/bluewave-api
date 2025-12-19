using Bluewave.Core.Domain;

namespace Bluewave.Inventory.Domain.Entities;

public class Product : BaseEntity
{
    // C# 'required' garante que essas props sejam preenchidas na instanciação
    public required string Sku { get; set; }
    public required string Name { get; set; }
    public string? Description { get; set; }

    // Foreign Keys
    public Guid CategoryId { get; set; }
    public virtual ProductCategory? Category { get; set; }

    public Guid UomId { get; set; }
    public virtual MeasurementUnit? Uom { get; set; }

    public Guid? PreferredSupplierId { get; set; }
    public virtual Supplier? PreferredSupplier { get; set; }

    // Controle de Estoque
    public decimal MinStockLevel { get; set; } = 0;
    public decimal? MaxStockLevel { get; set; }

    // Dados de Aquicultura
    public bool IsPerishable { get; set; }
    public bool RequiresColdChain { get; set; }
    public string? ActiveIngredient { get; set; }
    public decimal? Concentration { get; set; }

    // Financeiro
    public decimal StandardCost { get; set; }
    public bool IsActive { get; set; } = true;
}