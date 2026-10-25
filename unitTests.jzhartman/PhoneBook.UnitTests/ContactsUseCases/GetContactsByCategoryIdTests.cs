using Moq;
using PhoneBook.Application.Categories.DTOs;
using PhoneBook.Application.Contacts.GetContactsByCategoryId;
using PhoneBook.Application.Interfaces;
using PhoneBook.Domain.Entities;
using PhoneBook.Domain.Validation;
using PhoneBook.Domain.Validation.Errors;

namespace PhoneBook.UnitTests.ContactsUseCases;

public class GetContactsByCategoryIdTests
{
    private readonly GetAllContactsByCategoryIdHandler _sut;
    private readonly Mock<IContactRepository> _contactRepoMock = new Mock<IContactRepository>();

    public GetContactsByCategoryIdTests()
    {
        _sut = new GetAllContactsByCategoryIdHandler(_contactRepoMock.Object);
    }

    [Fact]
    public async Task GetAllContactsByCategoryId_ReturnsFailure_WhenResultIsNull()
    {
        // Arrange
        var category = new CategoryResponse(1, "Family");
        _contactRepoMock
            .Setup(x => x.GetByCategoryIdAsync(category.Id))
            .ReturnsAsync(() => null);

        // Act
        var result = await _sut.HandleAsync(category);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Contains(ContactRepositoryErrors.ContactNotFound, result.Errors);
    }

    [Fact]
    public async Task GetAllContactsByCategoryId_ReturnsFailure_WhenResultIsFailure()
    {
        // Arrange
        var category = new CategoryResponse(1, "Family");
        var error = new Error("TestCode", "TestMessage");
        _contactRepoMock
            .Setup(x => x.GetByCategoryIdAsync(category.Id))
            .ReturnsAsync(Result<List<Contact>>.Failure(error));

        // Act
        var result = await _sut.HandleAsync(category);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Contains(error, result.Errors);
    }

    [Fact]
    public async Task GetAllContactsByCategoryId_ReturnsFailure_WhenResultValueIsNull()
    {
        // Arrange
        var category = new CategoryResponse(1, "Family");
        _contactRepoMock
            .Setup(x => x.GetByCategoryIdAsync(category.Id))
            .ReturnsAsync(Result<List<Contact>>.Success(null));

        // Act
        var result = await _sut.HandleAsync(category);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Contains(ContactRepositoryErrors.ContactNotFound, result.Errors);
    }

    [Fact]
    public async Task GetAllContactsByCategoryId_ReturnsSuccess_WhenResultIsSuccess()
    {
        // Arrange
        var category = new CategoryResponse(1, "Family");
        var contacts = new List<Contact> {
            new Contact {Id = 1, FirstName = "Leeroy", LastName = "Jenkins", PhoneNumber = "1234567890", Email = "me@email.com", CategoryId = 1 },
            new Contact {Id = 1, FirstName = "Steve", LastName = "Irwin", PhoneNumber = "0987654321", Email = "crikey@crocs.com", CategoryId = 2 } ,
            new Contact {Id = 1, FirstName = "Jimmy", LastName = "John", PhoneNumber = "1111111111", Email = "jj@jj.com", CategoryId = 3 } };

        _contactRepoMock
            .Setup(x => x.GetByCategoryIdAsync(category.Id))
            .ReturnsAsync(Result<List<Contact>>.Success(contacts));

        // Act
        var result = await _sut.HandleAsync(category);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.True(result.Value.Count == 3);
    }
}
