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

        modelBuilder.Entity<IdentityRole>().HasData(
            new IdentityRole { Id = "1", Name = "admin", NormalizedName = "ADMIN" },
            new IdentityRole { Id = "2", Name = "user", NormalizedName = "USER" }
        );

        var hasher = new PasswordHasher<IdentityUser>();
        var adminUser = new IdentityUser
        {
            Id = "admin-user-id",
            UserName = "admin",
            NormalizedUserName = "ADMIN",
            Email = "admin@webgallery.com",
            NormalizedEmail = "ADMIN@WEBGALLERY.COM",
            EmailConfirmed = true,
            SecurityStamp = Guid.NewGuid().ToString()
        };
        adminUser.PasswordHash = hasher.HashPassword(adminUser, "Admin@123");

        modelBuilder.Entity<IdentityUser>().HasData(adminUser);

        modelBuilder.Entity<IdentityUserRole<string>>().HasData(
            new IdentityUserRole<string> { RoleId = "1", UserId = "admin-user-id" },
            new IdentityUserRole<string> { RoleId = "2", UserId = "admin-user-id" }
        );

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
                Description = "Cookie designs gallery",
                CreatedDate = DateTime.UtcNow,
                IsActive = true,
                CreatedBy = "System"
            }
        );
    }
}
