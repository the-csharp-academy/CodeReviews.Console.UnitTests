using Moq;
using PhoneBook.Application.Categories.DeleteCategory;
using PhoneBook.Application.Categories.DTOs;
using PhoneBook.Application.Interfaces;
using PhoneBook.Domain.Entities;
using PhoneBook.Domain.Validation;
using PhoneBook.Domain.Validation.Errors;

namespace PhoneBook.UnitTests.CategoryUseCases;

public class DeleteCategoryByIdTests
{
    public readonly DeleteCategoryByIdHandler _sut;
    private readonly Mock<ICategoryRepository> _categoryRepoMock = new Mock<ICategoryRepository>();

    public DeleteCategoryByIdTests()
    {
        _sut = new DeleteCategoryByIdHandler(_categoryRepoMock.Object);
    }

    [Fact]
    public async Task DeleteCategoryById_ReturnsFailure_WhenCategoryNameIsUncategorized()
    {
        // Arrange
        var categoryResponse = new CategoryResponse(1, "Uncategorized");

        // Act
        var result = await _sut.HandleAsync(categoryResponse);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Contains(CategoryRepositoryErrors.DeleteDefault, result.Errors);
    }

    [Fact]
    public async Task DeleteCategoryById_ReturnsFailure_WhenResultIsNull()
    {
        // Arrange
        var categoryResponse = new CategoryResponse(1, "Family");
        _categoryRepoMock
            .Setup(x => x.DeleteAsync(It.IsAny<ContactCategory>()))
            .ReturnsAsync(() => null);

        // Act
        var result = await _sut.HandleAsync(categoryResponse);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Contains(CategoryRepositoryErrors.DeleteResponseNull, result.Errors);
    }

    [Fact]
    public async Task DeleteCategoryById_ReturnsFailure_WhenResultIsFailure()
    {
        // Arrange
        var categoryResponse = new CategoryResponse(1, "Family");
        var error = new Error("TestCode", "TestMessage");
        _categoryRepoMock
            .Setup(x => x.DeleteAsync(It.IsAny<ContactCategory>()))
            .ReturnsAsync(Result.Failure(error));

        // Act
        var result = await _sut.HandleAsync(categoryResponse);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Contains(error, result.Errors);
    }

    [Fact]
    public async Task DeleteCategoryById_ReturnsSuccess_WhenResultIsSuccess()
    {
        // Arrange
        var categoryResponse = new CategoryResponse(1, "Family");
        _categoryRepoMock
            .Setup(x => x.DeleteAsync(It.Is<ContactCategory>(c => c.Id == 1 && c.Name == "Family")))
            .ReturnsAsync(Result.Success());

        // Act
        var result = await _sut.HandleAsync(categoryResponse);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.True(result.Errors.Count == 0);
    }
}
