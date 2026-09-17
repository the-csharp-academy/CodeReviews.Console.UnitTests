using Phonebook.Models;
using Phonebook.Services;

namespace Phonebook.IntegrationTests;

public class ContactServiceReadTests : IDisposable
{
    private readonly TestDatabase _db = new();

    public void Dispose() => _db.Dispose();

    [Fact]
    public async Task GetAllAsync_WhenContactsExist_ReturnsThemOrderedByNameAscending()
    {
        using var context = _db.NewContext();
        var service = new ContactService(context);

        var (added, addError) = await service.AddAsync(new Contact
        {
            Name = "Zanele Dlamini",
            Email = "zanele@example.com",
            PhoneNumber = "083 111 2222",
            Category = ContactCategory.Other,
        });
        Assert.True(added, addError);

        var (success, data, error) = await service.GetAllAsync();

        Assert.True(success, error);
        Assert.Equal(
            new[] { "Alice van der Merwe", "Bob Smith", "Carol Nkosi", "Zanele Dlamini" },
            data!.Select(c => c.Name).ToArray());
    }

    [Fact]
    public async Task GetByIdAsync_WhenIdExists_ReturnsThatContact()
    {
        using var context = _db.NewContext();
        var service = new ContactService(context);

        var (success, data, error) = await service.GetByIdAsync(2);

        Assert.True(success, error);
        Assert.NotNull(data);
        Assert.Equal("Bob Smith", data!.Name);
        Assert.Equal(ContactCategory.Work, data.Category);
    }

    [Fact]
    public async Task GetByIdAsync_WhenIdDoesNotExist_ReturnsFailureAndNotFoundMessage()
    {
        using var context = _db.NewContext();
        var service = new ContactService(context);

        var (success, data, error) = await service.GetByIdAsync(999);

        Assert.False(success);
        Assert.Null(data);
        Assert.Contains("999", error);
    }

    [Fact]
    public async Task GetAllAsync_WhenTableIsEmpty_ReturnsSuccessWithNoRows()
    {
        using var context = _db.NewContext();
        var service = new ContactService(context);

        for (var id = 1; id <= 3; id++)
        {
            var (deleted, deleteError) = await service.DeleteAsync(id);
            Assert.True(deleted, deleteError);
        }

        var (success, data, error) = await service.GetAllAsync();

        Assert.True(success, error);
        Assert.Empty(data!);
    }
}
