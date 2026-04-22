using Bluewave.Core.Domain;

namespace Bluewave.Inventory.Domain.Entities;

public class Supplier : BaseEntity
{
    public string CompanyName { get; private set; } = string.Empty;
    public string? TaxId { get; private set; } 
    public string? ContactName { get; private set; }
    public string? Email { get; private set; }
    public string? Phone { get; private set; }
    public string? City { get; private set; }
    public bool IsActive { get; private set; }

    protected Supplier() { }

    public Supplier(string companyName, string? taxId, string? contactName, string? email, string? phone, string? city)
    {
        CompanyName = companyName;
        TaxId = taxId;
        ContactName = contactName;
        Email = email;
        Phone = phone;
        City = city;
        IsActive = true;
    }

    public void Update(string companyName, string? taxId, string? contactName, string? email, string? phone, string? city)
    {
        CompanyName = companyName;
        TaxId = taxId;
        ContactName = contactName;
        Email = email;
        Phone = phone;
        City = city;
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