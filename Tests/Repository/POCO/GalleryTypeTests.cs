using Xunit;
using System;
using System.Collections.Generic;
using Repository.POCO;

namespace Tests.Repository.POCO
{
    public class GalleryTypeTests
    {
        [Fact]
        public void GalleryType_Constructor_CreatesInstance()
        {
            // Arrange & Act
            var galleryType = new GalleryType();

            // Assert
            Assert.NotNull(galleryType);
        }

        [Fact]
        public void GalleryType_GalleryID_CanSetAndGet()
        {
            // Arrange
            var galleryType = new GalleryType();
            int expectedId = 1;

            // Act
            galleryType.GalleryID = expectedId;
            var actualId = galleryType.GalleryID;

            // Assert
            Assert.Equal(expectedId, actualId);
        }

        [Fact]
        public void GalleryType_GalleryID_DefaultValueIsZero()
        {
            // Arrange & Act
            var galleryType = new GalleryType();

            // Assert
            Assert.Equal(0, galleryType.GalleryID);
        }

        [Fact]
        public void GalleryType_GalleryName_CanSetAndGet()
        {
            // Arrange
            var galleryType = new GalleryType();
            string expectedName = "Test Gallery";

            // Act
            galleryType.GalleryName = expectedName;
            var actualName = galleryType.GalleryName;

            // Assert
            Assert.Equal(expectedName, actualName);
        }

        [Fact]
        public void GalleryType_GalleryName_DefaultValueIsNull()
        {
            // Arrange & Act
            var galleryType = new GalleryType();

            // Assert
            Assert.Null(galleryType.GalleryName);
        }

        [Fact]
        public void GalleryType_GalleryName_CanSetNull()
        {
            // Arrange
            var galleryType = new GalleryType { GalleryName = "Test" };

            // Act
            galleryType.GalleryName = null;

            // Assert
            Assert.Null(galleryType.GalleryName);
        }

        [Fact]
        public void GalleryType_GalleryName_CanSetEmptyString()
        {
            // Arrange
            var galleryType = new GalleryType();

            // Act
            galleryType.GalleryName = "";

            // Assert
            Assert.Equal("", galleryType.GalleryName);
        }

        [Fact]
        public void GalleryType_Pictures_CanSetAndGet()
        {
            // Arrange
            var galleryType = new GalleryType();
            var pictures = new List<Picture>
            {
                new Picture { PictureID = 1, Name = "Picture1" },
                new Picture { PictureID = 2, Name = "Picture2" }
            };

            // Act
            galleryType.Pictures = pictures;
            var actualPictures = galleryType.Pictures;

            // Assert
            Assert.NotNull(actualPictures);
            Assert.Equal(2, actualPictures.Count);
        }

        [Fact]
        public void GalleryType_Pictures_DefaultValueIsNull()
        {
            // Arrange & Act
            var galleryType = new GalleryType();

            // Assert
            Assert.Null(galleryType.Pictures);
        }

        [Fact]
        public void GalleryType_Pictures_CanSetNull()
        {
            // Arrange
            var galleryType = new GalleryType { Pictures = new List<Picture>() };

            // Act
            galleryType.Pictures = null;

            // Assert
            Assert.Null(galleryType.Pictures);
        }

        [Fact]
        public void GalleryType_Pictures_IsVirtual()
        {
            // Arrange
            var type = typeof(GalleryType);
            var property = type.GetProperty("Pictures");

            // Act
            var isVirtual = property.GetGetMethod().IsVirtual;

            // Assert
            Assert.True(isVirtual);
        }
    }
}
