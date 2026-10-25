using Microsoft.EntityFrameworkCore;
using PhoneBook.Domain.Entities;
using PhoneBook.Domain.Validation.Errors;
using PhoneBook.IntegrationTests.Fixtures;

namespace PhoneBook.IntegrationTests.ContactRepositoryTests;

public class AddAsyncTests : IClassFixture<DatabaseFixture>
{
    private readonly DatabaseFixture _fixture;

    public AddAsyncTests(DatabaseFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task AddAsync_ReturnsFailure_IfContactAlreadyExists()
    {
        // Arrange
        var category = new ContactCategory { Id = 1, Name = "Family" };
        var contact = new Contact
        {
            Id = 1,
            FirstName = "Malcolm",
            LastName = "Reynolds",
            PhoneNumber = "1111111111",
            Email = "browncoat@serenity.com",
            CategoryId = category.Id
        };
        _fixture.ResetDatabase();
        _fixture.SeedCategoryTable(category);
        _fixture.SeedContactTable(contact);

        // Act
        var result = await _fixture.ContactRepository.AddAsync(contact);
        await _fixture.ContactRepository.SaveChangesAsync();

        // Assert
        Assert.True(result.IsFailure);
        Assert.Contains(ContactRepositoryErrors.ContactExists, result.Errors);
        Assert.True(_fixture.Context.Contacts.Count() == 1);
    }

    [Fact]
    public async Task AddAsync_ReturnsSuccess_ForDuplicateContactWithChangedFirstName()
    {
        // Arrange
        var category = new ContactCategory { Id = 1, Name = "Family" };
        var contact = new Contact
        {
            Id = 1,
            FirstName = "Malcolm",
            LastName = "Reynolds",
            PhoneNumber = "1111111111",
            Email = "browncoat@serenity.com",
            CategoryId = category.Id
        };
        _fixture.ResetDatabase();
        _fixture.SeedCategoryTable(category);
        _fixture.SeedContactTable(contact);
        contact.Id = 0;
        contact.FirstName = "John";

        // Act
        var result = await _fixture.ContactRepository.AddAsync(contact);
        await _fixture.ContactRepository.SaveChangesAsync();

        // Assert
        Assert.True(result.IsSuccess);
        Assert.True(_fixture.Context.Contacts.Count() == 2);

        _fixture.Context.ChangeTracker.Clear();
        var saved = await _fixture.Context.Contacts.FirstOrDefaultAsync(x => x.Id == result.Value.Id);
        Assert.Equal(contact.FirstName, saved.FirstName);
        Assert.Equal(contact.LastName, saved.LastName);
        Assert.Equal(contact.PhoneNumber, saved.PhoneNumber);
        Assert.Equal(contact.Email, saved.Email);
        Assert.Equal(contact.CategoryId, saved.CategoryId);
    }

    [Fact]
    public async Task AddAsync_ReturnsSuccess_ForDuplicateContactWithChangedLastName()
    {
        // Arrange
        var category = new ContactCategory { Id = 1, Name = "Family" };
        var contact = new Contact
        {
            Id = 1,
            FirstName = "Malcolm",
            LastName = "Reynolds",
            PhoneNumber = "1111111111",
            Email = "browncoat@serenity.com",
            CategoryId = category.Id
        };
        _fixture.ResetDatabase();
        _fixture.SeedCategoryTable(category);
        _fixture.SeedContactTable(contact);
        contact.Id = 0;
        contact.LastName = "Smith";

        // Act
        var result = await _fixture.ContactRepository.AddAsync(contact);
        await _fixture.ContactRepository.SaveChangesAsync();

        // Assert
        Assert.True(result.IsSuccess);
        Assert.True(_fixture.Context.Contacts.Count() == 2);

        _fixture.Context.ChangeTracker.Clear();
        var saved = await _fixture.Context.Contacts.FirstOrDefaultAsync(x => x.Id == result.Value.Id);
        Assert.Equal(contact.FirstName, saved.FirstName);
        Assert.Equal(contact.LastName, saved.LastName);
        Assert.Equal(contact.PhoneNumber, saved.PhoneNumber);
        Assert.Equal(contact.Email, saved.Email);
        Assert.Equal(contact.CategoryId, saved.CategoryId);
    }

    [Fact]
    public async Task AddAsync_ReturnsSuccess_ForDuplicateContactWithChangedPhoneNumber()
    {
        // Arrange
        var category = new ContactCategory { Id = 1, Name = "Family" };
        var contact = new Contact
        {
            Id = 1,
            FirstName = "Malcolm",
            LastName = "Reynolds",
            PhoneNumber = "1111111111",
            Email = "browncoat@serenity.com",
            CategoryId = category.Id
        };
        _fixture.ResetDatabase();
        _fixture.SeedCategoryTable(category);
        _fixture.SeedContactTable(contact);
        contact.Id = 0;
        contact.PhoneNumber = "999999999";

        // Act
        var result = await _fixture.ContactRepository.AddAsync(contact);
        await _fixture.ContactRepository.SaveChangesAsync();

        // Assert
        Assert.True(result.IsSuccess);
        Assert.True(_fixture.Context.Contacts.Count() == 2);

        _fixture.Context.ChangeTracker.Clear();
        var saved = await _fixture.Context.Contacts.FirstOrDefaultAsync(x => x.Id == result.Value.Id);
        Assert.Equal(contact.FirstName, saved.FirstName);
        Assert.Equal(contact.LastName, saved.LastName);
        Assert.Equal(contact.PhoneNumber, saved.PhoneNumber);
        Assert.Equal(contact.Email, saved.Email);
        Assert.Equal(contact.CategoryId, saved.CategoryId);
    }

    [Fact]
    public async Task AddAsync_ReturnsSuccess_ForDuplicateContactWithChangedEmail()
    {
        // Arrange
        var category = new ContactCategory { Id = 1, Name = "Family" };
        var contact = new Contact
        {
            Id = 1,
            FirstName = "Malcolm",
            LastName = "Reynolds",
            PhoneNumber = "1111111111",
            Email = "browncoat@serenity.com",
            CategoryId = category.Id
        };
        _fixture.ResetDatabase();
        _fixture.SeedCategoryTable(category);
        _fixture.SeedContactTable(contact);
        contact.Id = 0;
        contact.Email = "firefly@serenity.com";

        // Act
        var result = await _fixture.ContactRepository.AddAsync(contact);
        await _fixture.ContactRepository.SaveChangesAsync();

        // Assert
        Assert.True(result.IsSuccess);
        Assert.True(_fixture.Context.Contacts.Count() == 2);

        _fixture.Context.ChangeTracker.Clear();
        var saved = await _fixture.Context.Contacts.FirstOrDefaultAsync(x => x.Id == result.Value.Id);
        Assert.Equal(contact.FirstName, saved.FirstName);
        Assert.Equal(contact.LastName, saved.LastName);
        Assert.Equal(contact.PhoneNumber, saved.PhoneNumber);
        Assert.Equal(contact.Email, saved.Email);
        Assert.Equal(contact.CategoryId, saved.CategoryId);
    }

    [Fact]
    public async Task AddAsync_ReturnsSuccess_ForDuplicateContactWithChangedCategory()
    {
        // Arrange
        var categories = new List<ContactCategory>
        {
            new ContactCategory {Id = 1, Name = "Family" },
            new ContactCategory {Id = 2, Name = "Work"}
        };
        var contact = new Contact
        {
            Id = 1,
            FirstName = "Malcolm",
            LastName = "Reynolds",
            PhoneNumber = "1111111111",
            Email = "browncoat@serenity.com",
            CategoryId = categories[0].Id
        };
        _fixture.ResetDatabase();
        _fixture.SeedCategoryTable(categories);
        _fixture.SeedContactTable(contact);
        contact.Id = 0;
        contact.CategoryId = categories[1].Id;

        // Act
        var result = await _fixture.ContactRepository.AddAsync(contact);
        await _fixture.ContactRepository.SaveChangesAsync();

        // Assert
        Assert.True(result.IsSuccess);
        Assert.True(_fixture.Context.Contacts.Count() == 2);

        _fixture.Context.ChangeTracker.Clear();
        var saved = await _fixture.Context.Contacts.FirstOrDefaultAsync(x => x.Id == result.Value.Id);
        Assert.Equal(contact.FirstName, saved.FirstName);
        Assert.Equal(contact.LastName, saved.LastName);
        Assert.Equal(contact.PhoneNumber, saved.PhoneNumber);
        Assert.Equal(contact.Email, saved.Email);
        Assert.Equal(contact.CategoryId, saved.CategoryId);
    }
}