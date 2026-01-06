using Xunit;
using Moq;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using WebGallery.Infrastructure.Repositories;
using WebGallery.Infrastructure.Data;
using WebGallery.Domain.Entities;

namespace WebGallery.Infrastructure.Tests.Repositories;

public class PictureRepositoryTests
{
    private readonly Mock<ILogger<PictureRepository>> _mockLogger;

    public PictureRepositoryTests()
    {
        _mockLogger = new Mock<ILogger<PictureRepository>>();
    }

    private DbContextOptions<ApplicationDbContext> CreateNewContextOptions()
    {
        return new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
    }

    [Fact]
    public void Constructor_WithNullContext_ShouldThrowArgumentNullException()
    {
        // Arrange & Act & Assert
        Assert.Throws<ArgumentNullException>(() =>
            new PictureRepository(null!, _mockLogger.Object));
    }

    [Fact]
    public void Constructor_WithNullLogger_ShouldThrowArgumentNullException()
    {
        // Arrange
        var options = CreateNewContextOptions();
        var context = new ApplicationDbContext(options);

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() =>
            new PictureRepository(context, null!));
    }

    [Fact]
    public void Constructor_WithValidParameters_ShouldCreateInstance()
    {
        // Arrange
        var options = CreateNewContextOptions();
        var context = new ApplicationDbContext(options);

        // Act
        var repository = new PictureRepository(context, _mockLogger.Object);

        // Assert
        Assert.NotNull(repository);
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnOnlyActivePictures()
    {
        // Arrange
        var options = CreateNewContextOptions();
        using var context = new ApplicationDbContext(options);
        await context.Database.EnsureCreatedAsync();

        var activePicture = new Picture { Name = "Active", ImagePath = "/active.jpg", IsActive = true, GalleryTypeId = 1, CreatedBy = "test" };
        var inactivePicture = new Picture { Name = "Inactive", ImagePath = "/inactive.jpg", IsActive = false, GalleryTypeId = 1, CreatedBy = "test" };
        context.Pictures.AddRange(activePicture, inactivePicture);
        await context.SaveChangesAsync();

        var repository = new PictureRepository(context, _mockLogger.Object);

        // Act
        var result = await repository.GetAllAsync();

        // Assert
        Assert.Single(result);
        Assert.Contains(result, p => p.Name == "Active");
        Assert.DoesNotContain(result, p => p.Name == "Inactive");
    }

    [Fact]
    public async Task GetAllAsync_ShouldOrderByCreatedDateDescending()
    {
        // Arrange
        var options = CreateNewContextOptions();
        using var context = new ApplicationDbContext(options);
        await context.Database.EnsureCreatedAsync();

        var picture1 = new Picture { Name = "First", ImagePath = "/1.jpg", CreatedDate = DateTime.UtcNow.AddDays(-2), IsActive = true, GalleryTypeId = 1, CreatedBy = "test" };
        var picture2 = new Picture { Name = "Second", ImagePath = "/2.jpg", CreatedDate = DateTime.UtcNow.AddDays(-1), IsActive = true, GalleryTypeId = 1, CreatedBy = "test" };
        var picture3 = new Picture { Name = "Third", ImagePath = "/3.jpg", CreatedDate = DateTime.UtcNow, IsActive = true, GalleryTypeId = 1, CreatedBy = "test" };
        context.Pictures.AddRange(picture1, picture2, picture3);
        await context.SaveChangesAsync();

        var repository = new PictureRepository(context, _mockLogger.Object);

        // Act
        var result = (await repository.GetAllAsync()).ToList();

        // Assert
        Assert.Equal("Third", result[0].Name);
        Assert.Equal("Second", result[1].Name);
        Assert.Equal("First", result[2].Name);
    }

    [Fact]
    public async Task GetAllAsync_ShouldIncludeGalleryType()
    {
        // Arrange
        var options = CreateNewContextOptions();
        using var context = new ApplicationDbContext(options);
        await context.Database.EnsureCreatedAsync();

        var picture = new Picture { Name = "Test", ImagePath = "/test.jpg", IsActive = true, GalleryTypeId = 1, CreatedBy = "test" };
        context.Pictures.Add(picture);
        await context.SaveChangesAsync();

        var repository = new PictureRepository(context, _mockLogger.Object);

        // Act
        var result = await repository.GetAllAsync();

        // Assert
        var firstPicture = result.First();
        Assert.NotNull(firstPicture.GalleryType);
    }

    [Fact]
    public async Task GetByIdAsync_WithValidId_ShouldReturnPicture()
    {
        // Arrange
        var options = CreateNewContextOptions();
        using var context = new ApplicationDbContext(options);
        await context.Database.EnsureCreatedAsync();

        var picture = new Picture { Name = "Test", ImagePath = "/test.jpg", IsActive = true, GalleryTypeId = 1, CreatedBy = "test" };
        context.Pictures.Add(picture);
        await context.SaveChangesAsync();

        var repository = new PictureRepository(context, _mockLogger.Object);

        // Act
        var result = await repository.GetByIdAsync(picture.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(picture.Id, result.Id);
        Assert.Equal("Test", result.Name);
    }

    [Fact]
    public async Task GetByIdAsync_WithInvalidId_ShouldReturnNull()
    {
        // Arrange
        var options = CreateNewContextOptions();
        using var context = new ApplicationDbContext(options);
        await context.Database.EnsureCreatedAsync();

        var repository = new PictureRepository(context, _mockLogger.Object);

        // Act
        var result = await repository.GetByIdAsync(9999);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetByIdAsync_WithInactivePicture_ShouldReturnNull()
    {
        // Arrange
        var options = CreateNewContextOptions();
        using var context = new ApplicationDbContext(options);
        await context.Database.EnsureCreatedAsync();

        var picture = new Picture { Name = "Inactive", ImagePath = "/test.jpg", IsActive = false, GalleryTypeId = 1, CreatedBy = "test" };
        context.Pictures.Add(picture);
        await context.SaveChangesAsync();

        var repository = new PictureRepository(context, _mockLogger.Object);

        // Act
        var result = await repository.GetByIdAsync(picture.Id);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task AddAsync_WithValidPicture_ShouldAddToDatabase()
    {
        // Arrange
        var options = CreateNewContextOptions();
        using var context = new ApplicationDbContext(options);
        await context.Database.EnsureCreatedAsync();

        var repository = new PictureRepository(context, _mockLogger.Object);
        var picture = new Picture { Name = "New Picture", ImagePath = "/new.jpg", IsActive = true, GalleryTypeId = 1, CreatedBy = "test" };

        // Act
        var result = await repository.AddAsync(picture);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Id > 0);
        var savedPicture = await context.Pictures.FindAsync(result.Id);
        Assert.NotNull(savedPicture);
        Assert.Equal("New Picture", savedPicture.Name);
    }

    [Fact]
    public async Task UpdateAsync_WithValidPicture_ShouldUpdateInDatabase()
    {
        // Arrange
        var options = CreateNewContextOptions();
        using var context = new ApplicationDbContext(options);
        await context.Database.EnsureCreatedAsync();

        var picture = new Picture { Name = "Original", ImagePath = "/original.jpg", IsActive = true, GalleryTypeId = 1, CreatedBy = "test" };
        context.Pictures.Add(picture);
        await context.SaveChangesAsync();

        var repository = new PictureRepository(context, _mockLogger.Object);
        picture.Name = "Updated";

        // Act
        await repository.UpdateAsync(picture);

        // Assert
        var updatedPicture = await context.Pictures.FindAsync(picture.Id);
        Assert.NotNull(updatedPicture);
        Assert.Equal("Updated", updatedPicture.Name);
    }

    [Fact]
    public async Task DeleteAsync_WithValidId_ShouldMarkAsInactive()
    {
        // Arrange
        var options = CreateNewContextOptions();
        using var context = new ApplicationDbContext(options);
        await context.Database.EnsureCreatedAsync();

        var picture = new Picture { Name = "ToDelete", ImagePath = "/delete.jpg", IsActive = true, GalleryTypeId = 1, CreatedBy = "test" };
        context.Pictures.Add(picture);
        await context.SaveChangesAsync();

        var repository = new PictureRepository(context, _mockLogger.Object);

        // Act
        await repository.DeleteAsync(picture.Id);

        // Assert
        var deletedPicture = await context.Pictures.FindAsync(picture.Id);
        Assert.NotNull(deletedPicture);
        Assert.False(deletedPicture.IsActive);
    }

    [Fact]
    public async Task DeleteAsync_WithInvalidId_ShouldNotThrow()
    {
        // Arrange
        var options = CreateNewContextOptions();
        using var context = new ApplicationDbContext(options);
        await context.Database.EnsureCreatedAsync();

        var repository = new PictureRepository(context, _mockLogger.Object);

        // Act & Assert
        await repository.DeleteAsync(9999);
    }

    [Fact]
    public async Task ExistsAsync_WithExistingActivePicture_ShouldReturnTrue()
    {
        // Arrange
        var options = CreateNewContextOptions();
        using var context = new ApplicationDbContext(options);
        await context.Database.EnsureCreatedAsync();

        var picture = new Picture { Name = "Exists", ImagePath = "/exists.jpg", IsActive = true, GalleryTypeId = 1, CreatedBy = "test" };
        context.Pictures.Add(picture);
        await context.SaveChangesAsync();

        var repository = new PictureRepository(context, _mockLogger.Object);

        // Act
        var result = await repository.ExistsAsync(picture.Id);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task ExistsAsync_WithNonExistentPicture_ShouldReturnFalse()
    {
        // Arrange
        var options = CreateNewContextOptions();
        using var context = new ApplicationDbContext(options);
        await context.Database.EnsureCreatedAsync();

        var repository = new PictureRepository(context, _mockLogger.Object);

        // Act
        var result = await repository.ExistsAsync(9999);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task ExistsAsync_WithInactivePicture_ShouldReturnFalse()
    {
        // Arrange
        var options = CreateNewContextOptions();
        using var context = new ApplicationDbContext(options);
        await context.Database.EnsureCreatedAsync();

        var picture = new Picture { Name = "Inactive", ImagePath = "/inactive.jpg", IsActive = false, GalleryTypeId = 1, CreatedBy = "test" };
        context.Pictures.Add(picture);
        await context.SaveChangesAsync();

        var repository = new PictureRepository(context, _mockLogger.Object);

        // Act
        var result = await repository.ExistsAsync(picture.Id);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task SearchAsync_WithMatchingTerm_ShouldReturnMatchingPictures()
    {
        // Arrange
        var options = CreateNewContextOptions();
        using var context = new ApplicationDbContext(options);
        await context.Database.EnsureCreatedAsync();

        var picture1 = new Picture { Name = "Sunset Beach", ImagePath = "/1.jpg", IsActive = true, GalleryTypeId = 1, CreatedBy = "test" };
        var picture2 = new Picture { Name = "Mountain View", ImagePath = "/2.jpg", Description = "Beautiful sunset", IsActive = true, GalleryTypeId = 1, CreatedBy = "test" };
        var picture3 = new Picture { Name = "Ocean Wave", ImagePath = "/3.jpg", IsActive = true, GalleryTypeId = 1, CreatedBy = "test" };
        context.Pictures.AddRange(picture1, picture2, picture3);
        await context.SaveChangesAsync();

        var repository = new PictureRepository(context, _mockLogger.Object);

        // Act
        var result = await repository.SearchAsync("sunset");

        // Assert
        Assert.True(result.Count() >= 1);
        Assert.Contains(result, p => p.Name == "Sunset Beach" || p.Name == "Mountain View");
    }

    [Fact]
    public async Task SearchAsync_WithNoMatches_ShouldReturnEmptyList()
    {
        // Arrange
        var options = CreateNewContextOptions();
        using var context = new ApplicationDbContext(options);
        await context.Database.EnsureCreatedAsync();

        var picture = new Picture { Name = "Test", ImagePath = "/test.jpg", IsActive = true, GalleryTypeId = 1, CreatedBy = "test" };
        context.Pictures.Add(picture);
        await context.SaveChangesAsync();

        var repository = new PictureRepository(context, _mockLogger.Object);

        // Act
        var result = await repository.SearchAsync("nonexistent");

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetByGalleryTypeAsync_WithValidGalleryTypeId_ShouldReturnPictures()
    {
        // Arrange
        var options = CreateNewContextOptions();
        using var context = new ApplicationDbContext(options);
        await context.Database.EnsureCreatedAsync();

        var picture1 = new Picture { Name = "Pic1", ImagePath = "/1.jpg", GalleryTypeId = 1, IsActive = true, CreatedBy = "test" };
        var picture2 = new Picture { Name = "Pic2", ImagePath = "/2.jpg", GalleryTypeId = 1, IsActive = true, CreatedBy = "test" };
        var picture3 = new Picture { Name = "Pic3", ImagePath = "/3.jpg", GalleryTypeId = 2, IsActive = true, CreatedBy = "test" };
        context.Pictures.AddRange(picture1, picture2, picture3);
        await context.SaveChangesAsync();

        var repository = new PictureRepository(context, _mockLogger.Object);

        // Act
        var result = await repository.GetByGalleryTypeAsync(1);

        // Assert
        Assert.Equal(2, result.Count());
        Assert.All(result, p => Assert.Equal(1, p.GalleryTypeId));
    }

    [Fact]
    public async Task GetByGalleryTypeAsync_WithNonExistentGalleryTypeId_ShouldReturnEmptyList()
    {
        // Arrange
        var options = CreateNewContextOptions();
        using var context = new ApplicationDbContext(options);
        await context.Database.EnsureCreatedAsync();

        var repository = new PictureRepository(context, _mockLogger.Object);

        // Act
        var result = await repository.GetByGalleryTypeAsync(9999);

        // Assert
        Assert.Empty(result);
    }
}
