using Bluewave.Core.Domain;

namespace Bluewave.Inventory.Domain.Entities;

public class Warehouse : BaseEntity
{
    public string Name { get; private set; } = string.Empty;
    public string? Code { get; private set; }
    public string? Address { get; private set; }
    public bool IsVirtual { get; private set; }
    public bool IsActive { get; private set; }

    protected Warehouse() { }

    public Warehouse(string name, string? code, string? address, bool isVirtual)
    {
        Name = name;
        Code = code?.ToUpper(); 
        Address = address;
        IsVirtual = isVirtual;
        IsActive = true; 
    }

    public void Update(string name, string? code, string? address, bool isVirtual)
    {
        Name = name;
        Code = code?.ToUpper();
        Address = address;
        IsVirtual = isVirtual;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Deactivate()
    {
        IsActive = false;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Activate()
    {
        IsActive = true;
        UpdatedAt = DateTime.UtcNow;
    }
}