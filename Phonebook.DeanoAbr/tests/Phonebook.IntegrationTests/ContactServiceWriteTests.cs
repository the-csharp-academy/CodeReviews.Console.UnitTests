using Microsoft.EntityFrameworkCore;
using Phonebook.Models;
using Phonebook.Services;

namespace Phonebook.IntegrationTests;

public class ContactServiceWriteTests : IDisposable
{
    private readonly TestDatabase _db = new();

    public void Dispose() => _db.Dispose();

    [Fact]
    public async Task AddAsync_WhenContactIsValid_PersistsItForLaterContexts()
    {
        using (var writeContext = _db.NewContext())
        {
            var (success, error) = await new ContactService(writeContext).AddAsync(new Contact
            {
                Name = "Dineo Mokoena",
                Email = "dineo@example.com",
                PhoneNumber = "+27 84 555 1234",
                Category = ContactCategory.Work,
            });
            Assert.True(success, error);
        }   // context disposed: the row must be committed to the file, not just held in memory

        using var readContext = _db.NewContext();
        var stored = await readContext.Contacts.AsNoTracking().SingleOrDefaultAsync(c => c.Name == "Dineo Mokoena");

        Assert.NotNull(stored);
        Assert.True(stored!.Id > 0);
        Assert.Equal("dineo@example.com", stored.Email);
        Assert.Equal("+27 84 555 1234", stored.PhoneNumber);
        Assert.Equal(ContactCategory.Work, stored.Category);
    }

    [Fact]
    public async Task AddAsync_WhenNameIsNull_ReturnsFailureInsteadOfThrowingOrPersisting()
    {
        using var context = _db.NewContext();
        var service = new ContactService(context);

        var (success, error) = await service.AddAsync(new Contact
        {
            Name = null!,                       // deliberately violates the NOT NULL column
            Email = "nobody@example.com",
            PhoneNumber = "082 123 4567",
            Category = ContactCategory.Other,
        });

        Assert.False(success);
        Assert.False(string.IsNullOrWhiteSpace(error));

        using var readContext = _db.NewContext();
        Assert.Equal(3, await readContext.Contacts.CountAsync());   // only the seeded rows
    }

    [Fact]
    public async Task UpdateAsync_WhenContactExists_ChangesThePersistedValues()
    {
        using (var writeContext = _db.NewContext())
        {
            var (success, error) = await new ContactService(writeContext).UpdateAsync(new Contact
            {
                Id = 2,
                Name = "Robert Smith",
                Email = "robert@example.com",
                PhoneNumber = "082 555 0148",
                Category = ContactCategory.Friends,
            });
            Assert.True(success, error);
        }

        using var readContext = _db.NewContext();
        var stored = await readContext.Contacts.AsNoTracking().SingleAsync(c => c.Id == 2);

        Assert.Equal("Robert Smith", stored.Name);
        Assert.Equal("robert@example.com", stored.Email);
        Assert.Equal(ContactCategory.Friends, stored.Category);
    }

    [Fact]
    public async Task DeleteAsync_WhenContactExists_RemovesTheRow()
    {
        using (var writeContext = _db.NewContext())
        {
            var (success, error) = await new ContactService(writeContext).DeleteAsync(3);
            Assert.True(success, error);
        }

        using var readContext = _db.NewContext();
        Assert.Null(await readContext.Contacts.AsNoTracking().FirstOrDefaultAsync(c => c.Id == 3));
        Assert.Equal(2, await readContext.Contacts.CountAsync());
    }

    [Fact]
    public async Task DeleteAsync_WhenIdDoesNotExist_ReturnsFailureAndLeavesTableUntouched()
    {
        using var context = _db.NewContext();
        var service = new ContactService(context);

        var (success, error) = await service.DeleteAsync(999);

        Assert.False(success);
        Assert.Contains("999", error);
        Assert.Equal(3, await context.Contacts.CountAsync());
    }
}
