using AutoMapper;
using Xunit;
using WebGallery.Application.DTOs;
using WebGallery.Application.Mappings;
using WebGallery.Domain.Entities;

namespace Tests.WebGallery.Application;

public class MappingProfileTests
{
    private readonly IMapper _mapper;

    public MappingProfileTests()
    {
        var config = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>());
        _mapper = config.CreateMapper();
    }

    [Fact]
    public void MappingProfile_IsValid()
    {
        // Arrange
        var config = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>());

        // Act & Assert
        config.AssertConfigurationIsValid();
    }

    [Fact]
    public void Map_PictureToPictureDto_MapsAllProperties()
    {
        // Arrange
        var galleryType = new GalleryType { Id = 1, Name = "Cakes" };
        var picture = new Picture
        {
            Id = 1,
            Name = "Test Picture",
            Description = "Test Description",
            ImagePath = "/images/test.jpg",
            ThumbnailImagePath = "/images/thumb.jpg",
            GalleryTypeId = 1,
            CreatedDate = DateTime.Now,
            ModifiedDate = DateTime.Now.AddDays(1),
            GalleryType = galleryType
        };

        // Act
        var dto = _mapper.Map<PictureDto>(picture);

        // Assert
        Assert.Equal(picture.Id, dto.Id);
        Assert.Equal(picture.Name, dto.Name);
        Assert.Equal(picture.Description, dto.Description);
        Assert.Equal(picture.ImagePath, dto.ImagePath);
        Assert.Equal(picture.ThumbnailImagePath, dto.ThumbnailImagePath);
        Assert.Equal(picture.GalleryTypeId, dto.GalleryTypeId);
        Assert.Equal(picture.CreatedDate, dto.CreatedDate);
        Assert.Equal(picture.ModifiedDate, dto.ModifiedDate);
        Assert.Equal("Cakes", dto.GalleryTypeName);
    }

    [Fact]
    public void Map_PictureToPictureDto_WithNullGalleryType_MapsToEmptyString()
    {
        // Arrange
        var picture = new Picture
        {
            Id = 1,
            Name = "Test Picture",
            GalleryType = null
        };

        // Act
        var dto = _mapper.Map<PictureDto>(picture);

        // Assert
        Assert.Equal(string.Empty, dto.GalleryTypeName);
    }

    [Fact]
    public void Map_PictureCreateDtoToPicture_MapsAllProperties()
    {
        // Arrange
        var createDto = new PictureCreateDto
        {
            Name = "New Picture",
            Description = "New Description",
            ImagePath = "/images/new.jpg",
            ThumbnailImagePath = "/images/new_thumb.jpg",
            GalleryTypeId = 2
        };

        // Act
        var picture = _mapper.Map<Picture>(createDto);

        // Assert
        Assert.Equal(createDto.Name, picture.Name);
        Assert.Equal(createDto.Description, picture.Description);
        Assert.Equal(createDto.ImagePath, picture.ImagePath);
        Assert.Equal(createDto.ThumbnailImagePath, picture.ThumbnailImagePath);
        Assert.Equal(createDto.GalleryTypeId, picture.GalleryTypeId);
        Assert.True(picture.IsActive);
        Assert.Equal("System", picture.CreatedBy);
    }

    [Fact]
    public void Map_PictureUpdateDtoToPicture_MapsAllProperties()
    {
        // Arrange
        var updateDto = new PictureUpdateDto
        {
            Name = "Updated Picture",
            Description = "Updated Description",
            ImagePath = "/images/updated.jpg",
            ThumbnailImagePath = "/images/updated_thumb.jpg",
            GalleryTypeId = 3
        };

        // Act
        var picture = _mapper.Map<Picture>(updateDto);

        // Assert
        Assert.Equal(updateDto.Name, picture.Name);
        Assert.Equal(updateDto.Description, picture.Description);
        Assert.Equal(updateDto.ImagePath, picture.ImagePath);
        Assert.Equal(updateDto.ThumbnailImagePath, picture.ThumbnailImagePath);
        Assert.Equal(updateDto.GalleryTypeId, picture.GalleryTypeId);
        Assert.Equal("System", picture.ModifiedBy);
    }

    [Fact]
    public void Map_GalleryTypeToGalleryTypeDto_MapsAllProperties()
    {
        // Arrange
        var galleryType = new GalleryType
        {
            Id = 1,
            Name = "Cakes",
            Description = "Gallery for cakes",
            CreatedDate = DateTime.Now,
            ModifiedDate = DateTime.Now.AddDays(1),
            Pictures = new List<Picture>
            {
                new Picture { Id = 1 },
                new Picture { Id = 2 },
                new Picture { Id = 3 }
            }
        };

        // Act
        var dto = _mapper.Map<GalleryTypeDto>(galleryType);

        // Assert
        Assert.Equal(galleryType.Id, dto.Id);
        Assert.Equal(galleryType.Name, dto.Name);
        Assert.Equal(galleryType.Description, dto.Description);
        Assert.Equal(galleryType.CreatedDate, dto.CreatedDate);
        Assert.Equal(galleryType.ModifiedDate, dto.ModifiedDate);
        Assert.Equal(3, dto.PictureCount);
    }

    [Fact]
    public void Map_GalleryTypeToGalleryTypeDto_WithNullPictures_MapsToZeroCount()
    {
        // Arrange
        var galleryType = new GalleryType
        {
            Id = 1,
            Name = "Books",
            Pictures = null
        };

        // Act
        var dto = _mapper.Map<GalleryTypeDto>(galleryType);

        // Assert
        Assert.Equal(0, dto.PictureCount);
    }

    [Fact]
    public void Map_GalleryTypeCreateDtoToGalleryType_MapsAllProperties()
    {
        // Arrange
        var createDto = new GalleryTypeCreateDto
        {
            Name = "New Gallery",
            Description = "New gallery description"
        };

        // Act
        var galleryType = _mapper.Map<GalleryType>(createDto);

        // Assert
        Assert.Equal(createDto.Name, galleryType.Name);
        Assert.Equal(createDto.Description, galleryType.Description);
        Assert.True(galleryType.IsActive);
        Assert.Equal("System", galleryType.CreatedBy);
    }

    [Fact]
    public void Map_GalleryTypeUpdateDtoToGalleryType_MapsAllProperties()
    {
        // Arrange
        var updateDto = new GalleryTypeUpdateDto
        {
            Name = "Updated Gallery",
            Description = "Updated description"
        };

        // Act
        var galleryType = _mapper.Map<GalleryType>(updateDto);

        // Assert
        Assert.Equal(updateDto.Name, galleryType.Name);
        Assert.Equal(updateDto.Description, galleryType.Description);
        Assert.Equal("System", galleryType.ModifiedBy);
    }
}
