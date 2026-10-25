using Microsoft.Extensions.Configuration;
using silvermax.PhoneBook;
using silvermax.PhoneBook.Services;
using Spectre.Console;
using Spectre.Console.Testing;

namespace PhoneBook.IntegrationTests;

[Collection("Database")]
public class SMSServiceTests(DbFixture fixture) : IAsyncLifetime
{
    public Task DisposeAsync() => Task.CompletedTask;

    public Task InitializeAsync() => fixture.ResetAsync();

    [Fact]
    public async Task SMSServiceReturns_IfDatabaseDoesNotContainRecipient()
    {
        var output = await RunSendSmsAsync("+442071838750", new(), "TestContact2");

        Assert.Contains("Contact not found", output);
        Assert.DoesNotContain("Contact has no phone number configured", output);
    }

    [Fact]
    public async Task SMSServiceReturns_IfTheRecipientHasNoPhoneNumber()
    {
        var output = await RunSendSmsAsync("", new());

        Assert.Contains("Contact has no phone number configured", output);
    }

    [Theory]
    [InlineData(null, null)]
    [InlineData("fake-sid", null)]
    [InlineData(null, "fake-token")]
    public async Task SMSServiceReturns_IfTwilioCredentialsNotConfigured(string? sid, string? token)
    {
        var output = await RunSendSmsAsync("+442071838750", new()
        {
            ["Twilio:AccountSID"] = sid,
            ["Twilio:AuthToken"] = token
        });

        Assert.Contains("Twilio credentials not configured", output);
        Assert.DoesNotContain("Phone Number not configured", output);
    }

    [Fact]
    public async Task SMSServiceReturns_IfPhoneNumberOfTheSenderNotConfigured()
    {
        var output = await RunSendSmsAsync("+442071838750", new()
        {
            ["Twilio:AccountSID"] = "fake-sid",
            ["Twilio:AuthToken"] = "fake-token"
        });

        Assert.Contains("Phone Number not configured", output);
    }

    private IConfiguration BuildConfiguration(Dictionary<string, string?> configurationData) =>
        new ConfigurationBuilder()
            .AddInMemoryCollection(configurationData)
            .Build();

    private async Task<string> RunSendSmsAsync(
        string recipientPhoneNumber,
        Dictionary<string, string?> configData,
        string nameToType = "Contact1")
    {
        fixture.Db.Contacts.Add(new Contact
        {
            Id = Guid.NewGuid(),
            Name = "Contact1",
            Email = "Contact1@example.com",
            PhoneNumber = recipientPhoneNumber,
            AddedAt = DateTime.UtcNow
        });
        await fixture.Db.SaveChangesAsync();

        var console = new TestConsole();
        console.Profile.Capabilities.Interactive = true;
        console.Input.PushTextWithEnter(nameToType);
        console.Input.PushTextWithEnter("This is the message");
        console.Input.PushKey(ConsoleKey.Enter);
        AnsiConsole.Console = console;

        var service = new SMSService(
            fixture.Db, BuildConfiguration(configData), new UserInput(), CancellationToken.None);
        await service.SendSMS();

        return console.Output;
    }
}
