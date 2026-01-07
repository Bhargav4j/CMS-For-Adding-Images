using Xunit;
using WebGallery.Application.DTOs;

namespace Tests.WebGallery.Application;

public class PictureDtoTests
{
    [Fact]
    public void PictureDto_Constructor_InitializesWithDefaultValues()
    {
        // Arrange & Act
        var dto = new PictureDto();

        // Assert
        Assert.Equal(0, dto.Id);
        Assert.Equal(string.Empty, dto.Name);
        Assert.Equal(string.Empty, dto.Description);
        Assert.Equal(string.Empty, dto.ImagePath);
        Assert.Equal(string.Empty, dto.ThumbnailImagePath);
        Assert.Equal(0, dto.GalleryTypeId);
        Assert.Equal(string.Empty, dto.GalleryTypeName);
        Assert.Equal(default(DateTime), dto.CreatedDate);
        Assert.Null(dto.ModifiedDate);
    }

    [Fact]
    public void PictureDto_SetAllProperties_SetsValuesCorrectly()
    {
        // Arrange
        var dto = new PictureDto();
        var createdDate = DateTime.Now;
        var modifiedDate = DateTime.Now.AddDays(1);

        // Act
        dto.Id = 1;
        dto.Name = "Test Picture";
        dto.Description = "Test Description";
        dto.ImagePath = "/images/test.jpg";
        dto.ThumbnailImagePath = "/images/thumb.jpg";
        dto.GalleryTypeId = 5;
        dto.GalleryTypeName = "Cakes";
        dto.CreatedDate = createdDate;
        dto.ModifiedDate = modifiedDate;

        // Assert
        Assert.Equal(1, dto.Id);
        Assert.Equal("Test Picture", dto.Name);
        Assert.Equal("Test Description", dto.Description);
        Assert.Equal("/images/test.jpg", dto.ImagePath);
        Assert.Equal("/images/thumb.jpg", dto.ThumbnailImagePath);
        Assert.Equal(5, dto.GalleryTypeId);
        Assert.Equal("Cakes", dto.GalleryTypeName);
        Assert.Equal(createdDate, dto.CreatedDate);
        Assert.Equal(modifiedDate, dto.ModifiedDate);
    }

    [Fact]
    public void PictureDto_ModifiedDate_CanBeNull()
    {
        // Arrange
        var dto = new PictureDto();

        // Act
        dto.ModifiedDate = null;

        // Assert
        Assert.Null(dto.ModifiedDate);
    }
}

public class PictureCreateDtoTests
{
    [Fact]
    public void PictureCreateDto_Constructor_InitializesWithDefaultValues()
    {
        // Arrange & Act
        var dto = new PictureCreateDto();

        // Assert
        Assert.Equal(string.Empty, dto.Name);
        Assert.Equal(string.Empty, dto.Description);
        Assert.Equal(string.Empty, dto.ImagePath);
        Assert.Equal(string.Empty, dto.ThumbnailImagePath);
        Assert.Equal(0, dto.GalleryTypeId);
    }

    [Fact]
    public void PictureCreateDto_SetAllProperties_SetsValuesCorrectly()
    {
        // Arrange
        var dto = new PictureCreateDto();

        // Act
        dto.Name = "New Picture";
        dto.Description = "New Description";
        dto.ImagePath = "/images/new.jpg";
        dto.ThumbnailImagePath = "/images/new_thumb.jpg";
        dto.GalleryTypeId = 3;

        // Assert
        Assert.Equal("New Picture", dto.Name);
        Assert.Equal("New Description", dto.Description);
        Assert.Equal("/images/new.jpg", dto.ImagePath);
        Assert.Equal("/images/new_thumb.jpg", dto.ThumbnailImagePath);
        Assert.Equal(3, dto.GalleryTypeId);
    }

    [Fact]
    public void PictureCreateDto_WithEmptyValues_AcceptsEmptyStrings()
    {
        // Arrange
        var dto = new PictureCreateDto();

        // Act
        dto.Name = "";
        dto.Description = "";
        dto.ImagePath = "";
        dto.ThumbnailImagePath = "";

        // Assert
        Assert.Equal(string.Empty, dto.Name);
        Assert.Equal(string.Empty, dto.Description);
        Assert.Equal(string.Empty, dto.ImagePath);
        Assert.Equal(string.Empty, dto.ThumbnailImagePath);
    }
}

public class PictureUpdateDtoTests
{
    [Fact]
    public void PictureUpdateDto_Constructor_InitializesWithDefaultValues()
    {
        // Arrange & Act
        var dto = new PictureUpdateDto();

        // Assert
        Assert.Equal(string.Empty, dto.Name);
        Assert.Equal(string.Empty, dto.Description);
        Assert.Equal(string.Empty, dto.ImagePath);
        Assert.Equal(string.Empty, dto.ThumbnailImagePath);
        Assert.Equal(0, dto.GalleryTypeId);
    }

    [Fact]
    public void PictureUpdateDto_SetAllProperties_SetsValuesCorrectly()
    {
        // Arrange
        var dto = new PictureUpdateDto();

        // Act
        dto.Name = "Updated Picture";
        dto.Description = "Updated Description";
        dto.ImagePath = "/images/updated.jpg";
        dto.ThumbnailImagePath = "/images/updated_thumb.jpg";
        dto.GalleryTypeId = 7;

        // Assert
        Assert.Equal("Updated Picture", dto.Name);
        Assert.Equal("Updated Description", dto.Description);
        Assert.Equal("/images/updated.jpg", dto.ImagePath);
        Assert.Equal("/images/updated_thumb.jpg", dto.ThumbnailImagePath);
        Assert.Equal(7, dto.GalleryTypeId);
    }

    [Fact]
    public void PictureUpdateDto_WithEmptyValues_AcceptsEmptyStrings()
    {
        // Arrange
        var dto = new PictureUpdateDto();

        // Act
        dto.Name = "";
        dto.Description = "";
        dto.ImagePath = "";
        dto.ThumbnailImagePath = "";

        // Assert
        Assert.Equal(string.Empty, dto.Name);
        Assert.Equal(string.Empty, dto.Description);
        Assert.Equal(string.Empty, dto.ImagePath);
        Assert.Equal(string.Empty, dto.ThumbnailImagePath);
    }
}
