using Xunit;
using System;
using Services.Interfaces;

namespace Tests.Services.Interfaces
{
    public class IPathServiceTests
    {
        [Fact]
        public void IPathService_IsInterface()
        {
            // Arrange
            var type = typeof(IPathService);

            // Act & Assert
            Assert.True(type.IsInterface);
        }

        [Fact]
        public void IPathService_HasMapPathMethod()
        {
            // Arrange
            var type = typeof(IPathService);

            // Act
            var method = type.GetMethod("MapPath");

            // Assert
            Assert.NotNull(method);
            Assert.Equal(typeof(string), method.ReturnType);
        }

        [Fact]
        public void IPathService_MapPathMethod_HasCorrectParameters()
        {
            // Arrange
            var type = typeof(IPathService);

            // Act
            var method = type.GetMethod("MapPath");
            var parameters = method.GetParameters();

            // Assert
            Assert.Equal(1, parameters.Length);
            Assert.Equal(typeof(string), parameters[0].ParameterType);
            Assert.Equal("path", parameters[0].Name);
        }
    }
}
