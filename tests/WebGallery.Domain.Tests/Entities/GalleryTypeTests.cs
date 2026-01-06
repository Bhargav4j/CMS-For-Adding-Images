using Xunit;
using WebGallery.Domain.Entities;

namespace WebGallery.Domain.Tests.Entities;

public class GalleryTypeTests
{
    [Fact]
    public void GalleryType_Constructor_ShouldSetDefaultValues()
    {
        // Arrange & Act
        var galleryType = new GalleryType();

        // Assert
        Assert.Equal(0, galleryType.Id);
        Assert.Equal(string.Empty, galleryType.Name);
        Assert.Null(galleryType.Description);
        Assert.True(galleryType.CreatedDate <= DateTime.UtcNow);
        Assert.Null(galleryType.ModifiedDate);
        Assert.True(galleryType.IsActive);
        Assert.Equal(string.Empty, galleryType.CreatedBy);
        Assert.Null(galleryType.ModifiedBy);
        Assert.NotNull(galleryType.Pictures);
        Assert.Empty(galleryType.Pictures);
    }

    [Fact]
    public void GalleryType_SetId_ShouldUpdateValue()
    {
        // Arrange
        var galleryType = new GalleryType();
        var expectedId = 99;

        // Act
        galleryType.Id = expectedId;

        // Assert
        Assert.Equal(expectedId, galleryType.Id);
    }

    [Theory]
    [InlineData("Nature")]
    [InlineData("Abstract Art")]
    [InlineData("")]
    [InlineData("Gallery with special chars !@#")]
    public void GalleryType_SetName_ShouldUpdateValue(string name)
    {
        // Arrange
        var galleryType = new GalleryType();

        // Act
        galleryType.Name = name;

        // Assert
        Assert.Equal(name, galleryType.Name);
    }

    [Theory]
    [InlineData("Photography collection")]
    [InlineData(null)]
    [InlineData("")]
    public void GalleryType_SetDescription_ShouldUpdateValue(string? description)
    {
        // Arrange
        var galleryType = new GalleryType();

        // Act
        galleryType.Description = description;

        // Assert
        Assert.Equal(description, galleryType.Description);
    }

    [Fact]
    public void GalleryType_SetCreatedDate_ShouldUpdateValue()
    {
        // Arrange
        var galleryType = new GalleryType();
        var expectedDate = new DateTime(2023, 5, 15, 10, 30, 0, DateTimeKind.Utc);

        // Act
        galleryType.CreatedDate = expectedDate;

        // Assert
        Assert.Equal(expectedDate, galleryType.CreatedDate);
    }

    [Fact]
    public void GalleryType_SetModifiedDate_ShouldUpdateValue()
    {
        // Arrange
        var galleryType = new GalleryType();
        var expectedDate = new DateTime(2024, 8, 20, 14, 45, 0, DateTimeKind.Utc);

        // Act
        galleryType.ModifiedDate = expectedDate;

        // Assert
        Assert.Equal(expectedDate, galleryType.ModifiedDate);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void GalleryType_SetIsActive_ShouldUpdateValue(bool isActive)
    {
        // Arrange
        var galleryType = new GalleryType();

        // Act
        galleryType.IsActive = isActive;

        // Assert
        Assert.Equal(isActive, galleryType.IsActive);
    }

    [Theory]
    [InlineData("systemadmin")]
    [InlineData("user123")]
    [InlineData("")]
    public void GalleryType_SetCreatedBy_ShouldUpdateValue(string createdBy)
    {
        // Arrange
        var galleryType = new GalleryType();

        // Act
        galleryType.CreatedBy = createdBy;

        // Assert
        Assert.Equal(createdBy, galleryType.CreatedBy);
    }

    [Theory]
    [InlineData("editor")]
    [InlineData(null)]
    [InlineData("")]
    public void GalleryType_SetModifiedBy_ShouldUpdateValue(string? modifiedBy)
    {
        // Arrange
        var galleryType = new GalleryType();

        // Act
        galleryType.ModifiedBy = modifiedBy;

        // Assert
        Assert.Equal(modifiedBy, galleryType.ModifiedBy);
    }

    [Fact]
    public void GalleryType_AddPicture_ShouldAddToCollection()
    {
        // Arrange
        var galleryType = new GalleryType();
        var picture = new Picture { Id = 1, Name = "Test Picture" };

        // Act
        galleryType.Pictures.Add(picture);

        // Assert
        Assert.Single(galleryType.Pictures);
        Assert.Contains(picture, galleryType.Pictures);
    }

    [Fact]
    public void GalleryType_AddMultiplePictures_ShouldAddAllToCollection()
    {
        // Arrange
        var galleryType = new GalleryType();
        var picture1 = new Picture { Id = 1, Name = "Picture 1" };
        var picture2 = new Picture { Id = 2, Name = "Picture 2" };
        var picture3 = new Picture { Id = 3, Name = "Picture 3" };

        // Act
        galleryType.Pictures.Add(picture1);
        galleryType.Pictures.Add(picture2);
        galleryType.Pictures.Add(picture3);

        // Assert
        Assert.Equal(3, galleryType.Pictures.Count);
        Assert.Contains(picture1, galleryType.Pictures);
        Assert.Contains(picture2, galleryType.Pictures);
        Assert.Contains(picture3, galleryType.Pictures);
    }

    [Fact]
    public void GalleryType_RemovePicture_ShouldRemoveFromCollection()
    {
        // Arrange
        var galleryType = new GalleryType();
        var picture = new Picture { Id = 1, Name = "Test Picture" };
        galleryType.Pictures.Add(picture);

        // Act
        galleryType.Pictures.Remove(picture);

        // Assert
        Assert.Empty(galleryType.Pictures);
    }

    [Fact]
    public void GalleryType_CompleteObject_ShouldSetAllProperties()
    {
        // Arrange
        var createdDate = new DateTime(2023, 3, 10, 0, 0, 0, DateTimeKind.Utc);
        var modifiedDate = new DateTime(2024, 7, 15, 0, 0, 0, DateTimeKind.Utc);
        var pictures = new List<Picture>
        {
            new Picture { Id = 1, Name = "Pic1" },
            new Picture { Id = 2, Name = "Pic2" }
        };

        // Act
        var galleryType = new GalleryType
        {
            Id = 25,
            Name = "Wildlife",
            Description = "Wildlife photography collection",
            CreatedDate = createdDate,
            ModifiedDate = modifiedDate,
            IsActive = true,
            CreatedBy = "admin",
            ModifiedBy = "moderator",
            Pictures = pictures
        };

        // Assert
        Assert.Equal(25, galleryType.Id);
        Assert.Equal("Wildlife", galleryType.Name);
        Assert.Equal("Wildlife photography collection", galleryType.Description);
        Assert.Equal(createdDate, galleryType.CreatedDate);
        Assert.Equal(modifiedDate, galleryType.ModifiedDate);
        Assert.True(galleryType.IsActive);
        Assert.Equal("admin", galleryType.CreatedBy);
        Assert.Equal("moderator", galleryType.ModifiedBy);
        Assert.Equal(2, galleryType.Pictures.Count);
    }

    [Fact]
    public void GalleryType_ClearPictures_ShouldEmptyCollection()
    {
        // Arrange
        var galleryType = new GalleryType();
        galleryType.Pictures.Add(new Picture { Id = 1, Name = "Pic1" });
        galleryType.Pictures.Add(new Picture { Id = 2, Name = "Pic2" });

        // Act
        galleryType.Pictures.Clear();

        // Assert
        Assert.Empty(galleryType.Pictures);
    }
}
