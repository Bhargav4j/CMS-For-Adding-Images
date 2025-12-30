using Xunit;
using System;
using Repository.POCO;

namespace Tests.Repository.POCO
{
    public class PictureTests
    {
        [Fact]
        public void Picture_Constructor_CreatesInstance()
        {
            // Arrange & Act
            var picture = new Picture();

            // Assert
            Assert.NotNull(picture);
        }

        [Fact]
        public void Picture_PictureID_CanSetAndGet()
        {
            // Arrange
            var picture = new Picture();
            int expectedId = 1;

            // Act
            picture.PictureID = expectedId;
            var actualId = picture.PictureID;

            // Assert
            Assert.Equal(expectedId, actualId);
        }

        [Fact]
        public void Picture_PictureID_DefaultValueIsZero()
        {
            // Arrange & Act
            var picture = new Picture();

            // Assert
            Assert.Equal(0, picture.PictureID);
        }

        [Fact]
        public void Picture_Name_CanSetAndGet()
        {
            // Arrange
            var picture = new Picture();
            string expectedName = "Test Picture";

            // Act
            picture.Name = expectedName;
            var actualName = picture.Name;

            // Assert
            Assert.Equal(expectedName, actualName);
        }

        [Fact]
        public void Picture_Name_DefaultValueIsNull()
        {
            // Arrange & Act
            var picture = new Picture();

            // Assert
            Assert.Null(picture.Name);
        }

        [Fact]
        public void Picture_Name_CanSetNull()
        {
            // Arrange
            var picture = new Picture { Name = "Test" };

            // Act
            picture.Name = null;

            // Assert
            Assert.Null(picture.Name);
        }

        [Fact]
        public void Picture_Description_CanSetAndGet()
        {
            // Arrange
            var picture = new Picture();
            string expectedDescription = "Test Description";

            // Act
            picture.Description = expectedDescription;
            var actualDescription = picture.Description;

            // Assert
            Assert.Equal(expectedDescription, actualDescription);
        }

        [Fact]
        public void Picture_Description_DefaultValueIsNull()
        {
            // Arrange & Act
            var picture = new Picture();

            // Assert
            Assert.Null(picture.Description);
        }

        [Fact]
        public void Picture_ImagePath_CanSetAndGet()
        {
            // Arrange
            var picture = new Picture();
            string expectedPath = "/images/test.jpg";

            // Act
            picture.ImagePath = expectedPath;
            var actualPath = picture.ImagePath;

            // Assert
            Assert.Equal(expectedPath, actualPath);
        }

        [Fact]
        public void Picture_ImagePath_DefaultValueIsNull()
        {
            // Arrange & Act
            var picture = new Picture();

            // Assert
            Assert.Null(picture.ImagePath);
        }

        [Fact]
        public void Picture_ThumbnailImagePath_CanSetAndGet()
        {
            // Arrange
            var picture = new Picture();
            string expectedPath = "/images/thumb/test.jpg";

            // Act
            picture.ThumbnailImagePath = expectedPath;
            var actualPath = picture.ThumbnailImagePath;

            // Assert
            Assert.Equal(expectedPath, actualPath);
        }

        [Fact]
        public void Picture_ThumbnailImagePath_DefaultValueIsNull()
        {
            // Arrange & Act
            var picture = new Picture();

            // Assert
            Assert.Null(picture.ThumbnailImagePath);
        }

        [Fact]
        public void Picture_GalleryID_CanSetAndGet()
        {
            // Arrange
            var picture = new Picture();
            int expectedGalleryId = 2;

            // Act
            picture.GalleryID = expectedGalleryId;
            var actualGalleryId = picture.GalleryID;

            // Assert
            Assert.Equal(expectedGalleryId, actualGalleryId);
        }

        [Fact]
        public void Picture_GalleryID_DefaultValueIsZero()
        {
            // Arrange & Act
            var picture = new Picture();

            // Assert
            Assert.Equal(0, picture.GalleryID);
        }

        [Fact]
        public void Picture_GalleryType_CanSetAndGet()
        {
            // Arrange
            var picture = new Picture();
            var expectedGalleryType = new GalleryType { GalleryID = 1, GalleryName = "Test Gallery" };

            // Act
            picture.GalleryType = expectedGalleryType;
            var actualGalleryType = picture.GalleryType;

            // Assert
            Assert.Equal(expectedGalleryType, actualGalleryType);
        }

        [Fact]
        public void Picture_GalleryType_DefaultValueIsNull()
        {
            // Arrange & Act
            var picture = new Picture();

            // Assert
            Assert.Null(picture.GalleryType);
        }

        [Fact]
        public void Picture_GalleryType_IsVirtual()
        {
            // Arrange
            var type = typeof(Picture);
            var property = type.GetProperty("GalleryType");

            // Act
            var isVirtual = property.GetGetMethod().IsVirtual;

            // Assert
            Assert.True(isVirtual);
        }

        [Fact]
        public void Picture_CanInitializeWithObjectInitializer()
        {
            // Arrange & Act
            var picture = new Picture
            {
                PictureID = 1,
                Name = "Test",
                Description = "Description",
                ImagePath = "/images/test.jpg",
                ThumbnailImagePath = "/images/thumb/test.jpg",
                GalleryID = 1
            };

            // Assert
            Assert.Equal(1, picture.PictureID);
            Assert.Equal("Test", picture.Name);
            Assert.Equal("Description", picture.Description);
            Assert.Equal("/images/test.jpg", picture.ImagePath);
            Assert.Equal("/images/thumb/test.jpg", picture.ThumbnailImagePath);
            Assert.Equal(1, picture.GalleryID);
        }
    }
}
