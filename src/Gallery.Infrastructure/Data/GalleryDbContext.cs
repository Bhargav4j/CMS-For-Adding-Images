using Gallery.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Gallery.Infrastructure.Data;

/// <summary>
/// Database context for the Gallery application
/// </summary>
public class GalleryDbContext : IdentityDbContext<IdentityUser>
{
    public GalleryDbContext(DbContextOptions<GalleryDbContext> options)
        : base(options)
    {
    }

    public DbSet<Picture> Pictures { get; set; } = null!;
    public DbSet<GalleryType> GalleryTypes { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(GalleryDbContext).Assembly);

        // Seed data
        modelBuilder.Entity<GalleryType>().HasData(
            new GalleryType
            {
                Id = 1,
                Name = "картины",
                Description = "Gallery for paintings",
                CreatedDate = DateTime.UtcNow,
                IsActive = true,
                CreatedBy = "system"
            },
            new GalleryType
            {
                Id = 2,
                Name = "пряники",
                Description = "Gallery for cookies",
                CreatedDate = DateTime.UtcNow,
                IsActive = true,
                CreatedBy = "system"
            }
        );

        // Seed admin role and user
        var adminRoleId = "1";
        var userRoleId = "2";
        var adminUserId = "1";

        modelBuilder.Entity<IdentityRole>().HasData(
            new IdentityRole
            {
                Id = adminRoleId,
                Name = "admin",
                NormalizedName = "ADMIN",
                ConcurrencyStamp = Guid.NewGuid().ToString()
            },
            new IdentityRole
            {
                Id = userRoleId,
                Name = "user",
                NormalizedName = "USER",
                ConcurrencyStamp = Guid.NewGuid().ToString()
            }
        );

        var hasher = new PasswordHasher<IdentityUser>();
        var adminUser = new IdentityUser
        {
            Id = adminUserId,
            UserName = "admin",
            NormalizedUserName = "ADMIN",
            Email = "admin@gallery.com",
            NormalizedEmail = "ADMIN@GALLERY.COM",
            EmailConfirmed = true,
            SecurityStamp = Guid.NewGuid().ToString(),
            ConcurrencyStamp = Guid.NewGuid().ToString()
        };
        adminUser.PasswordHash = hasher.HashPassword(adminUser, "Admin123!");

        modelBuilder.Entity<IdentityUser>().HasData(adminUser);

        modelBuilder.Entity<IdentityUserRole<string>>().HasData(
            new IdentityUserRole<string>
            {
                RoleId = adminRoleId,
                UserId = adminUserId
            },
            new IdentityUserRole<string>
            {
                RoleId = userRoleId,
                UserId = adminUserId
            }
        );
    }
}
