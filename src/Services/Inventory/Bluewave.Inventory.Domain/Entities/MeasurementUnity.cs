using Bluewave.Core.Domain;

namespace Bluewave.Inventory.Domain.Entities;

public class MeasurementUnit : BaseEntity
{
    public string Code { get; private set; } = string.Empty;
    public string Name { get; private set; } = string.Empty;
    public string? Description { get; private set; }

    protected MeasurementUnit() { }

    public MeasurementUnit(string code, string name, string? description)
    {
        Code = code.ToUpper();
        Name = name;
        Description = description;
    }

    public void Update(string name, string? description)
    {
        Name = name;
        Description = description;
        UpdatedAt = DateTime.UtcNow;
    }
}