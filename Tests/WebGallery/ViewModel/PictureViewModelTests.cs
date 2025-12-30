using Xunit;
using System;
using WebGallery.ViewModel;

namespace Tests.WebGallery.ViewModel
{
    public class PictureViewModelTests
    {
        [Fact]
        public void PictureViewModel_Constructor_CreatesInstance()
        {
            // Arrange & Act
            var viewModel = new PictureViewModel();

            // Assert
            Assert.NotNull(viewModel);
        }

        [Fact]
        public void PictureViewModel_PictureID_CanSetAndGet()
        {
            // Arrange
            var viewModel = new PictureViewModel();
            int expectedId = 1;

            // Act
            viewModel.PictureID = expectedId;
            var actualId = viewModel.PictureID;

            // Assert
            Assert.Equal(expectedId, actualId);
        }

        [Fact]
        public void PictureViewModel_PictureID_DefaultValueIsZero()
        {
            // Arrange & Act
            var viewModel = new PictureViewModel();

            // Assert
            Assert.Equal(0, viewModel.PictureID);
        }

        [Fact]
        public void PictureViewModel_Name_CanSetAndGet()
        {
            // Arrange
            var viewModel = new PictureViewModel();
            string expectedName = "Test Picture";

            // Act
            viewModel.Name = expectedName;
            var actualName = viewModel.Name;

            // Assert
            Assert.Equal(expectedName, actualName);
        }

        [Fact]
        public void PictureViewModel_Name_DefaultValueIsNull()
        {
            // Arrange & Act
            var viewModel = new PictureViewModel();

            // Assert
            Assert.Null(viewModel.Name);
        }

        [Fact]
        public void PictureViewModel_Description_CanSetAndGet()
        {
            // Arrange
            var viewModel = new PictureViewModel();
            string expectedDescription = "Test Description";

            // Act
            viewModel.Description = expectedDescription;
            var actualDescription = viewModel.Description;

            // Assert
            Assert.Equal(expectedDescription, actualDescription);
        }

        [Fact]
        public void PictureViewModel_Description_DefaultValueIsNull()
        {
            // Arrange & Act
            var viewModel = new PictureViewModel();

            // Assert
            Assert.Null(viewModel.Description);
        }

        [Fact]
        public void PictureViewModel_ThumbnailImagePath_CanSetAndGet()
        {
            // Arrange
            var viewModel = new PictureViewModel();
            string expectedPath = "/images/thumb/test.jpg";

            // Act
            viewModel.ThumbnailImagePath = expectedPath;
            var actualPath = viewModel.ThumbnailImagePath;

            // Assert
            Assert.Equal(expectedPath, actualPath);
        }

        [Fact]
        public void PictureViewModel_ThumbnailImagePath_DefaultValueIsNull()
        {
            // Arrange & Act
            var viewModel = new PictureViewModel();

            // Assert
            Assert.Null(viewModel.ThumbnailImagePath);
        }

        [Fact]
        public void PictureViewModel_CanSetNullName()
        {
            // Arrange
            var viewModel = new PictureViewModel { Name = "test" };

            // Act
            viewModel.Name = null;

            // Assert
            Assert.Null(viewModel.Name);
        }

        [Fact]
        public void PictureViewModel_CanSetNullDescription()
        {
            // Arrange
            var viewModel = new PictureViewModel { Description = "test" };

            // Act
            viewModel.Description = null;

            // Assert
            Assert.Null(viewModel.Description);
        }

        [Fact]
        public void PictureViewModel_CanSetNullThumbnailImagePath()
        {
            // Arrange
            var viewModel = new PictureViewModel { ThumbnailImagePath = "test" };

            // Act
            viewModel.ThumbnailImagePath = null;

            // Assert
            Assert.Null(viewModel.ThumbnailImagePath);
        }

        [Fact]
        public void PictureViewModel_CanInitializeWithObjectInitializer()
        {
            // Arrange & Act
            var viewModel = new PictureViewModel
            {
                PictureID = 1,
                Name = "Test",
                Description = "Description",
                ThumbnailImagePath = "/thumb/test.jpg"
            };

            // Assert
            Assert.Equal(1, viewModel.PictureID);
            Assert.Equal("Test", viewModel.Name);
            Assert.Equal("Description", viewModel.Description);
            Assert.Equal("/thumb/test.jpg", viewModel.ThumbnailImagePath);
        }
    }
}
