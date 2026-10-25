using Microsoft.EntityFrameworkCore;
using PhoneBook.Domain.Entities;
using PhoneBook.Domain.Validation.Errors;
using PhoneBook.IntegrationTests.Fixtures;

namespace PhoneBook.IntegrationTests.ContactRepositoryTests;

public class SetCategoryIdForContactsAsyncTests : IClassFixture<DatabaseFixture>
{
    private readonly DatabaseFixture _fixture;

    public SetCategoryIdForContactsAsyncTests(DatabaseFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task SetCategoryIdTests_ReturnsFailure_WhenCategoryDoesNotExistByName()
    {
        // Arrange
        var category = new ContactCategory { Name = "Test Category" };
        var categories = new List<ContactCategory>
            {
                        new ContactCategory {Name = "Family"},
                        new ContactCategory {Name = "Friends"},
                        new ContactCategory {Name = "Work"},
                        new ContactCategory {Name = "School"}
            };
        _fixture.ResetDatabase();
        _fixture.SeedCategoryTable(categories);


        // Act
        var result = await _fixture.ContactRepository.SetCategoryIdForContactsToDefaultByCategoryIdAsync(category);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Contains(CategoryRepositoryErrors.CategoryDoesNotExist, result.Errors);
        Assert.False(await _fixture.Context.ContactCategories.AnyAsync(cat => cat.Name.ToUpper() == category.Name.ToUpper()));
    }

    [Fact]
    public async Task SetCategoryIdTests_ReturnsSuccess_IfNoContactsUseCategory()
    {
        // Arrange
        var category = new ContactCategory { Id = 5, Name = "Test Category" };
        var categories = new List<ContactCategory>
            {
                        new ContactCategory {Id = 1, Name = "Family"},
                        new ContactCategory {Id = 2, Name = "Friends"},
                        new ContactCategory {Id = 3, Name = "Work"},
                        new ContactCategory {Id = 4, Name = "School"},
                        category
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
                    CategoryId = categories[0].Id
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
                    CategoryId = categories[1].Id
                }
            };
        _fixture.ResetDatabase();
        _fixture.SeedCategoryTable(categories);
        _fixture.SeedContactTable(contacts);

        // Assert
        var result = await _fixture.ContactRepository.SetCategoryIdForContactsToDefaultByCategoryIdAsync(category);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.True(await _fixture.Context.ContactCategories.AnyAsync(cat => cat.Name.ToUpper() == category.Name.ToUpper()));
        Assert.False(await _fixture.Context.Contacts.AnyAsync(c => c.CategoryId == category.Id));
    }

    [Fact]
    public async Task SetCategoryIdTests_ReturnsFailure_IfNoRecordsAreUpdated()
    {
        // Arrange
        var defaultCategory = new ContactCategory { Id = 1, Name = "UNCATEGORIZED" };
        var inputCategory = new ContactCategory { Id = 2, Name = "Family" };
        var categories = new List<ContactCategory>
            {
                        defaultCategory,
                        inputCategory
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
                    CategoryId = categories[1].Id
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
                    CategoryId = categories[1].Id
                }
            };
        _fixture.ResetDatabase();
        _fixture.SeedCategoryTable(categories);
        _fixture.SeedContactTable(contacts);

        // Assert
        var result = await _fixture.ContactRepository.SetCategoryIdForContactsToDefaultByCategoryIdAsync(
            new ContactCategory { Id = 5, Name = "Family" });

        // Act
        Assert.True(result.IsSuccess);
        Assert.True(await _fixture.Context.Contacts.AnyAsync(c => c.CategoryId == inputCategory.Id));
        Assert.False(await _fixture.Context.Contacts.AnyAsync(c => c.CategoryId == defaultCategory.Id));
    }

    [Fact]
    public async Task SetCategoryIdTests_ReturnsSuccess_For()
    {
        // Arrange
        var defaultCategory = new ContactCategory { Id = 1, Name = "UNCATEGORIZED" };
        var inputCategory = new ContactCategory { Id = 2, Name = "Family" };
        var categories = new List<ContactCategory>
            {
                        defaultCategory,
                        inputCategory
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
                    CategoryId = categories[1].Id
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
                    CategoryId = categories[1].Id
                }
            };
        _fixture.ResetDatabase();
        _fixture.SeedCategoryTable(categories);
        _fixture.SeedContactTable(contacts);

        // Act
        var result = await _fixture.ContactRepository.SetCategoryIdForContactsToDefaultByCategoryIdAsync(inputCategory);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.False(await _fixture.Context.Contacts.AnyAsync(c => c.CategoryId == inputCategory.Id));
        Assert.True(await _fixture.Context.Contacts.AnyAsync(c => c.CategoryId == defaultCategory.Id));
    }
}