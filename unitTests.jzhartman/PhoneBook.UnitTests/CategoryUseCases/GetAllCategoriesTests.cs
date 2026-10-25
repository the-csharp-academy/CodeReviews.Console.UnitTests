using Moq;
using PhoneBook.Application.Categories.GetAllCategories;
using PhoneBook.Application.Interfaces;
using PhoneBook.Domain.Entities;
using PhoneBook.Domain.Validation;
using PhoneBook.Domain.Validation.Errors;

namespace PhoneBook.UnitTests.CategoryUseCases;

public class GetAllCategoriesTests
{
    private readonly GetAllCategoriesHandler _sut;
    private readonly Mock<ICategoryRepository> _categoryRepoMock = new Mock<ICategoryRepository>();

    public GetAllCategoriesTests()
    {
        _sut = new GetAllCategoriesHandler(_categoryRepoMock.Object);
    }

    [Fact]
    public async Task GetAllCategories_ReturnsFailure_WhenResultIsNull()
    {
        // Arrange
        _categoryRepoMock
            .Setup(x => x.GetAllAsync())
            .ReturnsAsync(() => null);

        // Act
        var result = await _sut.HandleAsync();

        // Assert
        Assert.True(result.IsFailure);
        Assert.Contains(CategoryRepositoryErrors.CategoryNotFound, result.Errors);
    }

    [Fact]
    public async Task GetAllCategories_ReturnsFailure_WhenResultsValueIsNull()
    {
        // Arrange
        _categoryRepoMock
            .Setup(x => x.GetAllAsync())
            .ReturnsAsync(Result<List<ContactCategory>>.Success(null));

        // Act
        var result = await _sut.HandleAsync();

        // Assert
        Assert.True(result.IsFailure);
        Assert.Contains(CategoryRepositoryErrors.CategoryNotFound, result.Errors);
    }

    [Fact]
    public async Task GetAllCategories_ReturnsFailure_WhenValueCountIsLessThanOne()
    {
        // Arrange
        _categoryRepoMock
            .Setup(x => x.GetAllAsync())
            .ReturnsAsync(Result<List<ContactCategory>>.Success(new List<ContactCategory>()));

        // Act
        var result = await _sut.HandleAsync();

        // Assert
        Assert.True(result.IsFailure);
        Assert.Contains(CategoryRepositoryErrors.CategoryNotFound, result.Errors);
    }

    [Fact]
    public async Task GetAllCategories_ReturnsFailure_WhenResultIsFailure()
    {
        // Arrange
        var error = new Error("TestCode", "TestMessage");
        _categoryRepoMock
            .Setup(x => x.GetAllAsync())
            .ReturnsAsync(Result<List<ContactCategory>>.Failure(error));

        // Act
        var result = await _sut.HandleAsync();

        // Assert
        Assert.True(result.IsFailure);
        Assert.Contains(error, result.Errors);
    }

    [Fact]
    public async Task GetAllCategories_ReturnsSuccess_WhenResultIsSuccess()
    {
        // Arrange
        var categories = new List<ContactCategory>
        {
            new ContactCategory{ Id = 1, Name = "Family" },
            new ContactCategory{ Id = 2, Name = "Friends" },
            new ContactCategory{ Id = 3, Name = "Work" },
            new ContactCategory{ Id = 4, Name = "School" },
            new ContactCategory{ Id = 5, Name = "Businesses" },
        };

        _categoryRepoMock
            .Setup(x => x.GetAllAsync())
                .ReturnsAsync(Result<List<ContactCategory>>.Success(categories));

        // Act
        var result = await _sut.HandleAsync();

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Contains(result.Value, x => x.Id == 1 && x.Name == "Family");
        Assert.Contains(result.Value, x => x.Id == 2 && x.Name == "Friends");
        Assert.Contains(result.Value, x => x.Id == 3 && x.Name == "Work");
        Assert.Contains(result.Value, x => x.Id == 4 && x.Name == "School");
        Assert.Contains(result.Value, x => x.Id == 5 && x.Name == "Businesses");
        Assert.True(result.Value.Count == 5);
    }
}