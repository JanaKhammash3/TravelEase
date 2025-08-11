using System.Net.Mail;
using System.Text;
using Microsoft.Extensions.Configuration;
using Moq;
using TravelEase.TravelEase.Application.Interfaces;
using TravelEase.TravelEase.Infrastructure.Services;

namespace TravelEase.TravelEase.Tests.UserUnitTests;

public class EmailServiceTests
{
    private readonly Mock<IConfiguration> _mockConfig;
    private readonly Mock<ISmtpClientWrapper> _mockSmtp;
    private readonly EmailService _emailService;

    public EmailServiceTests()
    {
        _mockConfig = new Mock<IConfiguration>();
        _mockSmtp = new Mock<ISmtpClientWrapper>();

        _mockConfig.Setup(c => c["Email:From"]).Returns("test@demo.com");

        _emailService = new EmailService(_mockConfig.Object, () => _mockSmtp.Object);
    }

    [Fact]
    public async Task SendEmailAsync_Sends_Email()
    {
        // Arrange
        string to = "user@example.com";
        string subject = "Test";
        string body = "Body";

        // Act
        await _emailService.SendEmailAsync(to, subject, body);

        // Assert
        _mockSmtp.Verify(smtp => smtp.SendMailAsync(It.Is<MailMessage>(
            msg => msg.To[0].Address == to &&
                   msg.Subject == subject &&
                   msg.Body == body &&
                   msg.From.Address == "test@demo.com"
        )), Times.Once);
    }

    [Fact]
    public async Task SendEmailWithAttachmentAsync_Sends_Email_With_Attachment()
    {
        // Arrange
        string to = "user@example.com";
        string subject = "Test";
        string body = "Body";
        byte[] data = Encoding.UTF8.GetBytes("sample content");
        string fileName = "test.txt";

        // Act
        await _emailService.SendEmailWithAttachmentAsync(to, subject, body, data, fileName);

        // Assert
        _mockSmtp.Verify(smtp => smtp.SendMailAsync(It.Is<MailMessage>(msg =>
            msg.To[0].Address == to &&
            msg.Attachments.Count == 1 &&
            msg.Attachments[0].Name == fileName
        )), Times.Once);
    }
}