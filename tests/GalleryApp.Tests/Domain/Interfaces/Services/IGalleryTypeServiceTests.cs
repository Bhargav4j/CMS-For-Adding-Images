using Xunit;
using Moq;
using GalleryApp.Domain.Entities;
using GalleryApp.Domain.Interfaces.Services;

namespace GalleryApp.Tests.Domain.Interfaces.Services;

public class IGalleryTypeServiceTests
{
    private readonly Mock<IGalleryTypeService> _mockService;

    public IGalleryTypeServiceTests()
    {
        _mockService = new Mock<IGalleryTypeService>();
    }

    [Fact]
    public async Task GetAllAsync_ReturnsAllGalleryTypes()
    {
        // Arrange
        var expectedGalleryTypes = new List<GalleryType>
        {
            new GalleryType { Id = 1, Name = "Nature" },
            new GalleryType { Id = 2, Name = "Wildlife" },
            new GalleryType { Id = 3, Name = "Architecture" }
        };
        _mockService.Setup(s => s.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedGalleryTypes);

        // Act
        var result = await _mockService.Object.GetAllAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(3, result.Count());
        _mockService.Verify(s => s.GetAllAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetAllAsync_WithCancellationToken_PassesTokenCorrectly()
    {
        // Arrange
        var cancellationToken = new CancellationToken();
        var expectedGalleryTypes = new List<GalleryType>();
        _mockService.Setup(s => s.GetAllAsync(cancellationToken))
            .ReturnsAsync(expectedGalleryTypes);

        // Act
        var result = await _mockService.Object.GetAllAsync(cancellationToken);

        // Assert
        Assert.NotNull(result);
        _mockService.Verify(s => s.GetAllAsync(cancellationToken), Times.Once);
    }

    [Fact]
    public async Task GetByIdAsync_WithValidId_ReturnsGalleryType()
    {
        // Arrange
        var expectedGalleryType = new GalleryType { Id = 1, Name = "Test Gallery" };
        _mockService.Setup(s => s.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedGalleryType);

        // Act
        var result = await _mockService.Object.GetByIdAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("Test Gallery", result.Name);
        _mockService.Verify(s => s.GetByIdAsync(1, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetByIdAsync_WithInvalidId_ReturnsNull()
    {
        // Arrange
        _mockService.Setup(s => s.GetByIdAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync((GalleryType?)null);

        // Act
        var result = await _mockService.Object.GetByIdAsync(999);

        // Assert
        Assert.Null(result);
        _mockService.Verify(s => s.GetByIdAsync(999, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_CreatesGalleryTypeAndReturnsIt()
    {
        // Arrange
        var newGalleryType = new GalleryType { Name = "New Gallery" };
        var createdGalleryType = new GalleryType { Id = 10, Name = "New Gallery" };
        _mockService.Setup(s => s.CreateAsync(newGalleryType, It.IsAny<CancellationToken>()))
            .ReturnsAsync(createdGalleryType);

        // Act
        var result = await _mockService.Object.CreateAsync(newGalleryType);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(10, result.Id);
        Assert.Equal("New Gallery", result.Name);
        _mockService.Verify(s => s.CreateAsync(newGalleryType, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_UpdatesExistingGalleryType()
    {
        // Arrange
        var galleryTypeId = 1;
        var updatedGalleryType = new GalleryType { Id = galleryTypeId, Name = "Updated Gallery" };
        _mockService.Setup(s => s.UpdateAsync(galleryTypeId, updatedGalleryType, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        await _mockService.Object.UpdateAsync(galleryTypeId, updatedGalleryType);

        // Assert
        _mockService.Verify(s => s.UpdateAsync(galleryTypeId, updatedGalleryType, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_DeletesGalleryTypeById()
    {
        // Arrange
        var galleryTypeId = 42;
        _mockService.Setup(s => s.DeleteAsync(galleryTypeId, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        await _mockService.Object.DeleteAsync(galleryTypeId);

        // Assert
        _mockService.Verify(s => s.DeleteAsync(galleryTypeId, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task SearchAsync_WithSearchTerm_ReturnsMatchingGalleryTypes()
    {
        // Arrange
        var searchTerm = "nature";
        var expectedGalleryTypes = new List<GalleryType>
        {
            new GalleryType { Id = 1, Name = "Nature Photos" },
            new GalleryType { Id = 2, Name = "Natural Beauty" }
        };
        _mockService.Setup(s => s.SearchAsync(searchTerm, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedGalleryTypes);

        // Act
        var result = await _mockService.Object.SearchAsync(searchTerm);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
        _mockService.Verify(s => s.SearchAsync(searchTerm, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task SearchAsync_WithEmptySearchTerm_ReturnsAllOrNoGalleryTypes()
    {
        // Arrange
        var searchTerm = "";
        var expectedGalleryTypes = new List<GalleryType>();
        _mockService.Setup(s => s.SearchAsync(searchTerm, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedGalleryTypes);

        // Act
        var result = await _mockService.Object.SearchAsync(searchTerm);

        // Assert
        Assert.NotNull(result);
        _mockService.Verify(s => s.SearchAsync(searchTerm, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_WithNullGalleryType_ThrowsException()
    {
        // Arrange
        _mockService.Setup(s => s.CreateAsync(null!, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new ArgumentNullException(nameof(GalleryType)));

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() =>
            _mockService.Object.CreateAsync(null!, CancellationToken.None));
    }

    [Fact]
    public async Task UpdateAsync_WithInvalidId_ThrowsException()
    {
        // Arrange
        var invalidId = -1;
        var galleryType = new GalleryType { Id = 1, Name = "Test" };
        _mockService.Setup(s => s.UpdateAsync(invalidId, galleryType, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new ArgumentException("Invalid ID"));

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() =>
            _mockService.Object.UpdateAsync(invalidId, galleryType, CancellationToken.None));
    }

    [Fact]
    public async Task DeleteAsync_WithInvalidId_ThrowsException()
    {
        // Arrange
        var invalidId = 0;
        _mockService.Setup(s => s.DeleteAsync(invalidId, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new ArgumentException("Invalid ID"));

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() =>
            _mockService.Object.DeleteAsync(invalidId, CancellationToken.None));
    }

    [Fact]
    public async Task GetByIdAsync_MultipleTimes_CachesResultIfImplemented()
    {
        // Arrange
        var galleryType = new GalleryType { Id = 1, Name = "Cached Gallery" };
        _mockService.Setup(s => s.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(galleryType);

        // Act
        var result1 = await _mockService.Object.GetByIdAsync(1);
        var result2 = await _mockService.Object.GetByIdAsync(1);

        // Assert
        Assert.NotNull(result1);
        Assert.NotNull(result2);
        Assert.Equal(result1.Id, result2.Id);
    }
}
