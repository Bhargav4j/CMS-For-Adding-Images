using Xunit;
using System;
using System.Collections.Generic;
using Repository.DAL.Interfaces;
using Repository.POCO;

namespace Tests.Repository.DAL.Interfaces
{
    public class IImagesRepositoryTests
    {
        [Fact]
        public void IImagesRepository_IsInterface()
        {
            // Arrange
            var type = typeof(IImagesRepository);

            // Act & Assert
            Assert.True(type.IsInterface);
        }

        [Fact]
        public void IImagesRepository_HasDeletePictureMethod()
        {
            // Arrange
            var type = typeof(IImagesRepository);

            // Act
            var method = type.GetMethod("DeletePicture");

            // Assert
            Assert.NotNull(method);
            Assert.Equal(typeof(void), method.ReturnType);
        }

        [Fact]
        public void IImagesRepository_HasGetPictureMethod()
        {
            // Arrange
            var type = typeof(IImagesRepository);

            // Act
            var method = type.GetMethod("GetPicture");

            // Assert
            Assert.NotNull(method);
            Assert.Equal(typeof(Picture), method.ReturnType);
        }

        [Fact]
        public void IImagesRepository_HasGetPicturesByGalleryIDMethod()
        {
            // Arrange
            var type = typeof(IImagesRepository);

            // Act
            var method = type.GetMethod("GetPicturesByGalleryID");

            // Assert
            Assert.NotNull(method);
            Assert.True(method.ReturnType.IsGenericType);
        }

        [Fact]
        public void IImagesRepository_HasInsertPictureMethod()
        {
            // Arrange
            var type = typeof(IImagesRepository);

            // Act
            var method = type.GetMethod("InsertPicture");

            // Assert
            Assert.NotNull(method);
            Assert.Equal(typeof(void), method.ReturnType);
        }

        [Fact]
        public void IImagesRepository_HasUpdatePictureMethod()
        {
            // Arrange
            var type = typeof(IImagesRepository);

            // Act
            var method = type.GetMethod("UpdatePicture");

            // Assert
            Assert.NotNull(method);
            Assert.Equal(typeof(void), method.ReturnType);
        }

        [Fact]
        public void IImagesRepository_HasGetGalleryTypesMethod()
        {
            // Arrange
            var type = typeof(IImagesRepository);

            // Act
            var method = type.GetMethod("GetGalleryTypes");

            // Assert
            Assert.NotNull(method);
            Assert.True(method.ReturnType.IsGenericType || method.ReturnType == typeof(List<GalleryType>));
        }
    }
}
