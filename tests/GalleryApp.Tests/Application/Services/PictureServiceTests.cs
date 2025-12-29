using Xunit;
using Moq;
using Microsoft.Extensions.Logging;
using GalleryApp.Domain.Entities;
using GalleryApp.Domain.Interfaces.Repositories;
using GalleryApp.Application.Services;

namespace GalleryApp.Tests.Application.Services;

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
    public async Task GetAllAsync_ReturnsAllPictures()
    {
        // Arrange
        var expectedPictures = new List<Picture>
        {
            new Picture { Id = 1, Name = "Picture 1" },
            new Picture { Id = 2, Name = "Picture 2" }
        };
        _mockRepository.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedPictures);

        // Act
        var result = await _service.GetAllAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
        _mockRepository.Verify(r => r.GetAllAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetAllAsync_LogsInformation()
    {
        // Arrange
        var pictures = new List<Picture>();
        _mockRepository.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(pictures);

        // Act
        await _service.GetAllAsync();

        // Assert
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Retrieving all pictures")),
                null,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task GetAllAsync_OnException_LogsErrorAndRethrows()
    {
        // Arrange
        var exception = new Exception("Database error");
        _mockRepository.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ThrowsAsync(exception);

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _service.GetAllAsync());
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Error retrieving all pictures")),
                exception,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task GetByIdAsync_WithValidId_ReturnsPicture()
    {
        // Arrange
        var expectedPicture = new Picture { Id = 1, Name = "Test Picture" };
        _mockRepository.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedPicture);

        // Act
        var result = await _service.GetByIdAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("Test Picture", result.Name);
        _mockRepository.Verify(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetByIdAsync_WithInvalidId_ReturnsNull()
    {
        // Arrange
        _mockRepository.Setup(r => r.GetByIdAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Picture?)null);

        // Act
        var result = await _service.GetByIdAsync(999);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetByIdAsync_LogsInformationWithId()
    {
        // Arrange
        _mockRepository.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Picture { Id = 1 });

        // Act
        await _service.GetByIdAsync(1);

        // Assert
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Retrieving picture with ID")),
                null,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task GetByGalleryTypeIdAsync_ReturnsMatchingPictures()
    {
        // Arrange
        var galleryTypeId = 5;
        var expectedPictures = new List<Picture>
        {
            new Picture { Id = 1, GalleryTypeId = galleryTypeId },
            new Picture { Id = 2, GalleryTypeId = galleryTypeId }
        };
        _mockRepository.Setup(r => r.GetByGalleryTypeIdAsync(galleryTypeId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedPictures);

        // Act
        var result = await _service.GetByGalleryTypeIdAsync(galleryTypeId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
        Assert.All(result, p => Assert.Equal(galleryTypeId, p.GalleryTypeId));
    }

    [Fact]
    public async Task CreateAsync_SetsDatesAndIsActiveFlag()
    {
        // Arrange
        var newPicture = new Picture { Name = "New Picture" };
        _mockRepository.Setup(r => r.AddAsync(It.IsAny<Picture>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Picture p, CancellationToken ct) => p);

        // Act
        var result = await _service.CreateAsync(newPicture);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.IsActive);
        Assert.NotEqual(default(DateTime), result.CreatedDate);
        _mockRepository.Verify(r => r.AddAsync(It.Is<Picture>(p =>
            p.IsActive && p.CreatedDate != default(DateTime)),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_LogsInformationWithPictureName()
    {
        // Arrange
        var newPicture = new Picture { Name = "New Picture" };
        _mockRepository.Setup(r => r.AddAsync(It.IsAny<Picture>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(newPicture);

        // Act
        await _service.CreateAsync(newPicture);

        // Assert
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Creating new picture")),
                null,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_UpdatesExistingPicture()
    {
        // Arrange
        var existingPicture = new Picture
        {
            Id = 1,
            Name = "Old Name",
            Description = "Old Description"
        };
        var updatedPicture = new Picture
        {
            Id = 1,
            Name = "New Name",
            Description = "New Description",
            ImagePath = "/images/new.jpg",
            ThumbnailImagePath = "/images/thumbnails/new.jpg",
            GalleryTypeId = 5,
            ModifiedBy = "user@example.com"
        };

        _mockRepository.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingPicture);
        _mockRepository.Setup(r => r.UpdateAsync(It.IsAny<Picture>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        await _service.UpdateAsync(1, updatedPicture);

        // Assert
        Assert.Equal("New Name", existingPicture.Name);
        Assert.Equal("New Description", existingPicture.Description);
        Assert.Equal("/images/new.jpg", existingPicture.ImagePath);
        Assert.Equal("/images/thumbnails/new.jpg", existingPicture.ThumbnailImagePath);
        Assert.Equal(5, existingPicture.GalleryTypeId);
        Assert.Equal("user@example.com", existingPicture.ModifiedBy);
        Assert.NotNull(existingPicture.ModifiedDate);
        _mockRepository.Verify(r => r.UpdateAsync(existingPicture, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_WithNonExistingId_ThrowsException()
    {
        // Arrange
        var picture = new Picture { Id = 1, Name = "Test" };
        _mockRepository.Setup(r => r.GetByIdAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Picture?)null);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            _service.UpdateAsync(999, picture));
    }

    [Fact]
    public async Task UpdateAsync_SetsModifiedDate()
    {
        // Arrange
        var existingPicture = new Picture { Id = 1, Name = "Old Name" };
        var updatedPicture = new Picture { Id = 1, Name = "New Name" };
        _mockRepository.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingPicture);
        _mockRepository.Setup(r => r.UpdateAsync(It.IsAny<Picture>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        await _service.UpdateAsync(1, updatedPicture);

        // Assert
        Assert.NotNull(existingPicture.ModifiedDate);
        Assert.True(existingPicture.ModifiedDate > DateTime.UtcNow.AddSeconds(-5));
    }

    [Fact]
    public async Task DeleteAsync_DeletesPicture()
    {
        // Arrange
        _mockRepository.Setup(r => r.DeleteAsync(1, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        await _service.DeleteAsync(1);

        // Assert
        _mockRepository.Verify(r => r.DeleteAsync(1, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_LogsInformationWithId()
    {
        // Arrange
        _mockRepository.Setup(r => r.DeleteAsync(1, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        await _service.DeleteAsync(1);

        // Assert
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Deleting picture with ID")),
                null,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task SearchAsync_ReturnsMatchingPictures()
    {
        // Arrange
        var searchTerm = "nature";
        var expectedPictures = new List<Picture>
        {
            new Picture { Id = 1, Name = "Nature Picture 1" },
            new Picture { Id = 2, Name = "Nature Picture 2" }
        };
        _mockRepository.Setup(r => r.SearchAsync(searchTerm, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedPictures);

        // Act
        var result = await _service.SearchAsync(searchTerm);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
        _mockRepository.Verify(r => r.SearchAsync(searchTerm, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task SearchAsync_LogsInformationWithSearchTerm()
    {
        // Arrange
        var searchTerm = "test";
        _mockRepository.Setup(r => r.SearchAsync(searchTerm, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Picture>());

        // Act
        await _service.SearchAsync(searchTerm);

        // Assert
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Searching pictures with term")),
                null,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }
}
