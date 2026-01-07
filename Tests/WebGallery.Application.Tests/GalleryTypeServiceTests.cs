using AutoMapper;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using WebGallery.Application.DTOs;
using WebGallery.Application.Services;
using WebGallery.Domain.Entities;
using WebGallery.Domain.Interfaces.Repositories;

namespace Tests.WebGallery.Application;

public class GalleryTypeServiceTests
{
    private readonly Mock<IGalleryTypeRepository> _mockRepository;
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<ILogger<GalleryTypeService>> _mockLogger;
    private readonly GalleryTypeService _service;

    public GalleryTypeServiceTests()
    {
        _mockRepository = new Mock<IGalleryTypeRepository>();
        _mockMapper = new Mock<IMapper>();
        _mockLogger = new Mock<ILogger<GalleryTypeService>>();
        _service = new GalleryTypeService(_mockRepository.Object, _mockMapper.Object, _mockLogger.Object);
    }

    [Fact]
    public void Constructor_WithNullRepository_ThrowsArgumentNullException()
    {
        // Arrange & Act & Assert
        Assert.Throws<ArgumentNullException>(() => new GalleryTypeService(null!, _mockMapper.Object, _mockLogger.Object));
    }

    [Fact]
    public void Constructor_WithNullMapper_ThrowsArgumentNullException()
    {
        // Arrange & Act & Assert
        Assert.Throws<ArgumentNullException>(() => new GalleryTypeService(_mockRepository.Object, null!, _mockLogger.Object));
    }

    [Fact]
    public void Constructor_WithNullLogger_ThrowsArgumentNullException()
    {
        // Arrange & Act & Assert
        Assert.Throws<ArgumentNullException>(() => new GalleryTypeService(_mockRepository.Object, _mockMapper.Object, null!));
    }

    [Fact]
    public async Task GetAllAsync_ReturnsAllGalleryTypes()
    {
        // Arrange
        var galleryTypes = new List<GalleryType>
        {
            new GalleryType { Id = 1, Name = "Cakes" },
            new GalleryType { Id = 2, Name = "Books" }
        };
        var galleryTypeDtos = new List<GalleryTypeDto>
        {
            new GalleryTypeDto { Id = 1, Name = "Cakes" },
            new GalleryTypeDto { Id = 2, Name = "Books" }
        };

        _mockRepository.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(galleryTypes);
        _mockMapper.Setup(m => m.Map<IEnumerable<GalleryTypeDto>>(galleryTypes))
            .Returns(galleryTypeDtos);

        // Act
        var result = await _service.GetAllAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, ((List<GalleryTypeDto>)result).Count);
        _mockRepository.Verify(r => r.GetAllAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetByIdAsync_WithValidId_ReturnsGalleryType()
    {
        // Arrange
        var galleryType = new GalleryType { Id = 1, Name = "Cakes" };
        var galleryTypeDto = new GalleryTypeDto { Id = 1, Name = "Cakes" };

        _mockRepository.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(galleryType);
        _mockMapper.Setup(m => m.Map<GalleryTypeDto>(galleryType))
            .Returns(galleryTypeDto);

        // Act
        var result = await _service.GetByIdAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("Cakes", result.Name);
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
        _mockRepository.Verify(r => r.GetByIdAsync(999, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_WithValidDto_CreatesAndReturnsGalleryType()
    {
        // Arrange
        var createDto = new GalleryTypeCreateDto { Name = "Prose" };
        var galleryType = new GalleryType { Id = 0, Name = "Prose" };
        var createdGalleryType = new GalleryType { Id = 1, Name = "Prose" };
        var galleryTypeDto = new GalleryTypeDto { Id = 1, Name = "Prose" };

        _mockMapper.Setup(m => m.Map<GalleryType>(createDto))
            .Returns(galleryType);
        _mockRepository.Setup(r => r.AddAsync(galleryType, It.IsAny<CancellationToken>()))
            .ReturnsAsync(createdGalleryType);
        _mockMapper.Setup(m => m.Map<GalleryTypeDto>(createdGalleryType))
            .Returns(galleryTypeDto);

        // Act
        var result = await _service.CreateAsync(createDto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("Prose", result.Name);
        _mockRepository.Verify(r => r.AddAsync(galleryType, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_WithValidIdAndDto_UpdatesGalleryType()
    {
        // Arrange
        var updateDto = new GalleryTypeUpdateDto { Name = "Updated Gallery" };
        var existingGalleryType = new GalleryType { Id = 1, Name = "Old Gallery" };

        _mockRepository.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingGalleryType);
        _mockMapper.Setup(m => m.Map(updateDto, existingGalleryType))
            .Returns(existingGalleryType);
        _mockRepository.Setup(r => r.UpdateAsync(existingGalleryType, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        await _service.UpdateAsync(1, updateDto);

        // Assert
        _mockRepository.Verify(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()), Times.Once);
        _mockRepository.Verify(r => r.UpdateAsync(existingGalleryType, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_WithInvalidId_ThrowsInvalidOperationException()
    {
        // Arrange
        var updateDto = new GalleryTypeUpdateDto { Name = "Updated Gallery" };

        _mockRepository.Setup(r => r.GetByIdAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync((GalleryType?)null);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _service.UpdateAsync(999, updateDto));
    }

    [Fact]
    public async Task DeleteAsync_WithValidId_DeletesGalleryType()
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
    public async Task SearchAsync_WithSearchTerm_ReturnsMatchingGalleryTypes()
    {
        // Arrange
        var searchTerm = "Cake";
        var galleryTypes = new List<GalleryType>
        {
            new GalleryType { Id = 1, Name = "Cakes" },
            new GalleryType { Id = 2, Name = "Birthday Cakes" }
        };
        var galleryTypeDtos = new List<GalleryTypeDto>
        {
            new GalleryTypeDto { Id = 1, Name = "Cakes" },
            new GalleryTypeDto { Id = 2, Name = "Birthday Cakes" }
        };

        _mockRepository.Setup(r => r.SearchAsync(searchTerm, It.IsAny<CancellationToken>()))
            .ReturnsAsync(galleryTypes);
        _mockMapper.Setup(m => m.Map<IEnumerable<GalleryTypeDto>>(galleryTypes))
            .Returns(galleryTypeDtos);

        // Act
        var result = await _service.SearchAsync(searchTerm);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, ((List<GalleryTypeDto>)result).Count);
        _mockRepository.Verify(r => r.SearchAsync(searchTerm, It.IsAny<CancellationToken>()), Times.Once);
    }
}
