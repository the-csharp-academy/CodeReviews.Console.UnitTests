using Moq;
using PhoneBook.Application.Contacts.GetAllContacts;
using PhoneBook.Application.Interfaces;
using PhoneBook.Domain.Entities;
using PhoneBook.Domain.Validation;
using PhoneBook.Domain.Validation.Errors;

namespace PhoneBook.UnitTests.ContactsUseCases;

public class GetAllContactsTests
{
    private readonly GetAllContactsHandler _sut;
    private readonly Mock<IContactRepository> _contactRepoMock = new Mock<IContactRepository>();

    public GetAllContactsTests()
    {
        _sut = new GetAllContactsHandler(_contactRepoMock.Object);
    }

    [Fact]
    public async Task GetAllContacts_ReturnsFailure_WhenResultIsNull()
    {
        // Arrange
        _contactRepoMock
            .Setup(x => x.GetAllAsync())
            .ReturnsAsync(() => null);

        // Act
        var result = await _sut.HandleAsync();

        // Assert
        Assert.True(result.IsFailure);
        Assert.Contains(ContactRepositoryErrors.ContactNotFound, result.Errors);
    }

    [Fact]
    public async Task GetAllContacts_ReturnsFailure_WhenResultIsFailure()
    {
        // Arrange
        var error = new Error("TestCode", "TestMessage");
        _contactRepoMock
            .Setup(x => x.GetAllAsync())
            .ReturnsAsync(Result<List<Contact>>.Failure(error));

        // Act
        var result = await _sut.HandleAsync();

        // Assert
        Assert.True(result.IsFailure);
        Assert.Contains(error, result.Errors);
    }

    [Fact]
    public async Task GetAllContacts_ReturnsFailure_WhenResultValueIsNull()
    {
        // Arrange
        _contactRepoMock
            .Setup(x => x.GetAllAsync())
            .ReturnsAsync(Result<List<Contact>>.Success(null));

        // Act
        var result = await _sut.HandleAsync();

        // Assert
        Assert.True(result.IsFailure);
        Assert.Contains(ContactRepositoryErrors.ContactNotFound, result.Errors);
    }

    [Fact]
    public async Task GetAllContacts_ReturnsFailure_WhenResultValueIsEmpty()
    {
        // Arrange
        _contactRepoMock
            .Setup(x => x.GetAllAsync())
            .ReturnsAsync(Result<List<Contact>>.Success(new List<Contact>()));

        // Act
        var result = await _sut.HandleAsync();

        // Assert
        Assert.True(result.IsFailure);
        Assert.Contains(ContactRepositoryErrors.ContactNotFound, result.Errors);
    }

    [Fact]
    public async Task GetAllContacts_ReturnsSuccess_WhenResultIsSuccessful()
    {
        // Arrange
        var contacts = new List<Contact> {
            new Contact {Id = 1, FirstName = "Leeroy", LastName = "Jenkins", PhoneNumber = "1234567890", Email = "me@email.com", CategoryId = 1 },
            new Contact {Id = 1, FirstName = "Steve", LastName = "Irwin", PhoneNumber = "0987654321", Email = "crikey@crocs.com", CategoryId = 2 } ,
            new Contact {Id = 1, FirstName = "Jimmy", LastName = "John", PhoneNumber = "1111111111", Email = "jj@jj.com", CategoryId = 3 } };
        _contactRepoMock
            .Setup(x => x.GetAllAsync())
            .ReturnsAsync(Result<List<Contact>>.Success(contacts));

        // Act
        var result = await _sut.HandleAsync();

        // Assert
        Assert.True(result.IsSuccess);
        Assert.True(result.Value.Count == 3);
    }
}