using PhoneBook.Domain.Entities;
using PhoneBook.IntegrationTests.Fixtures;

namespace PhoneBook.IntegrationTests.ContactRepositoryTests;

public class GetByIdAsyncTests : IClassFixture<DatabaseFixture>
{
    private readonly DatabaseFixture _fixture;

    public GetByIdAsyncTests(DatabaseFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsEmptyContact_WhenIdNotValid()
    {
        // Arrange
        _fixture.ResetDatabase();

        // Act
        var result = await _fixture.ContactRepository.GetByIdAsync(1);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Null(result.Value);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsContact_WhenIdIsValid()
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
        var result = await _fixture.ContactRepository.GetByIdAsync(contact.Id);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(contact.FirstName, result.Value.FirstName);
        Assert.Equal(contact.LastName, result.Value.LastName);
        Assert.Equal(contact.PhoneNumber, result.Value.PhoneNumber);
        Assert.Equal(contact.Email, result.Value.Email);
        Assert.Equal(contact.CategoryId, result.Value.CategoryId);
    }
}