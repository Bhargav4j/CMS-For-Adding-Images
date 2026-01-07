using AutoMapper;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using WebGallery.Application.DTOs;
using WebGallery.Application.Services;
using WebGallery.Domain.Entities;
using WebGallery.Domain.Interfaces.Repositories;

namespace Tests.WebGallery.Application;

public class PictureServiceTests
{
    private readonly Mock<IPictureRepository> _mockRepository;
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<ILogger<PictureService>> _mockLogger;
    private readonly PictureService _service;

    public PictureServiceTests()
    {
        _mockRepository = new Mock<IPictureRepository>();
        _mockMapper = new Mock<IMapper>();
        _mockLogger = new Mock<ILogger<PictureService>>();
        _service = new PictureService(_mockRepository.Object, _mockMapper.Object, _mockLogger.Object);
    }

    [Fact]
    public void Constructor_WithNullRepository_ThrowsArgumentNullException()
    {
        // Arrange & Act & Assert
        Assert.Throws<ArgumentNullException>(() => new PictureService(null!, _mockMapper.Object, _mockLogger.Object));
    }

    [Fact]
    public void Constructor_WithNullMapper_ThrowsArgumentNullException()
    {
        // Arrange & Act & Assert
        Assert.Throws<ArgumentNullException>(() => new PictureService(_mockRepository.Object, null!, _mockLogger.Object));
    }

    [Fact]
    public void Constructor_WithNullLogger_ThrowsArgumentNullException()
    {
        // Arrange & Act & Assert
        Assert.Throws<ArgumentNullException>(() => new PictureService(_mockRepository.Object, _mockMapper.Object, null!));
    }

    [Fact]
    public async Task GetAllAsync_ReturnsAllPictures()
    {
        // Arrange
        var pictures = new List<Picture>
        {
            new Picture { Id = 1, Name = "Picture 1" },
            new Picture { Id = 2, Name = "Picture 2" }
        };
        var pictureDtos = new List<PictureDto>
        {
            new PictureDto { Id = 1, Name = "Picture 1" },
            new PictureDto { Id = 2, Name = "Picture 2" }
        };

        _mockRepository.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(pictures);
        _mockMapper.Setup(m => m.Map<IEnumerable<PictureDto>>(pictures))
            .Returns(pictureDtos);

        // Act
        var result = await _service.GetAllAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, ((List<PictureDto>)result).Count);
        _mockRepository.Verify(r => r.GetAllAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetByIdAsync_WithValidId_ReturnsPicture()
    {
        // Arrange
        var picture = new Picture { Id = 1, Name = "Test Picture" };
        var pictureDto = new PictureDto { Id = 1, Name = "Test Picture" };

        _mockRepository.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(picture);
        _mockMapper.Setup(m => m.Map<PictureDto>(picture))
            .Returns(pictureDto);

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
        _mockRepository.Verify(r => r.GetByIdAsync(999, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_WithValidDto_CreatesAndReturnsPicture()
    {
        // Arrange
        var createDto = new PictureCreateDto { Name = "New Picture" };
        var picture = new Picture { Id = 0, Name = "New Picture" };
        var createdPicture = new Picture { Id = 1, Name = "New Picture" };
        var pictureDto = new PictureDto { Id = 1, Name = "New Picture" };

        _mockMapper.Setup(m => m.Map<Picture>(createDto))
            .Returns(picture);
        _mockRepository.Setup(r => r.AddAsync(picture, It.IsAny<CancellationToken>()))
            .ReturnsAsync(createdPicture);
        _mockMapper.Setup(m => m.Map<PictureDto>(createdPicture))
            .Returns(pictureDto);

        // Act
        var result = await _service.CreateAsync(createDto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("New Picture", result.Name);
        _mockRepository.Verify(r => r.AddAsync(picture, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_WithValidIdAndDto_UpdatesPicture()
    {
        // Arrange
        var updateDto = new PictureUpdateDto { Name = "Updated Picture" };
        var existingPicture = new Picture { Id = 1, Name = "Old Picture" };

        _mockRepository.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingPicture);
        _mockMapper.Setup(m => m.Map(updateDto, existingPicture))
            .Returns(existingPicture);
        _mockRepository.Setup(r => r.UpdateAsync(existingPicture, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        await _service.UpdateAsync(1, updateDto);

        // Assert
        _mockRepository.Verify(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()), Times.Once);
        _mockRepository.Verify(r => r.UpdateAsync(existingPicture, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_WithInvalidId_ThrowsInvalidOperationException()
    {
        // Arrange
        var updateDto = new PictureUpdateDto { Name = "Updated Picture" };

        _mockRepository.Setup(r => r.GetByIdAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Picture?)null);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _service.UpdateAsync(999, updateDto));
    }

    [Fact]
    public async Task DeleteAsync_WithValidId_DeletesPicture()
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
    public async Task SearchAsync_WithSearchTerm_ReturnsMatchingPictures()
    {
        // Arrange
        var searchTerm = "cake";
        var pictures = new List<Picture>
        {
            new Picture { Id = 1, Name = "Chocolate Cake" },
            new Picture { Id = 2, Name = "Vanilla Cake" }
        };
        var pictureDtos = new List<PictureDto>
        {
            new PictureDto { Id = 1, Name = "Chocolate Cake" },
            new PictureDto { Id = 2, Name = "Vanilla Cake" }
        };

        _mockRepository.Setup(r => r.SearchAsync(searchTerm, It.IsAny<CancellationToken>()))
            .ReturnsAsync(pictures);
        _mockMapper.Setup(m => m.Map<IEnumerable<PictureDto>>(pictures))
            .Returns(pictureDtos);

        // Act
        var result = await _service.SearchAsync(searchTerm);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, ((List<PictureDto>)result).Count);
        _mockRepository.Verify(r => r.SearchAsync(searchTerm, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetByGalleryTypeAsync_WithValidGalleryTypeId_ReturnsPictures()
    {
        // Arrange
        var galleryTypeId = 1;
        var pictures = new List<Picture>
        {
            new Picture { Id = 1, Name = "Picture 1", GalleryTypeId = 1 },
            new Picture { Id = 2, Name = "Picture 2", GalleryTypeId = 1 }
        };
        var pictureDtos = new List<PictureDto>
        {
            new PictureDto { Id = 1, Name = "Picture 1", GalleryTypeId = 1 },
            new PictureDto { Id = 2, Name = "Picture 2", GalleryTypeId = 1 }
        };

        _mockRepository.Setup(r => r.GetByGalleryTypeAsync(galleryTypeId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(pictures);
        _mockMapper.Setup(m => m.Map<IEnumerable<PictureDto>>(pictures))
            .Returns(pictureDtos);

        // Act
        var result = await _service.GetByGalleryTypeAsync(galleryTypeId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, ((List<PictureDto>)result).Count);
        _mockRepository.Verify(r => r.GetByGalleryTypeAsync(galleryTypeId, It.IsAny<CancellationToken>()), Times.Once);
    }
}
