using Bluewave.Core.Domain;

namespace Bluewave.Inventory.Domain.Entities;

public class MeasurementUnit : BaseEntity
{
    public required string Code { get; set; } // KG, L, UN
    public required string Name { get; set; }
    public string? Description { get; set; }
}