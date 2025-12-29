using GalleryApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GalleryApp.Infrastructure.Data.Configurations;

public class PictureConfiguration : IEntityTypeConfiguration<Picture>
{
    public void Configure(EntityTypeBuilder<Picture> builder)
    {
        builder.ToTable("Pictures");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(p => p.Description)
            .HasMaxLength(1000);

        builder.Property(p => p.ImagePath)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(p => p.ThumbnailImagePath)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(p => p.CreatedBy)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(p => p.ModifiedBy)
            .HasMaxLength(100);

        builder.Property(p => p.CreatedDate)
            .IsRequired();

        builder.Property(p => p.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        builder.HasOne(p => p.GalleryType)
            .WithMany(g => g.Pictures)
            .HasForeignKey(p => p.GalleryTypeId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(p => p.GalleryTypeId);
        builder.HasIndex(p => p.IsActive);
    }
}
