using Xunit;
using System;
using WebGallery.ViewModel;

namespace Tests.WebGallery.ViewModel
{
    public class GalleryPictureViewModelTests
    {
        [Fact]
        public void GalleryPictureViewModel_Constructor_CreatesInstance()
        {
            // Arrange & Act
            var viewModel = new GalleryPictureViewModel();

            // Assert
            Assert.NotNull(viewModel);
        }

        [Fact]
        public void GalleryPictureViewModel_ThumbnailImagePath_CanSetAndGet()
        {
            // Arrange
            var viewModel = new GalleryPictureViewModel();
            string expectedPath = "/images/thumb/test.jpg";

            // Act
            viewModel.ThumbnailImagePath = expectedPath;
            var actualPath = viewModel.ThumbnailImagePath;

            // Assert
            Assert.Equal(expectedPath, actualPath);
        }

        [Fact]
        public void GalleryPictureViewModel_ThumbnailImagePath_DefaultValueIsNull()
        {
            // Arrange & Act
            var viewModel = new GalleryPictureViewModel();

            // Assert
            Assert.Null(viewModel.ThumbnailImagePath);
        }

        [Fact]
        public void GalleryPictureViewModel_Name_CanSetAndGet()
        {
            // Arrange
            var viewModel = new GalleryPictureViewModel();
            string expectedName = "Test Picture";

            // Act
            viewModel.Name = expectedName;
            var actualName = viewModel.Name;

            // Assert
            Assert.Equal(expectedName, actualName);
        }

        [Fact]
        public void GalleryPictureViewModel_Name_DefaultValueIsNull()
        {
            // Arrange & Act
            var viewModel = new GalleryPictureViewModel();

            // Assert
            Assert.Null(viewModel.Name);
        }

        [Fact]
        public void GalleryPictureViewModel_Description_CanSetAndGet()
        {
            // Arrange
            var viewModel = new GalleryPictureViewModel();
            string expectedDescription = "Test Description";

            // Act
            viewModel.Description = expectedDescription;
            var actualDescription = viewModel.Description;

            // Assert
            Assert.Equal(expectedDescription, actualDescription);
        }

        [Fact]
        public void GalleryPictureViewModel_Description_DefaultValueIsNull()
        {
            // Arrange & Act
            var viewModel = new GalleryPictureViewModel();

            // Assert
            Assert.Null(viewModel.Description);
        }

        [Fact]
        public void GalleryPictureViewModel_ImagePath_CanSetAndGet()
        {
            // Arrange
            var viewModel = new GalleryPictureViewModel();
            string expectedPath = "/images/test.jpg";

            // Act
            viewModel.ImagePath = expectedPath;
            var actualPath = viewModel.ImagePath;

            // Assert
            Assert.Equal(expectedPath, actualPath);
        }

        [Fact]
        public void GalleryPictureViewModel_ImagePath_DefaultValueIsNull()
        {
            // Arrange & Act
            var viewModel = new GalleryPictureViewModel();

            // Assert
            Assert.Null(viewModel.ImagePath);
        }

        [Fact]
        public void GalleryPictureViewModel_CanSetNull()
        {
            // Arrange
            var viewModel = new GalleryPictureViewModel
            {
                ThumbnailImagePath = "test",
                Name = "test",
                Description = "test",
                ImagePath = "test"
            };

            // Act
            viewModel.ThumbnailImagePath = null;
            viewModel.Name = null;
            viewModel.Description = null;
            viewModel.ImagePath = null;

            // Assert
            Assert.Null(viewModel.ThumbnailImagePath);
            Assert.Null(viewModel.Name);
            Assert.Null(viewModel.Description);
            Assert.Null(viewModel.ImagePath);
        }

        [Fact]
        public void GalleryPictureViewModel_CanInitializeWithObjectInitializer()
        {
            // Arrange & Act
            var viewModel = new GalleryPictureViewModel
            {
                ThumbnailImagePath = "/thumb/test.jpg",
                Name = "Test",
                Description = "Description",
                ImagePath = "/images/test.jpg"
            };

            // Assert
            Assert.Equal("/thumb/test.jpg", viewModel.ThumbnailImagePath);
            Assert.Equal("Test", viewModel.Name);
            Assert.Equal("Description", viewModel.Description);
            Assert.Equal("/images/test.jpg", viewModel.ImagePath);
        }
    }
}
