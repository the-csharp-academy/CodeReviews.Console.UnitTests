using PhoneBook.Application.Interfaces;
using PhoneBook.Domain.Validation;
using PhoneBook.Domain.Validation.Errors;

namespace PhoneBook.Application.Email;

public class SendEmailHandler
{
    private readonly IEmailService _email;

    public SendEmailHandler(IEmailService email)
    {
        _email = email;
    }

    public async Task<Result> HandleAsync(EmailRequest request)
    {
        var result = await _email.SendEmailAsync(
            recipient: request.recipient,
            subject: request.subject,
            body: request.body);

        if (result is null)
            return Result.Failure(EmailErrors.NullResponse);

        if (result.IsFailure)
            return Result.Failure(result.Errors);

        return Result.Success();
    }
}
