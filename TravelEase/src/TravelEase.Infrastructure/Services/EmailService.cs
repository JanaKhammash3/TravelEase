using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;
using TravelEase.TravelEase.Application.Interfaces;

namespace TravelEase.TravelEase.Infrastructure.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendEmailAsync(string to, string subject, string body)
        {
            var smtpHost = _configuration["Email:SmtpHost"];
            var smtpPort = int.Parse(_configuration["Email:SmtpPort"]);
            var smtpUser = _configuration["Email:SmtpUser"];
            var smtpPass = _configuration["Email:SmtpPass"];
            var fromEmail = _configuration["Email:From"];

            using var client = new SmtpClient(smtpHost, smtpPort)
            {
                EnableSsl = true,
                Credentials = new NetworkCredential(smtpUser, smtpPass)
            };

            var message = new MailMessage(fromEmail, to, subject, body)
            {
                IsBodyHtml = false
            };

            await client.SendMailAsync(message);
        }
        
        public async Task SendEmailWithAttachmentAsync(string to, string subject, string body, byte[] attachmentData, string fileName)
        {
            var smtpHost = _configuration["Email:SmtpHost"];
            var smtpPort = int.Parse(_configuration["Email:SmtpPort"]);
            var smtpUser = _configuration["Email:SmtpUser"];
            var smtpPass = _configuration["Email:SmtpPass"];
            var fromEmail = _configuration["Email:From"];

            using var client = new SmtpClient(smtpHost, smtpPort)
            {
                EnableSsl = true,
                Credentials = new NetworkCredential(smtpUser, smtpPass)
            };

            var message = new MailMessage(fromEmail, to, subject, body)
            {
                IsBodyHtml = false
            };

            message.Attachments.Add(new Attachment(new MemoryStream(attachmentData), fileName));

            await client.SendMailAsync(message);
        }

    }
}