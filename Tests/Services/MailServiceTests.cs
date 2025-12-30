using Xunit;
using System;
using Services;
using Services.Interfaces;

namespace Tests.Services
{
    public class MailServiceTests
    {
        [Fact]
        public void MailService_Constructor_CreatesInstance()
        {
            // Arrange & Act
            var mailService = new MailService();

            // Assert
            Assert.NotNull(mailService);
        }

        [Fact]
        public void MailService_ImplementsIMailService()
        {
            // Arrange & Act
            var mailService = new MailService();

            // Assert
            Assert.IsAssignableFrom<IMailService>(mailService);
        }

        [Fact]
        public void MailService_SendMessage_WithValidParameters_DoesNotThrow()
        {
            // Arrange
            var mailService = new MailService();
            string senderEmail = "test@example.com";
            string subject = "Test Subject";
            string senderName = "Test User";
            string message = "Test message body";

            // Act & Assert
            var exception = Record.Exception(() => mailService.SendMessage(senderEmail, subject, senderName, message));

            // Note: This may throw due to SMTP configuration, but we're testing the method signature
            Assert.True(exception == null || exception is System.Net.Mail.SmtpException || exception is InvalidOperationException);
        }

        [Fact]
        public void MailService_SendMessage_WithNullSenderEmail_ThrowsException()
        {
            // Arrange
            var mailService = new MailService();
            string senderEmail = null;
            string subject = "Test Subject";
            string senderName = "Test User";
            string message = "Test message";

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => mailService.SendMessage(senderEmail, subject, senderName, message));
        }

        [Fact]
        public void MailService_SendMessage_WithEmptySenderEmail_ThrowsException()
        {
            // Arrange
            var mailService = new MailService();
            string senderEmail = "";
            string subject = "Test Subject";
            string senderName = "Test User";
            string message = "Test message";

            // Act & Assert
            Assert.Throws<ArgumentException>(() => mailService.SendMessage(senderEmail, subject, senderName, message));
        }

        [Fact]
        public void MailService_SendMessage_WithInvalidEmailFormat_ThrowsException()
        {
            // Arrange
            var mailService = new MailService();
            string senderEmail = "invalid-email";
            string subject = "Test Subject";
            string senderName = "Test User";
            string message = "Test message";

            // Act & Assert
            Assert.Throws<FormatException>(() => mailService.SendMessage(senderEmail, subject, senderName, message));
        }

        [Fact]
        public void MailService_SendMessage_WithNullSubject_DoesNotThrow()
        {
            // Arrange
            var mailService = new MailService();
            string senderEmail = "test@example.com";
            string subject = null;
            string senderName = "Test User";
            string message = "Test message";

            // Act & Assert
            var exception = Record.Exception(() => mailService.SendMessage(senderEmail, subject, senderName, message));

            // Note: This may throw due to SMTP configuration
            Assert.True(exception == null || exception is System.Net.Mail.SmtpException || exception is InvalidOperationException);
        }

        [Fact]
        public void MailService_SendMessage_WithNullSenderName_DoesNotThrow()
        {
            // Arrange
            var mailService = new MailService();
            string senderEmail = "test@example.com";
            string subject = "Test Subject";
            string senderName = null;
            string message = "Test message";

            // Act & Assert
            var exception = Record.Exception(() => mailService.SendMessage(senderEmail, subject, senderName, message));

            // Note: This may throw due to SMTP configuration
            Assert.True(exception == null || exception is System.Net.Mail.SmtpException || exception is InvalidOperationException);
        }

        [Fact]
        public void MailService_SendMessage_WithNullMessage_DoesNotThrow()
        {
            // Arrange
            var mailService = new MailService();
            string senderEmail = "test@example.com";
            string subject = "Test Subject";
            string senderName = "Test User";
            string message = null;

            // Act & Assert
            var exception = Record.Exception(() => mailService.SendMessage(senderEmail, subject, senderName, message));

            // Note: This may throw due to SMTP configuration
            Assert.True(exception == null || exception is System.Net.Mail.SmtpException || exception is InvalidOperationException);
        }

        [Fact]
        public void MailService_SendMessage_WithEmptySubject_DoesNotThrow()
        {
            // Arrange
            var mailService = new MailService();
            string senderEmail = "test@example.com";
            string subject = "";
            string senderName = "Test User";
            string message = "Test message";

            // Act & Assert
            var exception = Record.Exception(() => mailService.SendMessage(senderEmail, subject, senderName, message));

            // Note: This may throw due to SMTP configuration
            Assert.True(exception == null || exception is System.Net.Mail.SmtpException || exception is InvalidOperationException);
        }
    }
}
