using Microsoft.EntityFrameworkCore;
using PhoneBook.Domain.Entities;
using PhoneBook.Domain.Validation.Errors;
using PhoneBook.IntegrationTests.Fixtures;

namespace PhoneBook.IntegrationTests.CategoryRepositoryTests;

public class DeleteAsyncTests : IClassFixture<DatabaseFixture>
{
    private readonly DatabaseFixture _fixture;

    public DeleteAsyncTests(DatabaseFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task DeleteAsync_ReturnsFailure_WhenIdIsInvalid()
    {
        // Arrange
        var category = new ContactCategory { Name = "Family" };
        _fixture.ResetDatabase();

        // Act
        var result = await _fixture.CategoryRepository.DeleteAsync(category);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Contains(CategoryRepositoryErrors.CategoryDoesNotExist, result.Errors);
    }

    [Fact]
    public async Task DeleteAsync_ReturnsSuccess_WhenIdIsValid()
    {
        // Arrange
        var category = new ContactCategory { Id = 0, Name = "Family" };
        _fixture.ResetDatabase();
        _fixture.SeedCategoryTable(category);

        // Act
        var result = await _fixture.CategoryRepository.DeleteAsync(category);

        // Assert
        Assert.True(result.IsSuccess);
        var saved = await _fixture.Context.ContactCategories.FirstOrDefaultAsync(x => x.Id == category.Id);
        Assert.Null(saved);
    }
}
