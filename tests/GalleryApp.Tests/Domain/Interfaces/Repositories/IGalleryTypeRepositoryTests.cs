using Xunit;
using Moq;
using GalleryApp.Domain.Entities;
using GalleryApp.Domain.Interfaces.Repositories;

namespace GalleryApp.Tests.Domain.Interfaces.Repositories;

public class IGalleryTypeRepositoryTests
{
    private readonly Mock<IGalleryTypeRepository> _mockRepository;

    public IGalleryTypeRepositoryTests()
    {
        _mockRepository = new Mock<IGalleryTypeRepository>();
    }

    [Fact]
    public async Task GetAllAsync_ReturnsAllGalleryTypes()
    {
        // Arrange
        var expectedGalleryTypes = new List<GalleryType>
        {
            new GalleryType { Id = 1, Name = "Nature" },
            new GalleryType { Id = 2, Name = "Wildlife" }
        };
        _mockRepository.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedGalleryTypes);

        // Act
        var result = await _mockRepository.Object.GetAllAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
        _mockRepository.Verify(r => r.GetAllAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetAllAsync_WithCancellationToken_PassesTokenCorrectly()
    {
        // Arrange
        var cancellationToken = new CancellationToken();
        var expectedGalleryTypes = new List<GalleryType>();
        _mockRepository.Setup(r => r.GetAllAsync(cancellationToken))
            .ReturnsAsync(expectedGalleryTypes);

        // Act
        var result = await _mockRepository.Object.GetAllAsync(cancellationToken);

        // Assert
        Assert.NotNull(result);
        _mockRepository.Verify(r => r.GetAllAsync(cancellationToken), Times.Once);
    }

    [Fact]
    public async Task GetByIdAsync_WithValidId_ReturnsGalleryType()
    {
        // Arrange
        var expectedGalleryType = new GalleryType { Id = 1, Name = "Test Gallery" };
        _mockRepository.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedGalleryType);

        // Act
        var result = await _mockRepository.Object.GetByIdAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("Test Gallery", result.Name);
        _mockRepository.Verify(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetByIdAsync_WithInvalidId_ReturnsNull()
    {
        // Arrange
        _mockRepository.Setup(r => r.GetByIdAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync((GalleryType?)null);

        // Act
        var result = await _mockRepository.Object.GetByIdAsync(999);

        // Assert
        Assert.Null(result);
        _mockRepository.Verify(r => r.GetByIdAsync(999, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task AddAsync_AddsGalleryTypeAndReturnsIt()
    {
        // Arrange
        var newGalleryType = new GalleryType { Name = "New Gallery" };
        var addedGalleryType = new GalleryType { Id = 10, Name = "New Gallery" };
        _mockRepository.Setup(r => r.AddAsync(newGalleryType, It.IsAny<CancellationToken>()))
            .ReturnsAsync(addedGalleryType);

        // Act
        var result = await _mockRepository.Object.AddAsync(newGalleryType);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(10, result.Id);
        Assert.Equal("New Gallery", result.Name);
        _mockRepository.Verify(r => r.AddAsync(newGalleryType, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_UpdatesExistingGalleryType()
    {
        // Arrange
        var galleryTypeToUpdate = new GalleryType { Id = 1, Name = "Updated Gallery" };
        _mockRepository.Setup(r => r.UpdateAsync(galleryTypeToUpdate, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        await _mockRepository.Object.UpdateAsync(galleryTypeToUpdate);

        // Assert
        _mockRepository.Verify(r => r.UpdateAsync(galleryTypeToUpdate, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_DeletesGalleryTypeById()
    {
        // Arrange
        var galleryTypeId = 42;
        _mockRepository.Setup(r => r.DeleteAsync(galleryTypeId, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        await _mockRepository.Object.DeleteAsync(galleryTypeId);

        // Assert
        _mockRepository.Verify(r => r.DeleteAsync(galleryTypeId, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ExistsAsync_WithExistingId_ReturnsTrue()
    {
        // Arrange
        _mockRepository.Setup(r => r.ExistsAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act
        var result = await _mockRepository.Object.ExistsAsync(1);

        // Assert
        Assert.True(result);
        _mockRepository.Verify(r => r.ExistsAsync(1, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ExistsAsync_WithNonExistingId_ReturnsFalse()
    {
        // Arrange
        _mockRepository.Setup(r => r.ExistsAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        // Act
        var result = await _mockRepository.Object.ExistsAsync(999);

        // Assert
        Assert.False(result);
        _mockRepository.Verify(r => r.ExistsAsync(999, It.IsAny<CancellationToken>()), Times.Once);
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
        _mockRepository.Setup(r => r.SearchAsync(searchTerm, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedGalleryTypes);

        // Act
        var result = await _mockRepository.Object.SearchAsync(searchTerm);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
        _mockRepository.Verify(r => r.SearchAsync(searchTerm, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task SearchAsync_WithEmptySearchTerm_ReturnsAllOrNoGalleryTypes()
    {
        // Arrange
        var searchTerm = "";
        var expectedGalleryTypes = new List<GalleryType>();
        _mockRepository.Setup(r => r.SearchAsync(searchTerm, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedGalleryTypes);

        // Act
        var result = await _mockRepository.Object.SearchAsync(searchTerm);

        // Assert
        Assert.NotNull(result);
        _mockRepository.Verify(r => r.SearchAsync(searchTerm, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task AddAsync_WithNullGalleryType_ThrowsException()
    {
        // Arrange
        _mockRepository.Setup(r => r.AddAsync(null!, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new ArgumentNullException(nameof(GalleryType)));

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() =>
            _mockRepository.Object.AddAsync(null!, CancellationToken.None));
    }

    [Fact]
    public async Task UpdateAsync_WithNullGalleryType_ThrowsException()
    {
        // Arrange
        _mockRepository.Setup(r => r.UpdateAsync(null!, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new ArgumentNullException(nameof(GalleryType)));

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() =>
            _mockRepository.Object.UpdateAsync(null!, CancellationToken.None));
    }
}
