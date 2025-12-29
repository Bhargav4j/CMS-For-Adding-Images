using Xunit;
using Moq;
using GalleryApp.Domain.Entities;
using GalleryApp.Domain.Interfaces.Repositories;

namespace GalleryApp.Tests.Domain.Interfaces.Repositories;

public class IPictureRepositoryTests
{
    private readonly Mock<IPictureRepository> _mockRepository;

    public IPictureRepositoryTests()
    {
        _mockRepository = new Mock<IPictureRepository>();
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
        var expectedPictures = new List<Picture>();
        _mockRepository.Setup(r => r.GetAllAsync(cancellationToken))
            .ReturnsAsync(expectedPictures);

        // Act
        var result = await _mockRepository.Object.GetAllAsync(cancellationToken);

        // Assert
        Assert.NotNull(result);
        _mockRepository.Verify(r => r.GetAllAsync(cancellationToken), Times.Once);
    }

    [Fact]
    public async Task GetByIdAsync_WithValidId_ReturnsPicture()
    {
        // Arrange
        var expectedPicture = new Picture { Id = 1, Name = "Test Picture" };
        _mockRepository.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedPicture);

        // Act
        var result = await _mockRepository.Object.GetByIdAsync(1);

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
        var result = await _mockRepository.Object.GetByIdAsync(999);

        // Assert
        Assert.Null(result);
        _mockRepository.Verify(r => r.GetByIdAsync(999, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetByGalleryTypeIdAsync_ReturnsMatchingPictures()
    {
        // Arrange
        var galleryTypeId = 5;
        var expectedPictures = new List<Picture>
        {
            new Picture { Id = 1, Name = "Picture 1", GalleryTypeId = galleryTypeId },
            new Picture { Id = 2, Name = "Picture 2", GalleryTypeId = galleryTypeId }
        };
        _mockRepository.Setup(r => r.GetByGalleryTypeIdAsync(galleryTypeId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedPictures);

        // Act
        var result = await _mockRepository.Object.GetByGalleryTypeIdAsync(galleryTypeId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
        Assert.All(result, p => Assert.Equal(galleryTypeId, p.GalleryTypeId));
        _mockRepository.Verify(r => r.GetByGalleryTypeIdAsync(galleryTypeId, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task AddAsync_AddsPictureAndReturnsIt()
    {
        // Arrange
        var newPicture = new Picture { Name = "New Picture", ImagePath = "/images/new.jpg" };
        var addedPicture = new Picture { Id = 10, Name = "New Picture", ImagePath = "/images/new.jpg" };
        _mockRepository.Setup(r => r.AddAsync(newPicture, It.IsAny<CancellationToken>()))
            .ReturnsAsync(addedPicture);

        // Act
        var result = await _mockRepository.Object.AddAsync(newPicture);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(10, result.Id);
        Assert.Equal("New Picture", result.Name);
        _mockRepository.Verify(r => r.AddAsync(newPicture, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_UpdatesExistingPicture()
    {
        // Arrange
        var pictureToUpdate = new Picture { Id = 1, Name = "Updated Picture" };
        _mockRepository.Setup(r => r.UpdateAsync(pictureToUpdate, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        await _mockRepository.Object.UpdateAsync(pictureToUpdate);

        // Assert
        _mockRepository.Verify(r => r.UpdateAsync(pictureToUpdate, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_DeletesPictureById()
    {
        // Arrange
        var pictureId = 42;
        _mockRepository.Setup(r => r.DeleteAsync(pictureId, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        await _mockRepository.Object.DeleteAsync(pictureId);

        // Assert
        _mockRepository.Verify(r => r.DeleteAsync(pictureId, It.IsAny<CancellationToken>()), Times.Once);
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
    public async Task SearchAsync_WithSearchTerm_ReturnsMatchingPictures()
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
        var result = await _mockRepository.Object.SearchAsync(searchTerm);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
        _mockRepository.Verify(r => r.SearchAsync(searchTerm, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task SearchAsync_WithEmptySearchTerm_ReturnsAllOrNoPictures()
    {
        // Arrange
        var searchTerm = "";
        var expectedPictures = new List<Picture>();
        _mockRepository.Setup(r => r.SearchAsync(searchTerm, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedPictures);

        // Act
        var result = await _mockRepository.Object.SearchAsync(searchTerm);

        // Assert
        Assert.NotNull(result);
        _mockRepository.Verify(r => r.SearchAsync(searchTerm, It.IsAny<CancellationToken>()), Times.Once);
    }
}
