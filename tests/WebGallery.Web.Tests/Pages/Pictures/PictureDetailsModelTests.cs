using Xunit;
using Moq;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebGallery.Web.Pages.Pictures;
using WebGallery.Domain.Entities;
using WebGallery.Domain.Interfaces.Services;

namespace WebGallery.Web.Tests.Pages.Pictures;

public class PictureDetailsModelTests
{
    private readonly Mock<IPictureService> _mockPictureService;
    private readonly Mock<ILogger<PictureDetailsModel>> _mockLogger;
    private readonly PictureDetailsModel _pageModel;

    public PictureDetailsModelTests()
    {
        _mockPictureService = new Mock<IPictureService>();
        _mockLogger = new Mock<ILogger<PictureDetailsModel>>();
        _pageModel = new PictureDetailsModel(_mockPictureService.Object, _mockLogger.Object);
    }

    [Fact]
    public void Constructor_WithValidParameters_ShouldCreateInstance()
    {
        // Arrange & Act
        var pageModel = new PictureDetailsModel(_mockPictureService.Object, _mockLogger.Object);

        // Assert
        Assert.NotNull(pageModel);
    }

    [Fact]
    public async Task OnGetAsync_WithValidId_ShouldReturnPageResult()
    {
        // Arrange
        var picture = new Picture { Id = 1, Name = "Test Picture" };
        _mockPictureService.Setup(s => s.GetByIdAsync(1, default)).ReturnsAsync(picture);

        // Act
        var result = await _pageModel.OnGetAsync(1, default);

        // Assert
        Assert.IsType<PageResult>(result);
        Assert.NotNull(_pageModel.Picture);
        Assert.Equal("Test Picture", _pageModel.Picture.Name);
    }

    [Fact]
    public async Task OnGetAsync_WithInvalidId_ShouldReturnNotFound()
    {
        // Arrange
        _mockPictureService.Setup(s => s.GetByIdAsync(999, default)).ReturnsAsync((Picture?)null);

        // Act
        var result = await _pageModel.OnGetAsync(999, default);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task OnGetAsync_WhenExceptionOccurs_ShouldRedirectToIndex()
    {
        // Arrange
        _mockPictureService.Setup(s => s.GetByIdAsync(It.IsAny<int>(), default))
            .ThrowsAsync(new Exception("Database error"));

        // Act
        var result = await _pageModel.OnGetAsync(1, default);

        // Assert
        Assert.IsType<RedirectToPageResult>(result);
        var redirectResult = result as RedirectToPageResult;
        Assert.Equal("./Index", redirectResult?.PageName);
    }

    [Fact]
    public async Task OnGetAsync_WhenExceptionOccurs_ShouldLogError()
    {
        // Arrange
        _mockPictureService.Setup(s => s.GetByIdAsync(It.IsAny<int>(), default))
            .ThrowsAsync(new Exception("Database error"));

        // Act
        await _pageModel.OnGetAsync(1, default);

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
    public void Picture_DefaultValue_ShouldBeNull()
    {
        // Arrange & Act
        var pageModel = new PictureDetailsModel(_mockPictureService.Object, _mockLogger.Object);

        // Assert
        Assert.Null(pageModel.Picture);
    }

    [Fact]
    public async Task OnGetAsync_WithCancellationToken_ShouldPassTokenToService()
    {
        // Arrange
        var cancellationToken = new CancellationToken();
        var picture = new Picture { Id = 1, Name = "Test" };
        _mockPictureService.Setup(s => s.GetByIdAsync(1, cancellationToken)).ReturnsAsync(picture);

        // Act
        await _pageModel.OnGetAsync(1, cancellationToken);

        // Assert
        _mockPictureService.Verify(s => s.GetByIdAsync(1, cancellationToken), Times.Once);
    }
}
