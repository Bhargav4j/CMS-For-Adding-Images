using Xunit;
using System;
using Repository.DAL.Interfaces;

namespace Tests.Repository.DAL.Interfaces
{
    public class IUnitOfWorkTests
    {
        [Fact]
        public void IUnitOfWork_IsInterface()
        {
            // Arrange
            var type = typeof(IUnitOfWork);

            // Act & Assert
            Assert.True(type.IsInterface);
        }

        [Fact]
        public void IUnitOfWork_HasImagesProperty()
        {
            // Arrange
            var type = typeof(IUnitOfWork);

            // Act
            var property = type.GetProperty("Images");

            // Assert
            Assert.NotNull(property);
            Assert.Equal(typeof(IImagesRepository), property.PropertyType);
        }

        [Fact]
        public void IUnitOfWork_HasUsersProperty()
        {
            // Arrange
            var type = typeof(IUnitOfWork);

            // Act
            var property = type.GetProperty("Users");

            // Assert
            Assert.NotNull(property);
        }

        [Fact]
        public void IUnitOfWork_HasDisposeMethod()
        {
            // Arrange
            var type = typeof(IUnitOfWork);

            // Act
            var method = type.GetMethod("Dispose", Type.EmptyTypes);

            // Assert
            Assert.NotNull(method);
            Assert.Equal(typeof(void), method.ReturnType);
        }

        [Fact]
        public void IUnitOfWork_HasDisposeMethodWithBoolParameter()
        {
            // Arrange
            var type = typeof(IUnitOfWork);

            // Act
            var method = type.GetMethod("Dispose", new[] { typeof(bool) });

            // Assert
            Assert.NotNull(method);
            Assert.Equal(typeof(void), method.ReturnType);
        }

        [Fact]
        public void IUnitOfWork_HasSaveMethod()
        {
            // Arrange
            var type = typeof(IUnitOfWork);

            // Act
            var method = type.GetMethod("Save");

            // Assert
            Assert.NotNull(method);
            Assert.Equal(typeof(int), method.ReturnType);
        }
    }
}
