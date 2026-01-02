using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WebGallery.Domain.Entities;

namespace WebGallery.Infrastructure.Data;

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

        modelBuilder.Entity<GalleryType>().HasData(
            new GalleryType { Id = 1, Name = "paintings", Description = "Paintings gallery", CreatedDate = DateTime.UtcNow, IsActive = true, CreatedBy = "System" },
            new GalleryType { Id = 2, Name = "cookies", Description = "Cookies gallery", CreatedDate = DateTime.UtcNow, IsActive = true, CreatedBy = "System" }
        );
    }
}
