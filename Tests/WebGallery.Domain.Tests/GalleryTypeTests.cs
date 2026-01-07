using Xunit;
using WebGallery.Domain.Entities;

namespace Tests.WebGallery.Domain;

public class GalleryTypeTests
{
    [Fact]
    public void GalleryType_Constructor_InitializesWithDefaultValues()
    {
        // Arrange & Act
        var galleryType = new GalleryType();

        // Assert
        Assert.Equal(0, galleryType.Id);
        Assert.Equal(string.Empty, galleryType.Name);
        Assert.Null(galleryType.Description);
        Assert.Equal(default(DateTime), galleryType.CreatedDate);
        Assert.Null(galleryType.ModifiedDate);
        Assert.False(galleryType.IsActive);
        Assert.Equal(string.Empty, galleryType.CreatedBy);
        Assert.Null(galleryType.ModifiedBy);
        Assert.NotNull(galleryType.Pictures);
        Assert.Empty(galleryType.Pictures);
    }

    [Fact]
    public void GalleryType_SetId_SetsValueCorrectly()
    {
        // Arrange
        var galleryType = new GalleryType();

        // Act
        galleryType.Id = 1;

        // Assert
        Assert.Equal(1, galleryType.Id);
    }

    [Fact]
    public void GalleryType_SetName_SetsValueCorrectly()
    {
        // Arrange
        var galleryType = new GalleryType();

        // Act
        galleryType.Name = "Cakes";

        // Assert
        Assert.Equal("Cakes", galleryType.Name);
    }

    [Fact]
    public void GalleryType_SetDescription_SetsValueCorrectly()
    {
        // Arrange
        var galleryType = new GalleryType();

        // Act
        galleryType.Description = "Gallery for cakes";

        // Assert
        Assert.Equal("Gallery for cakes", galleryType.Description);
    }

    [Fact]
    public void GalleryType_SetDescription_CanBeNull()
    {
        // Arrange
        var galleryType = new GalleryType();

        // Act
        galleryType.Description = null;

        // Assert
        Assert.Null(galleryType.Description);
    }

    [Fact]
    public void GalleryType_SetCreatedDate_SetsValueCorrectly()
    {
        // Arrange
        var galleryType = new GalleryType();
        var date = DateTime.Now;

        // Act
        galleryType.CreatedDate = date;

        // Assert
        Assert.Equal(date, galleryType.CreatedDate);
    }

    [Fact]
    public void GalleryType_SetModifiedDate_SetsValueCorrectly()
    {
        // Arrange
        var galleryType = new GalleryType();
        var date = DateTime.Now;

        // Act
        galleryType.ModifiedDate = date;

        // Assert
        Assert.Equal(date, galleryType.ModifiedDate);
    }

    [Fact]
    public void GalleryType_SetModifiedDate_CanBeNull()
    {
        // Arrange
        var galleryType = new GalleryType();

        // Act
        galleryType.ModifiedDate = null;

        // Assert
        Assert.Null(galleryType.ModifiedDate);
    }

    [Fact]
    public void GalleryType_SetIsActive_SetsValueCorrectly()
    {
        // Arrange
        var galleryType = new GalleryType();

        // Act
        galleryType.IsActive = true;

        // Assert
        Assert.True(galleryType.IsActive);
    }

    [Fact]
    public void GalleryType_SetCreatedBy_SetsValueCorrectly()
    {
        // Arrange
        var galleryType = new GalleryType();

        // Act
        galleryType.CreatedBy = "admin";

        // Assert
        Assert.Equal("admin", galleryType.CreatedBy);
    }

    [Fact]
    public void GalleryType_SetModifiedBy_SetsValueCorrectly()
    {
        // Arrange
        var galleryType = new GalleryType();

        // Act
        galleryType.ModifiedBy = "user1";

        // Assert
        Assert.Equal("user1", galleryType.ModifiedBy);
    }

    [Fact]
    public void GalleryType_SetModifiedBy_CanBeNull()
    {
        // Arrange
        var galleryType = new GalleryType();

        // Act
        galleryType.ModifiedBy = null;

        // Assert
        Assert.Null(galleryType.ModifiedBy);
    }

    [Fact]
    public void GalleryType_Pictures_CanAddPicture()
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
    public void GalleryType_Pictures_CanAddMultiplePictures()
    {
        // Arrange
        var galleryType = new GalleryType();
        var picture1 = new Picture { Id = 1, Name = "Picture 1" };
        var picture2 = new Picture { Id = 2, Name = "Picture 2" };

        // Act
        galleryType.Pictures.Add(picture1);
        galleryType.Pictures.Add(picture2);

        // Assert
        Assert.Equal(2, galleryType.Pictures.Count);
        Assert.Contains(picture1, galleryType.Pictures);
        Assert.Contains(picture2, galleryType.Pictures);
    }

    [Fact]
    public void GalleryType_AllProperties_CanBeSetTogether()
    {
        // Arrange
        var galleryType = new GalleryType();
        var createdDate = DateTime.Now;
        var modifiedDate = DateTime.Now.AddDays(1);
        var picture = new Picture { Id = 1, Name = "Test Picture" };

        // Act
        galleryType.Id = 10;
        galleryType.Name = "Cakes";
        galleryType.Description = "Beautiful cakes gallery";
        galleryType.CreatedDate = createdDate;
        galleryType.ModifiedDate = modifiedDate;
        galleryType.IsActive = true;
        galleryType.CreatedBy = "admin";
        galleryType.ModifiedBy = "editor";
        galleryType.Pictures.Add(picture);

        // Assert
        Assert.Equal(10, galleryType.Id);
        Assert.Equal("Cakes", galleryType.Name);
        Assert.Equal("Beautiful cakes gallery", galleryType.Description);
        Assert.Equal(createdDate, galleryType.CreatedDate);
        Assert.Equal(modifiedDate, galleryType.ModifiedDate);
        Assert.True(galleryType.IsActive);
        Assert.Equal("admin", galleryType.CreatedBy);
        Assert.Equal("editor", galleryType.ModifiedBy);
        Assert.Single(galleryType.Pictures);
    }
}
