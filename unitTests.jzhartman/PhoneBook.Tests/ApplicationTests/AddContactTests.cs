using Moq;
using PhoneBook.Application.Contacts.AddContact;
using PhoneBook.Application.Interfaces;
using PhoneBook.Domain.Entities;
using PhoneBook.Domain.Validation;
using PhoneBook.Domain.Validation.Errors;

namespace PhoneBook.Tests.ApplicationTests;

public class AddContactTests
{
    [Fact]
    public async Task HandleAsync_ShouldReturnFailure_WhenRepositoryReturnsNull()
    {
        // Arrange
        var repoMock = new Mock<IContactRepository>();
        repoMock
            .Setup(r => r.AddAsync(It.IsAny<Contact>()))
            .ReturnsAsync((Result<Contact>?)null);

        var handler = new AddContactHandler(repoMock.Object);
        var request = new AddContactRequest("Billy", "Smith", "bs@mail.com", "555-0147");

        // Act
        var result = await handler.HandleAsync(request);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Contains(Errors.AddResponseNull, result.Errors);

    }

    [Fact]
    public async Task HandleAsync_ShouldReturnFailure_WhenRespositoryReturnsFailure()
    {
        // Arrange
        Error[] errors = { new("TestError", "Test error description") };

        var repoMock = new Mock<IContactRepository>();
        repoMock
            .Setup(r => r.AddAsync(It.IsAny<Contact>()))
            .ReturnsAsync(Result<Contact>.Failure(errors));

        var handler = new AddContactHandler(repoMock.Object);
        var request = new AddContactRequest("Billy", "Smith", "bs@mail.com", "555-0147");

        // Act
        var result = await handler.HandleAsync(request);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Contains(errors, result.Errors.ToArray());
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnSuccess_WhenRepositoryReturnsSuccess()
    {
        // Arrange
        var repoMock = new Mock<IContactRepository>();
        repoMock
            .Setup(r => r.AddAsync(It.IsAny<Contact>()))
            .ReturnsAsync(Result<Contact>.Success(new Contact()));

        var handler = new AddContactHandler(repoMock.Object);
        var request = new AddContactRequest("Billy", "Smith", "bs@mail.com", "555-0147");

        // Act
        var result = await handler.HandleAsync(request);

        // Assert
        Assert.True(result.IsSuccess);
    }
}
