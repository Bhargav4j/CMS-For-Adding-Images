using Xunit;
using GalleryApp.Infrastructure.Services;

namespace GalleryApp.Tests.Infrastructure.Services;

public class PathServiceTests
{
    [Fact]
    public void Constructor_WithValidPaths_InitializesService()
    {
        // Arrange
        var webRootPath = "/var/www/html/wwwroot";
        var contentRootPath = "/var/www/html";

        // Act
        var service = new PathService(webRootPath, contentRootPath);

        // Assert
        Assert.NotNull(service);
        Assert.Equal(webRootPath, service.GetWebRootPath());
        Assert.Equal(contentRootPath, service.GetContentRootPath());
    }

    [Fact]
    public void GetWebRootPath_ReturnsCorrectPath()
    {
        // Arrange
        var webRootPath = "/var/www/html/wwwroot";
        var contentRootPath = "/var/www/html";
        var service = new PathService(webRootPath, contentRootPath);

        // Act
        var result = service.GetWebRootPath();

        // Assert
        Assert.Equal(webRootPath, result);
    }

    [Fact]
    public void GetContentRootPath_ReturnsCorrectPath()
    {
        // Arrange
        var webRootPath = "/var/www/html/wwwroot";
        var contentRootPath = "/var/www/html";
        var service = new PathService(webRootPath, contentRootPath);

        // Act
        var result = service.GetContentRootPath();

        // Assert
        Assert.Equal(contentRootPath, result);
    }

    [Fact]
    public void MapPath_WithRelativePath_CombinesWithWebRoot()
    {
        // Arrange
        var webRootPath = "/var/www/html/wwwroot";
        var contentRootPath = "/var/www/html";
        var service = new PathService(webRootPath, contentRootPath);
        var relativePath = "images/test.jpg";

        // Act
        var result = service.MapPath(relativePath);

        // Assert
        Assert.Equal(Path.Combine(webRootPath, relativePath), result);
    }

    [Fact]
    public void MapPath_WithTildePath_RemovesTildeAndCombines()
    {
        // Arrange
        var webRootPath = "/var/www/html/wwwroot";
        var contentRootPath = "/var/www/html";
        var service = new PathService(webRootPath, contentRootPath);
        var tildePath = "~/images/test.jpg";

        // Act
        var result = service.MapPath(tildePath);

        // Assert
        Assert.Equal(Path.Combine(webRootPath, "images/test.jpg"), result);
    }

    [Fact]
    public void MapPath_WithSimplePath_ReturnsCombinedPath()
    {
        // Arrange
        var webRootPath = "C:\\inetpub\\wwwroot";
        var contentRootPath = "C:\\inetpub";
        var service = new PathService(webRootPath, contentRootPath);
        var simplePath = "uploads";

        // Act
        var result = service.MapPath(simplePath);

        // Assert
        Assert.Equal(Path.Combine(webRootPath, simplePath), result);
    }

    [Fact]
    public void MapPath_WithNestedPath_ReturnsCorrectPath()
    {
        // Arrange
        var webRootPath = "/var/www/html/wwwroot";
        var contentRootPath = "/var/www/html";
        var service = new PathService(webRootPath, contentRootPath);
        var nestedPath = "images/gallery/thumbnails/pic.jpg";

        // Act
        var result = service.MapPath(nestedPath);

        // Assert
        Assert.Equal(Path.Combine(webRootPath, nestedPath), result);
    }

    [Fact]
    public void MapPath_WithEmptyPath_ReturnsCombinedPath()
    {
        // Arrange
        var webRootPath = "/var/www/html/wwwroot";
        var contentRootPath = "/var/www/html";
        var service = new PathService(webRootPath, contentRootPath);
        var emptyPath = "";

        // Act
        var result = service.MapPath(emptyPath);

        // Assert
        Assert.Equal(Path.Combine(webRootPath, emptyPath), result);
    }

    [Fact]
    public void MapPath_MultipleCalls_ReturnsConsistentResults()
    {
        // Arrange
        var webRootPath = "/var/www/html/wwwroot";
        var contentRootPath = "/var/www/html";
        var service = new PathService(webRootPath, contentRootPath);
        var path = "images/test.jpg";

        // Act
        var result1 = service.MapPath(path);
        var result2 = service.MapPath(path);

        // Assert
        Assert.Equal(result1, result2);
    }

    [Fact]
    public void MapPath_WithWindowsPath_HandlesCorrectly()
    {
        // Arrange
        var webRootPath = "C:\\inetpub\\wwwroot";
        var contentRootPath = "C:\\inetpub";
        var service = new PathService(webRootPath, contentRootPath);
        var windowsPath = "images\\test.jpg";

        // Act
        var result = service.MapPath(windowsPath);

        // Assert
        Assert.Equal(Path.Combine(webRootPath, windowsPath), result);
    }

    [Fact]
    public void MapPath_WithTildePathAndSubdirectories_RemovesTildeCorrectly()
    {
        // Arrange
        var webRootPath = "/var/www/html/wwwroot";
        var contentRootPath = "/var/www/html";
        var service = new PathService(webRootPath, contentRootPath);
        var tildePath = "~/assets/css/styles.css";

        // Act
        var result = service.MapPath(tildePath);

        // Assert
        Assert.Equal(Path.Combine(webRootPath, "assets/css/styles.css"), result);
        Assert.DoesNotContain("~", result);
    }

    [Theory]
    [InlineData("images/test.jpg")]
    [InlineData("~/images/test.jpg")]
    [InlineData("uploads/documents/file.pdf")]
    [InlineData("~/uploads/documents/file.pdf")]
    public void MapPath_WithVariousPaths_ReturnsValidPath(string inputPath)
    {
        // Arrange
        var webRootPath = "/var/www/html/wwwroot";
        var contentRootPath = "/var/www/html";
        var service = new PathService(webRootPath, contentRootPath);

        // Act
        var result = service.MapPath(inputPath);

        // Assert
        Assert.NotNull(result);
        Assert.NotEmpty(result);
        Assert.Contains(webRootPath, result);
    }

    [Fact]
    public void IPathService_Interface_IsImplementedCorrectly()
    {
        // Arrange
        var webRootPath = "/var/www/html/wwwroot";
        var contentRootPath = "/var/www/html";
        IPathService service = new PathService(webRootPath, contentRootPath);

        // Act & Assert
        Assert.IsAssignableFrom<IPathService>(service);
        Assert.Equal(webRootPath, service.GetWebRootPath());
        Assert.Equal(contentRootPath, service.GetContentRootPath());
    }
}
