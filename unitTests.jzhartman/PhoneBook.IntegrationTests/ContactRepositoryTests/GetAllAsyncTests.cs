using PhoneBook.Domain.Entities;
using PhoneBook.IntegrationTests.Fixtures;

namespace PhoneBook.IntegrationTests.ContactRepositoryTests;

public class GetAllAsyncTests : IClassFixture<DatabaseFixture>
{
    private readonly DatabaseFixture _fixture;

    public GetAllAsyncTests(DatabaseFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task GetAllAsync_ReturnsEmptyList_WhenTableIsEmpty()
    {
        // Arrange
        _fixture.ResetDatabase();

        // Act
        var result = await _fixture.ContactRepository.GetAllAsync();

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Empty(result.Value);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsPopulatedList_WhenTableIsPopulated()
    {
        // Arrange
        var category = new ContactCategory { Id = 1, Name = "Crew" };

        var contacts = new List<Contact>
        {
            new Contact
            {
                Id = 1,
                FirstName = "Malcolm",
                LastName = "Reynolds",
                PhoneNumber = "1111111111",
                Email = "browncoat@serenity.com",
                CategoryId = category.Id
            },
            new Contact
            {
                Id = 2,
                FirstName = "Inara",
                LastName = "Serra",
                PhoneNumber = "2222222222",
                Email = "companion@serenity.com",
                CategoryId = category.Id
            },
            new Contact
            {
                Id = 3,
                FirstName = "Shepherd",
                LastName = "Book",
                PhoneNumber = "3333333333",
                Email = "book@serenity.com",
                CategoryId = category.Id
            },
            new Contact
            {
                Id = 4,
                FirstName = "Jayne",
                LastName = "Cobb",
                PhoneNumber = "4444444444",
                Email = "vera@serenity.com",
                CategoryId = category.Id
            },
            new Contact
            {
                Id = 5,
                FirstName = "Kaylee",
                LastName = "Frye",
                PhoneNumber = "5555555555",
                Email = "mechanic@serenity.com",
                CategoryId = category.Id
            },
            new Contact
            {
                Id = 6,
                FirstName = "Simon",
                LastName = "Tam",
                PhoneNumber = "6666666666",
                Email = "awesomeDoctor@serenity.com",
                CategoryId = category.Id
            },
            new Contact
            {
                Id = 7,
                FirstName = "River",
                LastName = "Tam",
                PhoneNumber = "7777777777",
                Email = "miranda@serenity.com",
                CategoryId = category.Id
            },
            new Contact
            {
                Id = 8,
                FirstName = "Hoban",
                LastName = "Washburne",
                PhoneNumber = "8888888",
                Email = "leafOnTheWind@serenity.com",
                CategoryId = category.Id
            },
            new Contact
            {
                Id = 9,
                FirstName = "Zoe",
                LastName = "Washburne",
                PhoneNumber = "99999999",
                Email = "browncoats@serenity.com",
                CategoryId = category.Id
            }
        };
        _fixture.ResetDatabase();
        _fixture.SeedCategoryTable(category);
        _fixture.SeedContactTable(contacts);

        // Act
        var result = await _fixture.ContactRepository.GetAllAsync();

        // Assert
        Assert.True(result.IsSuccess);
        Assert.True(contacts.Count == result.Value.Count);
        Assert.Equal(contacts.Select(x => x.Id), result.Value.Select(x => x.Id));
        Assert.Equal(contacts.Select(x => x.FirstName), result.Value.Select(x => x.FirstName));
        Assert.Equal(contacts.Select(x => x.LastName), result.Value.Select(x => x.LastName));
        Assert.Equal(contacts.Select(x => x.PhoneNumber), result.Value.Select(x => x.PhoneNumber));
        Assert.Equal(contacts.Select(x => x.Email), result.Value.Select(x => x.Email));
        Assert.Equal(contacts.Select(x => x.CategoryId), result.Value.Select(x => x.CategoryId));
    }
}