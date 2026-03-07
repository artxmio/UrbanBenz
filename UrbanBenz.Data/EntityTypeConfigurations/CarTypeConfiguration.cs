using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UrbanBenz.Data.Models;

namespace UrbanBenz.Data.EntityTypeConfigurations;

public class CarTypeConfiguration : IEntityTypeConfiguration<Car>
{
    public void Configure(EntityTypeBuilder<Car> builder)
    {
        builder.HasKey(c => c.Id);

        builder.Property(c => c.Model)
            .IsRequired();

        builder.Property(c => c.Type)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(c => c.Engine)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(c => c.Year)
            .IsRequired();

        builder.Property(c => c.Color)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(c => c.Price)
            .IsRequired()
            .HasPrecision(18, 2);

        builder.Property(c => c.VinNumber)
           .IsRequired()
           .HasMaxLength(17);

        builder.Property(c => c.IsAviable)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(c => c.Description)
            .HasMaxLength(1000);

        builder.Property(c => c.CreatedAt)
            .IsRequired();

        builder.HasIndex(c => c.Model);
        builder.HasIndex(c => c.Type);
        builder.HasIndex(c => c.Class);
        builder.HasIndex(c => c.Engine);
        builder.HasIndex(c => c.Year);
        builder.HasIndex(c => c.Price);
        builder.HasIndex(c => c.VinNumber).IsUnique();
        builder.HasIndex(c => c.IsAviable);
        builder.HasIndex(c => new { c.Type, c.Class, c.Engine });
    }
}
