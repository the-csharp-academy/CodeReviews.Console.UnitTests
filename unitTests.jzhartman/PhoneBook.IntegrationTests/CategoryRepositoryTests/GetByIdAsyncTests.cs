using PhoneBook.Domain.Entities;
using PhoneBook.IntegrationTests.Fixtures;

namespace PhoneBook.IntegrationTests.CategoryRepositoryTests;

public class GetByIdAsyncTests : IClassFixture<DatabaseFixture>
{
    private readonly DatabaseFixture _fixture;

    public GetByIdAsyncTests(DatabaseFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsNull_IfIdIsInvalid()
    {
        // Arrange
        var id = 1;
        var expected = new ContactCategory();
        _fixture.ResetDatabase();

        // Act
        var result = await _fixture.CategoryRepository.GetByIdAsync(id);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Null(result.Value);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsCategory_IfIdIsValid()
    {
        // Arrange
        var category = new ContactCategory { Id = 1, Name = "GetByIdAsync Test Category" };
        _fixture.ResetDatabase();
        _fixture.SeedCategoryTable(category);

        // Act
        var result = await _fixture.CategoryRepository.GetByIdAsync(category.Id);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(category.Id, result.Value.Id);
        Assert.Equal(category.Name, result.Value.Name);
    }
}
