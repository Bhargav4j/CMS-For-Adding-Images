using Microsoft.EntityFrameworkCore;
using WebGallery.Domain.Entities;

namespace WebGallery.Infrastructure.Data;

/// <summary>
/// Application database context using EF Core
/// </summary>
public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Picture> Pictures { get; set; } = null!;
    public DbSet<GalleryType> GalleryTypes { get; set; } = null!;
    public DbSet<User> Users { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }
}
