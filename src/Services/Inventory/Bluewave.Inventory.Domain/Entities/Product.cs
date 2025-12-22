using Bluewave.Core.Domain;

namespace Bluewave.Inventory.Domain.Entities;

public class Product : BaseEntity
{
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

    // Stock Control
    public decimal MinStockLevel { get; set; } = 0;
    public decimal? MaxStockLevel { get; set; }

    // aquaculture data
    // TODO: consider moving these properties to a separate entity if more aquaculture-specific data is needed
    public bool IsPerishable { get; set; }
    public bool RequiresColdChain { get; set; }
    public string? ActiveIngredient { get; set; }
    public decimal? Concentration { get; set; }

    // Pricing information
    // TODO: Consider creating a separate Pricing entity if more pricing details are needed
    public decimal StandardCost { get; set; }
    public bool IsActive { get; set; } = true;
}