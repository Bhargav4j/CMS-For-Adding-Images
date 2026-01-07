using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WebGallery.Domain.Entities;

namespace WebGallery.Infrastructure.Data;

/// <summary>
/// Application database context using EF Core 8
/// </summary>
public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Picture> Pictures { get; set; }
    public DbSet<GalleryType> GalleryTypes { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Picture configuration
        modelBuilder.Entity<Picture>(entity =>
        {
            entity.ToTable("Pictures");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Description).HasMaxLength(1000);
            entity.Property(e => e.ImagePath).IsRequired().HasMaxLength(500);
            entity.Property(e => e.ThumbnailImagePath).HasMaxLength(500);
            entity.Property(e => e.CreatedDate).IsRequired();
            entity.Property(e => e.IsActive).IsRequired().HasDefaultValue(true);
            entity.Property(e => e.CreatedBy).IsRequired().HasMaxLength(100);
            entity.Property(e => e.ModifiedBy).HasMaxLength(100);

            entity.HasOne(e => e.GalleryType)
                .WithMany(g => g.Pictures)
                .HasForeignKey(e => e.GalleryTypeId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // GalleryType configuration
        modelBuilder.Entity<GalleryType>(entity =>
        {
            entity.ToTable("GalleryTypes");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Description).HasMaxLength(1000);
            entity.Property(e => e.CreatedDate).IsRequired();
            entity.Property(e => e.IsActive).IsRequired().HasDefaultValue(true);
            entity.Property(e => e.CreatedBy).IsRequired().HasMaxLength(100);
            entity.Property(e => e.ModifiedBy).HasMaxLength(100);
        });

        // Seed data
        modelBuilder.Entity<GalleryType>().HasData(
            new GalleryType
            {
                Id = 1,
                Name = "Paintings",
                Description = "Art paintings gallery",
                CreatedDate = DateTime.UtcNow,
                IsActive = true,
                CreatedBy = "System"
            },
            new GalleryType
            {
                Id = 2,
                Name = "Cakes",
                Description = "Decorated cakes gallery",
                CreatedDate = DateTime.UtcNow,
                IsActive = true,
                CreatedBy = "System"
            }
        );
    }
}
