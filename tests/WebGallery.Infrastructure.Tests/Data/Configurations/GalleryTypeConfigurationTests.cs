using Xunit;
using Microsoft.EntityFrameworkCore;
using WebGallery.Infrastructure.Data;
using WebGallery.Domain.Entities;

namespace WebGallery.Infrastructure.Tests.Data.Configurations;

public class GalleryTypeConfigurationTests
{
    private DbContextOptions<ApplicationDbContext> CreateNewContextOptions()
    {
        return new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
    }

    [Fact]
    public async Task GalleryTypeConfiguration_TableName_ShouldBeGalleryTypes()
    {
        // Arrange
        var options = CreateNewContextOptions();
        using var context = new ApplicationDbContext(options);
        await context.Database.EnsureCreatedAsync();

        // Act
        var entityType = context.Model.FindEntityType(typeof(GalleryType));

        // Assert
        Assert.NotNull(entityType);
        Assert.Equal("GalleryTypes", entityType.GetTableName());
    }

    [Fact]
    public async Task GalleryTypeConfiguration_IdProperty_ShouldBePrimaryKey()
    {
        // Arrange
        var options = CreateNewContextOptions();
        using var context = new ApplicationDbContext(options);
        await context.Database.EnsureCreatedAsync();

        // Act
        var entityType = context.Model.FindEntityType(typeof(GalleryType));
        var primaryKey = entityType?.FindPrimaryKey();

        // Assert
        Assert.NotNull(primaryKey);
        Assert.Single(primaryKey.Properties);
        Assert.Equal("Id", primaryKey.Properties.First().Name);
    }

    [Fact]
    public async Task GalleryTypeConfiguration_NameProperty_ShouldBeRequired()
    {
        // Arrange
        var options = CreateNewContextOptions();
        using var context = new ApplicationDbContext(options);
        await context.Database.EnsureCreatedAsync();

        // Act
        var entityType = context.Model.FindEntityType(typeof(GalleryType));
        var nameProperty = entityType?.FindProperty("Name");

        // Assert
        Assert.NotNull(nameProperty);
        Assert.False(nameProperty.IsNullable);
    }

    [Fact]
    public async Task GalleryTypeConfiguration_DescriptionProperty_ShouldBeOptional()
    {
        // Arrange
        var options = CreateNewContextOptions();
        using var context = new ApplicationDbContext(options);
        await context.Database.EnsureCreatedAsync();

        // Act
        var entityType = context.Model.FindEntityType(typeof(GalleryType));
        var descProperty = entityType?.FindProperty("Description");

        // Assert
        Assert.NotNull(descProperty);
        Assert.True(descProperty.IsNullable);
    }

    [Fact]
    public async Task GalleryTypeConfiguration_CreatedDateProperty_ShouldBeRequired()
    {
        // Arrange
        var options = CreateNewContextOptions();
        using var context = new ApplicationDbContext(options);
        await context.Database.EnsureCreatedAsync();

        // Act
        var entityType = context.Model.FindEntityType(typeof(GalleryType));
        var createdDateProperty = entityType?.FindProperty("CreatedDate");

        // Assert
        Assert.NotNull(createdDateProperty);
        Assert.False(createdDateProperty.IsNullable);
    }

    [Fact]
    public async Task GalleryTypeConfiguration_IsActiveProperty_ShouldBeRequired()
    {
        // Arrange
        var options = CreateNewContextOptions();
        using var context = new ApplicationDbContext(options);
        await context.Database.EnsureCreatedAsync();

        // Act
        var entityType = context.Model.FindEntityType(typeof(GalleryType));
        var isActiveProperty = entityType?.FindProperty("IsActive");

        // Assert
        Assert.NotNull(isActiveProperty);
        Assert.False(isActiveProperty.IsNullable);
    }

    [Fact]
    public async Task GalleryTypeConfiguration_CreatedByProperty_ShouldBeRequired()
    {
        // Arrange
        var options = CreateNewContextOptions();
        using var context = new ApplicationDbContext(options);
        await context.Database.EnsureCreatedAsync();

        // Act
        var entityType = context.Model.FindEntityType(typeof(GalleryType));
        var createdByProperty = entityType?.FindProperty("CreatedBy");

        // Assert
        Assert.NotNull(createdByProperty);
        Assert.False(createdByProperty.IsNullable);
    }

    [Fact]
    public async Task GalleryTypeConfiguration_ModifiedByProperty_ShouldBeOptional()
    {
        // Arrange
        var options = CreateNewContextOptions();
        using var context = new ApplicationDbContext(options);
        await context.Database.EnsureCreatedAsync();

        // Act
        var entityType = context.Model.FindEntityType(typeof(GalleryType));
        var modifiedByProperty = entityType?.FindProperty("ModifiedBy");

        // Assert
        Assert.NotNull(modifiedByProperty);
        Assert.True(modifiedByProperty.IsNullable);
    }

    [Fact]
    public async Task GalleryTypeConfiguration_PicturesRelationship_ShouldExist()
    {
        // Arrange
        var options = CreateNewContextOptions();
        using var context = new ApplicationDbContext(options);
        await context.Database.EnsureCreatedAsync();

        // Act
        var entityType = context.Model.FindEntityType(typeof(GalleryType));
        var navigation = entityType?.FindNavigation("Pictures");

        // Assert
        Assert.NotNull(navigation);
        Assert.True(navigation.IsCollection);
    }

    [Fact]
    public async Task GalleryTypeConfiguration_IsActiveIndex_ShouldExist()
    {
        // Arrange
        var options = CreateNewContextOptions();
        using var context = new ApplicationDbContext(options);
        await context.Database.EnsureCreatedAsync();

        // Act
        var entityType = context.Model.FindEntityType(typeof(GalleryType));
        var indexes = entityType?.GetIndexes();
        var isActiveIndex = indexes?.FirstOrDefault(i =>
            i.Properties.Any(p => p.Name == "IsActive"));

        // Assert
        Assert.NotNull(isActiveIndex);
    }

    [Fact]
    public async Task GalleryTypeConfiguration_AddGalleryTypeWithAllProperties_ShouldSucceed()
    {
        // Arrange
        var options = CreateNewContextOptions();
        using var context = new ApplicationDbContext(options);
        await context.Database.EnsureCreatedAsync();

        var galleryType = new GalleryType
        {
            Name = "Wildlife",
            Description = "Wildlife photography collection",
            CreatedBy = "admin",
            IsActive = true
        };

        // Act
        context.GalleryTypes.Add(galleryType);
        await context.SaveChangesAsync();

        // Assert
        var savedGalleryType = await context.GalleryTypes.FindAsync(galleryType.Id);
        Assert.NotNull(savedGalleryType);
        Assert.Equal("Wildlife", savedGalleryType.Name);
    }

    [Fact]
    public async Task GalleryTypeConfiguration_ModifiedDateProperty_ShouldBeOptional()
    {
        // Arrange
        var options = CreateNewContextOptions();
        using var context = new ApplicationDbContext(options);
        await context.Database.EnsureCreatedAsync();

        // Act
        var entityType = context.Model.FindEntityType(typeof(GalleryType));
        var modifiedDateProperty = entityType?.FindProperty("ModifiedDate");

        // Assert
        Assert.NotNull(modifiedDateProperty);
        Assert.True(modifiedDateProperty.IsNullable);
    }
}
