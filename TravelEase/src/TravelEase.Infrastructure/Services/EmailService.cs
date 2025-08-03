using System.Net.Mail;
using Microsoft.Extensions.Configuration;
using TravelEase.TravelEase.Application.Interfaces;

namespace TravelEase.TravelEase.Infrastructure.Services;

public class EmailService : IEmailService
{
    private readonly IConfiguration _config;
    private readonly Func<ISmtpClientWrapper> _smtpFactory;

    public EmailService(IConfiguration config, Func<ISmtpClientWrapper> smtpFactory)
    {
        _config = config;
        _smtpFactory = smtpFactory;
    }

    public async Task SendEmailAsync(string to, string subject, string body)
    {
        var fromEmail = _config["Email:From"];
        if (string.IsNullOrWhiteSpace(fromEmail))
            throw new InvalidOperationException("Missing 'Email:From' configuration in appsettings.json.");

        using var client = _smtpFactory();
        var message = new MailMessage(fromEmail, to, subject, body) { IsBodyHtml = false };
        await client.SendMailAsync(message);
    }

    public async Task SendEmailWithAttachmentAsync(string to, string subject, string body, byte[] attachmentData, string fileName)
    {
        var fromEmail = _config["Email:From"];
        if (string.IsNullOrWhiteSpace(fromEmail))
            throw new InvalidOperationException("Missing 'Email:From' configuration in appsettings.json.");

        using var client = _smtpFactory();
        var message = new MailMessage(fromEmail, to, subject, body) { IsBodyHtml = false };
        message.Attachments.Add(new Attachment(new MemoryStream(attachmentData), fileName));
        await client.SendMailAsync(message);
    }

}