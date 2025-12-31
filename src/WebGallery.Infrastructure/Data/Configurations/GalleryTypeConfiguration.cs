using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WebGallery.Domain.Entities;

namespace WebGallery.Infrastructure.Data.Configurations;

/// <summary>
/// Entity configuration for GalleryType
/// </summary>
public class GalleryTypeConfiguration : IEntityTypeConfiguration<GalleryType>
{
    public void Configure(EntityTypeBuilder<GalleryType> builder)
    {
        builder.ToTable("GalleryTypes");

        builder.HasKey(g => g.Id);

        builder.Property(g => g.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(g => g.Description)
            .HasMaxLength(500);

        builder.Property(g => g.CreatedBy)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(g => g.ModifiedBy)
            .HasMaxLength(100);

        builder.Property(g => g.CreatedDate)
            .HasDefaultValueSql("GETUTCDATE()");

        builder.Property(g => g.IsActive)
            .HasDefaultValue(true);

        builder.HasIndex(g => g.Name);
        builder.HasIndex(g => g.IsActive);
    }
}
