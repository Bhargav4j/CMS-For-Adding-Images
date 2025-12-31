using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WebGallery.Domain.Entities;

namespace WebGallery.Infrastructure.Data;

/// <summary>
/// Main database context for the application
/// </summary>
public class ApplicationDbContext : IdentityDbContext<IdentityUser>
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

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

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
                Name = "Cookies",
                Description = "Decorative cookies gallery",
                CreatedDate = DateTime.UtcNow,
                IsActive = true,
                CreatedBy = "System"
            }
        );
    }
}
