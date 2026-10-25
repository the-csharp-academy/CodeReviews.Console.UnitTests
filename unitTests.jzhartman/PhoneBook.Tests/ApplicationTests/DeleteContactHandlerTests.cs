using Moq;
using PhoneBook.Application.Contacts.DeleteContact;
using PhoneBook.Application.Contacts.DTOs;
using PhoneBook.Application.Interfaces;
using PhoneBook.Domain.Entities;
using PhoneBook.Domain.Validation;
using PhoneBook.Domain.Validation.Errors;

namespace PhoneBook.Tests.ApplicationTests;

public class DeleteContactHandlerTests
{
    [Fact]
    public async Task HandleAsync_ShouldReturnFailure_WhenRepositoryReturnsNull()
    {
        // Arrange
        var repoMock = new Mock<IContactRepository>();
        repoMock
            .Setup(r => r.DeleteAsync(It.IsAny<Contact>()))
            .ReturnsAsync((Result<Contact>?)null);

        var handler = new DeleteContactHandler(repoMock.Object);
        var request = new ContactResponse(1, "Billy", "Smith", "555-0147", "bs@mail.com", 1);

        // Act
        var result = await handler.HandleAsync(request);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Contains(Errors.DeleteResponseNull, result.Errors);
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnFailure_WhenRepositoryResturnsFailure()
    {
        // Arrange
        Error[] errors = { new("TestError", "Test error description") };

        var repoMock = new Mock<IContactRepository>();
        repoMock
            .Setup(r => r.DeleteAsync(It.IsAny<Contact>()))
            .ReturnsAsync((Result<Contact>.Failure(errors)));

        var handler = new DeleteContactHandler(repoMock.Object);
        var request = new ContactResponse(1, "Billy", "Smith", "555-0147", "bs@mail.com", 1);

        // Act
        var result = await handler.HandleAsync(request);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Contains(errors, result.Errors.ToArray());
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnSuccessWhenRepositoryReturnsSuccess()
    {
        // Arrange
        var repoMock = new Mock<IContactRepository>();
        repoMock
            .Setup(r => r.DeleteAsync(It.IsAny<Contact>()))
            .ReturnsAsync((Result<Contact>.Success(new Contact())));

        var handler = new DeleteContactHandler(repoMock.Object);
        var request = new ContactResponse(1, "Billy", "Smith", "555-0147", "bs@mail.com", 1);

        // Act
        var result = await handler.HandleAsync(request);

        // Assert
        Assert.True(result.IsSuccess);
    }
}
