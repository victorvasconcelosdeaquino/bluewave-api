using Bluewave.Core.Domain;

namespace Bluewave.Inventory.Domain.Entities;

public class Product : BaseEntity
{
    public string Sku { get; private set; } = string.Empty;
    public string Name { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public Guid CategoryId { get; private set; }
    public Guid UomId { get; private set; }
    public Guid? PreferredSupplierId { get; private set; }
    public decimal MinStockLevel { get; private set; }
    public decimal? MaxStockLevel { get; private set; }
    public bool IsPerishable { get; private set; }
    public bool RequiresColdChain { get; private set; }
    public string? ActiveIngredient { get; private set; }
    public decimal? Concentration { get; private set; }
    public decimal StandardCost { get; private set; }
    public bool IsActive { get; private set; }

    public ProductCategory? Category { get; private set; }
    public MeasurementUnit? Uom { get; private set; }
    public Supplier? PreferredSupplier { get; private set; }

    protected Product() { }

    public Product(string sku, string name, string? description, Guid categoryId, Guid uomId, Guid? preferredSupplierId,
                   decimal minStockLevel, decimal? maxStockLevel, bool isPerishable, bool requiresColdChain,
                   string? activeIngredient, decimal? concentration, decimal standardCost)
    {
        Sku = sku.ToUpper(); 
        Name = name;
        Description = description;
        CategoryId = categoryId;
        UomId = uomId;
        PreferredSupplierId = preferredSupplierId;
        MinStockLevel = minStockLevel;
        MaxStockLevel = maxStockLevel;
        IsPerishable = isPerishable;
        RequiresColdChain = requiresColdChain;
        ActiveIngredient = activeIngredient;
        Concentration = concentration;
        StandardCost = standardCost;
        IsActive = true;
    }

    public void Update(string name, string? description, Guid categoryId, Guid uomId, Guid? preferredSupplierId,
                       decimal minStockLevel, decimal? maxStockLevel, bool isPerishable, bool requiresColdChain,
                       string? activeIngredient, decimal? concentration, decimal standardCost)
    {
        Name = name;
        Description = description;
        CategoryId = categoryId;
        UomId = uomId;
        PreferredSupplierId = preferredSupplierId;
        MinStockLevel = minStockLevel;
        MaxStockLevel = maxStockLevel;
        IsPerishable = isPerishable;
        RequiresColdChain = requiresColdChain;
        ActiveIngredient = activeIngredient;
        Concentration = concentration;
        StandardCost = standardCost;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Deactivate()
    {
        IsActive = false;
        UpdatedAt = DateTime.UtcNow;
    }
}