using Bluewave.Core.Domain;

namespace Bluewave.Inventory.Domain.Entities;

public class Warehouse : BaseEntity
{
    public required string Name { get; set; }
    public string? Code { get; set; }
    public string? Address { get; set; }

    // Se true, é um tanque ou local lógico, não físico
    public bool IsVirtual { get; set; }
    public bool IsActive { get; set; } = true;
}