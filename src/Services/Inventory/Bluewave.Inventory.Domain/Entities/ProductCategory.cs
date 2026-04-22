using Bluewave.Core.Domain;

namespace Bluewave.Inventory.Domain.Entities;

public class ProductCategory : BaseEntity
{
    public string Name { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public Guid? ParentId { get; private set; }

    public ProductCategory? Parent { get; private set; }
    private readonly List<ProductCategory> _subCategories = new();
    public IReadOnlyCollection<ProductCategory> SubCategories => _subCategories.AsReadOnly();

    protected ProductCategory() { }

    public ProductCategory(string name, string? description, Guid? parentId = null)
    {
        Name = name;
        Description = description;
        ParentId = parentId;
    }

    public void Update(string name, string? description, Guid? parentId = null)
    {
        Name = name;
        Description = description;
        ParentId = parentId;
        UpdatedAt = DateTime.UtcNow;
    }
}