using Moq;
using PhoneBook.Application.Categories.GetCategoryById;
using PhoneBook.Application.Interfaces;
using PhoneBook.Domain.Entities;
using PhoneBook.Domain.Validation;
using PhoneBook.Domain.Validation.Errors;

namespace PhoneBook.UnitTests.CategoryUseCases;

public class GetCategoryByIdTests
{
    private readonly GetCategoryByIdHandler _sut;
    private readonly Mock<ICategoryRepository> _categoryRepoMock = new Mock<ICategoryRepository>();

    public GetCategoryByIdTests()
    {
        _sut = new GetCategoryByIdHandler(_categoryRepoMock.Object);
    }

    [Fact]
    public async Task GetCategoryById_ReturnsFailure_WhenResultIsNull()
    {
        // Arrange
        _categoryRepoMock
            .Setup(x => x.GetByIdAsync(It.IsAny<int>()))
            .ReturnsAsync(() => null);

        // Act
        var result = await _sut.HandleAsync(1);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Contains(CategoryRepositoryErrors.CategoryNotFound, result.Errors);
    }

    [Fact]
    public async Task GetCategoryById_ReturnsFailure_WhenResultIsFailure()
    {
        // Arrange
        var error = new Error("TestCode", "TestMessage");
        _categoryRepoMock
            .Setup(x => x.GetByIdAsync(It.IsAny<int>()))
            .ReturnsAsync(Result<ContactCategory>.Failure(error));

        // Act
        var result = await _sut.HandleAsync(1);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Contains(error, result.Errors);
    }

    [Fact]
    public async Task GetCategoryById_ReturnsFailure_WhenValueIsNull()
    {
        // Arrange
        _categoryRepoMock
            .Setup(x => x.GetByIdAsync(It.IsAny<int>()))
            .ReturnsAsync(Result<ContactCategory>.Success(null));

        // Act
        var result = await _sut.HandleAsync(1);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Contains(CategoryRepositoryErrors.CategoryNotFound, result.Errors);
    }

    [Fact]
    public async Task GetCategoryById_ReturnsSuccess_WhenResultIsSuccessful()
    {
        // Arrange
        var contact = new ContactCategory { Id = 1, Name = "Family" };

        _categoryRepoMock
            .Setup(x => x.GetByIdAsync(contact.Id))
            .ReturnsAsync(Result<ContactCategory>.Success(contact));

        // Act
        var result = await _sut.HandleAsync(contact.Id);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(contact.Id, result.Value.Id);
        Assert.Equal(contact.Name, result.Value.Name);

    }
}
