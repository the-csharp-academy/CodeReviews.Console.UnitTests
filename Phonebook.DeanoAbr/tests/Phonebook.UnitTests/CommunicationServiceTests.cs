using Phonebook.Models;
using Phonebook.Services;

namespace Phonebook.UnitTests;

public class CommunicationServiceTests
{
    private static readonly string[] EmailVariables =
        ["SMTP_HOST", "SMTP_PORT", "SMTP_USER", "SMTP_PASS", "SMTP_FROM"];

    private static readonly string[] SmsVariables =
        ["TWILIO_ACCOUNT_SID", "TWILIO_AUTH_TOKEN", "TWILIO_FROM_NUMBER"];

    private static Dictionary<string, string?> Clear(IEnumerable<string> names)
    {
        var saved = new Dictionary<string, string?>();
        foreach (var name in names)
        {
            saved[name] = Environment.GetEnvironmentVariable(name);
            Environment.SetEnvironmentVariable(name, null);
        }
        return saved;
    }

    private static void Restore(Dictionary<string, string?> saved)
    {
        foreach (var (name, value) in saved)
            Environment.SetEnvironmentVariable(name, value);
    }

    [Fact]
    public async Task SendEmailAsync_WithoutSmtpCredentials_ComposesSandboxMessageInsteadOfSending()
    {
        var saved = Clear(EmailVariables);
        try
        {
            var service = new EmailService();
            Assert.False(service.IsConfigured);

            var (success, message) = await service.SendEmailAsync(
                new Contact { Email = "alice@example.com" }, "Hello", "Body text");

            Assert.True(success);
            Assert.Contains("sandbox", message!);
            Assert.Contains("alice@example.com", message!);
            Assert.Contains("Body text", message!);
        }
        finally
        {
            Restore(saved);
        }
    }

    [Fact]
    public async Task SendSmsAsync_WithoutTwilioCredentials_ComposesSandboxMessageInsteadOfCallingTheApi()
    {
        var saved = Clear(SmsVariables);
        try
        {
            var service = new SmsService();
            Assert.False(service.IsConfigured);

            var (success, message) = await service.SendSmsAsync(
                new Contact { PhoneNumber = "082 123 4567" }, "On my way");

            Assert.True(success);
            Assert.Contains("sandbox", message!);
            Assert.Contains("082 123 4567", message!);
            Assert.Contains("On my way", message!);
        }
        finally
        {
            Restore(saved);
        }
    }
}
