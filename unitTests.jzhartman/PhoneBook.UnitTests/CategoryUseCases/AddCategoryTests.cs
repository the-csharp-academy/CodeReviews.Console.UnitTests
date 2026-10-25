using Moq;
using PhoneBook.Application.Categories.AddCategory;
using PhoneBook.Application.Interfaces;
using PhoneBook.Domain.Entities;
using PhoneBook.Domain.Validation;
using PhoneBook.Domain.Validation.Errors;

namespace PhoneBook.UnitTests.CategoryUseCases;

public class AddCategoryTests
{
    private readonly AddCategoryHandler _sut;
    private readonly Mock<ICategoryRepository> _categoryRepoMock = new Mock<ICategoryRepository>();

    public AddCategoryTests()
    {
        _sut = new AddCategoryHandler(_categoryRepoMock.Object);
    }

    [Fact]
    public async Task AddCategory_ReturnsSuccess_WhenCategoryAdded()
    {
        // Arrange
        var addCategoryRequest = new AddCategoryRequest("Family");
        _categoryRepoMock
            .Setup(x => x.AddAsync(It.Is<ContactCategory>(c => c.Name == addCategoryRequest.Name)))
            .ReturnsAsync(Result.Success());

        // Act
        var result = await _sut.HandleAsync(addCategoryRequest);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.True(result.Errors.Count == 0);
    }

    [Fact]
    public async Task AddCategory_ReturnsFailure_WhenResultIsNull()
    {
        // Arrange
        var addCategoryRequest = new AddCategoryRequest("Family");
        _categoryRepoMock
            .Setup(x => x.AddAsync(It.IsAny<ContactCategory>()))
            .ReturnsAsync(() => null);

        // Act
        var result = await _sut.HandleAsync(addCategoryRequest);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Contains(CategoryRepositoryErrors.AddResponseNull, result.Errors);
    }

    [Fact]
    public async Task AddCategory_ReturnsFailure_WhenResultIsFailure()
    {
        // Arrange
        var addCategoryRequest = new AddCategoryRequest("Family");
        var error = new Error("TestCode", "TestMessage");
        _categoryRepoMock
            .Setup(x => x.AddAsync(It.IsAny<ContactCategory>()))
            .ReturnsAsync(Result.Failure(error));

        // Act
        var result = await _sut.HandleAsync(addCategoryRequest);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Contains(error, result.Errors);
    }
}
