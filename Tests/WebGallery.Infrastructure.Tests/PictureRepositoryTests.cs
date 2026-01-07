using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using WebGallery.Domain.Entities;
using WebGallery.Infrastructure.Data;
using WebGallery.Infrastructure.Repositories;

namespace Tests.WebGallery.Infrastructure;

public class PictureRepositoryTests
{
    private readonly Mock<ILogger<PictureRepository>> _mockLogger;

    public PictureRepositoryTests()
    {
        _mockLogger = new Mock<ILogger<PictureRepository>>();
    }

    private DbContextOptions<ApplicationDbContext> CreateInMemoryOptions()
    {
        return new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
    }

    [Fact]
    public void Constructor_WithNullContext_ThrowsArgumentNullException()
    {
        // Arrange & Act & Assert
        Assert.Throws<ArgumentNullException>(() => new PictureRepository(null!, _mockLogger.Object));
    }

    [Fact]
    public void Constructor_WithNullLogger_ThrowsArgumentNullException()
    {
        // Arrange
        var options = CreateInMemoryOptions();
        var context = new ApplicationDbContext(options);

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new PictureRepository(context, null!));
    }

    [Fact]
    public async Task GetAllAsync_ReturnsOnlyActivePictures()
    {
        // Arrange
        var options = CreateInMemoryOptions();
        using var context = new ApplicationDbContext(options);
        var repository = new PictureRepository(context, _mockLogger.Object);

        var activePicture = new Picture { Name = "Active", ImagePath = "/img1.jpg", CreatedDate = DateTime.UtcNow, CreatedBy = "user", IsActive = true, GalleryTypeId = 1 };
        var inactivePicture = new Picture { Name = "Inactive", ImagePath = "/img2.jpg", CreatedDate = DateTime.UtcNow, CreatedBy = "user", IsActive = false, GalleryTypeId = 1 };

        context.Pictures.AddRange(activePicture, inactivePicture);
        await context.SaveChangesAsync();

        // Act
        var result = await repository.GetAllAsync();

        // Assert
        Assert.Single(result);
        Assert.Equal("Active", result.First().Name);
    }

    [Fact]
    public async Task GetAllAsync_OrdersByCreatedDateDescending()
    {
        // Arrange
        var options = CreateInMemoryOptions();
        using var context = new ApplicationDbContext(options);
        var repository = new PictureRepository(context, _mockLogger.Object);

        var oldPicture = new Picture { Name = "Old", ImagePath = "/img1.jpg", CreatedDate = DateTime.UtcNow.AddDays(-2), CreatedBy = "user", IsActive = true, GalleryTypeId = 1 };
        var newPicture = new Picture { Name = "New", ImagePath = "/img2.jpg", CreatedDate = DateTime.UtcNow, CreatedBy = "user", IsActive = true, GalleryTypeId = 1 };

        context.Pictures.AddRange(oldPicture, newPicture);
        await context.SaveChangesAsync();

        // Act
        var result = await repository.GetAllAsync();

        // Assert
        Assert.Equal(2, result.Count());
        Assert.Equal("New", result.First().Name);
    }

    [Fact]
    public async Task GetByIdAsync_WithValidId_ReturnsPicture()
    {
        // Arrange
        var options = CreateInMemoryOptions();
        using var context = new ApplicationDbContext(options);
        var repository = new PictureRepository(context, _mockLogger.Object);

        var picture = new Picture { Name = "Test", ImagePath = "/img.jpg", CreatedDate = DateTime.UtcNow, CreatedBy = "user", IsActive = true, GalleryTypeId = 1 };
        context.Pictures.Add(picture);
        await context.SaveChangesAsync();

        // Act
        var result = await repository.GetByIdAsync(picture.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Test", result.Name);
    }

    [Fact]
    public async Task GetByIdAsync_WithInvalidId_ReturnsNull()
    {
        // Arrange
        var options = CreateInMemoryOptions();
        using var context = new ApplicationDbContext(options);
        var repository = new PictureRepository(context, _mockLogger.Object);

        // Act
        var result = await repository.GetByIdAsync(999);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task AddAsync_CreatesPictureWithCorrectDefaults()
    {
        // Arrange
        var options = CreateInMemoryOptions();
        using var context = new ApplicationDbContext(options);
        var repository = new PictureRepository(context, _mockLogger.Object);

        var picture = new Picture { Name = "New Picture", ImagePath = "/img.jpg", CreatedBy = "user", GalleryTypeId = 1 };

        // Act
        var result = await repository.AddAsync(picture);

        // Assert
        Assert.True(result.IsActive);
        Assert.NotEqual(default(DateTime), result.CreatedDate);
    }

    [Fact]
    public async Task UpdateAsync_UpdatesPictureAndSetsModifiedDate()
    {
        // Arrange
        var options = CreateInMemoryOptions();
        using var context = new ApplicationDbContext(options);
        var repository = new PictureRepository(context, _mockLogger.Object);

        var picture = new Picture { Name = "Original", ImagePath = "/img.jpg", CreatedDate = DateTime.UtcNow, CreatedBy = "user", IsActive = true, GalleryTypeId = 1 };
        context.Pictures.Add(picture);
        await context.SaveChangesAsync();

        // Act
        picture.Name = "Updated";
        await repository.UpdateAsync(picture);

        // Assert
        var updatedPicture = await context.Pictures.FindAsync(picture.Id);
        Assert.NotNull(updatedPicture);
        Assert.Equal("Updated", updatedPicture.Name);
        Assert.NotNull(updatedPicture.ModifiedDate);
    }

    [Fact]
    public async Task DeleteAsync_SoftDeletesPicture()
    {
        // Arrange
        var options = CreateInMemoryOptions();
        using var context = new ApplicationDbContext(options);
        var repository = new PictureRepository(context, _mockLogger.Object);

        var picture = new Picture { Name = "To Delete", ImagePath = "/img.jpg", CreatedDate = DateTime.UtcNow, CreatedBy = "user", IsActive = true, GalleryTypeId = 1 };
        context.Pictures.Add(picture);
        await context.SaveChangesAsync();

        // Act
        await repository.DeleteAsync(picture.Id);

        // Assert
        var deletedPicture = await context.Pictures.FindAsync(picture.Id);
        Assert.NotNull(deletedPicture);
        Assert.False(deletedPicture.IsActive);
        Assert.NotNull(deletedPicture.ModifiedDate);
    }

    [Fact]
    public async Task DeleteAsync_WithNonExistentId_DoesNotThrow()
    {
        // Arrange
        var options = CreateInMemoryOptions();
        using var context = new ApplicationDbContext(options);
        var repository = new PictureRepository(context, _mockLogger.Object);

        // Act
        await repository.DeleteAsync(999);

        // Assert - No exception should be thrown
        Assert.True(true);
    }

    [Fact]
    public async Task ExistsAsync_WithExistingId_ReturnsTrue()
    {
        // Arrange
        var options = CreateInMemoryOptions();
        using var context = new ApplicationDbContext(options);
        var repository = new PictureRepository(context, _mockLogger.Object);

        var picture = new Picture { Name = "Test", ImagePath = "/img.jpg", CreatedDate = DateTime.UtcNow, CreatedBy = "user", IsActive = true, GalleryTypeId = 1 };
        context.Pictures.Add(picture);
        await context.SaveChangesAsync();

        // Act
        var exists = await repository.ExistsAsync(picture.Id);

        // Assert
        Assert.True(exists);
    }

    [Fact]
    public async Task ExistsAsync_WithNonExistentId_ReturnsFalse()
    {
        // Arrange
        var options = CreateInMemoryOptions();
        using var context = new ApplicationDbContext(options);
        var repository = new PictureRepository(context, _mockLogger.Object);

        // Act
        var exists = await repository.ExistsAsync(999);

        // Assert
        Assert.False(exists);
    }

    [Fact]
    public async Task SearchAsync_FindsMatchingPictures()
    {
        // Arrange
        var options = CreateInMemoryOptions();
        using var context = new ApplicationDbContext(options);
        var repository = new PictureRepository(context, _mockLogger.Object);

        var cake1 = new Picture { Name = "Chocolate Cake", Description = "Delicious", ImagePath = "/img1.jpg", CreatedDate = DateTime.UtcNow, CreatedBy = "user", IsActive = true, GalleryTypeId = 1 };
        var cake2 = new Picture { Name = "Vanilla Cake", Description = "Sweet", ImagePath = "/img2.jpg", CreatedDate = DateTime.UtcNow, CreatedBy = "user", IsActive = true, GalleryTypeId = 1 };
        var book = new Picture { Name = "Book", Description = "Novel", ImagePath = "/img3.jpg", CreatedDate = DateTime.UtcNow, CreatedBy = "user", IsActive = true, GalleryTypeId = 1 };

        context.Pictures.AddRange(cake1, cake2, book);
        await context.SaveChangesAsync();

        // Act
        var result = await repository.SearchAsync("Cake");

        // Assert
        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task GetByGalleryTypeAsync_ReturnsOnlyPicturesFromSpecifiedType()
    {
        // Arrange
        var options = CreateInMemoryOptions();
        using var context = new ApplicationDbContext(options);
        var repository = new PictureRepository(context, _mockLogger.Object);

        var picture1 = new Picture { Name = "Cake 1", ImagePath = "/img1.jpg", CreatedDate = DateTime.UtcNow, CreatedBy = "user", IsActive = true, GalleryTypeId = 1 };
        var picture2 = new Picture { Name = "Cake 2", ImagePath = "/img2.jpg", CreatedDate = DateTime.UtcNow, CreatedBy = "user", IsActive = true, GalleryTypeId = 1 };
        var picture3 = new Picture { Name = "Book", ImagePath = "/img3.jpg", CreatedDate = DateTime.UtcNow, CreatedBy = "user", IsActive = true, GalleryTypeId = 2 };

        context.Pictures.AddRange(picture1, picture2, picture3);
        await context.SaveChangesAsync();

        // Act
        var result = await repository.GetByGalleryTypeAsync(1);

        // Assert
        Assert.Equal(2, result.Count());
        Assert.All(result, p => Assert.Equal(1, p.GalleryTypeId));
    }
}
