using Xunit;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using WebGallery.ViewModel;

namespace Tests.WebGallery.ViewModel
{
    public class LoginViewModelTests
    {
        [Fact]
        public void LoginViewModel_Constructor_CreatesInstance()
        {
            // Arrange & Act
            var viewModel = new LoginViewModel();

            // Assert
            Assert.NotNull(viewModel);
        }

        [Fact]
        public void LoginViewModel_UserName_CanSetAndGet()
        {
            // Arrange
            var viewModel = new LoginViewModel();
            string expectedUserName = "testuser";

            // Act
            viewModel.UserName = expectedUserName;
            var actualUserName = viewModel.UserName;

            // Assert
            Assert.Equal(expectedUserName, actualUserName);
        }

        [Fact]
        public void LoginViewModel_UserName_DefaultValueIsNull()
        {
            // Arrange & Act
            var viewModel = new LoginViewModel();

            // Assert
            Assert.Null(viewModel.UserName);
        }

        [Fact]
        public void LoginViewModel_Password_CanSetAndGet()
        {
            // Arrange
            var viewModel = new LoginViewModel();
            string expectedPassword = "password123";

            // Act
            viewModel.Password = expectedPassword;
            var actualPassword = viewModel.Password;

            // Assert
            Assert.Equal(expectedPassword, actualPassword);
        }

        [Fact]
        public void LoginViewModel_Password_DefaultValueIsNull()
        {
            // Arrange & Act
            var viewModel = new LoginViewModel();

            // Assert
            Assert.Null(viewModel.Password);
        }

        [Fact]
        public void LoginViewModel_UserName_HasRequiredAttribute()
        {
            // Arrange
            var property = typeof(LoginViewModel).GetProperty("UserName");

            // Act
            var attribute = property.GetCustomAttributes(typeof(RequiredAttribute), false).FirstOrDefault();

            // Assert
            Assert.NotNull(attribute);
        }

        [Fact]
        public void LoginViewModel_Password_HasRequiredAttribute()
        {
            // Arrange
            var property = typeof(LoginViewModel).GetProperty("Password");

            // Act
            var attribute = property.GetCustomAttributes(typeof(RequiredAttribute), false).FirstOrDefault();

            // Assert
            Assert.NotNull(attribute);
        }

        [Fact]
        public void LoginViewModel_Password_HasDataTypeAttribute()
        {
            // Arrange
            var property = typeof(LoginViewModel).GetProperty("Password");

            // Act
            var attribute = property.GetCustomAttributes(typeof(DataTypeAttribute), false).FirstOrDefault() as DataTypeAttribute;

            // Assert
            Assert.NotNull(attribute);
            Assert.Equal(DataType.Password, attribute.DataType);
        }

        [Fact]
        public void LoginViewModel_CanSetNullUserName()
        {
            // Arrange
            var viewModel = new LoginViewModel { UserName = "test" };

            // Act
            viewModel.UserName = null;

            // Assert
            Assert.Null(viewModel.UserName);
        }

        [Fact]
        public void LoginViewModel_CanSetNullPassword()
        {
            // Arrange
            var viewModel = new LoginViewModel { Password = "test" };

            // Act
            viewModel.Password = null;

            // Assert
            Assert.Null(viewModel.Password);
        }

        [Fact]
        public void LoginViewModel_CanInitializeWithObjectInitializer()
        {
            // Arrange & Act
            var viewModel = new LoginViewModel
            {
                UserName = "testuser",
                Password = "password123"
            };

            // Assert
            Assert.Equal("testuser", viewModel.UserName);
            Assert.Equal("password123", viewModel.Password);
        }
    }
}
