using PhoneBook.Domain.Entities;
using PhoneBook.IntegrationTests.Fixtures;

namespace PhoneBook.IntegrationTests.ContactRepositoryTests;

public class GetByCategoryIdAsyncTests : IClassFixture<DatabaseFixture>
{
    private readonly DatabaseFixture _fixture;

    public GetByCategoryIdAsyncTests(DatabaseFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task GetByCategoryIdAsync_ReturnsEmpty_WhenIdNotValid()
    {
        // Arrange
        _fixture.ResetDatabase();

        // Act
        var result = await _fixture.ContactRepository.GetByCategoryIdAsync(1);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Empty(result.Value);
    }

    [Fact]
    public async Task GetByCategoryIdAsync_ReturnsContactList_WhenCategoryIdIsValid()
    {
        // Arrange
        var categories = new List<ContactCategory>
        {
            new ContactCategory { Id = 1, Name = "Crew" },
            new ContactCategory { Id = 2, Name = "Passenger" },
        };
        var contacts = new List<Contact>
        {
            new Contact
            {
                Id = 1,
                FirstName = "Malcolm",
                LastName = "Reynolds",
                PhoneNumber = "1111111111",
                Email = "browncoat@serenity.com",
                CategoryId = categories[0].Id
            },
            new Contact
            {
                Id = 2,
                FirstName = "Inara",
                LastName = "Serra",
                PhoneNumber = "2222222222",
                Email = "companion@serenity.com",
                CategoryId = categories[1].Id
            },
            new Contact
            {
                Id = 3,
                FirstName = "Shepherd",
                LastName = "Book",
                PhoneNumber = "3333333333",
                Email = "book@serenity.com",
                CategoryId = categories[1].Id
            },
            new Contact
            {
                Id = 4,
                FirstName = "Jayne",
                LastName = "Cobb",
                PhoneNumber = "4444444444",
                Email = "vera@serenity.com",
                CategoryId = categories[0].Id
            },
            new Contact
            {
                Id = 5,
                FirstName = "Kaylee",
                LastName = "Frye",
                PhoneNumber = "5555555555",
                Email = "mechanic@serenity.com",
                CategoryId = categories[0].Id
            },
            new Contact
            {
                Id = 6,
                FirstName = "Simon",
                LastName = "Tam",
                PhoneNumber = "6666666666",
                Email = "awesomeDoctor@serenity.com",
                CategoryId = categories[1].Id
            },
            new Contact
            {
                Id = 7,
                FirstName = "River",
                LastName = "Tam",
                PhoneNumber = "7777777777",
                Email = "miranda@serenity.com",
                CategoryId = categories[1].Id
            },
            new Contact
            {
                Id = 8,
                FirstName = "Hoban",
                LastName = "Washburne",
                PhoneNumber = "8888888",
                Email = "leafOnTheWind@serenity.com",
                CategoryId = categories[0].Id
            },
            new Contact
            {
                Id = 9,
                FirstName = "Zoe",
                LastName = "Washburne",
                PhoneNumber = "99999999",
                Email = "browncoats@serenity.com",
                CategoryId = categories[0].Id
            }
        };
        _fixture.ResetDatabase();
        _fixture.SeedCategoryTable(categories);
        _fixture.SeedContactTable(contacts);

        // Act
        var result = await _fixture.ContactRepository.GetByCategoryIdAsync(categories[0].Id);

        // Assert
        var filteredContacts = contacts.Where(x => x.CategoryId == categories[0].Id);
        Assert.True(result.IsSuccess);
        Assert.True(filteredContacts.Count() == result.Value.Count);
        Assert.Equal(filteredContacts.Select(x => x.Id), result.Value.Select(x => x.Id));
        Assert.Equal(filteredContacts.Select(x => x.FirstName), result.Value.Select(x => x.FirstName));
        Assert.Equal(filteredContacts.Select(x => x.LastName), result.Value.Select(x => x.LastName));
        Assert.Equal(filteredContacts.Select(x => x.PhoneNumber), result.Value.Select(x => x.PhoneNumber));
        Assert.Equal(filteredContacts.Select(x => x.Email), result.Value.Select(x => x.Email));
        Assert.Equal(filteredContacts.Select(x => x.CategoryId), result.Value.Select(x => x.CategoryId));
    }
}