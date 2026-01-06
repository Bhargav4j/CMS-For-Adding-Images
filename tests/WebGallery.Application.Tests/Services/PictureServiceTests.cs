using Xunit;
using Moq;
using Microsoft.Extensions.Logging;
using WebGallery.Application.Services;
using WebGallery.Domain.Entities;
using WebGallery.Domain.Interfaces.Repositories;

namespace WebGallery.Application.Tests.Services;

public class PictureServiceTests
{
    private readonly Mock<IPictureRepository> _mockRepository;
    private readonly Mock<ILogger<PictureService>> _mockLogger;
    private readonly PictureService _service;

    public PictureServiceTests()
    {
        _mockRepository = new Mock<IPictureRepository>();
        _mockLogger = new Mock<ILogger<PictureService>>();
        _service = new PictureService(_mockRepository.Object, _mockLogger.Object);
    }

    [Fact]
    public void Constructor_WithNullRepository_ShouldThrowArgumentNullException()
    {
        // Arrange & Act & Assert
        Assert.Throws<ArgumentNullException>(() =>
            new PictureService(null!, _mockLogger.Object));
    }

    [Fact]
    public void Constructor_WithNullLogger_ShouldThrowArgumentNullException()
    {
        // Arrange & Act & Assert
        Assert.Throws<ArgumentNullException>(() =>
            new PictureService(_mockRepository.Object, null!));
    }

    [Fact]
    public void Constructor_WithValidParameters_ShouldCreateInstance()
    {
        // Arrange & Act
        var service = new PictureService(_mockRepository.Object, _mockLogger.Object);

        // Assert
        Assert.NotNull(service);
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllPictures()
    {
        // Arrange
        var pictures = new List<Picture>
        {
            new Picture { Id = 1, Name = "Picture1" },
            new Picture { Id = 2, Name = "Picture2" }
        };
        _mockRepository.Setup(r => r.GetAllAsync(default)).ReturnsAsync(pictures);

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
        var pictures = new List<Picture>();
        _mockRepository.Setup(r => r.GetAllAsync(cancellationToken)).ReturnsAsync(pictures);

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
    public async Task GetByIdAsync_WithValidId_ShouldReturnPicture()
    {
        // Arrange
        var picture = new Picture { Id = 1, Name = "Test Picture" };
        _mockRepository.Setup(r => r.GetByIdAsync(1, default)).ReturnsAsync(picture);

        // Act
        var result = await _service.GetByIdAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("Test Picture", result.Name);
        _mockRepository.Verify(r => r.GetByIdAsync(1, default), Times.Once);
    }

    [Fact]
    public async Task GetByIdAsync_WithInvalidId_ShouldReturnNull()
    {
        // Arrange
        _mockRepository.Setup(r => r.GetByIdAsync(999, default)).ReturnsAsync((Picture?)null);

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
    public async Task CreateAsync_WithValidPicture_ShouldCreateAndReturnPicture()
    {
        // Arrange
        var picture = new Picture { Name = "New Picture", ImagePath = "/images/new.jpg" };
        _mockRepository.Setup(r => r.AddAsync(It.IsAny<Picture>(), default))
            .ReturnsAsync((Picture p, CancellationToken ct) => { p.Id = 1; return p; });

        // Act
        var result = await _service.CreateAsync(picture);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.IsActive);
        Assert.True(result.CreatedDate <= DateTime.UtcNow);
        _mockRepository.Verify(r => r.AddAsync(It.IsAny<Picture>(), default), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_ShouldSetCreatedDateAndIsActive()
    {
        // Arrange
        var picture = new Picture { Name = "Test", IsActive = false };
        var beforeCreate = DateTime.UtcNow;
        _mockRepository.Setup(r => r.AddAsync(It.IsAny<Picture>(), default))
            .ReturnsAsync((Picture p, CancellationToken ct) => p);

        // Act
        var result = await _service.CreateAsync(picture);

        // Assert
        Assert.True(result.IsActive);
        Assert.True(result.CreatedDate >= beforeCreate && result.CreatedDate <= DateTime.UtcNow);
    }

    [Fact]
    public async Task CreateAsync_WhenRepositoryThrowsException_ShouldThrowException()
    {
        // Arrange
        var picture = new Picture { Name = "Test" };
        _mockRepository.Setup(r => r.AddAsync(It.IsAny<Picture>(), default))
            .ThrowsAsync(new Exception("Database error"));

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _service.CreateAsync(picture));
    }

    [Fact]
    public async Task UpdateAsync_WithValidPicture_ShouldUpdateExistingPicture()
    {
        // Arrange
        var existingPicture = new Picture { Id = 1, Name = "Old Name", ImagePath = "/old.jpg" };
        var updatedPicture = new Picture { Name = "New Name", ImagePath = "/new.jpg", GalleryTypeId = 5 };
        _mockRepository.Setup(r => r.GetByIdAsync(1, default)).ReturnsAsync(existingPicture);
        _mockRepository.Setup(r => r.UpdateAsync(It.IsAny<Picture>(), default)).Returns(Task.CompletedTask);

        // Act
        await _service.UpdateAsync(1, updatedPicture);

        // Assert
        Assert.Equal("New Name", existingPicture.Name);
        Assert.Equal("/new.jpg", existingPicture.ImagePath);
        Assert.Equal(5, existingPicture.GalleryTypeId);
        Assert.NotNull(existingPicture.ModifiedDate);
        _mockRepository.Verify(r => r.UpdateAsync(existingPicture, default), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_WithNonExistentId_ShouldThrowInvalidOperationException()
    {
        // Arrange
        _mockRepository.Setup(r => r.GetByIdAsync(999, default)).ReturnsAsync((Picture?)null);
        var updatedPicture = new Picture { Name = "Test" };

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            _service.UpdateAsync(999, updatedPicture));
    }

    [Fact]
    public async Task UpdateAsync_ShouldSetModifiedDate()
    {
        // Arrange
        var existingPicture = new Picture { Id = 1, Name = "Old" };
        var updatedPicture = new Picture { Name = "New" };
        var beforeUpdate = DateTime.UtcNow;
        _mockRepository.Setup(r => r.GetByIdAsync(1, default)).ReturnsAsync(existingPicture);
        _mockRepository.Setup(r => r.UpdateAsync(It.IsAny<Picture>(), default)).Returns(Task.CompletedTask);

        // Act
        await _service.UpdateAsync(1, updatedPicture);

        // Assert
        Assert.NotNull(existingPicture.ModifiedDate);
        Assert.True(existingPicture.ModifiedDate >= beforeUpdate && existingPicture.ModifiedDate <= DateTime.UtcNow);
    }

    [Fact]
    public async Task UpdateAsync_WhenRepositoryThrowsException_ShouldThrowException()
    {
        // Arrange
        _mockRepository.Setup(r => r.GetByIdAsync(It.IsAny<int>(), default))
            .ThrowsAsync(new Exception("Database error"));
        var updatedPicture = new Picture { Name = "Test" };

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _service.UpdateAsync(1, updatedPicture));
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
    public async Task SearchAsync_WithValidSearchTerm_ShouldReturnMatchingPictures()
    {
        // Arrange
        var pictures = new List<Picture>
        {
            new Picture { Id = 1, Name = "Sunset Beach" },
            new Picture { Id = 2, Name = "Mountain Sunset" }
        };
        _mockRepository.Setup(r => r.SearchAsync("sunset", default)).ReturnsAsync(pictures);

        // Act
        var result = await _service.SearchAsync("sunset");

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
        _mockRepository.Verify(r => r.SearchAsync("sunset", default), Times.Once);
    }

    [Fact]
    public async Task SearchAsync_WithEmptySearchTerm_ShouldCallRepository()
    {
        // Arrange
        _mockRepository.Setup(r => r.SearchAsync("", default)).ReturnsAsync(new List<Picture>());

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

    [Fact]
    public async Task GetByGalleryTypeAsync_WithValidGalleryTypeId_ShouldReturnPictures()
    {
        // Arrange
        var pictures = new List<Picture>
        {
            new Picture { Id = 1, Name = "Pic1", GalleryTypeId = 5 },
            new Picture { Id = 2, Name = "Pic2", GalleryTypeId = 5 }
        };
        _mockRepository.Setup(r => r.GetByGalleryTypeAsync(5, default)).ReturnsAsync(pictures);

        // Act
        var result = await _service.GetByGalleryTypeAsync(5);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
        Assert.All(result, p => Assert.Equal(5, p.GalleryTypeId));
        _mockRepository.Verify(r => r.GetByGalleryTypeAsync(5, default), Times.Once);
    }

    [Fact]
    public async Task GetByGalleryTypeAsync_WithNonExistentGalleryType_ShouldReturnEmptyList()
    {
        // Arrange
        _mockRepository.Setup(r => r.GetByGalleryTypeAsync(999, default))
            .ReturnsAsync(new List<Picture>());

        // Act
        var result = await _service.GetByGalleryTypeAsync(999);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetByGalleryTypeAsync_WhenRepositoryThrowsException_ShouldThrowException()
    {
        // Arrange
        _mockRepository.Setup(r => r.GetByGalleryTypeAsync(It.IsAny<int>(), default))
            .ThrowsAsync(new Exception("Database error"));

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _service.GetByGalleryTypeAsync(1));
    }
}
