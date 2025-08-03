using System.Net;
using System.Net.Mail;
using TravelEase.Application.Interfaces;
using TravelEase.TravelEase.Application.Interfaces;

namespace TravelEase.Infrastructure.Services;

public class SmtpClientWrapper : ISmtpClientWrapper
{
    private readonly SmtpClient _client;

    public SmtpClientWrapper(string host, int port, string user, string pass)
    {
        _client = new SmtpClient(host, port)
        {
            EnableSsl = true,
            Credentials = new NetworkCredential(user, pass)
        };
    }

    public Task SendMailAsync(MailMessage message) => _client.SendMailAsync(message);

    public void Dispose() => _client.Dispose();
}