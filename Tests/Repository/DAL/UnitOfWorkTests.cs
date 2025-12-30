using Xunit;
using System;
using Repository.DAL;
using Repository.DAL.Interfaces;

namespace Tests.Repository.DAL
{
    public class UnitOfWorkTests
    {
        [Fact]
        public void UnitOfWork_Constructor_CreatesInstance()
        {
            // Arrange & Act
            UnitOfWork unitOfWork = null;
            var exception = Record.Exception(() => unitOfWork = new UnitOfWork());

            // Assert
            Assert.Null(exception);
            Assert.NotNull(unitOfWork);
        }

        [Fact]
        public void UnitOfWork_Images_ReturnsImagesRepository()
        {
            // Arrange
            using (var unitOfWork = new UnitOfWork())
            {
                // Act
                var images = unitOfWork.Images;

                // Assert
                Assert.NotNull(images);
                Assert.IsAssignableFrom<IImagesRepository>(images);
            }
        }

        [Fact]
        public void UnitOfWork_Images_ReturnsSameInstanceOnMultipleCalls()
        {
            // Arrange
            using (var unitOfWork = new UnitOfWork())
            {
                // Act
                var images1 = unitOfWork.Images;
                var images2 = unitOfWork.Images;

                // Assert
                Assert.Same(images1, images2);
            }
        }

        [Fact]
        public void UnitOfWork_Users_ReturnsUserRepository()
        {
            // Arrange
            using (var unitOfWork = new UnitOfWork())
            {
                // Act
                var users = unitOfWork.Users;

                // Assert
                Assert.NotNull(users);
            }
        }

        [Fact]
        public void UnitOfWork_Users_ReturnsSameInstanceOnMultipleCalls()
        {
            // Arrange
            using (var unitOfWork = new UnitOfWork())
            {
                // Act
                var users1 = unitOfWork.Users;
                var users2 = unitOfWork.Users;

                // Assert
                Assert.Same(users1, users2);
            }
        }

        [Fact]
        public void UnitOfWork_Dispose_DisposesSuccessfully()
        {
            // Arrange
            var unitOfWork = new UnitOfWork();

            // Act
            var exception = Record.Exception(() => unitOfWork.Dispose());

            // Assert
            Assert.Null(exception);
        }

        [Fact]
        public void UnitOfWork_DisposeWithTrue_DisposesSuccessfully()
        {
            // Arrange
            var unitOfWork = new UnitOfWork();

            // Act
            var exception = Record.Exception(() => unitOfWork.Dispose(true));

            // Assert
            Assert.Null(exception);
        }

        [Fact]
        public void UnitOfWork_DisposeWithFalse_DisposesSuccessfully()
        {
            // Arrange
            var unitOfWork = new UnitOfWork();

            // Act
            var exception = Record.Exception(() => unitOfWork.Dispose(false));

            // Assert
            Assert.Null(exception);
        }

        [Fact]
        public void UnitOfWork_Save_ReturnsInteger()
        {
            // Arrange
            using (var unitOfWork = new UnitOfWork())
            {
                // Act
                var exception = Record.Exception(() => unitOfWork.Save());

                // Assert
                Assert.Null(exception);
            }
        }

        [Fact]
        public void UnitOfWork_ImplementsIUnitOfWork()
        {
            // Arrange & Act
            using (var unitOfWork = new UnitOfWork())
            {
                // Assert
                Assert.IsAssignableFrom<IUnitOfWork>(unitOfWork);
            }
        }

        [Fact]
        public void UnitOfWork_ImplementsIDisposable()
        {
            // Arrange & Act
            using (var unitOfWork = new UnitOfWork())
            {
                // Assert
                Assert.IsAssignableFrom<IDisposable>(unitOfWork);
            }
        }
    }
}
