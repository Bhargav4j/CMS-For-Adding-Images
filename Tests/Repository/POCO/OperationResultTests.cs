using Xunit;
using System;
using Repository.POCO;

namespace Tests.Repository.POCO
{
    public class OperationResultTests
    {
        [Fact]
        public void OperationResult_Constructor_CreatesInstance()
        {
            // Arrange & Act
            var result = new OperationResult();

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public void OperationResult_Message_CanSetAndGet()
        {
            // Arrange
            var result = new OperationResult();
            string expectedMessage = "Operation completed successfully";

            // Act
            result.Message = expectedMessage;
            var actualMessage = result.Message;

            // Assert
            Assert.Equal(expectedMessage, actualMessage);
        }

        [Fact]
        public void OperationResult_Message_DefaultValueIsNull()
        {
            // Arrange & Act
            var result = new OperationResult();

            // Assert
            Assert.Null(result.Message);
        }

        [Fact]
        public void OperationResult_Message_CanSetNull()
        {
            // Arrange
            var result = new OperationResult { Message = "Test" };

            // Act
            result.Message = null;

            // Assert
            Assert.Null(result.Message);
        }

        [Fact]
        public void OperationResult_Message_CanSetEmptyString()
        {
            // Arrange
            var result = new OperationResult();

            // Act
            result.Message = "";

            // Assert
            Assert.Equal("", result.Message);
        }

        [Fact]
        public void OperationResult_Succeded_CanSetAndGet()
        {
            // Arrange
            var result = new OperationResult();

            // Act
            result.Succeded = true;
            var actualSucceded = result.Succeded;

            // Assert
            Assert.True(actualSucceded);
        }

        [Fact]
        public void OperationResult_Succeded_DefaultValueIsFalse()
        {
            // Arrange & Act
            var result = new OperationResult();

            // Assert
            Assert.False(result.Succeded);
        }

        [Fact]
        public void OperationResult_Succeded_CanSetFalse()
        {
            // Arrange
            var result = new OperationResult { Succeded = true };

            // Act
            result.Succeded = false;

            // Assert
            Assert.False(result.Succeded);
        }

        [Fact]
        public void OperationResult_CanSetBothProperties()
        {
            // Arrange
            var result = new OperationResult();
            string expectedMessage = "Error occurred";
            bool expectedSucceded = false;

            // Act
            result.Message = expectedMessage;
            result.Succeded = expectedSucceded;

            // Assert
            Assert.Equal(expectedMessage, result.Message);
            Assert.Equal(expectedSucceded, result.Succeded);
        }

        [Fact]
        public void OperationResult_CanInitializeWithObjectInitializer()
        {
            // Arrange
            string expectedMessage = "Success";
            bool expectedSucceded = true;

            // Act
            var result = new OperationResult
            {
                Message = expectedMessage,
                Succeded = expectedSucceded
            };

            // Assert
            Assert.Equal(expectedMessage, result.Message);
            Assert.Equal(expectedSucceded, result.Succeded);
        }
    }
}
