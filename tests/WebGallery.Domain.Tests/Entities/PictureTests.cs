using Xunit;
using WebGallery.Domain.Entities;

namespace WebGallery.Domain.Tests.Entities;

public class PictureTests
{
    [Fact]
    public void Picture_Constructor_ShouldSetDefaultValues()
    {
        // Arrange & Act
        var picture = new Picture();

        // Assert
        Assert.Equal(0, picture.Id);
        Assert.Equal(string.Empty, picture.Name);
        Assert.Null(picture.Description);
        Assert.Equal(string.Empty, picture.ImagePath);
        Assert.Null(picture.ThumbnailImagePath);
        Assert.Equal(0, picture.GalleryTypeId);
        Assert.True(picture.CreatedDate <= DateTime.UtcNow);
        Assert.Null(picture.ModifiedDate);
        Assert.True(picture.IsActive);
        Assert.Equal(string.Empty, picture.CreatedBy);
        Assert.Null(picture.ModifiedBy);
        Assert.Null(picture.GalleryType);
    }

    [Fact]
    public void Picture_SetId_ShouldUpdateValue()
    {
        // Arrange
        var picture = new Picture();
        var expectedId = 42;

        // Act
        picture.Id = expectedId;

        // Assert
        Assert.Equal(expectedId, picture.Id);
    }

    [Theory]
    [InlineData("Test Picture")]
    [InlineData("")]
    [InlineData("A very long picture name with special characters !@#$%")]
    public void Picture_SetName_ShouldUpdateValue(string name)
    {
        // Arrange
        var picture = new Picture();

        // Act
        picture.Name = name;

        // Assert
        Assert.Equal(name, picture.Name);
    }

    [Theory]
    [InlineData("Test Description")]
    [InlineData(null)]
    [InlineData("")]
    public void Picture_SetDescription_ShouldUpdateValue(string? description)
    {
        // Arrange
        var picture = new Picture();

        // Act
        picture.Description = description;

        // Assert
        Assert.Equal(description, picture.Description);
    }

    [Theory]
    [InlineData("/images/pic1.jpg")]
    [InlineData("C:\\Images\\picture.png")]
    [InlineData("")]
    public void Picture_SetImagePath_ShouldUpdateValue(string imagePath)
    {
        // Arrange
        var picture = new Picture();

        // Act
        picture.ImagePath = imagePath;

        // Assert
        Assert.Equal(imagePath, picture.ImagePath);
    }

    [Theory]
    [InlineData("/thumbnails/thumb1.jpg")]
    [InlineData(null)]
    [InlineData("")]
    public void Picture_SetThumbnailImagePath_ShouldUpdateValue(string? thumbnailPath)
    {
        // Arrange
        var picture = new Picture();

        // Act
        picture.ThumbnailImagePath = thumbnailPath;

        // Assert
        Assert.Equal(thumbnailPath, picture.ThumbnailImagePath);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(100)]
    [InlineData(-1)]
    [InlineData(0)]
    public void Picture_SetGalleryTypeId_ShouldUpdateValue(int galleryTypeId)
    {
        // Arrange
        var picture = new Picture();

        // Act
        picture.GalleryTypeId = galleryTypeId;

        // Assert
        Assert.Equal(galleryTypeId, picture.GalleryTypeId);
    }

    [Fact]
    public void Picture_SetCreatedDate_ShouldUpdateValue()
    {
        // Arrange
        var picture = new Picture();
        var expectedDate = new DateTime(2024, 1, 1, 12, 0, 0, DateTimeKind.Utc);

        // Act
        picture.CreatedDate = expectedDate;

        // Assert
        Assert.Equal(expectedDate, picture.CreatedDate);
    }

    [Fact]
    public void Picture_SetModifiedDate_ShouldUpdateValue()
    {
        // Arrange
        var picture = new Picture();
        var expectedDate = new DateTime(2024, 6, 1, 12, 0, 0, DateTimeKind.Utc);

        // Act
        picture.ModifiedDate = expectedDate;

        // Assert
        Assert.Equal(expectedDate, picture.ModifiedDate);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void Picture_SetIsActive_ShouldUpdateValue(bool isActive)
    {
        // Arrange
        var picture = new Picture();

        // Act
        picture.IsActive = isActive;

        // Assert
        Assert.Equal(isActive, picture.IsActive);
    }

    [Theory]
    [InlineData("admin")]
    [InlineData("user@test.com")]
    [InlineData("")]
    public void Picture_SetCreatedBy_ShouldUpdateValue(string createdBy)
    {
        // Arrange
        var picture = new Picture();

        // Act
        picture.CreatedBy = createdBy;

        // Assert
        Assert.Equal(createdBy, picture.CreatedBy);
    }

    [Theory]
    [InlineData("admin")]
    [InlineData(null)]
    [InlineData("")]
    public void Picture_SetModifiedBy_ShouldUpdateValue(string? modifiedBy)
    {
        // Arrange
        var picture = new Picture();

        // Act
        picture.ModifiedBy = modifiedBy;

        // Assert
        Assert.Equal(modifiedBy, picture.ModifiedBy);
    }

    [Fact]
    public void Picture_SetGalleryType_ShouldUpdateNavigationProperty()
    {
        // Arrange
        var picture = new Picture();
        var galleryType = new GalleryType { Id = 1, Name = "Nature" };

        // Act
        picture.GalleryType = galleryType;

        // Assert
        Assert.NotNull(picture.GalleryType);
        Assert.Equal(1, picture.GalleryType.Id);
        Assert.Equal("Nature", picture.GalleryType.Name);
    }

    [Fact]
    public void Picture_CompleteObject_ShouldSetAllProperties()
    {
        // Arrange
        var galleryType = new GalleryType { Id = 5, Name = "Landscape" };
        var createdDate = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        var modifiedDate = new DateTime(2024, 6, 1, 0, 0, 0, DateTimeKind.Utc);

        // Act
        var picture = new Picture
        {
            Id = 10,
            Name = "Sunset",
            Description = "Beautiful sunset view",
            ImagePath = "/images/sunset.jpg",
            ThumbnailImagePath = "/thumbnails/sunset_thumb.jpg",
            GalleryTypeId = 5,
            CreatedDate = createdDate,
            ModifiedDate = modifiedDate,
            IsActive = true,
            CreatedBy = "admin",
            ModifiedBy = "editor",
            GalleryType = galleryType
        };

        // Assert
        Assert.Equal(10, picture.Id);
        Assert.Equal("Sunset", picture.Name);
        Assert.Equal("Beautiful sunset view", picture.Description);
        Assert.Equal("/images/sunset.jpg", picture.ImagePath);
        Assert.Equal("/thumbnails/sunset_thumb.jpg", picture.ThumbnailImagePath);
        Assert.Equal(5, picture.GalleryTypeId);
        Assert.Equal(createdDate, picture.CreatedDate);
        Assert.Equal(modifiedDate, picture.ModifiedDate);
        Assert.True(picture.IsActive);
        Assert.Equal("admin", picture.CreatedBy);
        Assert.Equal("editor", picture.ModifiedBy);
        Assert.NotNull(picture.GalleryType);
        Assert.Equal(5, picture.GalleryType.Id);
    }
}
