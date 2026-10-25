using Moq;
using PhoneBook.Application.Contacts.DTOs;
using PhoneBook.Application.Contacts.EditContact;
using PhoneBook.Application.Interfaces;
using PhoneBook.Domain.Entities;
using PhoneBook.Domain.Validation;
using PhoneBook.Domain.Validation.Errors;

namespace PhoneBook.UnitTests.ContactsUseCases;

public class EditContactTests
{
    private readonly EditContactHandler _sut;
    private readonly Mock<IContactRepository> _contactRepoMock = new Mock<IContactRepository>();

    public EditContactTests()
    {
        _sut = new EditContactHandler(_contactRepoMock.Object);
    }

    [Fact]
    public async Task EditContact_ReturnsFailure_WhenResultIsNull()
    {
        // Arrange
        var contactResponse = new ContactResponse(1, "Leeroy", "Jenkins", "0123456789", "lee@roy.com", 1);
        _contactRepoMock
            .Setup(x => x.UpdateAsync(It.IsAny<Contact>()))
            .ReturnsAsync(() => null);

        // Act
        var result = await _sut.HandleAsync(contactResponse);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Contains(ContactRepositoryErrors.UpdateResponseNull, result.Errors);
    }

    [Fact]
    public async Task EditContact_ReturnsFailure_WhenResultIsFailure()
    {
        // Arrange
        var contactResponse = new ContactResponse(1, "Leeroy", "Jenkins", "0123456789", "lee@roy.com", 1);
        var error = new Error("TestCode", "TestMessage");
        _contactRepoMock
            .Setup(x => x.UpdateAsync(It.IsAny<Contact>()))
            .ReturnsAsync(Result.Failure(error));

        // Act
        var result = await _sut.HandleAsync(contactResponse);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Contains(error, result.Errors);
    }

    [Fact]
    public async Task EditContact_ReturnsSuccess_WhenResultIsSuccessful()
    {
        // Arrange
        var contactResponse = new ContactResponse(1, "Leeroy", "Jenkins", "0123456789", "lee@roy.com", 1);
        _contactRepoMock
            .Setup(x => x.UpdateAsync(It.IsAny<Contact>()))
            .ReturnsAsync(Result.Success());

        // Act
        var result = await _sut.HandleAsync(contactResponse);

        // Assert
        Assert.True(result.IsSuccess);
    }
}
