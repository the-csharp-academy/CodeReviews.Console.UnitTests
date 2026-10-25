using Moq;
using PhoneBook.Application.Categories.UpdateCategory;
using PhoneBook.Application.Interfaces;
using PhoneBook.Domain.Entities;
using PhoneBook.Domain.Validation;
using PhoneBook.Domain.Validation.Errors;

namespace PhoneBook.UnitTests.CategoryUseCases;

public class UpdateCategoryTests
{
    private readonly UpdateCategoryNameHandler _sut;
    private readonly Mock<ICategoryRepository> _categoryRepoMock = new Mock<ICategoryRepository>();

    public UpdateCategoryTests()
    {
        _sut = new UpdateCategoryNameHandler(_categoryRepoMock.Object);
    }

    [Fact]
    public async Task UpdateCategory_ReturnsFailure_WhenNameIsUncategorized()
    {
        // Arrange
        var updateCategoryNameRequest = new UpdateCategoryNameRequest(1, "Uncategorized", "Family");

        // Act
        var result = await _sut.HandleAsync(updateCategoryNameRequest);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Contains(CategoryRepositoryErrors.UpdateDefault, result.Errors);
    }

    [Fact]
    public async Task UpdateCategoory_ReturnsFailure_WhenResultIsNull()
    {
        // Arrange
        var updateCategoryNameRequest = new UpdateCategoryNameRequest(1, "Family", "Friends");
        _categoryRepoMock
            .Setup(x => x.UpdateAsync(It.IsAny<ContactCategory>()))
            .ReturnsAsync(() => null);

        // Act
        var result = await _sut.HandleAsync(updateCategoryNameRequest);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Contains(CategoryRepositoryErrors.UpdateResponseNull, result.Errors);
    }

    [Fact]
    public async Task UpdateCategory_ReturnsFailure_WhenResultIsFailure()
    {
        //return Result.Failure(result.Errors);
        // Arrange
        var updateCategoryNameRequest = new UpdateCategoryNameRequest(1, "Family", "Friends");
        var error = new Error("TestCode", "TestMessage");
        _categoryRepoMock
            .Setup(x => x.UpdateAsync(It.IsAny<ContactCategory>()))
            .ReturnsAsync(Result.Failure(error));

        // Act
        var result = await _sut.HandleAsync(updateCategoryNameRequest);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Contains(error, result.Errors);
    }

    [Fact]
    public async Task UpdateCategory_ReturnsSuccess_WhenResultIsSuccess()
    {
        // Arrange
        var updateCategoryNameRequest = new UpdateCategoryNameRequest(1, "Family", "Friends");
        _categoryRepoMock
            .Setup(x => x.UpdateAsync(It.IsAny<ContactCategory>()))
            .ReturnsAsync(Result.Success());

        // Act
        var result = await _sut.HandleAsync(updateCategoryNameRequest);

        // Assert
        Assert.True(result.IsSuccess);
    }
}
