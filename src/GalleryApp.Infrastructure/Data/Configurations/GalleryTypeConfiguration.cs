using GalleryApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GalleryApp.Infrastructure.Data.Configurations;

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
            .HasMaxLength(200);

        builder.Property(g => g.Description)
            .HasMaxLength(1000);

        builder.Property(g => g.CreatedBy)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(g => g.ModifiedBy)
            .HasMaxLength(100);

        builder.Property(g => g.CreatedDate)
            .IsRequired();

        builder.Property(g => g.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        builder.HasMany(g => g.Pictures)
            .WithOne(p => p.GalleryType)
            .HasForeignKey(p => p.GalleryTypeId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(g => g.Name);
        builder.HasIndex(g => g.IsActive);
    }
}
