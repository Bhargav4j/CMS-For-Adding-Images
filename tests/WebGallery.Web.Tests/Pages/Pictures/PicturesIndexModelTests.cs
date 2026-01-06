using Xunit;
using Moq;
using Microsoft.Extensions.Logging;
using WebGallery.Web.Pages.Pictures;
using WebGallery.Domain.Entities;
using WebGallery.Domain.Interfaces.Services;

namespace WebGallery.Web.Tests.Pages.Pictures;

public class PicturesIndexModelTests
{
    private readonly Mock<IPictureService> _mockPictureService;
    private readonly Mock<ILogger<PicturesIndexModel>> _mockLogger;
    private readonly PicturesIndexModel _pageModel;

    public PicturesIndexModelTests()
    {
        _mockPictureService = new Mock<IPictureService>();
        _mockLogger = new Mock<ILogger<PicturesIndexModel>>();
        _pageModel = new PicturesIndexModel(_mockPictureService.Object, _mockLogger.Object);
    }

    [Fact]
    public void Constructor_WithValidParameters_ShouldCreateInstance()
    {
        // Arrange & Act
        var pageModel = new PicturesIndexModel(_mockPictureService.Object, _mockLogger.Object);

        // Assert
        Assert.NotNull(pageModel);
    }

    [Fact]
    public async Task OnGetAsync_WithoutGalleryTypeId_ShouldGetAllPictures()
    {
        // Arrange
        var pictures = new List<Picture>
        {
            new Picture { Id = 1, Name = "Pic1" },
            new Picture { Id = 2, Name = "Pic2" }
        };
        _mockPictureService.Setup(s => s.GetAllAsync(default)).ReturnsAsync(pictures);

        // Act
        await _pageModel.OnGetAsync(default);

        // Assert
        Assert.Equal(2, _pageModel.Pictures.Count());
        _mockPictureService.Verify(s => s.GetAllAsync(default), Times.Once);
    }

    [Fact]
    public async Task OnGetAsync_WithGalleryTypeId_ShouldGetPicturesByGalleryType()
    {
        // Arrange
        var pictures = new List<Picture>
        {
            new Picture { Id = 1, Name = "Pic1", GalleryTypeId = 5 }
        };
        _pageModel.GalleryTypeId = 5;
        _mockPictureService.Setup(s => s.GetByGalleryTypeAsync(5, default)).ReturnsAsync(pictures);

        // Act
        await _pageModel.OnGetAsync(default);

        // Assert
        Assert.Single(_pageModel.Pictures);
        _mockPictureService.Verify(s => s.GetByGalleryTypeAsync(5, default), Times.Once);
    }

    [Fact]
    public async Task OnGetAsync_WhenExceptionOccurs_ShouldSetEmptyPicturesList()
    {
        // Arrange
        _mockPictureService.Setup(s => s.GetAllAsync(default))
            .ThrowsAsync(new Exception("Database error"));

        // Act
        await _pageModel.OnGetAsync(default);

        // Assert
        Assert.Empty(_pageModel.Pictures);
    }

    [Fact]
    public async Task OnGetAsync_WhenExceptionOccurs_ShouldLogError()
    {
        // Arrange
        _mockPictureService.Setup(s => s.GetAllAsync(default))
            .ThrowsAsync(new Exception("Database error"));

        // Act
        await _pageModel.OnGetAsync(default);

        // Assert
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => true),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public void Pictures_DefaultValue_ShouldBeEmptyList()
    {
        // Arrange & Act
        var pageModel = new PicturesIndexModel(_mockPictureService.Object, _mockLogger.Object);

        // Assert
        Assert.NotNull(pageModel.Pictures);
        Assert.Empty(pageModel.Pictures);
    }

    [Fact]
    public void GalleryTypeId_CanBeSet()
    {
        // Arrange
        var pageModel = new PicturesIndexModel(_mockPictureService.Object, _mockLogger.Object);

        // Act
        pageModel.GalleryTypeId = 10;

        // Assert
        Assert.Equal(10, pageModel.GalleryTypeId);
    }

    [Fact]
    public async Task OnGetAsync_WithCancellationToken_ShouldPassTokenToService()
    {
        // Arrange
        var cancellationToken = new CancellationToken();
        var pictures = new List<Picture>();
        _mockPictureService.Setup(s => s.GetAllAsync(cancellationToken)).ReturnsAsync(pictures);

        // Act
        await _pageModel.OnGetAsync(cancellationToken);

        // Assert
        _mockPictureService.Verify(s => s.GetAllAsync(cancellationToken), Times.Once);
    }
}
