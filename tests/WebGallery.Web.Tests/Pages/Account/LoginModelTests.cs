using Xunit;
using Moq;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Http;
using WebGallery.Web.Pages.Account;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace WebGallery.Web.Tests.Pages.Account;

public class LoginModelTests
{
    private readonly Mock<SignInManager<IdentityUser>> _mockSignInManager;
    private readonly Mock<ILogger<LoginModel>> _mockLogger;
    private readonly LoginModel _pageModel;

    public LoginModelTests()
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

        _mockLogger = new Mock<ILogger<LoginModel>>();
        _pageModel = new LoginModel(_mockSignInManager.Object, _mockLogger.Object);
    }

    [Fact]
    public void Constructor_WithValidParameters_ShouldCreateInstance()
    {
        // Arrange & Act
        var pageModel = new LoginModel(_mockSignInManager.Object, _mockLogger.Object);

        // Assert
        Assert.NotNull(pageModel);
    }

    [Fact]
    public void InputModel_DefaultConstructor_ShouldSetDefaultValues()
    {
        // Arrange & Act
        var input = new LoginModel.InputModel();

        // Assert
        Assert.Equal(string.Empty, input.Username);
        Assert.Equal(string.Empty, input.Password);
    }

    [Fact]
    public void InputModel_SetUsername_ShouldUpdateValue()
    {
        // Arrange
        var input = new LoginModel.InputModel();

        // Act
        input.Username = "testuser";

        // Assert
        Assert.Equal("testuser", input.Username);
    }

    [Fact]
    public void InputModel_SetPassword_ShouldUpdateValue()
    {
        // Arrange
        var input = new LoginModel.InputModel();

        // Act
        input.Password = "password123";

        // Assert
        Assert.Equal("password123", input.Password);
    }

    [Fact]
    public void LoginModel_InputProperty_ShouldBeInitialized()
    {
        // Arrange & Act
        var pageModel = new LoginModel(_mockSignInManager.Object, _mockLogger.Object);

        // Assert
        Assert.NotNull(pageModel.Input);
    }

    [Fact]
    public void LoginModel_ReturnUrlProperty_ShouldBeNullableString()
    {
        // Arrange
        var pageModel = new LoginModel(_mockSignInManager.Object, _mockLogger.Object);

        // Act
        pageModel.ReturnUrl = "/test/path";

        // Assert
        Assert.Equal("/test/path", pageModel.ReturnUrl);
    }

    [Fact]
    public async Task OnGetAsync_WithReturnUrl_ShouldSetReturnUrl()
    {
        // Arrange
        var returnUrl = "/Pictures/Index";
        var httpContext = new DefaultHttpContext();
        _pageModel.PageContext = new PageContext { HttpContext = httpContext };

        // Act
        await _pageModel.OnGetAsync(returnUrl);

        // Assert
        Assert.Equal(returnUrl, _pageModel.ReturnUrl);
    }

    [Fact]
    public async Task OnGetAsync_WithNullReturnUrl_ShouldSetReturnUrlToNull()
    {
        // Arrange
        var httpContext = new DefaultHttpContext();
        _pageModel.PageContext = new PageContext { HttpContext = httpContext };

        // Act
        await _pageModel.OnGetAsync(null);

        // Assert
        Assert.Null(_pageModel.ReturnUrl);
    }

    [Fact]
    public async Task OnPostAsync_WithValidCredentials_ShouldRedirectToReturnUrl()
    {
        // Arrange
        var httpContext = new DefaultHttpContext();
        _pageModel.PageContext = new PageContext { HttpContext = httpContext };
        _pageModel.Url = GetUrlHelper();
        _pageModel.Input = new LoginModel.InputModel { Username = "admin", Password = "Admin@123" };

        _mockSignInManager.Setup(x => x.PasswordSignInAsync(
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<bool>(),
            It.IsAny<bool>()))
            .ReturnsAsync(Microsoft.AspNetCore.Identity.SignInResult.Success);

        // Act
        var result = await _pageModel.OnPostAsync("/Pictures");

        // Assert
        Assert.IsType<LocalRedirectResult>(result);
        var redirectResult = result as LocalRedirectResult;
        Assert.Equal("/Pictures", redirectResult?.Url);
    }

    [Fact]
    public async Task OnPostAsync_WithInvalidCredentials_ShouldReturnPageWithError()
    {
        // Arrange
        var httpContext = new DefaultHttpContext();
        _pageModel.PageContext = new PageContext { HttpContext = httpContext };
        _pageModel.Input = new LoginModel.InputModel { Username = "wronguser", Password = "wrongpass" };

        _mockSignInManager.Setup(x => x.PasswordSignInAsync(
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<bool>(),
            It.IsAny<bool>()))
            .ReturnsAsync(Microsoft.AspNetCore.Identity.SignInResult.Failed);

        // Act
        var result = await _pageModel.OnPostAsync();

        // Assert
        Assert.IsType<PageResult>(result);
        Assert.False(_pageModel.ModelState.IsValid);
    }

    [Fact]
    public async Task OnPostAsync_WithInvalidModelState_ShouldReturnPage()
    {
        // Arrange
        var httpContext = new DefaultHttpContext();
        _pageModel.PageContext = new PageContext { HttpContext = httpContext };
        _pageModel.ModelState.AddModelError("Username", "Required");

        // Act
        var result = await _pageModel.OnPostAsync();

        // Assert
        Assert.IsType<PageResult>(result);
    }

    [Fact]
    public async Task OnPostAsync_WithNullReturnUrl_ShouldRedirectToHome()
    {
        // Arrange
        var httpContext = new DefaultHttpContext();
        _pageModel.PageContext = new PageContext { HttpContext = httpContext };
        _pageModel.Url = GetUrlHelper();
        _pageModel.Input = new LoginModel.InputModel { Username = "admin", Password = "Admin@123" };

        _mockSignInManager.Setup(x => x.PasswordSignInAsync(
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<bool>(),
            It.IsAny<bool>()))
            .ReturnsAsync(Microsoft.AspNetCore.Identity.SignInResult.Success);

        // Act
        var result = await _pageModel.OnPostAsync(null);

        // Assert
        Assert.IsType<LocalRedirectResult>(result);
        var redirectResult = result as LocalRedirectResult;
        Assert.Equal("~/", redirectResult?.Url);
    }

    [Fact]
    public async Task OnPostAsync_OnSuccessfulLogin_ShouldLogInformation()
    {
        // Arrange
        var httpContext = new DefaultHttpContext();
        _pageModel.PageContext = new PageContext { HttpContext = httpContext };
        _pageModel.Url = GetUrlHelper();
        _pageModel.Input = new LoginModel.InputModel { Username = "admin", Password = "Admin@123" };

        _mockSignInManager.Setup(x => x.PasswordSignInAsync(
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<bool>(),
            It.IsAny<bool>()))
            .ReturnsAsync(Microsoft.AspNetCore.Identity.SignInResult.Success);

        // Act
        await _pageModel.OnPostAsync();

        // Assert
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("User logged in")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public void LoginModel_ShouldInheritFromPageModel()
    {
        // Arrange & Act
        var pageModel = new LoginModel(_mockSignInManager.Object, _mockLogger.Object);

        // Assert
        Assert.IsAssignableFrom<PageModel>(pageModel);
    }

    private IUrlHelper GetUrlHelper()
    {
        var mockUrlHelper = new Mock<IUrlHelper>();
        mockUrlHelper.Setup(u => u.Content(It.IsAny<string>()))
            .Returns((string url) => url);
        return mockUrlHelper.Object;
    }
}
