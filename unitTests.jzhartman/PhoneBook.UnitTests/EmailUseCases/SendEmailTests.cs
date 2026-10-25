using Moq;
using PhoneBook.Application.Email;
using PhoneBook.Application.Interfaces;
using PhoneBook.Domain.Validation;
using PhoneBook.Domain.Validation.Errors;


namespace PhoneBook.UnitTests.EmailUseCases;

public class SendEmailTests
{
    private readonly SendEmailHandler _sut;
    private readonly Mock<IEmailService> _emailServiceMock = new Mock<IEmailService>();

    public SendEmailTests()
    {
        _sut = new SendEmailHandler(_emailServiceMock.Object);
    }

    [Fact]
    public async Task SendEmail_ReturnsFailure_WhenResultIsNull()
    {
        // Arrange
        EmailRequest emailRequest = new EmailRequest("Test Recipient", "Test Subject Line", "Test Message Body");
        _emailServiceMock
            .Setup(x => x.SendEmailAsync(recipient: emailRequest.recipient, subject: emailRequest.subject, body: emailRequest.body))
            .ReturnsAsync(() => null);

        // Act
        var result = await _sut.HandleAsync(emailRequest);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Contains(EmailErrors.NullResponse, result.Errors);
    }

    [Fact]
    public async Task SendEmail_ReturnsFailure_WhenResultIsFailure()
    {
        // Arrange
        EmailRequest emailRequest = new EmailRequest("Test Recipient", "Test Subject Line", "Test Message Body");
        var error = new Error("TestCode", "TestMessage");
        _emailServiceMock
            .Setup(x => x.SendEmailAsync(recipient: emailRequest.recipient, subject: emailRequest.subject, body: emailRequest.body))
            .ReturnsAsync(Result.Failure(error));

        // Act
        var result = await _sut.HandleAsync(emailRequest);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Contains(error, result.Errors);
    }

    [Fact]
    public async Task SendEmail_ReturnsSuccess_WhenResultIsSuccess()
    {
        // Arrange
        EmailRequest emailRequest = new EmailRequest("Test Recipient", "Test Subject Line", "Test Message Body");
        _emailServiceMock
            .Setup(x => x.SendEmailAsync(recipient: emailRequest.recipient, subject: emailRequest.subject, body: emailRequest.body))
            .ReturnsAsync(Result.Success());

        // Act
        var result = await _sut.HandleAsync(emailRequest);

        // Assert
        Assert.True(result.IsSuccess);
    }
}
