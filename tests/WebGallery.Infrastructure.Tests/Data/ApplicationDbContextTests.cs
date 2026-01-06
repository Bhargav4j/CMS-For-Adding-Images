using Xunit;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using WebGallery.Infrastructure.Data;
using WebGallery.Domain.Entities;

namespace WebGallery.Infrastructure.Tests.Data;

public class ApplicationDbContextTests
{
    private DbContextOptions<ApplicationDbContext> CreateNewContextOptions()
    {
        return new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
    }

    [Fact]
    public void Constructor_WithValidOptions_ShouldCreateContext()
    {
        // Arrange
        var options = CreateNewContextOptions();

        // Act
        using var context = new ApplicationDbContext(options);

        // Assert
        Assert.NotNull(context);
    }

    [Fact]
    public void DbSet_Pictures_ShouldNotBeNull()
    {
        // Arrange
        var options = CreateNewContextOptions();

        // Act
        using var context = new ApplicationDbContext(options);

        // Assert
        Assert.NotNull(context.Pictures);
    }

    [Fact]
    public void DbSet_GalleryTypes_ShouldNotBeNull()
    {
        // Arrange
        var options = CreateNewContextOptions();

        // Act
        using var context = new ApplicationDbContext(options);

        // Assert
        Assert.NotNull(context.GalleryTypes);
    }

    [Fact]
    public async Task OnModelCreating_ShouldSeedAdminRole()
    {
        // Arrange
        var options = CreateNewContextOptions();

        // Act
        using var context = new ApplicationDbContext(options);
        await context.Database.EnsureCreatedAsync();

        // Assert
        var adminRole = await context.Roles.FirstOrDefaultAsync(r => r.Name == "admin");
        Assert.NotNull(adminRole);
        Assert.Equal("ADMIN", adminRole.NormalizedName);
    }

    [Fact]
    public async Task OnModelCreating_ShouldSeedUserRole()
    {
        // Arrange
        var options = CreateNewContextOptions();

        // Act
        using var context = new ApplicationDbContext(options);
        await context.Database.EnsureCreatedAsync();

        // Assert
        var userRole = await context.Roles.FirstOrDefaultAsync(r => r.Name == "user");
        Assert.NotNull(userRole);
        Assert.Equal("USER", userRole.NormalizedName);
    }

    [Fact]
    public async Task OnModelCreating_ShouldSeedAdminUser()
    {
        // Arrange
        var options = CreateNewContextOptions();

        // Act
        using var context = new ApplicationDbContext(options);
        await context.Database.EnsureCreatedAsync();

        // Assert
        var adminUser = await context.Users.FirstOrDefaultAsync(u => u.UserName == "admin");
        Assert.NotNull(adminUser);
        Assert.Equal("admin@webgallery.com", adminUser.Email);
        Assert.Equal("ADMIN@WEBGALLERY.COM", adminUser.NormalizedEmail);
        Assert.True(adminUser.EmailConfirmed);
        Assert.NotNull(adminUser.PasswordHash);
    }

    [Fact]
    public async Task OnModelCreating_ShouldAssignAdminRoleToAdminUser()
    {
        // Arrange
        var options = CreateNewContextOptions();

        // Act
        using var context = new ApplicationDbContext(options);
        await context.Database.EnsureCreatedAsync();

        // Assert
        var adminUser = await context.Users.FirstOrDefaultAsync(u => u.UserName == "admin");
        Assert.NotNull(adminUser);

        var userRoles = await context.UserRoles
            .Where(ur => ur.UserId == adminUser.Id)
            .ToListAsync();

        Assert.NotEmpty(userRoles);
        Assert.Contains(userRoles, ur => ur.RoleId == "1"); // Admin role
    }

    [Fact]
    public async Task OnModelCreating_ShouldAssignUserRoleToAdminUser()
    {
        // Arrange
        var options = CreateNewContextOptions();

        // Act
        using var context = new ApplicationDbContext(options);
        await context.Database.EnsureCreatedAsync();

        // Assert
        var adminUser = await context.Users.FirstOrDefaultAsync(u => u.UserName == "admin");
        Assert.NotNull(adminUser);

        var userRoles = await context.UserRoles
            .Where(ur => ur.UserId == adminUser.Id)
            .ToListAsync();

        Assert.Contains(userRoles, ur => ur.RoleId == "2"); // User role
    }

    [Fact]
    public async Task OnModelCreating_ShouldSeedPaintingsGalleryType()
    {
        // Arrange
        var options = CreateNewContextOptions();

        // Act
        using var context = new ApplicationDbContext(options);
        await context.Database.EnsureCreatedAsync();

        // Assert
        var paintingsGallery = await context.GalleryTypes.FindAsync(1);
        Assert.NotNull(paintingsGallery);
        Assert.Equal("Paintings", paintingsGallery.Name);
        Assert.Equal("Art paintings gallery", paintingsGallery.Description);
        Assert.True(paintingsGallery.IsActive);
        Assert.Equal("System", paintingsGallery.CreatedBy);
    }

    [Fact]
    public async Task OnModelCreating_ShouldSeedCookiesGalleryType()
    {
        // Arrange
        var options = CreateNewContextOptions();

        // Act
        using var context = new ApplicationDbContext(options);
        await context.Database.EnsureCreatedAsync();

        // Assert
        var cookiesGallery = await context.GalleryTypes.FindAsync(2);
        Assert.NotNull(cookiesGallery);
        Assert.Equal("Cookies", cookiesGallery.Name);
        Assert.Equal("Cookie designs gallery", cookiesGallery.Description);
        Assert.True(cookiesGallery.IsActive);
        Assert.Equal("System", cookiesGallery.CreatedBy);
    }

    [Fact]
    public async Task Pictures_Add_ShouldAddPictureToDatabase()
    {
        // Arrange
        var options = CreateNewContextOptions();
        using var context = new ApplicationDbContext(options);
        await context.Database.EnsureCreatedAsync();

        var picture = new Picture
        {
            Name = "Test Picture",
            ImagePath = "/images/test.jpg",
            GalleryTypeId = 1,
            CreatedBy = "testuser"
        };

        // Act
        context.Pictures.Add(picture);
        await context.SaveChangesAsync();

        // Assert
        var savedPicture = await context.Pictures.FirstOrDefaultAsync(p => p.Name == "Test Picture");
        Assert.NotNull(savedPicture);
        Assert.Equal("Test Picture", savedPicture.Name);
        Assert.Equal("/images/test.jpg", savedPicture.ImagePath);
    }

    [Fact]
    public async Task GalleryTypes_Add_ShouldAddGalleryTypeToDatabase()
    {
        // Arrange
        var options = CreateNewContextOptions();
        using var context = new ApplicationDbContext(options);
        await context.Database.EnsureCreatedAsync();

        var galleryType = new GalleryType
        {
            Name = "Nature",
            Description = "Nature photography",
            CreatedBy = "testuser"
        };

        // Act
        context.GalleryTypes.Add(galleryType);
        await context.SaveChangesAsync();

        // Assert
        var savedGalleryType = await context.GalleryTypes
            .FirstOrDefaultAsync(g => g.Name == "Nature");
        Assert.NotNull(savedGalleryType);
        Assert.Equal("Nature", savedGalleryType.Name);
        Assert.Equal("Nature photography", savedGalleryType.Description);
    }

    [Fact]
    public async Task Pictures_Remove_ShouldRemovePictureFromDatabase()
    {
        // Arrange
        var options = CreateNewContextOptions();
        using var context = new ApplicationDbContext(options);
        await context.Database.EnsureCreatedAsync();

        var picture = new Picture
        {
            Name = "Delete Me",
            ImagePath = "/images/delete.jpg",
            GalleryTypeId = 1,
            CreatedBy = "testuser"
        };
        context.Pictures.Add(picture);
        await context.SaveChangesAsync();

        // Act
        context.Pictures.Remove(picture);
        await context.SaveChangesAsync();

        // Assert
        var deletedPicture = await context.Pictures.FindAsync(picture.Id);
        Assert.Null(deletedPicture);
    }

    [Fact]
    public async Task Pictures_Update_ShouldUpdatePictureInDatabase()
    {
        // Arrange
        var options = CreateNewContextOptions();
        using var context = new ApplicationDbContext(options);
        await context.Database.EnsureCreatedAsync();

        var picture = new Picture
        {
            Name = "Original Name",
            ImagePath = "/images/original.jpg",
            GalleryTypeId = 1,
            CreatedBy = "testuser"
        };
        context.Pictures.Add(picture);
        await context.SaveChangesAsync();

        // Act
        picture.Name = "Updated Name";
        picture.ImagePath = "/images/updated.jpg";
        await context.SaveChangesAsync();

        // Assert
        var updatedPicture = await context.Pictures.FindAsync(picture.Id);
        Assert.NotNull(updatedPicture);
        Assert.Equal("Updated Name", updatedPicture.Name);
        Assert.Equal("/images/updated.jpg", updatedPicture.ImagePath);
    }

    [Fact]
    public async Task Pictures_WithGalleryType_ShouldLoadNavigationProperty()
    {
        // Arrange
        var options = CreateNewContextOptions();
        using var context = new ApplicationDbContext(options);
        await context.Database.EnsureCreatedAsync();

        var picture = new Picture
        {
            Name = "Test",
            ImagePath = "/test.jpg",
            GalleryTypeId = 1,
            CreatedBy = "test"
        };
        context.Pictures.Add(picture);
        await context.SaveChangesAsync();

        // Act
        var loadedPicture = await context.Pictures
            .Include(p => p.GalleryType)
            .FirstOrDefaultAsync(p => p.Id == picture.Id);

        // Assert
        Assert.NotNull(loadedPicture);
        Assert.NotNull(loadedPicture.GalleryType);
        Assert.Equal("Paintings", loadedPicture.GalleryType.Name);
    }

    [Fact]
    public async Task GalleryTypes_WithPictures_ShouldLoadNavigationProperty()
    {
        // Arrange
        var options = CreateNewContextOptions();
        using var context = new ApplicationDbContext(options);
        await context.Database.EnsureCreatedAsync();

        var picture1 = new Picture { Name = "Pic1", ImagePath = "/pic1.jpg", GalleryTypeId = 1, CreatedBy = "test" };
        var picture2 = new Picture { Name = "Pic2", ImagePath = "/pic2.jpg", GalleryTypeId = 1, CreatedBy = "test" };
        context.Pictures.AddRange(picture1, picture2);
        await context.SaveChangesAsync();

        // Act
        var galleryType = await context.GalleryTypes
            .Include(g => g.Pictures)
            .FirstOrDefaultAsync(g => g.Id == 1);

        // Assert
        Assert.NotNull(galleryType);
        Assert.NotNull(galleryType.Pictures);
        Assert.True(galleryType.Pictures.Count >= 2);
    }
}
