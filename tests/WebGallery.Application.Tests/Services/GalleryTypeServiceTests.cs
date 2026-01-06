using Xunit;
using Moq;
using Microsoft.Extensions.Logging;
using WebGallery.Application.Services;
using WebGallery.Domain.Entities;
using WebGallery.Domain.Interfaces.Repositories;

namespace WebGallery.Application.Tests.Services;

public class GalleryTypeServiceTests
{
    private readonly Mock<IGalleryTypeRepository> _mockRepository;
    private readonly Mock<ILogger<GalleryTypeService>> _mockLogger;
    private readonly GalleryTypeService _service;

    public GalleryTypeServiceTests()
    {
        _mockRepository = new Mock<IGalleryTypeRepository>();
        _mockLogger = new Mock<ILogger<GalleryTypeService>>();
        _service = new GalleryTypeService(_mockRepository.Object, _mockLogger.Object);
    }

    [Fact]
    public void Constructor_WithNullRepository_ShouldThrowArgumentNullException()
    {
        // Arrange & Act & Assert
        Assert.Throws<ArgumentNullException>(() =>
            new GalleryTypeService(null!, _mockLogger.Object));
    }

    [Fact]
    public void Constructor_WithNullLogger_ShouldThrowArgumentNullException()
    {
        // Arrange & Act & Assert
        Assert.Throws<ArgumentNullException>(() =>
            new GalleryTypeService(_mockRepository.Object, null!));
    }

    [Fact]
    public void Constructor_WithValidParameters_ShouldCreateInstance()
    {
        // Arrange & Act
        var service = new GalleryTypeService(_mockRepository.Object, _mockLogger.Object);

        // Assert
        Assert.NotNull(service);
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllGalleryTypes()
    {
        // Arrange
        var galleryTypes = new List<GalleryType>
        {
            new GalleryType { Id = 1, Name = "Nature" },
            new GalleryType { Id = 2, Name = "Abstract" }
        };
        _mockRepository.Setup(r => r.GetAllAsync(default)).ReturnsAsync(galleryTypes);

        // Act
        var result = await _service.GetAllAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
        _mockRepository.Verify(r => r.GetAllAsync(default), Times.Once);
    }

    [Fact]
    public async Task GetAllAsync_WithCancellationToken_ShouldPassTokenToRepository()
    {
        // Arrange
        var cancellationToken = new CancellationToken();
        var galleryTypes = new List<GalleryType>();
        _mockRepository.Setup(r => r.GetAllAsync(cancellationToken)).ReturnsAsync(galleryTypes);

        // Act
        await _service.GetAllAsync(cancellationToken);

        // Assert
        _mockRepository.Verify(r => r.GetAllAsync(cancellationToken), Times.Once);
    }

    [Fact]
    public async Task GetAllAsync_WhenRepositoryThrowsException_ShouldThrowException()
    {
        // Arrange
        _mockRepository.Setup(r => r.GetAllAsync(default))
            .ThrowsAsync(new Exception("Database error"));

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _service.GetAllAsync());
    }

    [Fact]
    public async Task GetByIdAsync_WithValidId_ShouldReturnGalleryType()
    {
        // Arrange
        var galleryType = new GalleryType { Id = 1, Name = "Wildlife" };
        _mockRepository.Setup(r => r.GetByIdAsync(1, default)).ReturnsAsync(galleryType);

        // Act
        var result = await _service.GetByIdAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("Wildlife", result.Name);
        _mockRepository.Verify(r => r.GetByIdAsync(1, default), Times.Once);
    }

    [Fact]
    public async Task GetByIdAsync_WithInvalidId_ShouldReturnNull()
    {
        // Arrange
        _mockRepository.Setup(r => r.GetByIdAsync(999, default)).ReturnsAsync((GalleryType?)null);

        // Act
        var result = await _service.GetByIdAsync(999);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetByIdAsync_WhenRepositoryThrowsException_ShouldThrowException()
    {
        // Arrange
        _mockRepository.Setup(r => r.GetByIdAsync(It.IsAny<int>(), default))
            .ThrowsAsync(new Exception("Database error"));

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _service.GetByIdAsync(1));
    }

    [Fact]
    public async Task CreateAsync_WithValidGalleryType_ShouldCreateAndReturnGalleryType()
    {
        // Arrange
        var galleryType = new GalleryType { Name = "Sports", Description = "Sports photos" };
        _mockRepository.Setup(r => r.AddAsync(It.IsAny<GalleryType>(), default))
            .ReturnsAsync((GalleryType g, CancellationToken ct) => { g.Id = 1; return g; });

        // Act
        var result = await _service.CreateAsync(galleryType);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.IsActive);
        Assert.True(result.CreatedDate <= DateTime.UtcNow);
        _mockRepository.Verify(r => r.AddAsync(It.IsAny<GalleryType>(), default), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_ShouldSetCreatedDateAndIsActive()
    {
        // Arrange
        var galleryType = new GalleryType { Name = "Test", IsActive = false };
        var beforeCreate = DateTime.UtcNow;
        _mockRepository.Setup(r => r.AddAsync(It.IsAny<GalleryType>(), default))
            .ReturnsAsync((GalleryType g, CancellationToken ct) => g);

        // Act
        var result = await _service.CreateAsync(galleryType);

        // Assert
        Assert.True(result.IsActive);
        Assert.True(result.CreatedDate >= beforeCreate && result.CreatedDate <= DateTime.UtcNow);
    }

    [Fact]
    public async Task CreateAsync_WhenRepositoryThrowsException_ShouldThrowException()
    {
        // Arrange
        var galleryType = new GalleryType { Name = "Test" };
        _mockRepository.Setup(r => r.AddAsync(It.IsAny<GalleryType>(), default))
            .ThrowsAsync(new Exception("Database error"));

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _service.CreateAsync(galleryType));
    }

    [Fact]
    public async Task UpdateAsync_WithValidGalleryType_ShouldUpdateExistingGalleryType()
    {
        // Arrange
        var existingGalleryType = new GalleryType { Id = 1, Name = "Old Name", Description = "Old desc" };
        var updatedGalleryType = new GalleryType { Name = "New Name", Description = "New desc" };
        _mockRepository.Setup(r => r.GetByIdAsync(1, default)).ReturnsAsync(existingGalleryType);
        _mockRepository.Setup(r => r.UpdateAsync(It.IsAny<GalleryType>(), default)).Returns(Task.CompletedTask);

        // Act
        await _service.UpdateAsync(1, updatedGalleryType);

        // Assert
        Assert.Equal("New Name", existingGalleryType.Name);
        Assert.Equal("New desc", existingGalleryType.Description);
        Assert.NotNull(existingGalleryType.ModifiedDate);
        _mockRepository.Verify(r => r.UpdateAsync(existingGalleryType, default), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_WithNonExistentId_ShouldThrowInvalidOperationException()
    {
        // Arrange
        _mockRepository.Setup(r => r.GetByIdAsync(999, default)).ReturnsAsync((GalleryType?)null);
        var updatedGalleryType = new GalleryType { Name = "Test" };

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            _service.UpdateAsync(999, updatedGalleryType));
    }

    [Fact]
    public async Task UpdateAsync_ShouldSetModifiedDate()
    {
        // Arrange
        var existingGalleryType = new GalleryType { Id = 1, Name = "Old" };
        var updatedGalleryType = new GalleryType { Name = "New" };
        var beforeUpdate = DateTime.UtcNow;
        _mockRepository.Setup(r => r.GetByIdAsync(1, default)).ReturnsAsync(existingGalleryType);
        _mockRepository.Setup(r => r.UpdateAsync(It.IsAny<GalleryType>(), default)).Returns(Task.CompletedTask);

        // Act
        await _service.UpdateAsync(1, updatedGalleryType);

        // Assert
        Assert.NotNull(existingGalleryType.ModifiedDate);
        Assert.True(existingGalleryType.ModifiedDate >= beforeUpdate && existingGalleryType.ModifiedDate <= DateTime.UtcNow);
    }

    [Fact]
    public async Task UpdateAsync_WhenRepositoryThrowsException_ShouldThrowException()
    {
        // Arrange
        _mockRepository.Setup(r => r.GetByIdAsync(It.IsAny<int>(), default))
            .ThrowsAsync(new Exception("Database error"));
        var updatedGalleryType = new GalleryType { Name = "Test" };

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _service.UpdateAsync(1, updatedGalleryType));
    }

    [Fact]
    public async Task DeleteAsync_WithValidId_ShouldCallRepositoryDelete()
    {
        // Arrange
        _mockRepository.Setup(r => r.DeleteAsync(1, default)).Returns(Task.CompletedTask);

        // Act
        await _service.DeleteAsync(1);

        // Assert
        _mockRepository.Verify(r => r.DeleteAsync(1, default), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_WithCancellationToken_ShouldPassTokenToRepository()
    {
        // Arrange
        var cancellationToken = new CancellationToken();
        _mockRepository.Setup(r => r.DeleteAsync(1, cancellationToken)).Returns(Task.CompletedTask);

        // Act
        await _service.DeleteAsync(1, cancellationToken);

        // Assert
        _mockRepository.Verify(r => r.DeleteAsync(1, cancellationToken), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_WhenRepositoryThrowsException_ShouldThrowException()
    {
        // Arrange
        _mockRepository.Setup(r => r.DeleteAsync(It.IsAny<int>(), default))
            .ThrowsAsync(new Exception("Database error"));

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _service.DeleteAsync(1));
    }

    [Fact]
    public async Task SearchAsync_WithValidSearchTerm_ShouldReturnMatchingGalleryTypes()
    {
        // Arrange
        var galleryTypes = new List<GalleryType>
        {
            new GalleryType { Id = 1, Name = "Nature Photography" },
            new GalleryType { Id = 2, Name = "Natural Landscapes" }
        };
        _mockRepository.Setup(r => r.SearchAsync("nature", default)).ReturnsAsync(galleryTypes);

        // Act
        var result = await _service.SearchAsync("nature");

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
        _mockRepository.Verify(r => r.SearchAsync("nature", default), Times.Once);
    }

    [Fact]
    public async Task SearchAsync_WithEmptySearchTerm_ShouldCallRepository()
    {
        // Arrange
        _mockRepository.Setup(r => r.SearchAsync("", default)).ReturnsAsync(new List<GalleryType>());

        // Act
        await _service.SearchAsync("");

        // Assert
        _mockRepository.Verify(r => r.SearchAsync("", default), Times.Once);
    }

    [Fact]
    public async Task SearchAsync_WhenRepositoryThrowsException_ShouldThrowException()
    {
        // Arrange
        _mockRepository.Setup(r => r.SearchAsync(It.IsAny<string>(), default))
            .ThrowsAsync(new Exception("Database error"));

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _service.SearchAsync("test"));
    }
}
