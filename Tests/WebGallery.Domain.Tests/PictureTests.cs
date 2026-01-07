using Xunit;
using WebGallery.Domain.Entities;

namespace Tests.WebGallery.Domain;

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
        Assert.Equal(string.Empty, picture.Description);
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
    public void Picture_SetId_SetsValueCorrectly()
    {
        // Arrange
        var picture = new Picture();

        // Act
        picture.Id = 1;

        // Assert
        Assert.Equal(1, picture.Id);
    }

    [Fact]
    public void Picture_SetName_SetsValueCorrectly()
    {
        // Arrange
        var picture = new Picture();

        // Act
        picture.Name = "Test Picture";

        // Assert
        Assert.Equal("Test Picture", picture.Name);
    }

    [Fact]
    public void Picture_SetDescription_SetsValueCorrectly()
    {
        // Arrange
        var picture = new Picture();

        // Act
        picture.Description = "Test Description";

        // Assert
        Assert.Equal("Test Description", picture.Description);
    }

    [Fact]
    public void Picture_SetImagePath_SetsValueCorrectly()
    {
        // Arrange
        var picture = new Picture();

        // Act
        picture.ImagePath = "/images/test.jpg";

        // Assert
        Assert.Equal("/images/test.jpg", picture.ImagePath);
    }

    [Fact]
    public void Picture_SetThumbnailImagePath_SetsValueCorrectly()
    {
        // Arrange
        var picture = new Picture();

        // Act
        picture.ThumbnailImagePath = "/images/thumb.jpg";

        // Assert
        Assert.Equal("/images/thumb.jpg", picture.ThumbnailImagePath);
    }

    [Fact]
    public void Picture_SetGalleryTypeId_SetsValueCorrectly()
    {
        // Arrange
        var picture = new Picture();

        // Act
        picture.GalleryTypeId = 5;

        // Assert
        Assert.Equal(5, picture.GalleryTypeId);
    }

    [Fact]
    public void Picture_SetCreatedDate_SetsValueCorrectly()
    {
        // Arrange
        var picture = new Picture();
        var date = DateTime.Now;

        // Act
        picture.CreatedDate = date;

        // Assert
        Assert.Equal(date, picture.CreatedDate);
    }

    [Fact]
    public void Picture_SetModifiedDate_SetsValueCorrectly()
    {
        // Arrange
        var picture = new Picture();
        var date = DateTime.Now;

        // Act
        picture.ModifiedDate = date;

        // Assert
        Assert.Equal(date, picture.ModifiedDate);
    }

    [Fact]
    public void Picture_SetModifiedDate_CanBeNull()
    {
        // Arrange
        var picture = new Picture();

        // Act
        picture.ModifiedDate = null;

        // Assert
        Assert.Null(picture.ModifiedDate);
    }

    [Fact]
    public void Picture_SetIsActive_SetsValueCorrectly()
    {
        // Arrange
        var picture = new Picture();

        // Act
        picture.IsActive = true;

        // Assert
        Assert.True(picture.IsActive);
    }

    [Fact]
    public void Picture_SetCreatedBy_SetsValueCorrectly()
    {
        // Arrange
        var picture = new Picture();

        // Act
        picture.CreatedBy = "admin";

        // Assert
        Assert.Equal("admin", picture.CreatedBy);
    }

    [Fact]
    public void Picture_SetModifiedBy_SetsValueCorrectly()
    {
        // Arrange
        var picture = new Picture();

        // Act
        picture.ModifiedBy = "user1";

        // Assert
        Assert.Equal("user1", picture.ModifiedBy);
    }

    [Fact]
    public void Picture_SetModifiedBy_CanBeNull()
    {
        // Arrange
        var picture = new Picture();

        // Act
        picture.ModifiedBy = null;

        // Assert
        Assert.Null(picture.ModifiedBy);
    }

    [Fact]
    public void Picture_SetGalleryType_SetsValueCorrectly()
    {
        // Arrange
        var picture = new Picture();
        var galleryType = new GalleryType { Id = 1, Name = "Cakes" };

        // Act
        picture.GalleryType = galleryType;

        // Assert
        Assert.NotNull(picture.GalleryType);
        Assert.Equal(1, picture.GalleryType.Id);
        Assert.Equal("Cakes", picture.GalleryType.Name);
    }

    [Fact]
    public void Picture_AllProperties_CanBeSetTogether()
    {
        // Arrange
        var picture = new Picture();
        var createdDate = DateTime.Now;
        var modifiedDate = DateTime.Now.AddDays(1);
        var galleryType = new GalleryType { Id = 1, Name = "Cakes" };

        // Act
        picture.Id = 10;
        picture.Name = "Birthday Cake";
        picture.Description = "Beautiful birthday cake";
        picture.ImagePath = "/images/cake.jpg";
        picture.ThumbnailImagePath = "/images/cake_thumb.jpg";
        picture.GalleryTypeId = 1;
        picture.CreatedDate = createdDate;
        picture.ModifiedDate = modifiedDate;
        picture.IsActive = true;
        picture.CreatedBy = "admin";
        picture.ModifiedBy = "editor";
        picture.GalleryType = galleryType;

        // Assert
        Assert.Equal(10, picture.Id);
        Assert.Equal("Birthday Cake", picture.Name);
        Assert.Equal("Beautiful birthday cake", picture.Description);
        Assert.Equal("/images/cake.jpg", picture.ImagePath);
        Assert.Equal("/images/cake_thumb.jpg", picture.ThumbnailImagePath);
        Assert.Equal(1, picture.GalleryTypeId);
        Assert.Equal(createdDate, picture.CreatedDate);
        Assert.Equal(modifiedDate, picture.ModifiedDate);
        Assert.True(picture.IsActive);
        Assert.Equal("admin", picture.CreatedBy);
        Assert.Equal("editor", picture.ModifiedBy);
        Assert.NotNull(picture.GalleryType);
    }
}
