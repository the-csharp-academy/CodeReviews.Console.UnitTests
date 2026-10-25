using Moq;
using PhoneBook.Application.Categories.DTOs;
using PhoneBook.Application.Contacts.SetCategoryIdToDefault;
using PhoneBook.Application.Interfaces;
using PhoneBook.Domain.Entities;
using PhoneBook.Domain.Validation;
using PhoneBook.Domain.Validation.Errors;

namespace PhoneBook.UnitTests.ContactsUseCases;

public class SetCategoryIdToDefaultTests
{
    private readonly SetCategoryIdForContactsToDefaultHandler _sut;
    private readonly Mock<IContactRepository> _contactRepoMock = new Mock<IContactRepository>();

    public SetCategoryIdToDefaultTests()
    {
        _sut = new SetCategoryIdForContactsToDefaultHandler(_contactRepoMock.Object);
    }

    [Fact]
    public async Task SetCategoryIdToDefault_ReturnsFailure_WhenCategoryNameIsUncategorized()
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
    public async Task SetCategoryIdToDefault_ReturnsFailure_WhenResultIsNull()
    {
        // Arrange
        var categoryResponse = new CategoryResponse(1, "Family");
        _contactRepoMock
            .Setup(x => x.SetCategoryIdForContactsToDefaultByCategoryIdAsync(It.IsAny<ContactCategory>()))
            .ReturnsAsync(() => null);

        // Act
        var result = await _sut.HandleAsync(categoryResponse);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Contains(ContactRepositoryErrors.NullResponse, result.Errors);
    }

    [Fact]
    public async Task SetCategoryIdToDefault_ReturnsFailure_WhenResultIsFailure()
    {
        // Arrange
        var categoryResponse = new CategoryResponse(1, "Family");
        var error = new Error("TestCode", "TestMessage");
        _contactRepoMock
            .Setup(x => x.SetCategoryIdForContactsToDefaultByCategoryIdAsync(It.IsAny<ContactCategory>()))
            .ReturnsAsync(Result.Failure(error));

        // Act
        var result = await _sut.HandleAsync(categoryResponse);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Contains(error, result.Errors);
    }

    [Fact]
    public async Task SetCategoryIdToDefault_ReturnsSuccess_WhenResultIsSuccessful()
    {
        // Arrange
        var categoryResponse = new CategoryResponse(1, "Family");
        _contactRepoMock
            .Setup(x => x.SetCategoryIdForContactsToDefaultByCategoryIdAsync(It.IsAny<ContactCategory>()))
            .ReturnsAsync(Result.Success);

        // Act
        var result = await _sut.HandleAsync(categoryResponse);

        // Assert
        Assert.True(result.IsSuccess);
    }
}