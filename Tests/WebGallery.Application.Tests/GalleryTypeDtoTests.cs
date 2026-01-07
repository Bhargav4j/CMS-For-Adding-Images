using Xunit;
using WebGallery.Application.DTOs;

namespace Tests.WebGallery.Application;

public class GalleryTypeDtoTests
{
    [Fact]
    public void GalleryTypeDto_Constructor_InitializesWithDefaultValues()
    {
        // Arrange & Act
        var dto = new GalleryTypeDto();

        // Assert
        Assert.Equal(0, dto.Id);
        Assert.Equal(string.Empty, dto.Name);
        Assert.Null(dto.Description);
        Assert.Equal(default(DateTime), dto.CreatedDate);
        Assert.Null(dto.ModifiedDate);
        Assert.Equal(0, dto.PictureCount);
    }

    [Fact]
    public void GalleryTypeDto_SetAllProperties_SetsValuesCorrectly()
    {
        // Arrange
        var dto = new GalleryTypeDto();
        var createdDate = DateTime.Now;
        var modifiedDate = DateTime.Now.AddDays(1);

        // Act
        dto.Id = 1;
        dto.Name = "Cakes";
        dto.Description = "Gallery for cakes";
        dto.CreatedDate = createdDate;
        dto.ModifiedDate = modifiedDate;
        dto.PictureCount = 10;

        // Assert
        Assert.Equal(1, dto.Id);
        Assert.Equal("Cakes", dto.Name);
        Assert.Equal("Gallery for cakes", dto.Description);
        Assert.Equal(createdDate, dto.CreatedDate);
        Assert.Equal(modifiedDate, dto.ModifiedDate);
        Assert.Equal(10, dto.PictureCount);
    }

    [Fact]
    public void GalleryTypeDto_Description_CanBeNull()
    {
        // Arrange
        var dto = new GalleryTypeDto();

        // Act
        dto.Description = null;

        // Assert
        Assert.Null(dto.Description);
    }

    [Fact]
    public void GalleryTypeDto_ModifiedDate_CanBeNull()
    {
        // Arrange
        var dto = new GalleryTypeDto();

        // Act
        dto.ModifiedDate = null;

        // Assert
        Assert.Null(dto.ModifiedDate);
    }

    [Fact]
    public void GalleryTypeDto_PictureCount_CanBeZero()
    {
        // Arrange
        var dto = new GalleryTypeDto();

        // Act
        dto.PictureCount = 0;

        // Assert
        Assert.Equal(0, dto.PictureCount);
    }

    [Fact]
    public void GalleryTypeDto_PictureCount_CanBePositive()
    {
        // Arrange
        var dto = new GalleryTypeDto();

        // Act
        dto.PictureCount = 100;

        // Assert
        Assert.Equal(100, dto.PictureCount);
    }
}

public class GalleryTypeCreateDtoTests
{
    [Fact]
    public void GalleryTypeCreateDto_Constructor_InitializesWithDefaultValues()
    {
        // Arrange & Act
        var dto = new GalleryTypeCreateDto();

        // Assert
        Assert.Equal(string.Empty, dto.Name);
        Assert.Null(dto.Description);
    }

    [Fact]
    public void GalleryTypeCreateDto_SetName_SetsValueCorrectly()
    {
        // Arrange
        var dto = new GalleryTypeCreateDto();

        // Act
        dto.Name = "Books";

        // Assert
        Assert.Equal("Books", dto.Name);
    }

    [Fact]
    public void GalleryTypeCreateDto_SetDescription_SetsValueCorrectly()
    {
        // Arrange
        var dto = new GalleryTypeCreateDto();

        // Act
        dto.Description = "Gallery for books";

        // Assert
        Assert.Equal("Gallery for books", dto.Description);
    }

    [Fact]
    public void GalleryTypeCreateDto_Description_CanBeNull()
    {
        // Arrange
        var dto = new GalleryTypeCreateDto();

        // Act
        dto.Description = null;

        // Assert
        Assert.Null(dto.Description);
    }

    [Fact]
    public void GalleryTypeCreateDto_WithEmptyName_AcceptsEmptyString()
    {
        // Arrange
        var dto = new GalleryTypeCreateDto();

        // Act
        dto.Name = "";

        // Assert
        Assert.Equal(string.Empty, dto.Name);
    }
}

public class GalleryTypeUpdateDtoTests
{
    [Fact]
    public void GalleryTypeUpdateDto_Constructor_InitializesWithDefaultValues()
    {
        // Arrange & Act
        var dto = new GalleryTypeUpdateDto();

        // Assert
        Assert.Equal(string.Empty, dto.Name);
        Assert.Null(dto.Description);
    }

    [Fact]
    public void GalleryTypeUpdateDto_SetName_SetsValueCorrectly()
    {
        // Arrange
        var dto = new GalleryTypeUpdateDto();

        // Act
        dto.Name = "Updated Gallery";

        // Assert
        Assert.Equal("Updated Gallery", dto.Name);
    }

    [Fact]
    public void GalleryTypeUpdateDto_SetDescription_SetsValueCorrectly()
    {
        // Arrange
        var dto = new GalleryTypeUpdateDto();

        // Act
        dto.Description = "Updated description";

        // Assert
        Assert.Equal("Updated description", dto.Description);
    }

    [Fact]
    public void GalleryTypeUpdateDto_Description_CanBeNull()
    {
        // Arrange
        var dto = new GalleryTypeUpdateDto();

        // Act
        dto.Description = null;

        // Assert
        Assert.Null(dto.Description);
    }

    [Fact]
    public void GalleryTypeUpdateDto_WithEmptyName_AcceptsEmptyString()
    {
        // Arrange
        var dto = new GalleryTypeUpdateDto();

        // Act
        dto.Name = "";

        // Assert
        Assert.Equal(string.Empty, dto.Name);
    }
}
