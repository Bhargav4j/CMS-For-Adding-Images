using Xunit;
using GalleryApp.Domain.Entities;

namespace GalleryApp.Tests.Domain.Entities;

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
    public void GalleryType_SetId_SetsIdCorrectly()
    {
        // Arrange
        var galleryType = new GalleryType();
        var expectedId = 456;

        // Act
        galleryType.Id = expectedId;

        // Assert
        Assert.Equal(expectedId, galleryType.Id);
    }

    [Fact]
    public void GalleryType_SetName_SetsNameCorrectly()
    {
        // Arrange
        var galleryType = new GalleryType();
        var expectedName = "Wildlife Gallery";

        // Act
        galleryType.Name = expectedName;

        // Assert
        Assert.Equal(expectedName, galleryType.Name);
    }

    [Fact]
    public void GalleryType_SetDescription_SetsDescriptionCorrectly()
    {
        // Arrange
        var galleryType = new GalleryType();
        var expectedDescription = "Photos of wildlife from around the world";

        // Act
        galleryType.Description = expectedDescription;

        // Assert
        Assert.Equal(expectedDescription, galleryType.Description);
    }

    [Fact]
    public void GalleryType_SetDescription_AllowsNull()
    {
        // Arrange
        var galleryType = new GalleryType { Description = "Initial description" };

        // Act
        galleryType.Description = null;

        // Assert
        Assert.Null(galleryType.Description);
    }

    [Fact]
    public void GalleryType_SetCreatedDate_SetsCreatedDateCorrectly()
    {
        // Arrange
        var galleryType = new GalleryType();
        var expectedDate = new DateTime(2024, 3, 10, 8, 15, 0);

        // Act
        galleryType.CreatedDate = expectedDate;

        // Assert
        Assert.Equal(expectedDate, galleryType.CreatedDate);
    }

    [Fact]
    public void GalleryType_SetModifiedDate_SetsModifiedDateCorrectly()
    {
        // Arrange
        var galleryType = new GalleryType();
        var expectedDate = new DateTime(2024, 4, 20, 10, 30, 0);

        // Act
        galleryType.ModifiedDate = expectedDate;

        // Assert
        Assert.Equal(expectedDate, galleryType.ModifiedDate);
    }

    [Fact]
    public void GalleryType_SetModifiedDate_AllowsNull()
    {
        // Arrange
        var galleryType = new GalleryType { ModifiedDate = DateTime.Now };

        // Act
        galleryType.ModifiedDate = null;

        // Assert
        Assert.Null(galleryType.ModifiedDate);
    }

    [Fact]
    public void GalleryType_SetIsActive_SetsIsActiveCorrectly()
    {
        // Arrange
        var galleryType = new GalleryType();

        // Act
        galleryType.IsActive = true;

        // Assert
        Assert.True(galleryType.IsActive);
    }

    [Fact]
    public void GalleryType_SetCreatedBy_SetsCreatedByCorrectly()
    {
        // Arrange
        var galleryType = new GalleryType();
        var expectedUser = "curator@example.com";

        // Act
        galleryType.CreatedBy = expectedUser;

        // Assert
        Assert.Equal(expectedUser, galleryType.CreatedBy);
    }

    [Fact]
    public void GalleryType_SetModifiedBy_SetsModifiedByCorrectly()
    {
        // Arrange
        var galleryType = new GalleryType();
        var expectedUser = "admin@example.com";

        // Act
        galleryType.ModifiedBy = expectedUser;

        // Assert
        Assert.Equal(expectedUser, galleryType.ModifiedBy);
    }

    [Fact]
    public void GalleryType_SetModifiedBy_AllowsNull()
    {
        // Arrange
        var galleryType = new GalleryType { ModifiedBy = "someone" };

        // Act
        galleryType.ModifiedBy = null;

        // Assert
        Assert.Null(galleryType.ModifiedBy);
    }

    [Fact]
    public void GalleryType_Pictures_InitializesAsEmptyList()
    {
        // Arrange & Act
        var galleryType = new GalleryType();

        // Assert
        Assert.NotNull(galleryType.Pictures);
        Assert.Empty(galleryType.Pictures);
        Assert.IsAssignableFrom<ICollection<Picture>>(galleryType.Pictures);
    }

    [Fact]
    public void GalleryType_Pictures_CanAddPictures()
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
    public void GalleryType_Pictures_CanBeReplacedWithNewCollection()
    {
        // Arrange
        var galleryType = new GalleryType();
        var newPictures = new List<Picture>
        {
            new Picture { Id = 10, Name = "New Picture 1" },
            new Picture { Id = 20, Name = "New Picture 2" },
            new Picture { Id = 30, Name = "New Picture 3" }
        };

        // Act
        galleryType.Pictures = newPictures;

        // Assert
        Assert.Equal(3, galleryType.Pictures.Count);
        Assert.Equal(newPictures, galleryType.Pictures);
    }

    [Fact]
    public void GalleryType_AllProperties_CanBeSetTogether()
    {
        // Arrange
        var createdDate = DateTime.Now.AddDays(-30);
        var modifiedDate = DateTime.Now;
        var pictures = new List<Picture>
        {
            new Picture { Id = 1, Name = "Picture 1" },
            new Picture { Id = 2, Name = "Picture 2" }
        };

        // Act
        var galleryType = new GalleryType
        {
            Id = 999,
            Name = "Nature Photography",
            Description = "Beautiful nature scenes",
            CreatedDate = createdDate,
            ModifiedDate = modifiedDate,
            IsActive = true,
            CreatedBy = "photographer@example.com",
            ModifiedBy = "editor@example.com",
            Pictures = pictures
        };

        // Assert
        Assert.Equal(999, galleryType.Id);
        Assert.Equal("Nature Photography", galleryType.Name);
        Assert.Equal("Beautiful nature scenes", galleryType.Description);
        Assert.Equal(createdDate, galleryType.CreatedDate);
        Assert.Equal(modifiedDate, galleryType.ModifiedDate);
        Assert.True(galleryType.IsActive);
        Assert.Equal("photographer@example.com", galleryType.CreatedBy);
        Assert.Equal("editor@example.com", galleryType.ModifiedBy);
        Assert.Equal(2, galleryType.Pictures.Count);
    }
}
