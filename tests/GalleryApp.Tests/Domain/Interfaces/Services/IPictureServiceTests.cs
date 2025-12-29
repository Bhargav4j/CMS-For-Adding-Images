using Xunit;
using Moq;
using GalleryApp.Domain.Entities;
using GalleryApp.Domain.Interfaces.Services;

namespace GalleryApp.Tests.Domain.Interfaces.Services;

public class IPictureServiceTests
{
    private readonly Mock<IPictureService> _mockService;

    public IPictureServiceTests()
    {
        _mockService = new Mock<IPictureService>();
    }

    [Fact]
    public async Task GetAllAsync_ReturnsAllPictures()
    {
        // Arrange
        var expectedPictures = new List<Picture>
        {
            new Picture { Id = 1, Name = "Picture 1" },
            new Picture { Id = 2, Name = "Picture 2" },
            new Picture { Id = 3, Name = "Picture 3" }
        };
        _mockService.Setup(s => s.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedPictures);

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
        var expectedPictures = new List<Picture>();
        _mockService.Setup(s => s.GetAllAsync(cancellationToken))
            .ReturnsAsync(expectedPictures);

        // Act
        var result = await _mockService.Object.GetAllAsync(cancellationToken);

        // Assert
        Assert.NotNull(result);
        _mockService.Verify(s => s.GetAllAsync(cancellationToken), Times.Once);
    }

    [Fact]
    public async Task GetByIdAsync_WithValidId_ReturnsPicture()
    {
        // Arrange
        var expectedPicture = new Picture { Id = 1, Name = "Test Picture" };
        _mockService.Setup(s => s.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedPicture);

        // Act
        var result = await _mockService.Object.GetByIdAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("Test Picture", result.Name);
        _mockService.Verify(s => s.GetByIdAsync(1, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetByIdAsync_WithInvalidId_ReturnsNull()
    {
        // Arrange
        _mockService.Setup(s => s.GetByIdAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Picture?)null);

        // Act
        var result = await _mockService.Object.GetByIdAsync(999);

        // Assert
        Assert.Null(result);
        _mockService.Verify(s => s.GetByIdAsync(999, It.IsAny<CancellationToken>()), Times.Once);
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
        _mockService.Setup(s => s.GetByGalleryTypeIdAsync(galleryTypeId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedPictures);

        // Act
        var result = await _mockService.Object.GetByGalleryTypeIdAsync(galleryTypeId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
        Assert.All(result, p => Assert.Equal(galleryTypeId, p.GalleryTypeId));
        _mockService.Verify(s => s.GetByGalleryTypeIdAsync(galleryTypeId, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_CreatesPictureAndReturnsIt()
    {
        // Arrange
        var newPicture = new Picture { Name = "New Picture", ImagePath = "/images/new.jpg" };
        var createdPicture = new Picture { Id = 10, Name = "New Picture", ImagePath = "/images/new.jpg" };
        _mockService.Setup(s => s.CreateAsync(newPicture, It.IsAny<CancellationToken>()))
            .ReturnsAsync(createdPicture);

        // Act
        var result = await _mockService.Object.CreateAsync(newPicture);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(10, result.Id);
        Assert.Equal("New Picture", result.Name);
        _mockService.Verify(s => s.CreateAsync(newPicture, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_UpdatesExistingPicture()
    {
        // Arrange
        var pictureId = 1;
        var updatedPicture = new Picture { Id = pictureId, Name = "Updated Picture" };
        _mockService.Setup(s => s.UpdateAsync(pictureId, updatedPicture, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        await _mockService.Object.UpdateAsync(pictureId, updatedPicture);

        // Assert
        _mockService.Verify(s => s.UpdateAsync(pictureId, updatedPicture, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_DeletesPictureById()
    {
        // Arrange
        var pictureId = 42;
        _mockService.Setup(s => s.DeleteAsync(pictureId, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        await _mockService.Object.DeleteAsync(pictureId);

        // Assert
        _mockService.Verify(s => s.DeleteAsync(pictureId, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task SearchAsync_WithSearchTerm_ReturnsMatchingPictures()
    {
        // Arrange
        var searchTerm = "sunset";
        var expectedPictures = new List<Picture>
        {
            new Picture { Id = 1, Name = "Sunset at Beach" },
            new Picture { Id = 2, Name = "Mountain Sunset" }
        };
        _mockService.Setup(s => s.SearchAsync(searchTerm, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedPictures);

        // Act
        var result = await _mockService.Object.SearchAsync(searchTerm);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
        _mockService.Verify(s => s.SearchAsync(searchTerm, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task SearchAsync_WithEmptySearchTerm_ReturnsAllOrNoPictures()
    {
        // Arrange
        var searchTerm = "";
        var expectedPictures = new List<Picture>();
        _mockService.Setup(s => s.SearchAsync(searchTerm, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedPictures);

        // Act
        var result = await _mockService.Object.SearchAsync(searchTerm);

        // Assert
        Assert.NotNull(result);
        _mockService.Verify(s => s.SearchAsync(searchTerm, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_WithNullPicture_ThrowsException()
    {
        // Arrange
        _mockService.Setup(s => s.CreateAsync(null!, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new ArgumentNullException(nameof(Picture)));

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() =>
            _mockService.Object.CreateAsync(null!, CancellationToken.None));
    }

    [Fact]
    public async Task UpdateAsync_WithInvalidId_ThrowsException()
    {
        // Arrange
        var invalidId = -1;
        var picture = new Picture { Id = 1, Name = "Test" };
        _mockService.Setup(s => s.UpdateAsync(invalidId, picture, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new ArgumentException("Invalid ID"));

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() =>
            _mockService.Object.UpdateAsync(invalidId, picture, CancellationToken.None));
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
}
