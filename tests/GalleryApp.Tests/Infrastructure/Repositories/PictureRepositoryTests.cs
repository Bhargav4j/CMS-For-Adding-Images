using Xunit;
using Moq;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using GalleryApp.Domain.Entities;
using GalleryApp.Infrastructure.Data;
using GalleryApp.Infrastructure.Repositories;

namespace GalleryApp.Tests.Infrastructure.Repositories;

public class PictureRepositoryTests
{
    private DbContextOptions<ApplicationDbContext> CreateNewContextOptions()
    {
        return new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
    }

    private Mock<ILogger<PictureRepository>> CreateMockLogger()
    {
        return new Mock<ILogger<PictureRepository>>();
    }

    [Fact]
    public async Task GetAllAsync_ReturnsOnlyActivePictures()
    {
        // Arrange
        var options = CreateNewContextOptions();
        var mockLogger = CreateMockLogger();

        using (var context = new ApplicationDbContext(options))
        {
            context.Pictures.AddRange(
                new Picture { Id = 1, Name = "Active Picture", IsActive = true, CreatedDate = DateTime.UtcNow, ImagePath = "/test1.jpg", ThumbnailImagePath = "/thumb1.jpg", GalleryTypeId = 1, CreatedBy = "test" },
                new Picture { Id = 2, Name = "Inactive Picture", IsActive = false, CreatedDate = DateTime.UtcNow, ImagePath = "/test2.jpg", ThumbnailImagePath = "/thumb2.jpg", GalleryTypeId = 1, CreatedBy = "test" }
            );
            await context.SaveChangesAsync();
        }

        using (var context = new ApplicationDbContext(options))
        {
            var repository = new PictureRepository(context, mockLogger.Object);

            // Act
            var result = await repository.GetAllAsync();

            // Assert
            Assert.Single(result);
            Assert.All(result, p => Assert.True(p.IsActive));
        }
    }

    [Fact]
    public async Task GetAllAsync_IncludesGalleryType()
    {
        // Arrange
        var options = CreateNewContextOptions();
        var mockLogger = CreateMockLogger();

        using (var context = new ApplicationDbContext(options))
        {
            var galleryType = new GalleryType { Id = 1, Name = "Test Gallery", IsActive = true, CreatedDate = DateTime.UtcNow, CreatedBy = "test" };
            context.GalleryTypes.Add(galleryType);
            context.Pictures.Add(new Picture
            {
                Id = 1,
                Name = "Test Picture",
                IsActive = true,
                GalleryTypeId = 1,
                CreatedDate = DateTime.UtcNow,
                ImagePath = "/test.jpg",
                ThumbnailImagePath = "/thumb.jpg",
                CreatedBy = "test"
            });
            await context.SaveChangesAsync();
        }

        using (var context = new ApplicationDbContext(options))
        {
            var repository = new PictureRepository(context, mockLogger.Object);

            // Act
            var result = await repository.GetAllAsync();

            // Assert
            var picture = result.First();
            Assert.NotNull(picture.GalleryType);
            Assert.Equal("Test Gallery", picture.GalleryType.Name);
        }
    }

    [Fact]
    public async Task GetByIdAsync_WithValidId_ReturnsPicture()
    {
        // Arrange
        var options = CreateNewContextOptions();
        var mockLogger = CreateMockLogger();

        using (var context = new ApplicationDbContext(options))
        {
            context.Pictures.Add(new Picture
            {
                Id = 1,
                Name = "Test Picture",
                IsActive = true,
                CreatedDate = DateTime.UtcNow,
                ImagePath = "/test.jpg",
                ThumbnailImagePath = "/thumb.jpg",
                GalleryTypeId = 1,
                CreatedBy = "test"
            });
            await context.SaveChangesAsync();
        }

        using (var context = new ApplicationDbContext(options))
        {
            var repository = new PictureRepository(context, mockLogger.Object);

            // Act
            var result = await repository.GetByIdAsync(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Test Picture", result.Name);
        }
    }

    [Fact]
    public async Task GetByIdAsync_WithInactivePicture_ReturnsNull()
    {
        // Arrange
        var options = CreateNewContextOptions();
        var mockLogger = CreateMockLogger();

        using (var context = new ApplicationDbContext(options))
        {
            context.Pictures.Add(new Picture
            {
                Id = 1,
                Name = "Inactive Picture",
                IsActive = false,
                CreatedDate = DateTime.UtcNow,
                ImagePath = "/test.jpg",
                ThumbnailImagePath = "/thumb.jpg",
                GalleryTypeId = 1,
                CreatedBy = "test"
            });
            await context.SaveChangesAsync();
        }

        using (var context = new ApplicationDbContext(options))
        {
            var repository = new PictureRepository(context, mockLogger.Object);

            // Act
            var result = await repository.GetByIdAsync(1);

            // Assert
            Assert.Null(result);
        }
    }

    [Fact]
    public async Task GetByGalleryTypeIdAsync_ReturnsMatchingPictures()
    {
        // Arrange
        var options = CreateNewContextOptions();
        var mockLogger = CreateMockLogger();

        using (var context = new ApplicationDbContext(options))
        {
            context.Pictures.AddRange(
                new Picture { Id = 1, Name = "Picture 1", GalleryTypeId = 1, IsActive = true, CreatedDate = DateTime.UtcNow, ImagePath = "/test1.jpg", ThumbnailImagePath = "/thumb1.jpg", CreatedBy = "test" },
                new Picture { Id = 2, Name = "Picture 2", GalleryTypeId = 1, IsActive = true, CreatedDate = DateTime.UtcNow, ImagePath = "/test2.jpg", ThumbnailImagePath = "/thumb2.jpg", CreatedBy = "test" },
                new Picture { Id = 3, Name = "Picture 3", GalleryTypeId = 2, IsActive = true, CreatedDate = DateTime.UtcNow, ImagePath = "/test3.jpg", ThumbnailImagePath = "/thumb3.jpg", CreatedBy = "test" }
            );
            await context.SaveChangesAsync();
        }

        using (var context = new ApplicationDbContext(options))
        {
            var repository = new PictureRepository(context, mockLogger.Object);

            // Act
            var result = await repository.GetByGalleryTypeIdAsync(1);

            // Assert
            Assert.Equal(2, result.Count());
            Assert.All(result, p => Assert.Equal(1, p.GalleryTypeId));
        }
    }

    [Fact]
    public async Task AddAsync_AddsPictureAndReturnsSavedEntity()
    {
        // Arrange
        var options = CreateNewContextOptions();
        var mockLogger = CreateMockLogger();
        var newPicture = new Picture
        {
            Name = "New Picture",
            ImagePath = "/new.jpg",
            ThumbnailImagePath = "/thumb_new.jpg",
            GalleryTypeId = 1,
            CreatedDate = DateTime.UtcNow,
            IsActive = true,
            CreatedBy = "test@example.com"
        };

        using (var context = new ApplicationDbContext(options))
        {
            var repository = new PictureRepository(context, mockLogger.Object);

            // Act
            var result = await repository.AddAsync(newPicture);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("New Picture", result.Name);
            Assert.True(result.Id > 0);
        }
    }

    [Fact]
    public async Task UpdateAsync_UpdatesExistingPicture()
    {
        // Arrange
        var options = CreateNewContextOptions();
        var mockLogger = CreateMockLogger();

        using (var context = new ApplicationDbContext(options))
        {
            context.Pictures.Add(new Picture
            {
                Id = 1,
                Name = "Original Name",
                ImagePath = "/original.jpg",
                ThumbnailImagePath = "/thumb_original.jpg",
                GalleryTypeId = 1,
                CreatedDate = DateTime.UtcNow,
                IsActive = true,
                CreatedBy = "test"
            });
            await context.SaveChangesAsync();
        }

        using (var context = new ApplicationDbContext(options))
        {
            var repository = new PictureRepository(context, mockLogger.Object);
            var picture = await context.Pictures.FindAsync(1);
            picture!.Name = "Updated Name";

            // Act
            await repository.UpdateAsync(picture);

            // Assert
            var updatedPicture = await context.Pictures.FindAsync(1);
            Assert.Equal("Updated Name", updatedPicture!.Name);
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
            context.Pictures.Add(new Picture
            {
                Id = 1,
                Name = "To Delete",
                ImagePath = "/delete.jpg",
                ThumbnailImagePath = "/thumb_delete.jpg",
                GalleryTypeId = 1,
                CreatedDate = DateTime.UtcNow,
                IsActive = true,
                CreatedBy = "test"
            });
            await context.SaveChangesAsync();
        }

        using (var context = new ApplicationDbContext(options))
        {
            var repository = new PictureRepository(context, mockLogger.Object);

            // Act
            await repository.DeleteAsync(1);

            // Assert
            var deletedPicture = await context.Pictures.FindAsync(1);
            Assert.NotNull(deletedPicture);
            Assert.False(deletedPicture.IsActive);
            Assert.NotNull(deletedPicture.ModifiedDate);
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
            var repository = new PictureRepository(context, mockLogger.Object);

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
            context.Pictures.Add(new Picture
            {
                Id = 1,
                Name = "Exists",
                ImagePath = "/exists.jpg",
                ThumbnailImagePath = "/thumb_exists.jpg",
                GalleryTypeId = 1,
                CreatedDate = DateTime.UtcNow,
                IsActive = true,
                CreatedBy = "test"
            });
            await context.SaveChangesAsync();
        }

        using (var context = new ApplicationDbContext(options))
        {
            var repository = new PictureRepository(context, mockLogger.Object);

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
            context.Pictures.Add(new Picture
            {
                Id = 1,
                Name = "Inactive",
                ImagePath = "/inactive.jpg",
                ThumbnailImagePath = "/thumb_inactive.jpg",
                GalleryTypeId = 1,
                CreatedDate = DateTime.UtcNow,
                IsActive = false,
                CreatedBy = "test"
            });
            await context.SaveChangesAsync();
        }

        using (var context = new ApplicationDbContext(options))
        {
            var repository = new PictureRepository(context, mockLogger.Object);

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
            context.Pictures.AddRange(
                new Picture { Id = 1, Name = "Sunset Photo", IsActive = true, CreatedDate = DateTime.UtcNow, ImagePath = "/sunset.jpg", ThumbnailImagePath = "/thumb_sunset.jpg", GalleryTypeId = 1, CreatedBy = "test" },
                new Picture { Id = 2, Name = "Mountain View", IsActive = true, CreatedDate = DateTime.UtcNow, ImagePath = "/mountain.jpg", ThumbnailImagePath = "/thumb_mountain.jpg", GalleryTypeId = 1, CreatedBy = "test" }
            );
            await context.SaveChangesAsync();
        }

        using (var context = new ApplicationDbContext(options))
        {
            var repository = new PictureRepository(context, mockLogger.Object);

            // Act
            var result = await repository.SearchAsync("Sunset");

            // Assert
            Assert.Single(result);
            Assert.Contains(result, p => p.Name.Contains("Sunset"));
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
            context.Pictures.AddRange(
                new Picture { Id = 1, Name = "Photo 1", Description = "Beautiful sunset", IsActive = true, CreatedDate = DateTime.UtcNow, ImagePath = "/photo1.jpg", ThumbnailImagePath = "/thumb_photo1.jpg", GalleryTypeId = 1, CreatedBy = "test" },
                new Picture { Id = 2, Name = "Photo 2", Description = "Mountain landscape", IsActive = true, CreatedDate = DateTime.UtcNow, ImagePath = "/photo2.jpg", ThumbnailImagePath = "/thumb_photo2.jpg", GalleryTypeId = 1, CreatedBy = "test" }
            );
            await context.SaveChangesAsync();
        }

        using (var context = new ApplicationDbContext(options))
        {
            var repository = new PictureRepository(context, mockLogger.Object);

            // Act
            var result = await repository.SearchAsync("sunset");

            // Assert
            Assert.Single(result);
            Assert.Contains(result, p => p.Description != null && p.Description.Contains("sunset"));
        }
    }
}
