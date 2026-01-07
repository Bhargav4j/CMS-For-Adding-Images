using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using WebGallery.Domain.Entities;
using WebGallery.Infrastructure.Data;
using WebGallery.Infrastructure.Repositories;

namespace Tests.WebGallery.Infrastructure;

public class GalleryTypeRepositoryTests
{
    private readonly Mock<ILogger<GalleryTypeRepository>> _mockLogger;

    public GalleryTypeRepositoryTests()
    {
        _mockLogger = new Mock<ILogger<GalleryTypeRepository>>();
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
        Assert.Throws<ArgumentNullException>(() => new GalleryTypeRepository(null!, _mockLogger.Object));
    }

    [Fact]
    public void Constructor_WithNullLogger_ThrowsArgumentNullException()
    {
        // Arrange
        var options = CreateInMemoryOptions();
        var context = new ApplicationDbContext(options);

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new GalleryTypeRepository(context, null!));
    }

    [Fact]
    public async Task GetAllAsync_ReturnsOnlyActiveGalleryTypes()
    {
        // Arrange
        var options = CreateInMemoryOptions();
        using var context = new ApplicationDbContext(options);
        var repository = new GalleryTypeRepository(context, _mockLogger.Object);

        var activeGallery = new GalleryType { Name = "Active Gallery", CreatedDate = DateTime.UtcNow, CreatedBy = "user", IsActive = true };
        var inactiveGallery = new GalleryType { Name = "Inactive Gallery", CreatedDate = DateTime.UtcNow, CreatedBy = "user", IsActive = false };

        context.GalleryTypes.AddRange(activeGallery, inactiveGallery);
        await context.SaveChangesAsync();

        // Act
        var result = await repository.GetAllAsync();

        // Assert
        var resultList = result.ToList();
        Assert.Contains(resultList, g => g.Name == "Active Gallery");
        Assert.DoesNotContain(resultList, g => g.Name == "Inactive Gallery");
    }

    [Fact]
    public async Task GetAllAsync_OrdersByName()
    {
        // Arrange
        var options = CreateInMemoryOptions();
        using var context = new ApplicationDbContext(options);
        var repository = new GalleryTypeRepository(context, _mockLogger.Object);

        var galleryZ = new GalleryType { Name = "Z Gallery", CreatedDate = DateTime.UtcNow, CreatedBy = "user", IsActive = true };
        var galleryA = new GalleryType { Name = "A Gallery", CreatedDate = DateTime.UtcNow, CreatedBy = "user", IsActive = true };

        context.GalleryTypes.AddRange(galleryZ, galleryA);
        await context.SaveChangesAsync();

        // Act
        var result = await repository.GetAllAsync();

        // Assert
        var resultList = result.ToList();
        var customGalleries = resultList.Where(g => g.Name == "A Gallery" || g.Name == "Z Gallery").ToList();
        if (customGalleries.Count == 2)
        {
            Assert.True(customGalleries[0].Name.CompareTo(customGalleries[1].Name) < 0);
        }
    }

    [Fact]
    public async Task GetByIdAsync_WithValidId_ReturnsGalleryType()
    {
        // Arrange
        var options = CreateInMemoryOptions();
        using var context = new ApplicationDbContext(options);
        var repository = new GalleryTypeRepository(context, _mockLogger.Object);

        var gallery = new GalleryType { Name = "Test Gallery", CreatedDate = DateTime.UtcNow, CreatedBy = "user", IsActive = true };
        context.GalleryTypes.Add(gallery);
        await context.SaveChangesAsync();

        // Act
        var result = await repository.GetByIdAsync(gallery.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Test Gallery", result.Name);
    }

    [Fact]
    public async Task GetByIdAsync_WithInvalidId_ReturnsNull()
    {
        // Arrange
        var options = CreateInMemoryOptions();
        using var context = new ApplicationDbContext(options);
        var repository = new GalleryTypeRepository(context, _mockLogger.Object);

        // Act
        var result = await repository.GetByIdAsync(999);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task AddAsync_CreatesGalleryTypeWithCorrectDefaults()
    {
        // Arrange
        var options = CreateInMemoryOptions();
        using var context = new ApplicationDbContext(options);
        var repository = new GalleryTypeRepository(context, _mockLogger.Object);

        var gallery = new GalleryType { Name = "New Gallery", CreatedBy = "user" };

        // Act
        var result = await repository.AddAsync(gallery);

        // Assert
        Assert.True(result.IsActive);
        Assert.NotEqual(default(DateTime), result.CreatedDate);
    }

    [Fact]
    public async Task UpdateAsync_UpdatesGalleryTypeAndSetsModifiedDate()
    {
        // Arrange
        var options = CreateInMemoryOptions();
        using var context = new ApplicationDbContext(options);
        var repository = new GalleryTypeRepository(context, _mockLogger.Object);

        var gallery = new GalleryType { Name = "Original", CreatedDate = DateTime.UtcNow, CreatedBy = "user", IsActive = true };
        context.GalleryTypes.Add(gallery);
        await context.SaveChangesAsync();

        // Act
        gallery.Name = "Updated";
        await repository.UpdateAsync(gallery);

        // Assert
        var updatedGallery = await context.GalleryTypes.FindAsync(gallery.Id);
        Assert.NotNull(updatedGallery);
        Assert.Equal("Updated", updatedGallery.Name);
        Assert.NotNull(updatedGallery.ModifiedDate);
    }

    [Fact]
    public async Task DeleteAsync_SoftDeletesGalleryType()
    {
        // Arrange
        var options = CreateInMemoryOptions();
        using var context = new ApplicationDbContext(options);
        var repository = new GalleryTypeRepository(context, _mockLogger.Object);

        var gallery = new GalleryType { Name = "To Delete", CreatedDate = DateTime.UtcNow, CreatedBy = "user", IsActive = true };
        context.GalleryTypes.Add(gallery);
        await context.SaveChangesAsync();

        // Act
        await repository.DeleteAsync(gallery.Id);

        // Assert
        var deletedGallery = await context.GalleryTypes.FindAsync(gallery.Id);
        Assert.NotNull(deletedGallery);
        Assert.False(deletedGallery.IsActive);
        Assert.NotNull(deletedGallery.ModifiedDate);
    }

    [Fact]
    public async Task DeleteAsync_WithNonExistentId_DoesNotThrow()
    {
        // Arrange
        var options = CreateInMemoryOptions();
        using var context = new ApplicationDbContext(options);
        var repository = new GalleryTypeRepository(context, _mockLogger.Object);

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
        var repository = new GalleryTypeRepository(context, _mockLogger.Object);

        var gallery = new GalleryType { Name = "Test", CreatedDate = DateTime.UtcNow, CreatedBy = "user", IsActive = true };
        context.GalleryTypes.Add(gallery);
        await context.SaveChangesAsync();

        // Act
        var exists = await repository.ExistsAsync(gallery.Id);

        // Assert
        Assert.True(exists);
    }

    [Fact]
    public async Task ExistsAsync_WithNonExistentId_ReturnsFalse()
    {
        // Arrange
        var options = CreateInMemoryOptions();
        using var context = new ApplicationDbContext(options);
        var repository = new GalleryTypeRepository(context, _mockLogger.Object);

        // Act
        var exists = await repository.ExistsAsync(999);

        // Assert
        Assert.False(exists);
    }

    [Fact]
    public async Task SearchAsync_FindsMatchingGalleryTypesByName()
    {
        // Arrange
        var options = CreateInMemoryOptions();
        using var context = new ApplicationDbContext(options);
        var repository = new GalleryTypeRepository(context, _mockLogger.Object);

        var gallery1 = new GalleryType { Name = "Cake Gallery", Description = "For cakes", CreatedDate = DateTime.UtcNow, CreatedBy = "user", IsActive = true };
        var gallery2 = new GalleryType { Name = "Book Gallery", Description = "For books", CreatedDate = DateTime.UtcNow, CreatedBy = "user", IsActive = true };

        context.GalleryTypes.AddRange(gallery1, gallery2);
        await context.SaveChangesAsync();

        // Act
        var result = await repository.SearchAsync("Cake");

        // Assert
        Assert.Single(result);
        Assert.Equal("Cake Gallery", result.First().Name);
    }

    [Fact]
    public async Task SearchAsync_FindsMatchingGalleryTypesByDescription()
    {
        // Arrange
        var options = CreateInMemoryOptions();
        using var context = new ApplicationDbContext(options);
        var repository = new GalleryTypeRepository(context, _mockLogger.Object);

        var gallery1 = new GalleryType { Name = "Gallery 1", Description = "Chocolate treats", CreatedDate = DateTime.UtcNow, CreatedBy = "user", IsActive = true };
        var gallery2 = new GalleryType { Name = "Gallery 2", Description = "Vanilla items", CreatedDate = DateTime.UtcNow, CreatedBy = "user", IsActive = true };

        context.GalleryTypes.AddRange(gallery1, gallery2);
        await context.SaveChangesAsync();

        // Act
        var result = await repository.SearchAsync("Chocolate");

        // Assert
        Assert.Single(result);
        Assert.Equal("Gallery 1", result.First().Name);
    }

    [Fact]
    public async Task SearchAsync_WithNullDescription_DoesNotThrow()
    {
        // Arrange
        var options = CreateInMemoryOptions();
        using var context = new ApplicationDbContext(options);
        var repository = new GalleryTypeRepository(context, _mockLogger.Object);

        var gallery = new GalleryType { Name = "Test Gallery", Description = null, CreatedDate = DateTime.UtcNow, CreatedBy = "user", IsActive = true };
        context.GalleryTypes.Add(gallery);
        await context.SaveChangesAsync();

        // Act
        var result = await repository.SearchAsync("Test");

        // Assert
        Assert.Single(result);
    }
}
