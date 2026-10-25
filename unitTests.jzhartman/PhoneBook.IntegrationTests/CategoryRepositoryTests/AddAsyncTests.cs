using Microsoft.EntityFrameworkCore;
using PhoneBook.Domain.Entities;
using PhoneBook.Domain.Validation.Errors;
using PhoneBook.IntegrationTests.Fixtures;

namespace PhoneBook.IntegrationTests.CategoryRepositoryTests;

public class AddAsyncTests : IClassFixture<DatabaseFixture>
{
    private readonly DatabaseFixture _fixture;

    public AddAsyncTests(DatabaseFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task AddAsync_ReturnsFailure_WhenCategoryNotUnique()
    {
        // Arrange
        var category = new ContactCategory { Name = "Family" };
        _fixture.ResetDatabase();
        _fixture.SeedCategoryTable(
            new List<ContactCategory>
            {
                        new ContactCategory {Name = "Family"},
                        new ContactCategory {Name = "Friends"},
                        new ContactCategory {Name = "Work"},
                        new ContactCategory {Name = "School"}
            });

        // Act
        var result = await _fixture.CategoryRepository.AddAsync(category);
        await _fixture.CategoryRepository.SaveChangesAsync();

        // Assert: Repository result
        Assert.True(result.IsFailure);

        // Assert: Database state
        var saved = await _fixture.Context.ContactCategories.FirstOrDefaultAsync(x => x.Name == category.Name);
        Assert.NotNull(saved);
        Assert.Equal(category.Name, saved.Name);
        Assert.Contains(CategoryRepositoryErrors.CategoryExists, result.Errors);
    }

    [Fact]
    public async Task AddAsync_ReturnsSuccess_WhenUniqueCategoryIsUnique()
    {
        // Arrange
        var category = new ContactCategory { Name = "Test Category" };
        _fixture.ResetDatabase();
        _fixture.SeedCategoryTable(
            new List<ContactCategory>
            {
                        new ContactCategory {Name = "Family"},
                        new ContactCategory {Name = "Friends"},
                        new ContactCategory {Name = "Work"},
                        new ContactCategory {Name = "School"}
            });

        // Act
        var result = await _fixture.CategoryRepository.AddAsync(category);
        await _fixture.CategoryRepository.SaveChangesAsync();

        // Assert: Repository result
        Assert.True(result.IsSuccess);

        // Assert: Database state
        var saved = await _fixture.Context.ContactCategories.FirstOrDefaultAsync(x => x.Name == category.Name);
        Assert.NotNull(saved);
        Assert.Equal(category.Name, saved.Name);
    }
}
