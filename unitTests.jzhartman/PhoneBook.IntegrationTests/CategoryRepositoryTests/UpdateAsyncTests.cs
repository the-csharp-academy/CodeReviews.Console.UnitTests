using Microsoft.EntityFrameworkCore;
using PhoneBook.Domain.Entities;
using PhoneBook.Domain.Validation.Errors;
using PhoneBook.IntegrationTests.Fixtures;

namespace PhoneBook.IntegrationTests.CategoryRepositoryTests;

public class UpdateAsyncTests : IClassFixture<DatabaseFixture>
{
    private readonly DatabaseFixture _fixture;

    public UpdateAsyncTests(DatabaseFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task UpdateAsync_ReturnsFailure_IfNameAlreadyExists()
    {
        // Arrange
        var category = new ContactCategory { Name = "Family" };
        _fixture.ResetDatabase();
        _fixture.SeedCategoryTable(category);

        // Act
        var result = await _fixture.CategoryRepository.UpdateAsync(category);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Contains(CategoryRepositoryErrors.CategoryExists, result.Errors);
    }

    [Fact]
    public async Task UpdateAsync_ReturnsFailure_IfUpdatedRowResponseEqualsZero()
    {
        // Arrange
        var category = new ContactCategory { Id = 1, Name = "Family" };
        _fixture.ResetDatabase();
        _fixture.SeedCategoryTable(category);

        // Act
        var result = await _fixture.CategoryRepository.UpdateAsync(new ContactCategory { Id = 2, Name = "Work" });

        // Assert
        Assert.True(result.IsFailure);
        Assert.Contains(CategoryRepositoryErrors.UpdateDataFailed, result.Errors);
        var saved = await _fixture.Context.ContactCategories.FirstOrDefaultAsync(x => x.Id == category.Id);
        Assert.Equal(category.Name, saved.Name);
    }

    [Fact]
    public async Task UpdateAsync_ReturnsSuccess_WhenNameIsUnique()
    {
        // Arrange
        var seedCategory = new ContactCategory { Id = 1, Name = "Family" };
        var updatedCategory = new ContactCategory { Id = 1, Name = "Work" };
        _fixture.ResetDatabase();
        _fixture.SeedCategoryTable(seedCategory);

        // Act
        var result = await _fixture.CategoryRepository.UpdateAsync(updatedCategory);

        // Assert
        Assert.True(result.IsSuccess);
        _fixture.Context.ChangeTracker.Clear();
        var saved = await _fixture.Context.ContactCategories.FirstOrDefaultAsync(x => x.Id == updatedCategory.Id);
        Assert.Equal(updatedCategory.Name, saved.Name);
    }
}
