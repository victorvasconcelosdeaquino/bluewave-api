using Bluewave.Inventory.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Bluewave.Inventory.Infrastructure.Persistence.Configurations;

public class InventoryTransactionConfiguration : IEntityTypeConfiguration<InventoryTransaction>
{
    public void Configure(EntityTypeBuilder<InventoryTransaction> builder)
    {
        builder.ToTable("InventoryTransactions");

        builder.HasKey(t => t.Id);

        // Conversão de Enum para String no banco (facilita leitura humana no SQL)
        builder.Property(t => t.TransactionType)
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(t => t.Quantity)
            .HasPrecision(15, 3)
            .IsRequired();

        builder.Property(t => t.UnitCost)
            .HasPrecision(15, 4);

        // Índices para performance em relatórios
        builder.HasIndex(t => t.BatchNumber);
        builder.HasIndex(t => t.CreatedAt); // Importante para filtros de data
    }
}