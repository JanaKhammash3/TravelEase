
using System.Net.Mail;

namespace TravelEase.Application.Interfaces;
public interface ISmtpClientWrapper : IDisposable
{
    Task SendMailAsync(MailMessage message);
}