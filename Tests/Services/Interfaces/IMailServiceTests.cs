using Xunit;
using System;
using Services.Interfaces;

namespace Tests.Services.Interfaces
{
    public class IMailServiceTests
    {
        [Fact]
        public void IMailService_IsInterface()
        {
            // Arrange
            var type = typeof(IMailService);

            // Act & Assert
            Assert.True(type.IsInterface);
        }

        [Fact]
        public void IMailService_HasSendMessageMethod()
        {
            // Arrange
            var type = typeof(IMailService);

            // Act
            var method = type.GetMethod("SendMessage");

            // Assert
            Assert.NotNull(method);
            Assert.Equal(typeof(void), method.ReturnType);
        }

        [Fact]
        public void IMailService_SendMessageMethod_HasCorrectParameters()
        {
            // Arrange
            var type = typeof(IMailService);

            // Act
            var method = type.GetMethod("SendMessage");
            var parameters = method.GetParameters();

            // Assert
            Assert.Equal(4, parameters.Length);
            Assert.Equal(typeof(string), parameters[0].ParameterType);
            Assert.Equal("senderEmail", parameters[0].Name);
            Assert.Equal(typeof(string), parameters[1].ParameterType);
            Assert.Equal("subject", parameters[1].Name);
            Assert.Equal(typeof(string), parameters[2].ParameterType);
            Assert.Equal("senderName", parameters[2].Name);
            Assert.Equal(typeof(string), parameters[3].ParameterType);
            Assert.Equal("message", parameters[3].Name);
        }
    }
}
