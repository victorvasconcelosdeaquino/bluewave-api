using Bluewave.Core.Domain;

namespace Bluewave.Inventory.Domain.Entities;

public class Warehouse : BaseEntity
{
    public required string Name { get; set; }
    public string? Code { get; set; }
    public string? Address { get; set; }

    // If true, it's a tank or logical location, not physical
    public bool IsVirtual { get; set; }
    public bool IsActive { get; set; } = true;
}