using Xunit;
using Moq;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using GalleryApp.Domain.Entities;
using GalleryApp.Infrastructure.Data;
using GalleryApp.Infrastructure.Repositories;

namespace GalleryApp.Tests.Infrastructure.Repositories;

public class GalleryTypeRepositoryTests
{
    private DbContextOptions<ApplicationDbContext> CreateNewContextOptions()
    {
        return new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
    }

    private Mock<ILogger<GalleryTypeRepository>> CreateMockLogger()
    {
        return new Mock<ILogger<GalleryTypeRepository>>();
    }

    [Fact]
    public async Task GetAllAsync_ReturnsOnlyActiveGalleryTypes()
    {
        // Arrange
        var options = CreateNewContextOptions();
        var mockLogger = CreateMockLogger();

        using (var context = new ApplicationDbContext(options))
        {
            context.GalleryTypes.AddRange(
                new GalleryType { Id = 1, Name = "Active Gallery", IsActive = true, CreatedDate = DateTime.UtcNow, CreatedBy = "test" },
                new GalleryType { Id = 2, Name = "Inactive Gallery", IsActive = false, CreatedDate = DateTime.UtcNow, CreatedBy = "test" }
            );
            await context.SaveChangesAsync();
        }

        using (var context = new ApplicationDbContext(options))
        {
            var repository = new GalleryTypeRepository(context, mockLogger.Object);

            // Act
            var result = await repository.GetAllAsync();

            // Assert
            Assert.Single(result);
            Assert.All(result, g => Assert.True(g.IsActive));
        }
    }

    [Fact]
    public async Task GetAllAsync_ReturnsEmptyListWhenNoActiveGalleryTypes()
    {
        // Arrange
        var options = CreateNewContextOptions();
        var mockLogger = CreateMockLogger();

        using (var context = new ApplicationDbContext(options))
        {
            var repository = new GalleryTypeRepository(context, mockLogger.Object);

            // Act
            var result = await repository.GetAllAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }
    }

    [Fact]
    public async Task GetByIdAsync_WithValidId_ReturnsGalleryType()
    {
        // Arrange
        var options = CreateNewContextOptions();
        var mockLogger = CreateMockLogger();

        using (var context = new ApplicationDbContext(options))
        {
            context.GalleryTypes.Add(new GalleryType
            {
                Id = 1,
                Name = "Test Gallery",
                IsActive = true,
                CreatedDate = DateTime.UtcNow,
                CreatedBy = "test"
            });
            await context.SaveChangesAsync();
        }

        using (var context = new ApplicationDbContext(options))
        {
            var repository = new GalleryTypeRepository(context, mockLogger.Object);

            // Act
            var result = await repository.GetByIdAsync(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Test Gallery", result.Name);
        }
    }

    [Fact]
    public async Task GetByIdAsync_WithInactiveGalleryType_ReturnsNull()
    {
        // Arrange
        var options = CreateNewContextOptions();
        var mockLogger = CreateMockLogger();

        using (var context = new ApplicationDbContext(options))
        {
            context.GalleryTypes.Add(new GalleryType
            {
                Id = 1,
                Name = "Inactive Gallery",
                IsActive = false,
                CreatedDate = DateTime.UtcNow,
                CreatedBy = "test"
            });
            await context.SaveChangesAsync();
        }

        using (var context = new ApplicationDbContext(options))
        {
            var repository = new GalleryTypeRepository(context, mockLogger.Object);

            // Act
            var result = await repository.GetByIdAsync(1);

            // Assert
            Assert.Null(result);
        }
    }

    [Fact]
    public async Task GetByIdAsync_IncludesPictures()
    {
        // Arrange
        var options = CreateNewContextOptions();
        var mockLogger = CreateMockLogger();

        using (var context = new ApplicationDbContext(options))
        {
            var galleryType = new GalleryType
            {
                Id = 1,
                Name = "Test Gallery",
                IsActive = true,
                CreatedDate = DateTime.UtcNow,
                CreatedBy = "test"
            };
            context.GalleryTypes.Add(galleryType);
            context.Pictures.Add(new Picture
            {
                Id = 1,
                Name = "Test Picture",
                GalleryTypeId = 1,
                IsActive = true,
                CreatedDate = DateTime.UtcNow,
                ImagePath = "/test.jpg",
                ThumbnailImagePath = "/thumb.jpg",
                CreatedBy = "test"
            });
            await context.SaveChangesAsync();
        }

        using (var context = new ApplicationDbContext(options))
        {
            var repository = new GalleryTypeRepository(context, mockLogger.Object);

            // Act
            var result = await repository.GetByIdAsync(1);

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result.Pictures);
            Assert.Single(result.Pictures);
        }
    }

    [Fact]
    public async Task AddAsync_AddsGalleryTypeAndReturnsSavedEntity()
    {
        // Arrange
        var options = CreateNewContextOptions();
        var mockLogger = CreateMockLogger();
        var newGalleryType = new GalleryType
        {
            Name = "New Gallery",
            CreatedDate = DateTime.UtcNow,
            IsActive = true,
            CreatedBy = "test@example.com"
        };

        using (var context = new ApplicationDbContext(options))
        {
            var repository = new GalleryTypeRepository(context, mockLogger.Object);

            // Act
            var result = await repository.AddAsync(newGalleryType);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("New Gallery", result.Name);
            Assert.True(result.Id > 0);
        }
    }

    [Fact]
    public async Task UpdateAsync_UpdatesExistingGalleryType()
    {
        // Arrange
        var options = CreateNewContextOptions();
        var mockLogger = CreateMockLogger();

        using (var context = new ApplicationDbContext(options))
        {
            context.GalleryTypes.Add(new GalleryType
            {
                Id = 1,
                Name = "Original Name",
                CreatedDate = DateTime.UtcNow,
                IsActive = true,
                CreatedBy = "test"
            });
            await context.SaveChangesAsync();
        }

        using (var context = new ApplicationDbContext(options))
        {
            var repository = new GalleryTypeRepository(context, mockLogger.Object);
            var galleryType = await context.GalleryTypes.FindAsync(1);
            galleryType!.Name = "Updated Name";

            // Act
            await repository.UpdateAsync(galleryType);

            // Assert
            var updatedGalleryType = await context.GalleryTypes.FindAsync(1);
            Assert.Equal("Updated Name", updatedGalleryType!.Name);
        }
    }

    [Fact]
    public async Task DeleteAsync_SetsIsActiveToFalse()
    {
        // Arrange
        var options = CreateNewContextOptions();
        var mockLogger = CreateMockLogger();

        using (var context = new ApplicationDbContext(options))
        {
            context.GalleryTypes.Add(new GalleryType
            {
                Id = 1,
                Name = "To Delete",
                CreatedDate = DateTime.UtcNow,
                IsActive = true,
                CreatedBy = "test"
            });
            await context.SaveChangesAsync();
        }

        using (var context = new ApplicationDbContext(options))
        {
            var repository = new GalleryTypeRepository(context, mockLogger.Object);

            // Act
            await repository.DeleteAsync(1);

            // Assert
            var deletedGalleryType = await context.GalleryTypes.FindAsync(1);
            Assert.NotNull(deletedGalleryType);
            Assert.False(deletedGalleryType.IsActive);
            Assert.NotNull(deletedGalleryType.ModifiedDate);
        }
    }

    [Fact]
    public async Task DeleteAsync_WithNonExistentId_DoesNotThrow()
    {
        // Arrange
        var options = CreateNewContextOptions();
        var mockLogger = CreateMockLogger();

        using (var context = new ApplicationDbContext(options))
        {
            var repository = new GalleryTypeRepository(context, mockLogger.Object);

            // Act & Assert
            await repository.DeleteAsync(999);
        }
    }

    [Fact]
    public async Task ExistsAsync_WithExistingActiveId_ReturnsTrue()
    {
        // Arrange
        var options = CreateNewContextOptions();
        var mockLogger = CreateMockLogger();

        using (var context = new ApplicationDbContext(options))
        {
            context.GalleryTypes.Add(new GalleryType
            {
                Id = 1,
                Name = "Exists",
                CreatedDate = DateTime.UtcNow,
                IsActive = true,
                CreatedBy = "test"
            });
            await context.SaveChangesAsync();
        }

        using (var context = new ApplicationDbContext(options))
        {
            var repository = new GalleryTypeRepository(context, mockLogger.Object);

            // Act
            var result = await repository.ExistsAsync(1);

            // Assert
            Assert.True(result);
        }
    }

    [Fact]
    public async Task ExistsAsync_WithInactiveId_ReturnsFalse()
    {
        // Arrange
        var options = CreateNewContextOptions();
        var mockLogger = CreateMockLogger();

        using (var context = new ApplicationDbContext(options))
        {
            context.GalleryTypes.Add(new GalleryType
            {
                Id = 1,
                Name = "Inactive",
                CreatedDate = DateTime.UtcNow,
                IsActive = false,
                CreatedBy = "test"
            });
            await context.SaveChangesAsync();
        }

        using (var context = new ApplicationDbContext(options))
        {
            var repository = new GalleryTypeRepository(context, mockLogger.Object);

            // Act
            var result = await repository.ExistsAsync(1);

            // Assert
            Assert.False(result);
        }
    }

    [Fact]
    public async Task SearchAsync_WithMatchingName_ReturnsResults()
    {
        // Arrange
        var options = CreateNewContextOptions();
        var mockLogger = CreateMockLogger();

        using (var context = new ApplicationDbContext(options))
        {
            context.GalleryTypes.AddRange(
                new GalleryType { Id = 1, Name = "Nature Gallery", IsActive = true, CreatedDate = DateTime.UtcNow, CreatedBy = "test" },
                new GalleryType { Id = 2, Name = "Wildlife Photos", IsActive = true, CreatedDate = DateTime.UtcNow, CreatedBy = "test" }
            );
            await context.SaveChangesAsync();
        }

        using (var context = new ApplicationDbContext(options))
        {
            var repository = new GalleryTypeRepository(context, mockLogger.Object);

            // Act
            var result = await repository.SearchAsync("Nature");

            // Assert
            Assert.Single(result);
            Assert.Contains(result, g => g.Name.Contains("Nature"));
        }
    }

    [Fact]
    public async Task SearchAsync_WithMatchingDescription_ReturnsResults()
    {
        // Arrange
        var options = CreateNewContextOptions();
        var mockLogger = CreateMockLogger();

        using (var context = new ApplicationDbContext(options))
        {
            context.GalleryTypes.AddRange(
                new GalleryType { Id = 1, Name = "Gallery 1", Description = "Beautiful nature photos", IsActive = true, CreatedDate = DateTime.UtcNow, CreatedBy = "test" },
                new GalleryType { Id = 2, Name = "Gallery 2", Description = "Urban landscapes", IsActive = true, CreatedDate = DateTime.UtcNow, CreatedBy = "test" }
            );
            await context.SaveChangesAsync();
        }

        using (var context = new ApplicationDbContext(options))
        {
            var repository = new GalleryTypeRepository(context, mockLogger.Object);

            // Act
            var result = await repository.SearchAsync("nature");

            // Assert
            Assert.Single(result);
            Assert.Contains(result, g => g.Description != null && g.Description.Contains("nature"));
        }
    }

    [Fact]
    public async Task SearchAsync_OnlyReturnsActiveGalleryTypes()
    {
        // Arrange
        var options = CreateNewContextOptions();
        var mockLogger = CreateMockLogger();

        using (var context = new ApplicationDbContext(options))
        {
            context.GalleryTypes.AddRange(
                new GalleryType { Id = 1, Name = "Active Search", IsActive = true, CreatedDate = DateTime.UtcNow, CreatedBy = "test" },
                new GalleryType { Id = 2, Name = "Inactive Search", IsActive = false, CreatedDate = DateTime.UtcNow, CreatedBy = "test" }
            );
            await context.SaveChangesAsync();
        }

        using (var context = new ApplicationDbContext(options))
        {
            var repository = new GalleryTypeRepository(context, mockLogger.Object);

            // Act
            var result = await repository.SearchAsync("Search");

            // Assert
            Assert.Single(result);
            Assert.All(result, g => Assert.True(g.IsActive));
        }
    }
}
