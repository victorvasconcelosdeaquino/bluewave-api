using Bluewave.Core.Domain;

namespace Bluewave.Inventory.Domain.Entities;

public class Supplier : BaseEntity
{
    public required string CompanyName { get; set; }
    public string? TaxId { get; set; } // CNPJ
    public string? ContactName { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? City { get; set; }
    public bool IsActive { get; set; } = true;
}