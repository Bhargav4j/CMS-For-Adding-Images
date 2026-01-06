using Xunit;
using Moq;
using Microsoft.Extensions.Logging;
using WebGallery.Web.Pages;

namespace WebGallery.Web.Tests.Pages;

public class IndexModelTests
{
    private readonly Mock<ILogger<IndexModel>> _mockLogger;
    private readonly IndexModel _pageModel;

    public IndexModelTests()
    {
        _mockLogger = new Mock<ILogger<IndexModel>>();
        _pageModel = new IndexModel(_mockLogger.Object);
    }

    [Fact]
    public void Constructor_WithNullLogger_ShouldThrowArgumentNullException()
    {
        // Arrange & Act & Assert
        Assert.Throws<ArgumentNullException>(() => new IndexModel(null!));
    }

    [Fact]
    public void Constructor_WithValidLogger_ShouldCreateInstance()
    {
        // Arrange & Act
        var pageModel = new IndexModel(_mockLogger.Object);

        // Assert
        Assert.NotNull(pageModel);
    }

    [Fact]
    public void OnGet_ShouldExecuteWithoutErrors()
    {
        // Arrange & Act
        _pageModel.OnGet();

        // Assert - No exception thrown
        Assert.NotNull(_pageModel);
    }

    [Fact]
    public void OnGet_ShouldLogInformation()
    {
        // Arrange & Act
        _pageModel.OnGet();

        // Assert
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Home page accessed")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public void IndexModel_ShouldInheritFromPageModel()
    {
        // Arrange & Act
        var pageModel = new IndexModel(_mockLogger.Object);

        // Assert
        Assert.IsAssignableFrom<Microsoft.AspNetCore.Mvc.RazorPages.PageModel>(pageModel);
    }

    [Fact]
    public void OnGet_CalledMultipleTimes_ShouldLogMultipleTimes()
    {
        // Arrange & Act
        _pageModel.OnGet();
        _pageModel.OnGet();
        _pageModel.OnGet();

        // Assert
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Home page accessed")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Exactly(3));
    }
}
