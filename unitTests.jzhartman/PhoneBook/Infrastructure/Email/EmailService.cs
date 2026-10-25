using MailKit.Net.Smtp;
using Microsoft.Extensions.Options;
using MimeKit;
using PhoneBook.Application.Interfaces;
using PhoneBook.Domain.Validation;

namespace PhoneBook.Infrastructure.Email;

internal class EmailService : IEmailService
{
    private readonly SmtpSettings _settings;

    public EmailService(IOptions<SmtpSettings> settings)
    {
        _settings = settings.Value;
    }

    public async Task<Result> SendEmailAsync(string recipient, string subject, string body)
    {
        try
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(_settings.FromName, _settings.FromEmail));
            message.To.Add(new MailboxAddress(recipient, recipient));
            message.Subject = subject;
            message.Body = new TextPart("plain") { Text = body };

            using var client = new SmtpClient();
            await client.ConnectAsync(_settings.Host, _settings.Port, _settings.UseSsl);
            await client.AuthenticateAsync(_settings.Username, _settings.Password);
            await client.SendAsync(message);
            await client.DisconnectAsync(true);

            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure(new[] { new Error("EmailError", ex.Message) });
        }
    }
}
