using Xunit;
using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.Linq;
using WebGallery.ViewModel;

namespace Tests.WebGallery.ViewModel
{
    public class CreateUserViewModelTests
    {
        [Fact]
        public void CreateUserViewModel_Constructor_CreatesInstance()
        {
            // Arrange & Act
            var viewModel = new CreateUserViewModel();

            // Assert
            Assert.NotNull(viewModel);
        }

        [Fact]
        public void CreateUserViewModel_UserName_CanSetAndGet()
        {
            // Arrange
            var viewModel = new CreateUserViewModel();
            string expectedUserName = "testuser";

            // Act
            viewModel.UserName = expectedUserName;
            var actualUserName = viewModel.UserName;

            // Assert
            Assert.Equal(expectedUserName, actualUserName);
        }

        [Fact]
        public void CreateUserViewModel_UserName_DefaultValueIsNull()
        {
            // Arrange & Act
            var viewModel = new CreateUserViewModel();

            // Assert
            Assert.Null(viewModel.UserName);
        }

        [Fact]
        public void CreateUserViewModel_Password_CanSetAndGet()
        {
            // Arrange
            var viewModel = new CreateUserViewModel();
            string expectedPassword = "password123";

            // Act
            viewModel.Password = expectedPassword;
            var actualPassword = viewModel.Password;

            // Assert
            Assert.Equal(expectedPassword, actualPassword);
        }

        [Fact]
        public void CreateUserViewModel_Password_DefaultValueIsNull()
        {
            // Arrange & Act
            var viewModel = new CreateUserViewModel();

            // Assert
            Assert.Null(viewModel.Password);
        }

        [Fact]
        public void CreateUserViewModel_ConfirmPassword_CanSetAndGet()
        {
            // Arrange
            var viewModel = new CreateUserViewModel();
            string expectedConfirmPassword = "password123";

            // Act
            viewModel.ConfirmPassword = expectedConfirmPassword;
            var actualConfirmPassword = viewModel.ConfirmPassword;

            // Assert
            Assert.Equal(expectedConfirmPassword, actualConfirmPassword);
        }

        [Fact]
        public void CreateUserViewModel_ConfirmPassword_DefaultValueIsNull()
        {
            // Arrange & Act
            var viewModel = new CreateUserViewModel();

            // Assert
            Assert.Null(viewModel.ConfirmPassword);
        }

        [Fact]
        public void CreateUserViewModel_UserName_HasRequiredAttribute()
        {
            // Arrange
            var property = typeof(CreateUserViewModel).GetProperty("UserName");

            // Act
            var attribute = property.GetCustomAttributes(typeof(RequiredAttribute), false).FirstOrDefault();

            // Assert
            Assert.NotNull(attribute);
        }

        [Fact]
        public void CreateUserViewModel_Password_HasRequiredAttribute()
        {
            // Arrange
            var property = typeof(CreateUserViewModel).GetProperty("Password");

            // Act
            var attribute = property.GetCustomAttributes(typeof(RequiredAttribute), false).FirstOrDefault();

            // Assert
            Assert.NotNull(attribute);
        }

        [Fact]
        public void CreateUserViewModel_Password_HasStringLengthAttribute()
        {
            // Arrange
            var property = typeof(CreateUserViewModel).GetProperty("Password");

            // Act
            var attribute = property.GetCustomAttributes(typeof(StringLengthAttribute), false).FirstOrDefault() as StringLengthAttribute;

            // Assert
            Assert.NotNull(attribute);
            Assert.Equal(100, attribute.MaximumLength);
            Assert.Equal(6, attribute.MinimumLength);
        }

        [Fact]
        public void CreateUserViewModel_Password_HasDataTypeAttribute()
        {
            // Arrange
            var property = typeof(CreateUserViewModel).GetProperty("Password");

            // Act
            var attribute = property.GetCustomAttributes(typeof(DataTypeAttribute), false).FirstOrDefault() as DataTypeAttribute;

            // Assert
            Assert.NotNull(attribute);
            Assert.Equal(DataType.Password, attribute.DataType);
        }

        [Fact]
        public void CreateUserViewModel_ConfirmPassword_HasRequiredAttribute()
        {
            // Arrange
            var property = typeof(CreateUserViewModel).GetProperty("ConfirmPassword");

            // Act
            var attribute = property.GetCustomAttributes(typeof(RequiredAttribute), false).FirstOrDefault();

            // Assert
            Assert.NotNull(attribute);
        }

        [Fact]
        public void CreateUserViewModel_ConfirmPassword_HasCompareAttribute()
        {
            // Arrange
            var property = typeof(CreateUserViewModel).GetProperty("ConfirmPassword");

            // Act
            var attribute = property.GetCustomAttributes(typeof(CompareAttribute), false).FirstOrDefault() as CompareAttribute;

            // Assert
            Assert.NotNull(attribute);
            Assert.Equal("Password", attribute.OtherProperty);
        }

        [Fact]
        public void CreateUserViewModel_ConfirmPassword_HasDataTypeAttribute()
        {
            // Arrange
            var property = typeof(CreateUserViewModel).GetProperty("ConfirmPassword");

            // Act
            var attribute = property.GetCustomAttributes(typeof(DataTypeAttribute), false).FirstOrDefault() as DataTypeAttribute;

            // Assert
            Assert.NotNull(attribute);
            Assert.Equal(DataType.Password, attribute.DataType);
        }

        [Fact]
        public void CreateUserViewModel_CanInitializeWithObjectInitializer()
        {
            // Arrange & Act
            var viewModel = new CreateUserViewModel
            {
                UserName = "testuser",
                Password = "password123",
                ConfirmPassword = "password123"
            };

            // Assert
            Assert.Equal("testuser", viewModel.UserName);
            Assert.Equal("password123", viewModel.Password);
            Assert.Equal("password123", viewModel.ConfirmPassword);
        }
    }
}
