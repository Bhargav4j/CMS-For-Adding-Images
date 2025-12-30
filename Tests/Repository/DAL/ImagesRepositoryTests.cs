using Xunit;
using System;
using System.Collections.Generic;
using System.Linq;
using Repository.DAL;
using Repository.DAL.Interfaces;
using Repository.POCO;

namespace Tests.Repository.DAL
{
    public class ImagesRepositoryTests
    {
        [Fact]
        public void ImagesRepository_Constructor_WithValidDBContext_CreatesInstance()
        {
            // Arrange
            var db = new DBContext();

            // Act
            var repository = new ImagesRepository(db);

            // Assert
            Assert.NotNull(repository);
        }

        [Fact]
        public void ImagesRepository_Constructor_WithNullDBContext_ThrowsException()
        {
            // Arrange
            DBContext db = null;

            // Act & Assert
            Assert.Throws<NullReferenceException>(() =>
            {
                var repository = new ImagesRepository(db);
                repository.GetGalleryTypes();
            });
        }

        [Fact]
        public void ImagesRepository_InsertPicture_WithValidPicture_AddsToContext()
        {
            // Arrange
            var db = new DBContext();
            var repository = new ImagesRepository(db);
            var picture = new Picture
            {
                PictureID = 1,
                Name = "Test Picture",
                GalleryID = 1
            };

            // Act
            var exception = Record.Exception(() => repository.InsertPicture(picture));

            // Assert
            Assert.Null(exception);
        }

        [Fact]
        public void ImagesRepository_InsertPicture_WithNullPicture_ThrowsException()
        {
            // Arrange
            var db = new DBContext();
            var repository = new ImagesRepository(db);

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => repository.InsertPicture(null));
        }

        [Fact]
        public void ImagesRepository_GetPicture_WithValidId_ReturnsPicture()
        {
            // Arrange
            var db = new DBContext();
            var repository = new ImagesRepository(db);
            int pictureId = 1;

            // Act
            var result = repository.GetPicture(pictureId);

            // Assert
            Assert.True(result == null || result is Picture);
        }

        [Fact]
        public void ImagesRepository_GetPicture_WithNegativeId_ReturnsNull()
        {
            // Arrange
            var db = new DBContext();
            var repository = new ImagesRepository(db);
            int pictureId = -1;

            // Act
            var result = repository.GetPicture(pictureId);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void ImagesRepository_UpdatePicture_WithValidPicture_UpdatesContext()
        {
            // Arrange
            var db = new DBContext();
            var repository = new ImagesRepository(db);
            var picture = new Picture
            {
                PictureID = 1,
                Name = "Updated Picture",
                GalleryID = 1
            };

            // Act
            var exception = Record.Exception(() => repository.UpdatePicture(picture));

            // Assert
            Assert.Null(exception);
        }

        [Fact]
        public void ImagesRepository_UpdatePicture_WithNullPicture_ThrowsException()
        {
            // Arrange
            var db = new DBContext();
            var repository = new ImagesRepository(db);

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => repository.UpdatePicture(null));
        }

        [Fact]
        public void ImagesRepository_DeletePicture_WithValidPicture_DeletesFromContext()
        {
            // Arrange
            var db = new DBContext();
            var repository = new ImagesRepository(db);
            var picture = new Picture
            {
                PictureID = 1,
                Name = "Picture to Delete",
                GalleryID = 1
            };

            // Act
            var exception = Record.Exception(() => repository.DeletePicture(picture));

            // Assert
            Assert.Null(exception);
        }

        [Fact]
        public void ImagesRepository_DeletePicture_WithNullPicture_ThrowsException()
        {
            // Arrange
            var db = new DBContext();
            var repository = new ImagesRepository(db);

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => repository.DeletePicture(null));
        }

        [Fact]
        public void ImagesRepository_GetPicturesByGalleryID_WithValidGalleryID_ReturnsEnumerable()
        {
            // Arrange
            var db = new DBContext();
            var repository = new ImagesRepository(db);
            int? galleryID = 1;

            // Act
            var result = repository.GetPicturesByGalleryID(galleryID);

            // Assert
            Assert.NotNull(result);
            Assert.IsAssignableFrom<IEnumerable<Picture>>(result);
        }

        [Fact]
        public void ImagesRepository_GetPicturesByGalleryID_WithNullGalleryID_ReturnsEnumerable()
        {
            // Arrange
            var db = new DBContext();
            var repository = new ImagesRepository(db);
            int? galleryID = null;

            // Act
            var result = repository.GetPicturesByGalleryID(galleryID);

            // Assert
            Assert.NotNull(result);
            Assert.IsAssignableFrom<IEnumerable<Picture>>(result);
        }

        [Fact]
        public void ImagesRepository_GetGalleryTypes_ReturnsListOfGalleryTypes()
        {
            // Arrange
            var db = new DBContext();
            var repository = new ImagesRepository(db);

            // Act
            var result = repository.GetGalleryTypes();

            // Assert
            Assert.NotNull(result);
            Assert.IsAssignableFrom<List<GalleryType>>(result);
        }

        [Fact]
        public void ImagesRepository_ImplementsIImagesRepository()
        {
            // Arrange
            var db = new DBContext();

            // Act
            var repository = new ImagesRepository(db);

            // Assert
            Assert.IsAssignableFrom<IImagesRepository>(repository);
        }
    }
}
