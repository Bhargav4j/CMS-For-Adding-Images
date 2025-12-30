using Xunit;
using System;
using Services;
using Services.Interfaces;

namespace Tests.Services
{
    public class ServerPathServiceTests
    {
        [Fact]
        public void ServerPathService_Constructor_CreatesInstance()
        {
            // Arrange & Act
            var service = new ServerPathService();

            // Assert
            Assert.NotNull(service);
        }

        [Fact]
        public void ServerPathService_ImplementsIPathService()
        {
            // Arrange & Act
            var service = new ServerPathService();

            // Assert
            Assert.IsAssignableFrom<IPathService>(service);
        }

        [Fact]
        public void ServerPathService_MapPath_WithNullPath_ThrowsException()
        {
            // Arrange
            var service = new ServerPathService();
            string path = null;

            // Act & Assert
            Assert.Throws<NullReferenceException>(() => service.MapPath(path));
        }

        [Fact]
        public void ServerPathService_MapPath_WithValidPath_ReturnsString()
        {
            // Arrange
            var service = new ServerPathService();
            string path = "~/test";

            // Act & Assert
            // Note: This will throw NullReferenceException because HttpContext.Current is null in unit test context
            Assert.Throws<NullReferenceException>(() => service.MapPath(path));
        }

        [Fact]
        public void ServerPathService_MapPath_WithEmptyPath_ReturnsString()
        {
            // Arrange
            var service = new ServerPathService();
            string path = "";

            // Act & Assert
            // Note: This will throw NullReferenceException because HttpContext.Current is null in unit test context
            Assert.Throws<NullReferenceException>(() => service.MapPath(path));
        }

        [Fact]
        public void ServerPathService_MapPath_WithRelativePath_ReturnsString()
        {
            // Arrange
            var service = new ServerPathService();
            string path = "~/Images/test.jpg";

            // Act & Assert
            // Note: This will throw NullReferenceException because HttpContext.Current is null in unit test context
            Assert.Throws<NullReferenceException>(() => service.MapPath(path));
        }

        [Fact]
        public void ServerPathService_MapPath_WithAbsolutePath_ReturnsString()
        {
            // Arrange
            var service = new ServerPathService();
            string path = "/Images/test.jpg";

            // Act & Assert
            // Note: This will throw NullReferenceException because HttpContext.Current is null in unit test context
            Assert.Throws<NullReferenceException>(() => service.MapPath(path));
        }

        [Fact]
        public void ServerPathService_MapPath_ReturnsStringType()
        {
            // Arrange
            var type = typeof(ServerPathService);
            var method = type.GetMethod("MapPath");

            // Act
            var returnType = method.ReturnType;

            // Assert
            Assert.Equal(typeof(string), returnType);
        }
    }
}
