using Xunit;
using Microsoft.EntityFrameworkCore;
using GalleryApp.Domain.Entities;
using GalleryApp.Infrastructure.Data;

namespace GalleryApp.Tests.Infrastructure.Data;

public class ApplicationDbContextTests
{
    private DbContextOptions<ApplicationDbContext> CreateNewContextOptions()
    {
        return new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
    }

    [Fact]
    public void Constructor_WithValidOptions_CreatesContext()
    {
        // Arrange
        var options = CreateNewContextOptions();

        // Act
        using var context = new ApplicationDbContext(options);

        // Assert
        Assert.NotNull(context);
        Assert.NotNull(context.Pictures);
        Assert.NotNull(context.GalleryTypes);
    }

    [Fact]
    public void Pictures_PropertyExists_ReturnsDbSet()
    {
        // Arrange
        var options = CreateNewContextOptions();
        using var context = new ApplicationDbContext(options);

        // Act
        var picturesDbSet = context.Pictures;

        // Assert
        Assert.NotNull(picturesDbSet);
        Assert.IsAssignableFrom<DbSet<Picture>>(picturesDbSet);
    }

    [Fact]
    public void GalleryTypes_PropertyExists_ReturnsDbSet()
    {
        // Arrange
        var options = CreateNewContextOptions();
        using var context = new ApplicationDbContext(options);

        // Act
        var galleryTypesDbSet = context.GalleryTypes;

        // Assert
        Assert.NotNull(galleryTypesDbSet);
        Assert.IsAssignableFrom<DbSet<GalleryType>>(galleryTypesDbSet);
    }

    [Fact]
    public async Task AddPicture_SavesSuccessfully()
    {
        // Arrange
        var options = CreateNewContextOptions();
        using var context = new ApplicationDbContext(options);

        var picture = new Picture
        {
            Name = "Test Picture",
            Description = "Test Description",
            ImagePath = "/images/test.jpg",
            ThumbnailImagePath = "/images/thumbnails/test.jpg",
            GalleryTypeId = 1,
            CreatedDate = DateTime.UtcNow,
            IsActive = true,
            CreatedBy = "test@example.com"
        };

        // Act
        context.Pictures.Add(picture);
        await context.SaveChangesAsync();

        // Assert
        var savedPicture = await context.Pictures.FirstOrDefaultAsync(p => p.Name == "Test Picture");
        Assert.NotNull(savedPicture);
        Assert.Equal("Test Picture", savedPicture.Name);
        Assert.Equal("Test Description", savedPicture.Description);
    }

    [Fact]
    public async Task AddGalleryType_SavesSuccessfully()
    {
        // Arrange
        var options = CreateNewContextOptions();
        using var context = new ApplicationDbContext(options);

        var galleryType = new GalleryType
        {
            Name = "Test Gallery",
            Description = "Test Gallery Description",
            CreatedDate = DateTime.UtcNow,
            IsActive = true,
            CreatedBy = "test@example.com"
        };

        // Act
        context.GalleryTypes.Add(galleryType);
        await context.SaveChangesAsync();

        // Assert
        var savedGalleryType = await context.GalleryTypes.FirstOrDefaultAsync(g => g.Name == "Test Gallery");
        Assert.NotNull(savedGalleryType);
        Assert.Equal("Test Gallery", savedGalleryType.Name);
        Assert.Equal("Test Gallery Description", savedGalleryType.Description);
    }

    [Fact]
    public async Task UpdatePicture_UpdatesSuccessfully()
    {
        // Arrange
        var options = CreateNewContextOptions();
        using var context = new ApplicationDbContext(options);

        var picture = new Picture
        {
            Name = "Original Name",
            ImagePath = "/images/original.jpg",
            ThumbnailImagePath = "/images/thumbnails/original.jpg",
            GalleryTypeId = 1,
            CreatedDate = DateTime.UtcNow,
            IsActive = true,
            CreatedBy = "test@example.com"
        };

        context.Pictures.Add(picture);
        await context.SaveChangesAsync();

        // Act
        picture.Name = "Updated Name";
        picture.ModifiedDate = DateTime.UtcNow;
        picture.ModifiedBy = "updater@example.com";
        await context.SaveChangesAsync();

        // Assert
        var updatedPicture = await context.Pictures.FindAsync(picture.Id);
        Assert.NotNull(updatedPicture);
        Assert.Equal("Updated Name", updatedPicture.Name);
        Assert.NotNull(updatedPicture.ModifiedDate);
        Assert.Equal("updater@example.com", updatedPicture.ModifiedBy);
    }

    [Fact]
    public async Task DeletePicture_DeletesSuccessfully()
    {
        // Arrange
        var options = CreateNewContextOptions();
        using var context = new ApplicationDbContext(options);

        var picture = new Picture
        {
            Name = "To Delete",
            ImagePath = "/images/delete.jpg",
            ThumbnailImagePath = "/images/thumbnails/delete.jpg",
            GalleryTypeId = 1,
            CreatedDate = DateTime.UtcNow,
            IsActive = true,
            CreatedBy = "test@example.com"
        };

        context.Pictures.Add(picture);
        await context.SaveChangesAsync();
        var pictureId = picture.Id;

        // Act
        context.Pictures.Remove(picture);
        await context.SaveChangesAsync();

        // Assert
        var deletedPicture = await context.Pictures.FindAsync(pictureId);
        Assert.Null(deletedPicture);
    }

    [Fact]
    public async Task OnModelCreating_SeedsGalleryTypes()
    {
        // Arrange
        var options = CreateNewContextOptions();
        using var context = new ApplicationDbContext(options);

        // Act
        await context.Database.EnsureCreatedAsync();

        // Assert
        var galleryTypes = await context.GalleryTypes.ToListAsync();
        Assert.NotEmpty(galleryTypes);
        Assert.Contains(galleryTypes, g => g.Name == "картины");
        Assert.Contains(galleryTypes, g => g.Name == "пряники");
    }

    [Fact]
    public async Task OnModelCreating_SeedsAdminUser()
    {
        // Arrange
        var options = CreateNewContextOptions();
        using var context = new ApplicationDbContext(options);

        // Act
        await context.Database.EnsureCreatedAsync();

        // Assert
        var adminUser = await context.Users.FirstOrDefaultAsync(u => u.UserName == "admin");
        Assert.NotNull(adminUser);
        Assert.Equal("admin@galleryapp.com", adminUser.Email);
        Assert.True(adminUser.EmailConfirmed);
    }

    [Fact]
    public async Task OnModelCreating_SeedsRoles()
    {
        // Arrange
        var options = CreateNewContextOptions();
        using var context = new ApplicationDbContext(options);

        // Act
        await context.Database.EnsureCreatedAsync();

        // Assert
        var roles = await context.Roles.ToListAsync();
        Assert.NotEmpty(roles);
        Assert.Contains(roles, r => r.Name == "admin");
        Assert.Contains(roles, r => r.Name == "user");
    }

    [Fact]
    public async Task Picture_GalleryTypeRelationship_WorksCorrectly()
    {
        // Arrange
        var options = CreateNewContextOptions();
        using var context = new ApplicationDbContext(options);

        var galleryType = new GalleryType
        {
            Name = "Test Gallery",
            CreatedDate = DateTime.UtcNow,
            IsActive = true,
            CreatedBy = "system"
        };
        context.GalleryTypes.Add(galleryType);
        await context.SaveChangesAsync();

        var picture = new Picture
        {
            Name = "Test Picture",
            ImagePath = "/images/test.jpg",
            ThumbnailImagePath = "/images/thumbnails/test.jpg",
            GalleryTypeId = galleryType.Id,
            CreatedDate = DateTime.UtcNow,
            IsActive = true,
            CreatedBy = "test@example.com"
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
        Assert.Equal(galleryType.Name, loadedPicture.GalleryType.Name);
    }

    [Fact]
    public async Task GalleryType_PicturesRelationship_WorksCorrectly()
    {
        // Arrange
        var options = CreateNewContextOptions();
        using var context = new ApplicationDbContext(options);

        var galleryType = new GalleryType
        {
            Name = "Test Gallery",
            CreatedDate = DateTime.UtcNow,
            IsActive = true,
            CreatedBy = "system"
        };
        context.GalleryTypes.Add(galleryType);
        await context.SaveChangesAsync();

        var picture1 = new Picture
        {
            Name = "Picture 1",
            ImagePath = "/images/pic1.jpg",
            ThumbnailImagePath = "/images/thumbnails/pic1.jpg",
            GalleryTypeId = galleryType.Id,
            CreatedDate = DateTime.UtcNow,
            IsActive = true,
            CreatedBy = "test@example.com"
        };
        var picture2 = new Picture
        {
            Name = "Picture 2",
            ImagePath = "/images/pic2.jpg",
            ThumbnailImagePath = "/images/thumbnails/pic2.jpg",
            GalleryTypeId = galleryType.Id,
            CreatedDate = DateTime.UtcNow,
            IsActive = true,
            CreatedBy = "test@example.com"
        };
        context.Pictures.AddRange(picture1, picture2);
        await context.SaveChangesAsync();

        // Act
        var loadedGalleryType = await context.GalleryTypes
            .Include(g => g.Pictures)
            .FirstOrDefaultAsync(g => g.Id == galleryType.Id);

        // Assert
        Assert.NotNull(loadedGalleryType);
        Assert.NotNull(loadedGalleryType.Pictures);
        Assert.Equal(2, loadedGalleryType.Pictures.Count);
    }
}
