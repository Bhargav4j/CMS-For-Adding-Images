using Xunit;
using System;
using System.Linq;
using Services.Interfaces;

namespace Tests.Services.Interfaces
{
    public class IMappingServiceTests
    {
        [Fact]
        public void IMappingService_IsInterface()
        {
            // Arrange
            var type = typeof(IMappingService);

            // Act & Assert
            Assert.True(type.IsInterface);
        }

        [Fact]
        public void IMappingService_HasMapMethod()
        {
            // Arrange
            var type = typeof(IMappingService);

            // Act
            var method = type.GetMethod("Map");

            // Assert
            Assert.NotNull(method);
        }

        [Fact]
        public void IMappingService_MapMethod_IsGeneric()
        {
            // Arrange
            var type = typeof(IMappingService);

            // Act
            var method = type.GetMethod("Map");

            // Assert
            Assert.True(method.IsGenericMethod);
        }

        [Fact]
        public void IMappingService_MapMethod_HasTwoGenericParameters()
        {
            // Arrange
            var type = typeof(IMappingService);

            // Act
            var method = type.GetMethod("Map");
            var genericArguments = method.GetGenericArguments();

            // Assert
            Assert.Equal(2, genericArguments.Length);
        }

        [Fact]
        public void IMappingService_MapMethod_HasOneParameter()
        {
            // Arrange
            var type = typeof(IMappingService);

            // Act
            var method = type.GetMethod("Map");
            var parameters = method.GetParameters();

            // Assert
            Assert.Equal(1, parameters.Length);
        }

        [Fact]
        public void IMappingService_MapMethod_GenericParametersHaveCorrectNames()
        {
            // Arrange
            var type = typeof(IMappingService);

            // Act
            var method = type.GetMethod("Map");
            var genericArguments = method.GetGenericArguments();

            // Assert
            Assert.Equal("TSrc", genericArguments[0].Name);
            Assert.Equal("TDest", genericArguments[1].Name);
        }

        [Fact]
        public void IMappingService_MapMethod_TDestHasClassConstraint()
        {
            // Arrange
            var type = typeof(IMappingService);

            // Act
            var method = type.GetMethod("Map");
            var genericArguments = method.GetGenericArguments();
            var tDestConstraints = genericArguments[1].GetGenericParameterConstraints();
            var tDestAttributes = genericArguments[1].GenericParameterAttributes;

            // Assert
            Assert.True((tDestAttributes & System.Reflection.GenericParameterAttributes.ReferenceTypeConstraint) != 0);
        }
    }
}
