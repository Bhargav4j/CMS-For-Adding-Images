using Xunit;
using Moq;
using Microsoft.Extensions.Logging;
using GalleryApp.Domain.Entities;
using GalleryApp.Domain.Interfaces.Repositories;
using GalleryApp.Application.Services;

namespace GalleryApp.Tests.Application.Services;

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
        var galleryTypes = new List<GalleryType>();
        _mockRepository.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(galleryTypes);

        // Act
        await _service.GetAllAsync();

        // Assert
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Retrieving all gallery types")),
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
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Error retrieving all gallery types")),
                exception,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task GetByIdAsync_WithValidId_ReturnsGalleryType()
    {
        // Arrange
        var expectedGalleryType = new GalleryType { Id = 1, Name = "Test Gallery" };
        _mockRepository.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedGalleryType);

        // Act
        var result = await _service.GetByIdAsync(1);

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
        var result = await _service.GetByIdAsync(999);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetByIdAsync_LogsInformationWithId()
    {
        // Arrange
        _mockRepository.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new GalleryType { Id = 1 });

        // Act
        await _service.GetByIdAsync(1);

        // Assert
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Retrieving gallery type with ID")),
                null,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task CreateAsync_SetsDatesAndIsActiveFlag()
    {
        // Arrange
        var newGalleryType = new GalleryType { Name = "New Gallery" };
        _mockRepository.Setup(r => r.AddAsync(It.IsAny<GalleryType>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((GalleryType g, CancellationToken ct) => g);

        // Act
        var result = await _service.CreateAsync(newGalleryType);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.IsActive);
        Assert.NotEqual(default(DateTime), result.CreatedDate);
        _mockRepository.Verify(r => r.AddAsync(It.Is<GalleryType>(g =>
            g.IsActive && g.CreatedDate != default(DateTime)),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_LogsInformationWithGalleryTypeName()
    {
        // Arrange
        var newGalleryType = new GalleryType { Name = "New Gallery" };
        _mockRepository.Setup(r => r.AddAsync(It.IsAny<GalleryType>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(newGalleryType);

        // Act
        await _service.CreateAsync(newGalleryType);

        // Assert
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Creating new gallery type")),
                null,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_UpdatesExistingGalleryType()
    {
        // Arrange
        var existingGalleryType = new GalleryType
        {
            Id = 1,
            Name = "Old Name",
            Description = "Old Description"
        };
        var updatedGalleryType = new GalleryType
        {
            Id = 1,
            Name = "New Name",
            Description = "New Description",
            ModifiedBy = "user@example.com"
        };

        _mockRepository.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingGalleryType);
        _mockRepository.Setup(r => r.UpdateAsync(It.IsAny<GalleryType>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        await _service.UpdateAsync(1, updatedGalleryType);

        // Assert
        Assert.Equal("New Name", existingGalleryType.Name);
        Assert.Equal("New Description", existingGalleryType.Description);
        Assert.Equal("user@example.com", existingGalleryType.ModifiedBy);
        Assert.NotNull(existingGalleryType.ModifiedDate);
        _mockRepository.Verify(r => r.UpdateAsync(existingGalleryType, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_WithNonExistingId_ThrowsException()
    {
        // Arrange
        var galleryType = new GalleryType { Id = 1, Name = "Test" };
        _mockRepository.Setup(r => r.GetByIdAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync((GalleryType?)null);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            _service.UpdateAsync(999, galleryType));
    }

    [Fact]
    public async Task UpdateAsync_SetsModifiedDate()
    {
        // Arrange
        var existingGalleryType = new GalleryType { Id = 1, Name = "Old Name" };
        var updatedGalleryType = new GalleryType { Id = 1, Name = "New Name" };
        _mockRepository.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingGalleryType);
        _mockRepository.Setup(r => r.UpdateAsync(It.IsAny<GalleryType>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        await _service.UpdateAsync(1, updatedGalleryType);

        // Assert
        Assert.NotNull(existingGalleryType.ModifiedDate);
        Assert.True(existingGalleryType.ModifiedDate > DateTime.UtcNow.AddSeconds(-5));
    }

    [Fact]
    public async Task DeleteAsync_DeletesGalleryType()
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
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Deleting gallery type with ID")),
                null,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task SearchAsync_ReturnsMatchingGalleryTypes()
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
            .ReturnsAsync(new List<GalleryType>());

        // Act
        await _service.SearchAsync(searchTerm);

        // Assert
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Searching gallery types with term")),
                null,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }
}
