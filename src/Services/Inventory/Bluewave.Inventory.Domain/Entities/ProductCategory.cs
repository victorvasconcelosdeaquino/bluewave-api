using Bluewave.Core.Domain;

namespace Bluewave.Inventory.Domain.Entities;

public class ProductCategory : BaseEntity
{
    public required string Name { get; set; }
    public string? Description { get; set; }

    // Auto-relacionamento para hierarquia (Pai -> Filho)
    public Guid? ParentId { get; set; }
    public virtual ProductCategory? Parent { get; set; }
    public virtual ICollection<ProductCategory> SubCategories { get; set; } = new List<ProductCategory>();
}