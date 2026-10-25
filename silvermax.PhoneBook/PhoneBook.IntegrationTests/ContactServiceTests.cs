using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using silvermax.PhoneBook;
using silvermax.PhoneBook.Services;
using Spectre.Console;
using Spectre.Console.Testing;

[assembly: CollectionBehavior(DisableTestParallelization = true)]

namespace PhoneBook.IntegrationTests;

[Collection("Database")]
public class ContactServiceTests(DbFixture fixture) : IAsyncLifetime
{

    public Task InitializeAsync() => fixture.ResetAsync();

    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public async Task ListContactsReturnsEmpty_WhenDatabaseIsEmpty()
    {
        var contactService = CreateContactservice();

        var contacts = await contactService.ListContacts();

        Assert.Empty(contacts);
    }

    [Fact]
    public async Task ListContactsReturnsCorrectNumberOfContactsInDatabase()
    {
        fixture.Db.Contacts.AddRange(Enumerable.Range(1, 5).Select(x => new Contact
        {
            Id = Guid.NewGuid(),
            Name = "TestContact" + x,
            Email = "TestContact" + x + "@example.com",
            PhoneNumber = "+44589558969" + x,
            AddedAt = DateTime.UtcNow
        }));

        await fixture.Db.SaveChangesAsync();

        var contactService = CreateContactservice();

        var contacts = await contactService.ListContacts();

        contacts.Should().HaveCount(5);
    }

    [Fact]
    public async Task UpdateContact_UpdatesTheContactInTheDatabase()
    {
        fixture.Db.Contacts.Add(new Contact
        {
            Id = Guid.NewGuid(),
            Name = "TestContact1",
            Email = "TestContact1@example.com",
            PhoneNumber = "+442071838750",
            AddedAt = DateTime.UtcNow
        });
        await fixture.Db.SaveChangesAsync();

        var console = new TestConsole();
        console.Profile.Capabilities.Interactive = true;
        console.Input.PushTextWithEnter("TestContact1");
        console.Input.PushTextWithEnter("TestContact2");
        console.Input.PushTextWithEnter("TestContact2@example.com");
        console.Input.PushTextWithEnter("+4930901820");
        AnsiConsole.Console = console;

        var service = CreateContactservice();
        await service.UpdateContact();

        var updated = await fixture.Db.Contacts.SingleAsync();
        Assert.Equal("TestContact2", updated.Name);
        Assert.Equal("TestContact2@example.com", updated.Email);
        Assert.Equal("+4930901820", updated.PhoneNumber);
    }

    [Fact]
    public async Task DeletingAContact_DeletesTheContactInTheDatabase()
    {
        fixture.Db.Contacts.Add(new Contact
        {
            Id = Guid.NewGuid(),
            Name = "TestContact1",
            Email = "TestContact1@example.com",
            PhoneNumber = "+442071838750",
            AddedAt = DateTime.UtcNow
        });
        await fixture.Db.SaveChangesAsync();

        var console = new TestConsole();
        console.Profile.Capabilities.Interactive = true;
        console.Input.PushTextWithEnter("TestContact1");
        console.Input.PushTextWithEnter("y");
        console.Input.PushKey(ConsoleKey.Enter);
        AnsiConsole.Console = console;

        var service = CreateContactservice();
        await service.DeleteContact();

        fixture.Db.ChangeTracker.Clear();
        (await fixture.Db.Contacts.AnyAsync()).Should().BeFalse();
    }

    [Fact]
    public async Task AddContactToDb_ReallyAddsAContactToDatabase()
    {
        var console = new TestConsole();
        console.Profile.Capabilities.Interactive = true;
        console.Input.PushTextWithEnter("TestContact");
        console.Input.PushTextWithEnter("TestContact@example.com");
        console.Input.PushTextWithEnter("+442071838750");
        AnsiConsole.Console = console;

        var service = CreateContactservice();
        await service.AddContactToDb();

        var saved = await fixture.Db.Contacts.SingleAsync();
        saved.Name.Should().Be("TestContact".ToLower());
    }

    [Fact]
    public async Task AddContactToDb_PreventsAddingContactWithSameNumber()
    {
        var console = new TestConsole();
        console.Profile.Capabilities.Interactive = true;
        console.Input.PushTextWithEnter("TestContact");
        console.Input.PushTextWithEnter("TestContact@example.com");
        console.Input.PushTextWithEnter("+442071838750");
        AnsiConsole.Console = console;

        var service = CreateContactservice();
        await service.AddContactToDb();

        console.Profile.Capabilities.Interactive = true;
        console.Input.PushTextWithEnter("TestContact1");
        console.Input.PushTextWithEnter("TestContact1@example.com");
        console.Input.PushTextWithEnter("+442071838750");
        AnsiConsole.Console = console;

        await service.AddContactToDb();

        var savedContact = await fixture.Db.Contacts.SingleAsync();
        var contacts = await service.ListContacts();

        contacts.Should().HaveCount(1);
        savedContact.Name.Should().Be("TestContact".ToLower());
    }

    [Fact]
    public async Task AddingAContactWhoseNameAlreadyExistsInTheDatabaseThrows()
    {
        var console = new TestConsole();
        console.Profile.Capabilities.Interactive = true;
        console.Input.PushTextWithEnter("TestContact");
        console.Input.PushTextWithEnter("TestContact@example.com");
        console.Input.PushTextWithEnter("+442071838750");
        AnsiConsole.Console = console;

        var service = CreateContactservice();
        await service.AddContactToDb();

        console.Profile.Capabilities.Interactive = true;
        console.Input.PushTextWithEnter("TestContact");
        console.Input.PushTextWithEnter("TestContact1@example.com");
        console.Input.PushTextWithEnter("+442071838750123");
        AnsiConsole.Console = console;

        var exception = Assert.ThrowsAsync<DbUpdateException>(async () =>
        {
            await service.AddContactToDb();
        });
    }

    [Fact]
    public Task UpdateContact_WithUnknownName_ChangesNothing() =>
        AssertDatabaseUnchanged(service => service.UpdateContact(), "nobody");

    [Fact]
    public Task DeleteContact_WithUnknownName_ChangesNothing() =>
        AssertDatabaseUnchanged(service => service.DeleteContact(), "nobody", "y");

    private async Task AssertDatabaseUnchanged(Func<ContactService, Task> action, params string[] inputs)
    {
        fixture.Db.Contacts.Add(new Contact
        {
            Id = Guid.NewGuid(),
            Name = "TestContact1",
            Email = "TestContact1@example.com",
            PhoneNumber = "+442071838750",
            AddedAt = DateTime.UtcNow
        });
        await fixture.Db.SaveChangesAsync();

        var console = new TestConsole();
        console.Profile.Capabilities.Interactive = true;
        foreach (var input in inputs)
            console.Input.PushTextWithEnter(input);
        AnsiConsole.Console = console;

        await action(CreateContactservice());

        fixture.Db.ChangeTracker.Clear();
        var contact = await fixture.Db.Contacts.SingleAsync();
        contact.Name.Should().Be("TestContact1");
        contact.Email.Should().Be("TestContact1@example.com");
        contact.PhoneNumber.Should().Be("+442071838750");
    }

    private ContactService CreateContactservice() =>
        new ContactService(fixture.Db, new UserInput(), CancellationToken.None);
}
