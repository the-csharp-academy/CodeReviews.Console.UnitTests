using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using silvermax.PhoneBook;

namespace PhoneBook.IntegrationTests;

[Collection("Database")]
public class DataSeederTests(DbFixture fixture) : IAsyncLifetime
{
    public Task DisposeAsync() => Task.CompletedTask;

    public Task InitializeAsync() => fixture.ResetAsync();

    [Fact]
    public async Task SeedAsync_OnEmptyDatabase_SeedsTenContacts()
    {
        await DataSeeder.SeedAsync(fixture.Db);

        (await fixture.Db.Contacts.CountAsync()).Should().Be(10);
    }

    [Fact]
    public async Task SeedAsync_RunTwice_DoesNotDuplicate()
    {
        await DataSeeder.SeedAsync(fixture.Db);
        await DataSeeder.SeedAsync(fixture.Db);

        (await fixture.Db.Contacts.CountAsync()).Should().Be(10);
    }

    [Fact]
    public async Task SeedAsync_WhenContactsExist_SeedsNothing()
    {
        fixture.Db.Contacts.Add(new Contact
        {
            Id = Guid.NewGuid(),
            Name = "existing",
            Email = "existing@example.com",
            PhoneNumber = "+442071838750",
            AddedAt = DateTime.UtcNow
        });
        await fixture.Db.SaveChangesAsync();

        await DataSeeder.SeedAsync(fixture.Db);

        fixture.Db.ChangeTracker.Clear();
        var contacts = await fixture.Db.Contacts.ToListAsync();
        contacts.Should().ContainSingle().Which.Name.Should().Be("existing");
    }
}
