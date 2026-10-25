using Moq;
using PhoneBook.Application.Contacts.SaveChanges;
using PhoneBook.Application.Interfaces;
using PhoneBook.Domain.Validation;
using PhoneBook.Domain.Validation.Errors;

namespace PhoneBook.UnitTests.ContactsUseCases;

public class SaveChangesTests
{
    private readonly SaveChangesHandler _sut;
    private readonly Mock<IContactRepository> _contactRepoMock = new Mock<IContactRepository>();

    public SaveChangesTests()
    {
        _sut = new SaveChangesHandler(_contactRepoMock.Object);
    }

    [Fact]
    public async Task SaveChanges_ReturnsFailure_WhenResultIsNull()
    {
        // Arrange
        _contactRepoMock
            .Setup(x => x.SaveChangesAsync())
            .ReturnsAsync(() => null);

        // Act
        var result = await _sut.HandleAsync();

        // Assert
        Assert.True(result.IsFailure);
        Assert.Contains(ContactRepositoryErrors.SaveResponseNull, result.Errors);
    }

    [Fact]
    public async Task SaveChanges_ReturnsFailure_WhenResultIsFailiuire()
    {
        // Arrange
        var error = new Error("TestCode", "TestMessage");
        _contactRepoMock
            .Setup(x => x.SaveChangesAsync())
            .ReturnsAsync(Result.Failure(error));

        // Act
        var result = await _sut.HandleAsync();

        // Assert
        Assert.True(result.IsFailure);
        Assert.Contains(error, result.Errors);
    }

    [Fact]
    public async Task SaveChanges_ReturnsSuccess_WhenResultIsSuccess()
    {
        // Arrange
        _contactRepoMock
            .Setup(x => x.SaveChangesAsync())
            .ReturnsAsync(Result.Success());

        // Act
        var result = await _sut.HandleAsync();

        // Assert
        Assert.True(result.IsSuccess);
    }
}
