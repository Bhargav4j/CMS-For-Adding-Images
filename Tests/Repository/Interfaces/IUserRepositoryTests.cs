using Xunit;
using System;
using Repository.Interfaces;
using Repository.POCO;

namespace Tests.Repository.Interfaces
{
    public class IUserRepositoryTests
    {
        [Fact]
        public void IUserRepository_IsInterface()
        {
            // Arrange
            var type = typeof(IUserRepository);

            // Act & Assert
            Assert.True(type.IsInterface);
        }

        [Fact]
        public void IUserRepository_HasUserIsInRoleMethod()
        {
            // Arrange
            var type = typeof(IUserRepository);

            // Act
            var method = type.GetMethod("UserIsInRole");

            // Assert
            Assert.NotNull(method);
            Assert.Equal(typeof(bool), method.ReturnType);
        }

        [Fact]
        public void IUserRepository_HasCreateUserMethod()
        {
            // Arrange
            var type = typeof(IUserRepository);

            // Act
            var method = type.GetMethod("CreateUser");

            // Assert
            Assert.NotNull(method);
            Assert.Equal(typeof(OperationResult), method.ReturnType);
        }

        [Fact]
        public void IUserRepository_HasLoginUserMethod()
        {
            // Arrange
            var type = typeof(IUserRepository);

            // Act
            var method = type.GetMethod("LoginUser");

            // Assert
            Assert.NotNull(method);
            Assert.Equal(typeof(OperationResult), method.ReturnType);
        }

        [Fact]
        public void IUserRepository_UserIsInRoleMethod_HasCorrectParameters()
        {
            // Arrange
            var type = typeof(IUserRepository);

            // Act
            var method = type.GetMethod("UserIsInRole");
            var parameters = method.GetParameters();

            // Assert
            Assert.Equal(2, parameters.Length);
            Assert.Equal(typeof(string), parameters[0].ParameterType);
            Assert.Equal(typeof(string), parameters[1].ParameterType);
        }

        [Fact]
        public void IUserRepository_CreateUserMethod_HasCorrectParameters()
        {
            // Arrange
            var type = typeof(IUserRepository);

            // Act
            var method = type.GetMethod("CreateUser");
            var parameters = method.GetParameters();

            // Assert
            Assert.Equal(3, parameters.Length);
            Assert.Equal(typeof(string), parameters[0].ParameterType);
            Assert.Equal(typeof(string), parameters[1].ParameterType);
        }

        [Fact]
        public void IUserRepository_LoginUserMethod_HasCorrectParameters()
        {
            // Arrange
            var type = typeof(IUserRepository);

            // Act
            var method = type.GetMethod("LoginUser");
            var parameters = method.GetParameters();

            // Assert
            Assert.Equal(3, parameters.Length);
            Assert.Equal(typeof(string), parameters[0].ParameterType);
            Assert.Equal(typeof(string), parameters[1].ParameterType);
        }
    }
}
