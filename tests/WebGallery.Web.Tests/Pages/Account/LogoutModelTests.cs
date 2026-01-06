using Xunit;
using Moq;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Http;
using WebGallery.Web.Pages.Account;

namespace WebGallery.Web.Tests.Pages.Account;

public class LogoutModelTests
{
    private readonly Mock<SignInManager<IdentityUser>> _mockSignInManager;
    private readonly Mock<ILogger<LogoutModel>> _mockLogger;
    private readonly LogoutModel _pageModel;

    public LogoutModelTests()
    {
        var mockUserStore = new Mock<IUserStore<IdentityUser>>();
        var mockUserManager = new Mock<UserManager<IdentityUser>>(
            mockUserStore.Object, null, null, null, null, null, null, null, null);

        var mockContextAccessor = new Mock<IHttpContextAccessor>();
        var mockUserPrincipalFactory = new Mock<IUserClaimsPrincipalFactory<IdentityUser>>();

        _mockSignInManager = new Mock<SignInManager<IdentityUser>>(
            mockUserManager.Object,
            mockContextAccessor.Object,
            mockUserPrincipalFactory.Object,
            null, null, null, null);

        _mockLogger = new Mock<ILogger<LogoutModel>>();
        _pageModel = new LogoutModel(_mockSignInManager.Object, _mockLogger.Object);
    }

    [Fact]
    public void Constructor_WithValidParameters_ShouldCreateInstance()
    {
        // Arrange & Act
        var pageModel = new LogoutModel(_mockSignInManager.Object, _mockLogger.Object);

        // Assert
        Assert.NotNull(pageModel);
    }

    [Fact]
    public async Task OnPost_ShouldCallSignOutAsync()
    {
        // Arrange
        var httpContext = new DefaultHttpContext();
        _pageModel.PageContext = new PageContext { HttpContext = httpContext };
        _pageModel.Url = GetUrlHelper();
        _mockSignInManager.Setup(x => x.SignOutAsync()).Returns(Task.CompletedTask);

        // Act
        await _pageModel.OnPost();

        // Assert
        _mockSignInManager.Verify(x => x.SignOutAsync(), Times.Once);
    }

    [Fact]
    public async Task OnPost_WithNullReturnUrl_ShouldRedirectToIndex()
    {
        // Arrange
        var httpContext = new DefaultHttpContext();
        _pageModel.PageContext = new PageContext { HttpContext = httpContext };
        _pageModel.Url = GetUrlHelper();
        _mockSignInManager.Setup(x => x.SignOutAsync()).Returns(Task.CompletedTask);

        // Act
        var result = await _pageModel.OnPost(null);

        // Assert
        Assert.IsType<RedirectToPageResult>(result);
        var redirectResult = result as RedirectToPageResult;
        Assert.Equal("/Index", redirectResult?.PageName);
    }

    [Fact]
    public async Task OnPost_WithReturnUrl_ShouldRedirectToReturnUrl()
    {
        // Arrange
        var returnUrl = "/Pictures/Index";
        var httpContext = new DefaultHttpContext();
        _pageModel.PageContext = new PageContext { HttpContext = httpContext };
        _pageModel.Url = GetUrlHelper();
        _mockSignInManager.Setup(x => x.SignOutAsync()).Returns(Task.CompletedTask);

        // Act
        var result = await _pageModel.OnPost(returnUrl);

        // Assert
        Assert.IsType<LocalRedirectResult>(result);
        var redirectResult = result as LocalRedirectResult;
        Assert.Equal(returnUrl, redirectResult?.Url);
    }

    [Fact]
    public async Task OnPost_ShouldLogInformation()
    {
        // Arrange
        var httpContext = new DefaultHttpContext();
        _pageModel.PageContext = new PageContext { HttpContext = httpContext };
        _pageModel.Url = GetUrlHelper();
        _mockSignInManager.Setup(x => x.SignOutAsync()).Returns(Task.CompletedTask);

        // Act
        await _pageModel.OnPost();

        // Assert
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("User logged out")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public void LogoutModel_ShouldInheritFromPageModel()
    {
        // Arrange & Act
        var pageModel = new LogoutModel(_mockSignInManager.Object, _mockLogger.Object);

        // Assert
        Assert.IsAssignableFrom<PageModel>(pageModel);
    }

    [Fact]
    public async Task OnPost_WithEmptyReturnUrl_ShouldRedirectToIndex()
    {
        // Arrange
        var httpContext = new DefaultHttpContext();
        _pageModel.PageContext = new PageContext { HttpContext = httpContext };
        _pageModel.Url = GetUrlHelper();
        _mockSignInManager.Setup(x => x.SignOutAsync()).Returns(Task.CompletedTask);

        // Act
        var result = await _pageModel.OnPost("");

        // Assert
        Assert.IsType<LocalRedirectResult>(result);
    }

    [Fact]
    public async Task OnPost_CalledMultipleTimes_ShouldSignOutEachTime()
    {
        // Arrange
        var httpContext = new DefaultHttpContext();
        _pageModel.PageContext = new PageContext { HttpContext = httpContext };
        _pageModel.Url = GetUrlHelper();
        _mockSignInManager.Setup(x => x.SignOutAsync()).Returns(Task.CompletedTask);

        // Act
        await _pageModel.OnPost();
        await _pageModel.OnPost();
        await _pageModel.OnPost();

        // Assert
        _mockSignInManager.Verify(x => x.SignOutAsync(), Times.Exactly(3));
    }

    private IUrlHelper GetUrlHelper()
    {
        var mockUrlHelper = new Mock<IUrlHelper>();
        mockUrlHelper.Setup(u => u.Content(It.IsAny<string>()))
            .Returns((string url) => url);
        return mockUrlHelper.Object;
    }
}
