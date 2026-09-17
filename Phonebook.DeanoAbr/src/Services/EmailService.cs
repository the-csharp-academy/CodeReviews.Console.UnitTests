using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using Phonebook.Models;

namespace Phonebook.Services;

/// <summary>
/// Sends email with MailKit. Reads SMTP settings from environment variables
/// (SMTP_HOST, SMTP_PORT, SMTP_USER, SMTP_PASS, SMTP_FROM). If they are not set,
/// the service runs in sandbox mode and returns the composed message instead of
/// sending — so the app works out of the box without any credentials.
/// </summary>
public class EmailService
{
    private readonly string? _host = Environment.GetEnvironmentVariable("SMTP_HOST");
    private readonly int _port = int.TryParse(Environment.GetEnvironmentVariable("SMTP_PORT"), out var p) ? p : 587;
    private readonly string? _user = Environment.GetEnvironmentVariable("SMTP_USER");
    private readonly string? _pass = Environment.GetEnvironmentVariable("SMTP_PASS");
    private readonly string? _from = Environment.GetEnvironmentVariable("SMTP_FROM");

    public bool IsConfigured =>
        !string.IsNullOrWhiteSpace(_host) &&
        !string.IsNullOrWhiteSpace(_user) &&
        !string.IsNullOrWhiteSpace(_pass);

    public async Task<(bool Success, string? Message)> SendEmailAsync(Contact to, string subject, string body)
    {
        try
        {
            var message = new MimeMessage();
            message.From.Add(MailboxAddress.Parse(_from ?? "phonebook@localhost"));
            message.To.Add(MailboxAddress.Parse(to.Email));
            message.Subject = subject;
            message.Body = new TextPart("plain") { Text = body };

            if (!IsConfigured)
            {
                return (true,
                    $"Email ready to send (sandbox: set SMTP_HOST, SMTP_USER and SMTP_PASS to send for real).\n" +
                    $"To: {to.Email}\nSubject: {subject}\n\n{body}");
            }

            using var client = new SmtpClient();
            await client.ConnectAsync(_host!, _port, SecureSocketOptions.StartTls);
            await client.AuthenticateAsync(_user!, _pass!);
            await client.SendAsync(message);
            await client.DisconnectAsync(true);
            return (true, $"Email sent to {to.Email}.");
        }
        catch (Exception ex)
        {
            return (false, $"Could not send email: {ex.Message}");
        }
    }
}
