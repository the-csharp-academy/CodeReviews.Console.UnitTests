using PhoneBook.Domain.Entities;
using PhoneBook.IntegrationTests.Fixtures;

namespace PhoneBook.IntegrationTests.CategoryRepositoryTests;

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
        var result = await _fixture.CategoryRepository.GetAllAsync();

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Empty(result.Value);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsPopulatedList_WhenTableIsPopulated()
    {
        // Arrange
        var expected = new List<ContactCategory>
        {
            new ContactCategory {Name = "Family"},
            new ContactCategory {Name = "Friends"},
            new ContactCategory {Name = "Work"},
            new ContactCategory {Name = "School"}
        };
        _fixture.ResetDatabase();
        _fixture.SeedCategoryTable(expected);

        // Act
        var result = await _fixture.CategoryRepository.GetAllAsync();

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(
            expected.Select(x => x.Name),
            result.Value.Select(x => x.Name));
    }
}
