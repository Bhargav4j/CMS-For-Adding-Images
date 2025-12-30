using Xunit;
using System;
using System.Collections.Generic;
using WebGallery.ViewModel;
using Repository.POCO;

namespace Tests.WebGallery.ViewModel
{
    public class CreatePictureViewModelTests
    {
        [Fact]
        public void CreatePictureViewModel_Constructor_CreatesInstance()
        {
            // Arrange & Act
            var viewModel = new CreatePictureViewModel();

            // Assert
            Assert.NotNull(viewModel);
        }

        [Fact]
        public void CreatePictureViewModel_Name_CanSetAndGet()
        {
            // Arrange
            var viewModel = new CreatePictureViewModel();
            string expectedName = "Test Picture";

            // Act
            viewModel.Name = expectedName;
            var actualName = viewModel.Name;

            // Assert
            Assert.Equal(expectedName, actualName);
        }

        [Fact]
        public void CreatePictureViewModel_Name_DefaultValueIsNull()
        {
            // Arrange & Act
            var viewModel = new CreatePictureViewModel();

            // Assert
            Assert.Null(viewModel.Name);
        }

        [Fact]
        public void CreatePictureViewModel_Description_CanSetAndGet()
        {
            // Arrange
            var viewModel = new CreatePictureViewModel();
            string expectedDescription = "Test Description";

            // Act
            viewModel.Description = expectedDescription;
            var actualDescription = viewModel.Description;

            // Assert
            Assert.Equal(expectedDescription, actualDescription);
        }

        [Fact]
        public void CreatePictureViewModel_Description_DefaultValueIsNull()
        {
            // Arrange & Act
            var viewModel = new CreatePictureViewModel();

            // Assert
            Assert.Null(viewModel.Description);
        }

        [Fact]
        public void CreatePictureViewModel_GalleryID_CanSetAndGet()
        {
            // Arrange
            var viewModel = new CreatePictureViewModel();
            int expectedGalleryId = 1;

            // Act
            viewModel.GalleryID = expectedGalleryId;
            var actualGalleryId = viewModel.GalleryID;

            // Assert
            Assert.Equal(expectedGalleryId, actualGalleryId);
        }

        [Fact]
        public void CreatePictureViewModel_GalleryID_DefaultValueIsZero()
        {
            // Arrange & Act
            var viewModel = new CreatePictureViewModel();

            // Assert
            Assert.Equal(0, viewModel.GalleryID);
        }

        [Fact]
        public void CreatePictureViewModel_GalleryTypes_CanSetAndGet()
        {
            // Arrange
            var viewModel = new CreatePictureViewModel();
            var galleryTypes = new List<GalleryType>
            {
                new GalleryType { GalleryID = 1, GalleryName = "Gallery1" },
                new GalleryType { GalleryID = 2, GalleryName = "Gallery2" }
            };

            // Act
            viewModel.GalleryTypes = galleryTypes;
            var actualGalleryTypes = viewModel.GalleryTypes;

            // Assert
            Assert.NotNull(actualGalleryTypes);
            Assert.Equal(2, actualGalleryTypes.Count);
        }

        [Fact]
        public void CreatePictureViewModel_GalleryTypes_DefaultValueIsNull()
        {
            // Arrange & Act
            var viewModel = new CreatePictureViewModel();

            // Assert
            Assert.Null(viewModel.GalleryTypes);
        }

        [Fact]
        public void CreatePictureViewModel_CanInitializeWithObjectInitializer()
        {
            // Arrange & Act
            var viewModel = new CreatePictureViewModel
            {
                Name = "Test",
                Description = "Description",
                GalleryID = 1,
                GalleryTypes = new List<GalleryType>()
            };

            // Assert
            Assert.Equal("Test", viewModel.Name);
            Assert.Equal("Description", viewModel.Description);
            Assert.Equal(1, viewModel.GalleryID);
            Assert.NotNull(viewModel.GalleryTypes);
        }
    }
}
