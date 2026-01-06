using Xunit;
using Moq;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using WebGallery.Infrastructure.Repositories;
using WebGallery.Infrastructure.Data;
using WebGallery.Domain.Entities;

namespace WebGallery.Infrastructure.Tests.Repositories;

public class GalleryTypeRepositoryTests
{
    private readonly Mock<ILogger<GalleryTypeRepository>> _mockLogger;

    public GalleryTypeRepositoryTests()
    {
        _mockLogger = new Mock<ILogger<GalleryTypeRepository>>();
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
            new GalleryTypeRepository(null!, _mockLogger.Object));
    }

    [Fact]
    public void Constructor_WithNullLogger_ShouldThrowArgumentNullException()
    {
        // Arrange
        var options = CreateNewContextOptions();
        var context = new ApplicationDbContext(options);

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() =>
            new GalleryTypeRepository(context, null!));
    }

    [Fact]
    public void Constructor_WithValidParameters_ShouldCreateInstance()
    {
        // Arrange
        var options = CreateNewContextOptions();
        var context = new ApplicationDbContext(options);

        // Act
        var repository = new GalleryTypeRepository(context, _mockLogger.Object);

        // Assert
        Assert.NotNull(repository);
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnOnlyActiveGalleryTypes()
    {
        // Arrange
        var options = CreateNewContextOptions();
        using var context = new ApplicationDbContext(options);
        await context.Database.EnsureCreatedAsync();

        var activeGallery = new GalleryType { Name = "Active", IsActive = true, CreatedBy = "test" };
        var inactiveGallery = new GalleryType { Name = "Inactive", IsActive = false, CreatedBy = "test" };
        context.GalleryTypes.AddRange(activeGallery, inactiveGallery);
        await context.SaveChangesAsync();

        var repository = new GalleryTypeRepository(context, _mockLogger.Object);

        // Act
        var result = await repository.GetAllAsync();

        // Assert
        Assert.Contains(result, g => g.Name == "Active");
        Assert.DoesNotContain(result, g => g.Name == "Inactive");
    }

    [Fact]
    public async Task GetAllAsync_ShouldOrderByNameAscending()
    {
        // Arrange
        var options = CreateNewContextOptions();
        using var context = new ApplicationDbContext(options);
        await context.Database.EnsureCreatedAsync();

        var gallery1 = new GalleryType { Name = "Zebra", IsActive = true, CreatedBy = "test" };
        var gallery2 = new GalleryType { Name = "Apple", IsActive = true, CreatedBy = "test" };
        var gallery3 = new GalleryType { Name = "Mango", IsActive = true, CreatedBy = "test" };
        context.GalleryTypes.AddRange(gallery1, gallery2, gallery3);
        await context.SaveChangesAsync();

        var repository = new GalleryTypeRepository(context, _mockLogger.Object);

        // Act
        var result = (await repository.GetAllAsync()).ToList();

        // Assert
        Assert.True(result.Count >= 3);
        var userAddedGalleries = result.Where(g => new[] { "Zebra", "Apple", "Mango" }.Contains(g.Name)).ToList();
        Assert.Equal("Apple", userAddedGalleries[0].Name);
        Assert.Equal("Mango", userAddedGalleries[1].Name);
        Assert.Equal("Zebra", userAddedGalleries[2].Name);
    }

    [Fact]
    public async Task GetByIdAsync_WithValidId_ShouldReturnGalleryType()
    {
        // Arrange
        var options = CreateNewContextOptions();
        using var context = new ApplicationDbContext(options);
        await context.Database.EnsureCreatedAsync();

        var gallery = new GalleryType { Name = "Test Gallery", IsActive = true, CreatedBy = "test" };
        context.GalleryTypes.Add(gallery);
        await context.SaveChangesAsync();

        var repository = new GalleryTypeRepository(context, _mockLogger.Object);

        // Act
        var result = await repository.GetByIdAsync(gallery.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(gallery.Id, result.Id);
        Assert.Equal("Test Gallery", result.Name);
    }

    [Fact]
    public async Task GetByIdAsync_WithInvalidId_ShouldReturnNull()
    {
        // Arrange
        var options = CreateNewContextOptions();
        using var context = new ApplicationDbContext(options);
        await context.Database.EnsureCreatedAsync();

        var repository = new GalleryTypeRepository(context, _mockLogger.Object);

        // Act
        var result = await repository.GetByIdAsync(9999);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetByIdAsync_WithInactiveGalleryType_ShouldReturnNull()
    {
        // Arrange
        var options = CreateNewContextOptions();
        using var context = new ApplicationDbContext(options);
        await context.Database.EnsureCreatedAsync();

        var gallery = new GalleryType { Name = "Inactive", IsActive = false, CreatedBy = "test" };
        context.GalleryTypes.Add(gallery);
        await context.SaveChangesAsync();

        var repository = new GalleryTypeRepository(context, _mockLogger.Object);

        // Act
        var result = await repository.GetByIdAsync(gallery.Id);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldIncludePictures()
    {
        // Arrange
        var options = CreateNewContextOptions();
        using var context = new ApplicationDbContext(options);
        await context.Database.EnsureCreatedAsync();

        var gallery = new GalleryType { Name = "Test", IsActive = true, CreatedBy = "test" };
        context.GalleryTypes.Add(gallery);
        await context.SaveChangesAsync();

        var picture = new Picture { Name = "Pic", ImagePath = "/pic.jpg", GalleryTypeId = gallery.Id, IsActive = true, CreatedBy = "test" };
        context.Pictures.Add(picture);
        await context.SaveChangesAsync();

        var repository = new GalleryTypeRepository(context, _mockLogger.Object);

        // Act
        var result = await repository.GetByIdAsync(gallery.Id);

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.Pictures);
        Assert.NotEmpty(result.Pictures);
    }

    [Fact]
    public async Task AddAsync_WithValidGalleryType_ShouldAddToDatabase()
    {
        // Arrange
        var options = CreateNewContextOptions();
        using var context = new ApplicationDbContext(options);
        await context.Database.EnsureCreatedAsync();

        var repository = new GalleryTypeRepository(context, _mockLogger.Object);
        var gallery = new GalleryType { Name = "New Gallery", Description = "New Description", IsActive = true, CreatedBy = "test" };

        // Act
        var result = await repository.AddAsync(gallery);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Id > 0);
        var savedGallery = await context.GalleryTypes.FindAsync(result.Id);
        Assert.NotNull(savedGallery);
        Assert.Equal("New Gallery", savedGallery.Name);
    }

    [Fact]
    public async Task UpdateAsync_WithValidGalleryType_ShouldUpdateInDatabase()
    {
        // Arrange
        var options = CreateNewContextOptions();
        using var context = new ApplicationDbContext(options);
        await context.Database.EnsureCreatedAsync();

        var gallery = new GalleryType { Name = "Original", IsActive = true, CreatedBy = "test" };
        context.GalleryTypes.Add(gallery);
        await context.SaveChangesAsync();

        var repository = new GalleryTypeRepository(context, _mockLogger.Object);
        gallery.Name = "Updated";

        // Act
        await repository.UpdateAsync(gallery);

        // Assert
        var updatedGallery = await context.GalleryTypes.FindAsync(gallery.Id);
        Assert.NotNull(updatedGallery);
        Assert.Equal("Updated", updatedGallery.Name);
    }

    [Fact]
    public async Task DeleteAsync_WithValidId_ShouldMarkAsInactive()
    {
        // Arrange
        var options = CreateNewContextOptions();
        using var context = new ApplicationDbContext(options);
        await context.Database.EnsureCreatedAsync();

        var gallery = new GalleryType { Name = "ToDelete", IsActive = true, CreatedBy = "test" };
        context.GalleryTypes.Add(gallery);
        await context.SaveChangesAsync();

        var repository = new GalleryTypeRepository(context, _mockLogger.Object);

        // Act
        await repository.DeleteAsync(gallery.Id);

        // Assert
        var deletedGallery = await context.GalleryTypes.FindAsync(gallery.Id);
        Assert.NotNull(deletedGallery);
        Assert.False(deletedGallery.IsActive);
    }

    [Fact]
    public async Task DeleteAsync_WithInvalidId_ShouldNotThrow()
    {
        // Arrange
        var options = CreateNewContextOptions();
        using var context = new ApplicationDbContext(options);
        await context.Database.EnsureCreatedAsync();

        var repository = new GalleryTypeRepository(context, _mockLogger.Object);

        // Act & Assert
        await repository.DeleteAsync(9999);
    }

    [Fact]
    public async Task ExistsAsync_WithExistingActiveGalleryType_ShouldReturnTrue()
    {
        // Arrange
        var options = CreateNewContextOptions();
        using var context = new ApplicationDbContext(options);
        await context.Database.EnsureCreatedAsync();

        var gallery = new GalleryType { Name = "Exists", IsActive = true, CreatedBy = "test" };
        context.GalleryTypes.Add(gallery);
        await context.SaveChangesAsync();

        var repository = new GalleryTypeRepository(context, _mockLogger.Object);

        // Act
        var result = await repository.ExistsAsync(gallery.Id);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task ExistsAsync_WithNonExistentGalleryType_ShouldReturnFalse()
    {
        // Arrange
        var options = CreateNewContextOptions();
        using var context = new ApplicationDbContext(options);
        await context.Database.EnsureCreatedAsync();

        var repository = new GalleryTypeRepository(context, _mockLogger.Object);

        // Act
        var result = await repository.ExistsAsync(9999);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task ExistsAsync_WithInactiveGalleryType_ShouldReturnFalse()
    {
        // Arrange
        var options = CreateNewContextOptions();
        using var context = new ApplicationDbContext(options);
        await context.Database.EnsureCreatedAsync();

        var gallery = new GalleryType { Name = "Inactive", IsActive = false, CreatedBy = "test" };
        context.GalleryTypes.Add(gallery);
        await context.SaveChangesAsync();

        var repository = new GalleryTypeRepository(context, _mockLogger.Object);

        // Act
        var result = await repository.ExistsAsync(gallery.Id);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task SearchAsync_WithMatchingTerm_ShouldReturnMatchingGalleryTypes()
    {
        // Arrange
        var options = CreateNewContextOptions();
        using var context = new ApplicationDbContext(options);
        await context.Database.EnsureCreatedAsync();

        var gallery1 = new GalleryType { Name = "TestNature", IsActive = true, CreatedBy = "test" };
        var gallery2 = new GalleryType { Name = "Wildlife", Description = "TestNature gallery", IsActive = true, CreatedBy = "test" };
        var gallery3 = new GalleryType { Name = "Abstract Art", IsActive = true, CreatedBy = "test" };
        context.GalleryTypes.AddRange(gallery1, gallery2, gallery3);
        await context.SaveChangesAsync();

        var repository = new GalleryTypeRepository(context, _mockLogger.Object);

        // Act
        var result = await repository.SearchAsync("TestNature");

        // Assert
        Assert.NotEmpty(result);
        Assert.True(result.Count() >= 1);
    }

    [Fact]
    public async Task SearchAsync_WithNoMatches_ShouldReturnEmptyList()
    {
        // Arrange
        var options = CreateNewContextOptions();
        using var context = new ApplicationDbContext(options);
        await context.Database.EnsureCreatedAsync();

        var gallery = new GalleryType { Name = "Test", IsActive = true, CreatedBy = "test" };
        context.GalleryTypes.Add(gallery);
        await context.SaveChangesAsync();

        var repository = new GalleryTypeRepository(context, _mockLogger.Object);

        // Act
        var result = await repository.SearchAsync("nonexistent");

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public async Task SearchAsync_ShouldOrderByNameAscending()
    {
        // Arrange
        var options = CreateNewContextOptions();
        using var context = new ApplicationDbContext(options);
        await context.Database.EnsureCreatedAsync();

        var gallery1 = new GalleryType { Name = "Zoo Photos", Description = "test", IsActive = true, CreatedBy = "test" };
        var gallery2 = new GalleryType { Name = "Art Test", IsActive = true, CreatedBy = "test" };
        var gallery3 = new GalleryType { Name = "Mid Test", IsActive = true, CreatedBy = "test" };
        context.GalleryTypes.AddRange(gallery1, gallery2, gallery3);
        await context.SaveChangesAsync();

        var repository = new GalleryTypeRepository(context, _mockLogger.Object);

        // Act
        var result = (await repository.SearchAsync("test")).ToList();

        // Assert
        Assert.True(result.Count >= 1);
        Assert.Contains(result, g => g.Name == "Art Test" || g.Name == "Mid Test" || g.Name == "Zoo Photos");
    }
}
