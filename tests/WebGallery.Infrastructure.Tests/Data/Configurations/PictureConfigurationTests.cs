using Xunit;
using Microsoft.EntityFrameworkCore;
using WebGallery.Infrastructure.Data;
using WebGallery.Domain.Entities;

namespace WebGallery.Infrastructure.Tests.Data.Configurations;

public class PictureConfigurationTests
{
    private DbContextOptions<ApplicationDbContext> CreateNewContextOptions()
    {
        return new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
    }

    [Fact]
    public async Task PictureConfiguration_TableName_ShouldBePictures()
    {
        // Arrange
        var options = CreateNewContextOptions();
        using var context = new ApplicationDbContext(options);
        await context.Database.EnsureCreatedAsync();

        // Act
        var entityType = context.Model.FindEntityType(typeof(Picture));

        // Assert
        Assert.NotNull(entityType);
        Assert.Equal("Pictures", entityType.GetTableName());
    }

    [Fact]
    public async Task PictureConfiguration_IdProperty_ShouldBePrimaryKey()
    {
        // Arrange
        var options = CreateNewContextOptions();
        using var context = new ApplicationDbContext(options);
        await context.Database.EnsureCreatedAsync();

        // Act
        var entityType = context.Model.FindEntityType(typeof(Picture));
        var primaryKey = entityType?.FindPrimaryKey();

        // Assert
        Assert.NotNull(primaryKey);
        Assert.Single(primaryKey.Properties);
        Assert.Equal("Id", primaryKey.Properties.First().Name);
    }

    [Fact]
    public async Task PictureConfiguration_NameProperty_ShouldBeRequired()
    {
        // Arrange
        var options = CreateNewContextOptions();
        using var context = new ApplicationDbContext(options);
        await context.Database.EnsureCreatedAsync();

        // Act
        var entityType = context.Model.FindEntityType(typeof(Picture));
        var nameProperty = entityType?.FindProperty("Name");

        // Assert
        Assert.NotNull(nameProperty);
        Assert.False(nameProperty.IsNullable);
    }

    [Fact]
    public async Task PictureConfiguration_DescriptionProperty_ShouldBeOptional()
    {
        // Arrange
        var options = CreateNewContextOptions();
        using var context = new ApplicationDbContext(options);
        await context.Database.EnsureCreatedAsync();

        // Act
        var entityType = context.Model.FindEntityType(typeof(Picture));
        var descProperty = entityType?.FindProperty("Description");

        // Assert
        Assert.NotNull(descProperty);
        Assert.True(descProperty.IsNullable);
    }

    [Fact]
    public async Task PictureConfiguration_ImagePathProperty_ShouldBeRequired()
    {
        // Arrange
        var options = CreateNewContextOptions();
        using var context = new ApplicationDbContext(options);
        await context.Database.EnsureCreatedAsync();

        // Act
        var entityType = context.Model.FindEntityType(typeof(Picture));
        var imagePathProperty = entityType?.FindProperty("ImagePath");

        // Assert
        Assert.NotNull(imagePathProperty);
        Assert.False(imagePathProperty.IsNullable);
    }

    [Fact]
    public async Task PictureConfiguration_ThumbnailImagePathProperty_ShouldBeOptional()
    {
        // Arrange
        var options = CreateNewContextOptions();
        using var context = new ApplicationDbContext(options);
        await context.Database.EnsureCreatedAsync();

        // Act
        var entityType = context.Model.FindEntityType(typeof(Picture));
        var thumbnailProperty = entityType?.FindProperty("ThumbnailImagePath");

        // Assert
        Assert.NotNull(thumbnailProperty);
        Assert.True(thumbnailProperty.IsNullable);
    }

    [Fact]
    public async Task PictureConfiguration_CreatedDateProperty_ShouldBeRequired()
    {
        // Arrange
        var options = CreateNewContextOptions();
        using var context = new ApplicationDbContext(options);
        await context.Database.EnsureCreatedAsync();

        // Act
        var entityType = context.Model.FindEntityType(typeof(Picture));
        var createdDateProperty = entityType?.FindProperty("CreatedDate");

        // Assert
        Assert.NotNull(createdDateProperty);
        Assert.False(createdDateProperty.IsNullable);
    }

    [Fact]
    public async Task PictureConfiguration_IsActiveProperty_ShouldBeRequired()
    {
        // Arrange
        var options = CreateNewContextOptions();
        using var context = new ApplicationDbContext(options);
        await context.Database.EnsureCreatedAsync();

        // Act
        var entityType = context.Model.FindEntityType(typeof(Picture));
        var isActiveProperty = entityType?.FindProperty("IsActive");

        // Assert
        Assert.NotNull(isActiveProperty);
        Assert.False(isActiveProperty.IsNullable);
    }

    [Fact]
    public async Task PictureConfiguration_CreatedByProperty_ShouldBeRequired()
    {
        // Arrange
        var options = CreateNewContextOptions();
        using var context = new ApplicationDbContext(options);
        await context.Database.EnsureCreatedAsync();

        // Act
        var entityType = context.Model.FindEntityType(typeof(Picture));
        var createdByProperty = entityType?.FindProperty("CreatedBy");

        // Assert
        Assert.NotNull(createdByProperty);
        Assert.False(createdByProperty.IsNullable);
    }

    [Fact]
    public async Task PictureConfiguration_ModifiedByProperty_ShouldBeOptional()
    {
        // Arrange
        var options = CreateNewContextOptions();
        using var context = new ApplicationDbContext(options);
        await context.Database.EnsureCreatedAsync();

        // Act
        var entityType = context.Model.FindEntityType(typeof(Picture));
        var modifiedByProperty = entityType?.FindProperty("ModifiedBy");

        // Assert
        Assert.NotNull(modifiedByProperty);
        Assert.True(modifiedByProperty.IsNullable);
    }

    [Fact]
    public async Task PictureConfiguration_GalleryTypeRelationship_ShouldExist()
    {
        // Arrange
        var options = CreateNewContextOptions();
        using var context = new ApplicationDbContext(options);
        await context.Database.EnsureCreatedAsync();

        // Act
        var entityType = context.Model.FindEntityType(typeof(Picture));
        var navigation = entityType?.FindNavigation("GalleryType");

        // Assert
        Assert.NotNull(navigation);
        Assert.Equal(typeof(GalleryType), navigation.TargetEntityType.ClrType);
    }

    [Fact]
    public async Task PictureConfiguration_GalleryTypeIdIndex_ShouldExist()
    {
        // Arrange
        var options = CreateNewContextOptions();
        using var context = new ApplicationDbContext(options);
        await context.Database.EnsureCreatedAsync();

        // Act
        var entityType = context.Model.FindEntityType(typeof(Picture));
        var indexes = entityType?.GetIndexes();
        var galleryTypeIndex = indexes?.FirstOrDefault(i =>
            i.Properties.Any(p => p.Name == "GalleryTypeId"));

        // Assert
        Assert.NotNull(galleryTypeIndex);
    }

    [Fact]
    public async Task PictureConfiguration_IsActiveIndex_ShouldExist()
    {
        // Arrange
        var options = CreateNewContextOptions();
        using var context = new ApplicationDbContext(options);
        await context.Database.EnsureCreatedAsync();

        // Act
        var entityType = context.Model.FindEntityType(typeof(Picture));
        var indexes = entityType?.GetIndexes();
        var isActiveIndex = indexes?.FirstOrDefault(i =>
            i.Properties.Any(p => p.Name == "IsActive"));

        // Assert
        Assert.NotNull(isActiveIndex);
    }

    [Fact]
    public async Task PictureConfiguration_AddPictureWithAllProperties_ShouldSucceed()
    {
        // Arrange
        var options = CreateNewContextOptions();
        using var context = new ApplicationDbContext(options);
        await context.Database.EnsureCreatedAsync();

        var picture = new Picture
        {
            Name = "Test Picture",
            Description = "Test Description",
            ImagePath = "/images/test.jpg",
            ThumbnailImagePath = "/thumbnails/test_thumb.jpg",
            GalleryTypeId = 1,
            CreatedBy = "admin",
            IsActive = true
        };

        // Act
        context.Pictures.Add(picture);
        await context.SaveChangesAsync();

        // Assert
        var savedPicture = await context.Pictures.FindAsync(picture.Id);
        Assert.NotNull(savedPicture);
        Assert.Equal("Test Picture", savedPicture.Name);
    }
}
