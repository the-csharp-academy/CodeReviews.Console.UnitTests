using Moq;
using PhoneBook.Application.Contacts.AddContact;
using PhoneBook.Application.Interfaces;
using PhoneBook.Domain.Entities;
using PhoneBook.Domain.Validation;
using PhoneBook.Domain.Validation.Errors;

namespace PhoneBook.UnitTests.ContactsUseCases;

public class AddContactTests
{
    private readonly AddContactHandler _sut;
    private readonly Mock<IContactRepository> _contactRepoMock = new Mock<IContactRepository>();

    public AddContactTests()
    {
        _sut = new AddContactHandler(_contactRepoMock.Object);
    }

    [Fact]
    public async Task AddContact_ReturnsFailure_WhenResultIsNull()
    {
        // Arrange
        var addContactRequest = new AddContactRequest("Leeroy", "Jenkins", "0123456789", "lee@roy.com", 1);
        _contactRepoMock
            .Setup(x => x.AddAsync(It.IsAny<Contact>()))
            .ReturnsAsync(() => null);

        // Act
        var result = await _sut.HandleAsync(addContactRequest);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Contains(ContactRepositoryErrors.AddResponseNull, result.Errors);
    }

    [Fact]
    public async Task AddContact_ReturnsFailure_WhenResultIsFailure()
    {
        // Arrange
        var addContactRequest = new AddContactRequest("Leeroy", "Jenkins", "0123456789", "lee@roy.com", 1);
        var error = new Error("TestCode", "TestMessage");
        _contactRepoMock
            .Setup(x => x.AddAsync(It.IsAny<Contact>()))
            .ReturnsAsync(Result<Contact>.Failure(error));

        // Act
        var result = await _sut.HandleAsync(addContactRequest);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Contains(error, result.Errors);
    }

    [Fact]
    public async Task AddContact_ReturnsSuccess_WhenResultIsSuccessful()
    {
        // Arrange
        var addContactRequest = new AddContactRequest("Leeroy", "Jenkins", "0123456789", "lee@roy.com", 1);
        _contactRepoMock
            .Setup(x => x.AddAsync(It.IsAny<Contact>()))
            .ReturnsAsync(Result<Contact>.Success(It.IsAny<Contact>()));

        // Act
        var result = await _sut.HandleAsync(addContactRequest);

        // Assert
        Assert.True(result.IsSuccess);
    }
}