using Moq;
using PhoneBook.Application.GetById;
using PhoneBook.Application.Interfaces;
using PhoneBook.Domain.Entities;
using PhoneBook.Domain.Validation;
using PhoneBook.Domain.Validation.Errors;

namespace PhoneBook.UnitTests.ContactsUseCases;

public class GetContactByIdTests
{
    private readonly GetContactByIdHandler _sut;
    private readonly Mock<IContactRepository> _contactRepoMock = new Mock<IContactRepository>();

    public GetContactByIdTests()
    {
        _sut = new GetContactByIdHandler(_contactRepoMock.Object);
    }

    [Fact]
    public async Task GetContactById_ReturnsFailure_WhenResultIsNull()
    {
        // Arrange
        int id = 1;
        _contactRepoMock
            .Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync(() => null);

        // Act
        var result = await _sut.HandleAsync(id);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Contains(ContactRepositoryErrors.ContactNotFound, result.Errors);
    }

    [Fact]
    public async Task GetContactById_ReturnsFailure_WhenResultIsFailure()
    {
        // Arrange
        int id = 1;
        var error = new Error("TestCode", "TestMessage");
        _contactRepoMock
            .Setup(x => x.GetByIdAsync(id))
            .ReturnsAsync(Result<Contact>.Failure(error));

        // Act
        var result = await _sut.HandleAsync(id);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Contains(error, result.Errors);
    }

    [Fact]
    public async Task GetContactById_ReturnsFailure_WhenResultValueIsNull()
    {
        // Arrange
        int id = 1;
        _contactRepoMock
            .Setup(x => x.GetByIdAsync(id))
            .ReturnsAsync(Result<Contact>.Success(null));

        // Act
        var result = await _sut.HandleAsync(id);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Contains(ContactRepositoryErrors.ContactNotFound, result.Errors);
    }

    [Fact]
    public async Task GetContactById_ReturnsSuccess_WhenResultIsSuccess()
    {
        // Arrange
        int id = 1;
        var contact = new Contact { Id = 1, FirstName = "Leeroy", LastName = "Jenkins", PhoneNumber = "1234567890", Email = "me@email.com", CategoryId = 1 };
        _contactRepoMock
            .Setup(x => x.GetByIdAsync(id))
            .ReturnsAsync(Result<Contact>.Success(contact));

        // Act
        var result = await _sut.HandleAsync(id);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.True(
            result.Value.ContactId == contact.Id &&
            result.Value.FirstName == contact.FirstName &&
            result.Value.LastName == contact.LastName &&
            result.Value.PhoneNumber == contact.PhoneNumber &&
            result.Value.Email == contact.Email &&
            result.Value.CategoryId == contact.CategoryId);
    }
}
