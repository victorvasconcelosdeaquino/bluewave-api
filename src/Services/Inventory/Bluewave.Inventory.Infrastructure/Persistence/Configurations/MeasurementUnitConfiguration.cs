using Bluewave.Inventory.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Bluewave.Inventory.Infrastructure.Persistence.Configurations;

public class MeasurementUnitConfiguration : IEntityTypeConfiguration<MeasurementUnit>
{
    public void Configure(EntityTypeBuilder<MeasurementUnit> builder)
    {
        builder.HasKey(u => u.Id);

        builder.HasIndex(u => u.Code).IsUnique(); // KG, L, UN únicos

        builder.Property(u => u.Code)
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(u => u.Name)
            .HasMaxLength(100)
            .IsRequired();
    }
}