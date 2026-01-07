using Microsoft.EntityFrameworkCore;
using Xunit;
using WebGallery.Domain.Entities;
using WebGallery.Infrastructure.Data;

namespace Tests.WebGallery.Infrastructure;

public class ApplicationDbContextTests
{
    private DbContextOptions<ApplicationDbContext> CreateInMemoryOptions()
    {
        return new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
    }

    [Fact]
    public void Constructor_WithValidOptions_CreatesContext()
    {
        // Arrange
        var options = CreateInMemoryOptions();

        // Act
        using var context = new ApplicationDbContext(options);

        // Assert
        Assert.NotNull(context);
        Assert.NotNull(context.Pictures);
        Assert.NotNull(context.GalleryTypes);
    }

    [Fact]
    public void DbContext_HasPicturesDbSet()
    {
        // Arrange
        var options = CreateInMemoryOptions();

        // Act
        using var context = new ApplicationDbContext(options);

        // Assert
        Assert.NotNull(context.Pictures);
    }

    [Fact]
    public void DbContext_HasGalleryTypesDbSet()
    {
        // Arrange
        var options = CreateInMemoryOptions();

        // Act
        using var context = new ApplicationDbContext(options);

        // Assert
        Assert.NotNull(context.GalleryTypes);
    }

    [Fact]
    public async Task DbContext_CanAddPicture()
    {
        // Arrange
        var options = CreateInMemoryOptions();
        using var context = new ApplicationDbContext(options);

        var picture = new Picture
        {
            Name = "Test Picture",
            Description = "Test Description",
            ImagePath = "/images/test.jpg",
            ThumbnailImagePath = "/images/thumb.jpg",
            GalleryTypeId = 1,
            CreatedDate = DateTime.UtcNow,
            IsActive = true,
            CreatedBy = "admin"
        };

        // Act
        context.Pictures.Add(picture);
        await context.SaveChangesAsync();

        // Assert
        var savedPicture = await context.Pictures.FirstOrDefaultAsync();
        Assert.NotNull(savedPicture);
        Assert.Equal("Test Picture", savedPicture.Name);
    }

    [Fact]
    public async Task DbContext_CanAddGalleryType()
    {
        // Arrange
        var options = CreateInMemoryOptions();
        using var context = new ApplicationDbContext(options);

        var galleryType = new GalleryType
        {
            Name = "Test Gallery",
            Description = "Test Description",
            CreatedDate = DateTime.UtcNow,
            IsActive = true,
            CreatedBy = "admin"
        };

        // Act
        context.GalleryTypes.Add(galleryType);
        await context.SaveChangesAsync();

        // Assert
        var savedGalleryType = await context.GalleryTypes.FirstOrDefaultAsync(g => g.Name == "Test Gallery");
        Assert.NotNull(savedGalleryType);
        Assert.Equal("Test Gallery", savedGalleryType.Name);
    }

    [Fact]
    public async Task DbContext_CanQueryPictures()
    {
        // Arrange
        var options = CreateInMemoryOptions();
        using var context = new ApplicationDbContext(options);

        var picture1 = new Picture { Name = "Picture 1", ImagePath = "/img1.jpg", CreatedDate = DateTime.UtcNow, CreatedBy = "user", GalleryTypeId = 1 };
        var picture2 = new Picture { Name = "Picture 2", ImagePath = "/img2.jpg", CreatedDate = DateTime.UtcNow, CreatedBy = "user", GalleryTypeId = 1 };

        context.Pictures.AddRange(picture1, picture2);
        await context.SaveChangesAsync();

        // Act
        var pictures = await context.Pictures.ToListAsync();

        // Assert
        Assert.Equal(2, pictures.Count);
    }

    [Fact]
    public async Task DbContext_CanQueryGalleryTypes()
    {
        // Arrange
        var options = CreateInMemoryOptions();
        using var context = new ApplicationDbContext(options);

        var gallery1 = new GalleryType { Name = "Gallery 1", CreatedDate = DateTime.UtcNow, CreatedBy = "admin" };
        var gallery2 = new GalleryType { Name = "Gallery 2", CreatedDate = DateTime.UtcNow, CreatedBy = "admin" };

        context.GalleryTypes.AddRange(gallery1, gallery2);
        await context.SaveChangesAsync();

        // Act
        var galleries = await context.GalleryTypes.ToListAsync();

        // Assert
        Assert.True(galleries.Count >= 2);
    }

    [Fact]
    public async Task DbContext_CanUpdatePicture()
    {
        // Arrange
        var options = CreateInMemoryOptions();
        using var context = new ApplicationDbContext(options);

        var picture = new Picture
        {
            Name = "Original Name",
            ImagePath = "/images/test.jpg",
            CreatedDate = DateTime.UtcNow,
            CreatedBy = "admin",
            GalleryTypeId = 1
        };

        context.Pictures.Add(picture);
        await context.SaveChangesAsync();

        // Act
        picture.Name = "Updated Name";
        await context.SaveChangesAsync();

        // Assert
        var updatedPicture = await context.Pictures.FirstOrDefaultAsync();
        Assert.NotNull(updatedPicture);
        Assert.Equal("Updated Name", updatedPicture.Name);
    }

    [Fact]
    public async Task DbContext_CanDeletePicture()
    {
        // Arrange
        var options = CreateInMemoryOptions();
        using var context = new ApplicationDbContext(options);

        var picture = new Picture
        {
            Name = "To Delete",
            ImagePath = "/images/test.jpg",
            CreatedDate = DateTime.UtcNow,
            CreatedBy = "admin",
            GalleryTypeId = 1
        };

        context.Pictures.Add(picture);
        await context.SaveChangesAsync();

        // Act
        context.Pictures.Remove(picture);
        await context.SaveChangesAsync();

        // Assert
        var deletedPicture = await context.Pictures.FirstOrDefaultAsync();
        Assert.Null(deletedPicture);
    }

    [Fact]
    public async Task DbContext_PictureConfiguration_NameIsRequired()
    {
        // Arrange
        var options = CreateInMemoryOptions();
        using var context = new ApplicationDbContext(options);

        var picture = new Picture
        {
            Name = null!,
            ImagePath = "/images/test.jpg",
            CreatedDate = DateTime.UtcNow,
            CreatedBy = "admin",
            GalleryTypeId = 1
        };

        context.Pictures.Add(picture);

        // Act & Assert
        await Assert.ThrowsAsync<DbUpdateException>(() => context.SaveChangesAsync());
    }

    [Fact]
    public async Task DbContext_GalleryTypeConfiguration_NameIsRequired()
    {
        // Arrange
        var options = CreateInMemoryOptions();
        using var context = new ApplicationDbContext(options);

        var galleryType = new GalleryType
        {
            Name = null!,
            CreatedDate = DateTime.UtcNow,
            CreatedBy = "admin"
        };

        context.GalleryTypes.Add(galleryType);

        // Act & Assert
        await Assert.ThrowsAsync<DbUpdateException>(() => context.SaveChangesAsync());
    }
}
