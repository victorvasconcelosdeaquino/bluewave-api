using Bluewave.Inventory.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Bluewave.Inventory.Infrastructure.Persistence.Configurations;

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.HasKey(p => p.Id);

        builder.HasIndex(p => p.Sku)
            .IsUnique(); // Garante SKU único no banco

        builder.Property(p => p.Sku)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(p => p.Name)
            .HasMaxLength(200)
            .IsRequired();

        // Configuração de Precisão Decimal (Importante para evitar arredondamentos errados)
        builder.Property(p => p.StandardCost)
            .HasPrecision(15, 4);

        builder.Property(p => p.MinStockLevel)
            .HasPrecision(15, 3)
            .HasDefaultValue(0);

        builder.Property(p => p.MaxStockLevel)
            .HasPrecision(15, 3);

        builder.Property(p => p.Concentration)
            .HasPrecision(10, 2);

        // Relacionamentos
        builder.HasOne(p => p.Category)
            .WithMany()
            .HasForeignKey(p => p.CategoryId)
            .OnDelete(DeleteBehavior.Restrict); // Evita deletar Categoria se houver produtos

        builder.HasOne(p => p.Uom)
            .WithMany()
            .HasForeignKey(p => p.UomId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(p => p.PreferredSupplier)
            .WithMany()
            .HasForeignKey(p => p.PreferredSupplierId);
    }
}