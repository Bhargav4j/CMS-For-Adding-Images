using GalleryApp.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace GalleryApp.Infrastructure.Data;

/// <summary>
/// Database context for Gallery application
/// </summary>
public class GalleryDbContext : IdentityDbContext<IdentityUser>
{
    public GalleryDbContext(DbContextOptions<GalleryDbContext> options) : base(options)
    {
    }

    public DbSet<Picture> Pictures => Set<Picture>();
    public DbSet<GalleryType> GalleryTypes => Set<GalleryType>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(GalleryDbContext).Assembly);

        // Seed data
        SeedData(modelBuilder);
    }

    private void SeedData(ModelBuilder modelBuilder)
    {
        // Seed gallery types
        modelBuilder.Entity<GalleryType>().HasData(
            new GalleryType
            {
                Id = 1,
                Name = "картины",
                Description = "Paintings gallery",
                CreatedDate = DateTime.UtcNow,
                CreatedBy = "system",
                IsActive = true
            },
            new GalleryType
            {
                Id = 2,
                Name = "пряники",
                Description = "Gingerbread gallery",
                CreatedDate = DateTime.UtcNow,
                CreatedBy = "system",
                IsActive = true
            }
        );

        // Seed admin user
        var hasher = new PasswordHasher<IdentityUser>();
        var adminUser = new IdentityUser
        {
            Id = "admin-user-id",
            UserName = "admin",
            NormalizedUserName = "ADMIN",
            Email = "admin@gallery.com",
            NormalizedEmail = "ADMIN@GALLERY.COM",
            EmailConfirmed = true,
            SecurityStamp = Guid.NewGuid().ToString()
        };
        adminUser.PasswordHash = hasher.HashPassword(adminUser, "123123");

        modelBuilder.Entity<IdentityUser>().HasData(adminUser);

        // Seed roles
        var adminRole = new IdentityRole
        {
            Id = "admin-role-id",
            Name = "admin",
            NormalizedName = "ADMIN"
        };

        var userRole = new IdentityRole
        {
            Id = "user-role-id",
            Name = "user",
            NormalizedName = "USER"
        };

        modelBuilder.Entity<IdentityRole>().HasData(adminRole, userRole);

        // Seed user-role relationship
        modelBuilder.Entity<IdentityUserRole<string>>().HasData(
            new IdentityUserRole<string>
            {
                UserId = adminUser.Id,
                RoleId = adminRole.Id
            },
            new IdentityUserRole<string>
            {
                UserId = adminUser.Id,
                RoleId = userRole.Id
            }
        );
    }
}
