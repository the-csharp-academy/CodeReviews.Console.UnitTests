using Microsoft.EntityFrameworkCore;
using PhoneBook.Domain.Entities;
using PhoneBook.Domain.Validation.Errors;
using PhoneBook.IntegrationTests.Fixtures;

namespace PhoneBook.IntegrationTests.ContactRepositoryTests;

public class UpdateAsyncTetss : IClassFixture<DatabaseFixture>
{
    private readonly DatabaseFixture _fixture;

    public UpdateAsyncTetss(DatabaseFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task UpdateAsync_ReturnsFailure_IfUpdatedRowResponseEqualsZero()
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
        var result = await _fixture.ContactRepository.UpdateAsync(
            new Contact
            {
                Id = 2,
                FirstName = "Malcolm",
                LastName = "Reynolds",
                PhoneNumber = "1111111111",
                Email = "browncoat@serenity.com",
                CategoryId = category.Id
            });

        // Assert
        Assert.True(result.IsFailure);
        Assert.Contains(ContactRepositoryErrors.UpdateDataFailed, result.Errors);
        var saved = await _fixture.Context.Contacts.FirstOrDefaultAsync(x => x.Id == contact.Id);
        Assert.Equal(contact.FirstName, saved.FirstName);
        Assert.Equal(contact.LastName, saved.LastName);
        Assert.Equal(contact.PhoneNumber, saved.PhoneNumber);
        Assert.Equal(contact.Email, saved.Email);
        Assert.Equal(contact.CategoryId, saved.CategoryId);
    }

    [Fact]
    public async Task UpdateAsync_ReturnsSuccess_WhenContactIdMatchesAndNoDataIsChanged()
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
        var result = await _fixture.ContactRepository.UpdateAsync(contact);

        // Assert
        Assert.True(result.IsSuccess);
        var saved = await _fixture.Context.Contacts.FirstOrDefaultAsync(x => x.Id == contact.Id);
        Assert.Equal(contact.FirstName, saved.FirstName);
        Assert.Equal(contact.LastName, saved.LastName);
        Assert.Equal(contact.PhoneNumber, saved.PhoneNumber);
        Assert.Equal(contact.Email, saved.Email);
        Assert.Equal(contact.CategoryId, saved.CategoryId);
    }

    [Fact]
    public async Task UpdateAsync_ReturnsSuccess_WhenContactIdMatchesAndDataIsChanged()
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
        var result = await _fixture.ContactRepository.UpdateAsync(
            new Contact
            {
                Id = 1,
                FirstName = "Inara",
                LastName = "Serra",
                PhoneNumber = "2222222222",
                Email = "companion@serenity.com",
                CategoryId = category.Id
            });

        // Assert
        Assert.True(result.IsSuccess);
        var saved = await _fixture.Context.Contacts.FirstOrDefaultAsync(x => x.Id == contact.Id);
        Assert.Equal(contact.FirstName, saved.FirstName);
        Assert.Equal(contact.LastName, saved.LastName);
        Assert.Equal(contact.PhoneNumber, saved.PhoneNumber);
        Assert.Equal(contact.Email, saved.Email);
        Assert.Equal(contact.CategoryId, saved.CategoryId);
    }
}
