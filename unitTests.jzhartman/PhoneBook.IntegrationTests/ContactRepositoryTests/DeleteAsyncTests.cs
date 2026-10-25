using PhoneBook.Domain.Entities;
using PhoneBook.Domain.Validation.Errors;
using PhoneBook.IntegrationTests.Fixtures;

namespace PhoneBook.IntegrationTests.ContactRepositoryTests;

public class DeleteAsyncTests : IClassFixture<DatabaseFixture>
{
    private readonly DatabaseFixture _fixture;

    public DeleteAsyncTests(DatabaseFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task DeleteAsync_ReturnsFailure_IfContactDoesNotExist()
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
        var result = await _fixture.ContactRepository.DeleteAsync(
                    new Contact
                    {
                        Id = 2,
                        FirstName = "Jayne",
                        LastName = "Cobb",
                        PhoneNumber = "4444444444",
                        Email = "vera@serenity.com",
                        CategoryId = 2
                    });

        // Assert
        Assert.True(result.IsFailure);
        Assert.Contains(ContactRepositoryErrors.ContactDoesNotExist, result.Errors);
        Assert.True(_fixture.Context.Contacts.Count() == 1);
    }

    [Fact]
    public async Task DeleteAsync_ReturnsSuccess_WhenContactIsValid()
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
        var result = await _fixture.ContactRepository.DeleteAsync(contact);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.True(_fixture.Context.Contacts.Count() == 0);
    }
}