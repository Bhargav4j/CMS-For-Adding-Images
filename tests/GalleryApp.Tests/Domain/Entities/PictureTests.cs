using Xunit;
using GalleryApp.Domain.Entities;

namespace GalleryApp.Tests.Domain.Entities;

public class PictureTests
{
    [Fact]
    public void Picture_Constructor_InitializesWithDefaultValues()
    {
        // Arrange & Act
        var picture = new Picture();

        // Assert
        Assert.Equal(0, picture.Id);
        Assert.Equal(string.Empty, picture.Name);
        Assert.Null(picture.Description);
        Assert.Equal(string.Empty, picture.ImagePath);
        Assert.Equal(string.Empty, picture.ThumbnailImagePath);
        Assert.Equal(0, picture.GalleryTypeId);
        Assert.Equal(default(DateTime), picture.CreatedDate);
        Assert.Null(picture.ModifiedDate);
        Assert.False(picture.IsActive);
        Assert.Equal(string.Empty, picture.CreatedBy);
        Assert.Null(picture.ModifiedBy);
        Assert.Null(picture.GalleryType);
    }

    [Fact]
    public void Picture_SetId_SetsIdCorrectly()
    {
        // Arrange
        var picture = new Picture();
        var expectedId = 123;

        // Act
        picture.Id = expectedId;

        // Assert
        Assert.Equal(expectedId, picture.Id);
    }

    [Fact]
    public void Picture_SetName_SetsNameCorrectly()
    {
        // Arrange
        var picture = new Picture();
        var expectedName = "Test Picture";

        // Act
        picture.Name = expectedName;

        // Assert
        Assert.Equal(expectedName, picture.Name);
    }

    [Fact]
    public void Picture_SetDescription_SetsDescriptionCorrectly()
    {
        // Arrange
        var picture = new Picture();
        var expectedDescription = "Test Description";

        // Act
        picture.Description = expectedDescription;

        // Assert
        Assert.Equal(expectedDescription, picture.Description);
    }

    [Fact]
    public void Picture_SetDescription_AllowsNull()
    {
        // Arrange
        var picture = new Picture { Description = "Initial" };

        // Act
        picture.Description = null;

        // Assert
        Assert.Null(picture.Description);
    }

    [Fact]
    public void Picture_SetImagePath_SetsImagePathCorrectly()
    {
        // Arrange
        var picture = new Picture();
        var expectedPath = "/images/test.jpg";

        // Act
        picture.ImagePath = expectedPath;

        // Assert
        Assert.Equal(expectedPath, picture.ImagePath);
    }

    [Fact]
    public void Picture_SetThumbnailImagePath_SetsThumbnailImagePathCorrectly()
    {
        // Arrange
        var picture = new Picture();
        var expectedPath = "/images/thumbnails/test.jpg";

        // Act
        picture.ThumbnailImagePath = expectedPath;

        // Assert
        Assert.Equal(expectedPath, picture.ThumbnailImagePath);
    }

    [Fact]
    public void Picture_SetGalleryTypeId_SetsGalleryTypeIdCorrectly()
    {
        // Arrange
        var picture = new Picture();
        var expectedId = 42;

        // Act
        picture.GalleryTypeId = expectedId;

        // Assert
        Assert.Equal(expectedId, picture.GalleryTypeId);
    }

    [Fact]
    public void Picture_SetCreatedDate_SetsCreatedDateCorrectly()
    {
        // Arrange
        var picture = new Picture();
        var expectedDate = new DateTime(2024, 1, 15, 10, 30, 0);

        // Act
        picture.CreatedDate = expectedDate;

        // Assert
        Assert.Equal(expectedDate, picture.CreatedDate);
    }

    [Fact]
    public void Picture_SetModifiedDate_SetsModifiedDateCorrectly()
    {
        // Arrange
        var picture = new Picture();
        var expectedDate = new DateTime(2024, 2, 20, 14, 45, 0);

        // Act
        picture.ModifiedDate = expectedDate;

        // Assert
        Assert.Equal(expectedDate, picture.ModifiedDate);
    }

    [Fact]
    public void Picture_SetModifiedDate_AllowsNull()
    {
        // Arrange
        var picture = new Picture { ModifiedDate = DateTime.Now };

        // Act
        picture.ModifiedDate = null;

        // Assert
        Assert.Null(picture.ModifiedDate);
    }

    [Fact]
    public void Picture_SetIsActive_SetsIsActiveCorrectly()
    {
        // Arrange
        var picture = new Picture();

        // Act
        picture.IsActive = true;

        // Assert
        Assert.True(picture.IsActive);
    }

    [Fact]
    public void Picture_SetCreatedBy_SetsCreatedByCorrectly()
    {
        // Arrange
        var picture = new Picture();
        var expectedUser = "admin@example.com";

        // Act
        picture.CreatedBy = expectedUser;

        // Assert
        Assert.Equal(expectedUser, picture.CreatedBy);
    }

    [Fact]
    public void Picture_SetModifiedBy_SetsModifiedByCorrectly()
    {
        // Arrange
        var picture = new Picture();
        var expectedUser = "editor@example.com";

        // Act
        picture.ModifiedBy = expectedUser;

        // Assert
        Assert.Equal(expectedUser, picture.ModifiedBy);
    }

    [Fact]
    public void Picture_SetModifiedBy_AllowsNull()
    {
        // Arrange
        var picture = new Picture { ModifiedBy = "someone" };

        // Act
        picture.ModifiedBy = null;

        // Assert
        Assert.Null(picture.ModifiedBy);
    }

    [Fact]
    public void Picture_SetGalleryType_SetsGalleryTypeCorrectly()
    {
        // Arrange
        var picture = new Picture();
        var galleryType = new GalleryType { Id = 1, Name = "Nature" };

        // Act
        picture.GalleryType = galleryType;

        // Assert
        Assert.NotNull(picture.GalleryType);
        Assert.Equal(galleryType.Id, picture.GalleryType.Id);
        Assert.Equal(galleryType.Name, picture.GalleryType.Name);
    }

    [Fact]
    public void Picture_SetGalleryType_AllowsNull()
    {
        // Arrange
        var picture = new Picture { GalleryType = new GalleryType() };

        // Act
        picture.GalleryType = null;

        // Assert
        Assert.Null(picture.GalleryType);
    }

    [Fact]
    public void Picture_AllProperties_CanBeSetTogether()
    {
        // Arrange
        var createdDate = DateTime.Now.AddDays(-10);
        var modifiedDate = DateTime.Now;
        var galleryType = new GalleryType { Id = 5, Name = "Wildlife" };

        // Act
        var picture = new Picture
        {
            Id = 100,
            Name = "Lion in Savanna",
            Description = "A majestic lion",
            ImagePath = "/images/lion.jpg",
            ThumbnailImagePath = "/images/thumbnails/lion.jpg",
            GalleryTypeId = 5,
            CreatedDate = createdDate,
            ModifiedDate = modifiedDate,
            IsActive = true,
            CreatedBy = "photographer@example.com",
            ModifiedBy = "editor@example.com",
            GalleryType = galleryType
        };

        // Assert
        Assert.Equal(100, picture.Id);
        Assert.Equal("Lion in Savanna", picture.Name);
        Assert.Equal("A majestic lion", picture.Description);
        Assert.Equal("/images/lion.jpg", picture.ImagePath);
        Assert.Equal("/images/thumbnails/lion.jpg", picture.ThumbnailImagePath);
        Assert.Equal(5, picture.GalleryTypeId);
        Assert.Equal(createdDate, picture.CreatedDate);
        Assert.Equal(modifiedDate, picture.ModifiedDate);
        Assert.True(picture.IsActive);
        Assert.Equal("photographer@example.com", picture.CreatedBy);
        Assert.Equal("editor@example.com", picture.ModifiedBy);
        Assert.NotNull(picture.GalleryType);
        Assert.Equal(galleryType.Id, picture.GalleryType.Id);
    }
}
