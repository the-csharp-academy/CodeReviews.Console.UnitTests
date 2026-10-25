using Microsoft.Extensions.Configuration;
using silvermax.PhoneBook;
using silvermax.PhoneBook.Services;
using Spectre.Console;
using Spectre.Console.Testing;

namespace PhoneBook.IntegrationTests;

[Collection("Database")]
public class MailServiceTests(DbFixture fixture) : IAsyncLifetime
{
    public Task DisposeAsync() => Task.CompletedTask;

    public Task InitializeAsync() => fixture.ResetAsync();

    [Fact]
    public async Task MailServiceReturns_IfDatabaseDoesNotContainRecipient()
    {
        var output = await RunSendMailAsync("TestContact1@example.com", new(), "TestContact2");

        Assert.Contains("Contact not found", output);
        Assert.DoesNotContain("Contact has no email address configured", output);
    }

    [Fact]
    public async Task MailServiceReturns_IfTheRecipientHasNoEmailAddress()
    {
        var output = await RunSendMailAsync("", new());

        Assert.Contains("Contact has no email address configured", output);
    }

    [Fact]
    public async Task MailServiceReturns_IfEmailAddressIsNotValid()
    {
        var output = await RunSendMailAsync("string.com", new());

        Assert.Contains("Contact email is not a valid address", output);
    }

    [Theory]
    [InlineData(null, null)]
    [InlineData("fake-email", null)]
    [InlineData(null, "fake-password")]
    public async Task MailServiceReturns_IfSMTPCredentialsNotConfigured(string? personal, string? password)
    {
        var output = await RunSendMailAsync("Contact1@example.com", new()
        {
            ["Email:Personal"] = personal,
            ["Email:Password"] = password
        });

        Assert.Contains("SMTP credentials not configured", output);
        Assert.DoesNotContain("SMTP authentication failed: invalid username or password.", output);
    }

    private async Task<string> RunSendMailAsync(
        string recipientEmailAddress,
        Dictionary<string, string?> configData,
        string nameToType = "Contact1")
    {
        fixture.Db.Contacts.Add(new Contact
        {
            Id = Guid.NewGuid(),
            Name = "Contact1",
            Email = recipientEmailAddress,
            PhoneNumber = "+4478687457895",
            AddedAt = DateTime.UtcNow
        });
        await fixture.Db.SaveChangesAsync();

        var console = new TestConsole();
        console.Profile.Capabilities.Interactive = true;
        console.Input.PushTextWithEnter(nameToType);
        console.Input.PushTextWithEnter("This is the header");
        console.Input.PushTextWithEnter("This is the message");
        console.Input.PushKey(ConsoleKey.Enter);
        AnsiConsole.Console = console;

        var service = new MailService(
            BuildConfiguration(configData), new UserInput(), fixture.Db, CancellationToken.None);
        await service.SendMail();

        return console.Output;
    }

    private IConfiguration BuildConfiguration(Dictionary<string, string?> configurationData) =>
        new ConfigurationBuilder()
            .AddInMemoryCollection(configurationData)
            .Build();
}