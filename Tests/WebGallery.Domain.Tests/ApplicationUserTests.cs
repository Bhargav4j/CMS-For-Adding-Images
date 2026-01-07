using Xunit;
using WebGallery.Domain.Entities;

namespace Tests.WebGallery.Domain;

public class ApplicationUserTests
{
    [Fact]
    public void ApplicationUser_Constructor_InitializesWithDefaultValues()
    {
        // Arrange & Act
        var user = new ApplicationUser();

        // Assert
        Assert.Equal(default(DateTime), user.CreatedDate);
        Assert.Null(user.ModifiedDate);
        Assert.False(user.IsActive);
    }

    [Fact]
    public void ApplicationUser_SetCreatedDate_SetsValueCorrectly()
    {
        // Arrange
        var user = new ApplicationUser();
        var date = DateTime.Now;

        // Act
        user.CreatedDate = date;

        // Assert
        Assert.Equal(date, user.CreatedDate);
    }

    [Fact]
    public void ApplicationUser_SetModifiedDate_SetsValueCorrectly()
    {
        // Arrange
        var user = new ApplicationUser();
        var date = DateTime.Now;

        // Act
        user.ModifiedDate = date;

        // Assert
        Assert.Equal(date, user.ModifiedDate);
    }

    [Fact]
    public void ApplicationUser_SetModifiedDate_CanBeNull()
    {
        // Arrange
        var user = new ApplicationUser();

        // Act
        user.ModifiedDate = null;

        // Assert
        Assert.Null(user.ModifiedDate);
    }

    [Fact]
    public void ApplicationUser_SetIsActive_SetsValueCorrectly()
    {
        // Arrange
        var user = new ApplicationUser();

        // Act
        user.IsActive = true;

        // Assert
        Assert.True(user.IsActive);
    }

    [Fact]
    public void ApplicationUser_InheritsFromIdentityUser()
    {
        // Arrange & Act
        var user = new ApplicationUser();

        // Assert
        Assert.NotNull(user.UserName);
        Assert.NotNull(user.Email);
    }

    [Fact]
    public void ApplicationUser_CanSetIdentityUserProperties()
    {
        // Arrange
        var user = new ApplicationUser();

        // Act
        user.UserName = "testuser";
        user.Email = "test@example.com";

        // Assert
        Assert.Equal("testuser", user.UserName);
        Assert.Equal("test@example.com", user.Email);
    }

    [Fact]
    public void ApplicationUser_AllProperties_CanBeSetTogether()
    {
        // Arrange
        var user = new ApplicationUser();
        var createdDate = DateTime.Now;
        var modifiedDate = DateTime.Now.AddDays(1);

        // Act
        user.UserName = "admin";
        user.Email = "admin@example.com";
        user.CreatedDate = createdDate;
        user.ModifiedDate = modifiedDate;
        user.IsActive = true;

        // Assert
        Assert.Equal("admin", user.UserName);
        Assert.Equal("admin@example.com", user.Email);
        Assert.Equal(createdDate, user.CreatedDate);
        Assert.Equal(modifiedDate, user.ModifiedDate);
        Assert.True(user.IsActive);
    }
}
