using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UrbanBenz.Data.Models;

namespace UrbanBenz.Data.EntityTypeConfigurations;

public class CarImageTypeConfiguration : IEntityTypeConfiguration<CarImage>
{
    public void Configure(EntityTypeBuilder<CarImage> builder)
    {
        builder.HasKey(ci => ci.Id);

        builder.Property(ci => ci.Path)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(ci => ci.CarId)
            .IsRequired();

        builder.Property(ci => ci.IsMain)
            .IsRequired()
            .HasDefaultValue(false);

        builder.HasIndex(ci => ci.CarId); 
        builder.HasIndex(ci => new { ci.CarId, ci.IsMain }); 

        builder.HasOne(ci => ci.Car)
            .WithMany(c => c.Images)
            .HasForeignKey(ci => ci.CarId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(ci => new { ci.CarId, ci.IsMain })
            .IsUnique();
    }
}