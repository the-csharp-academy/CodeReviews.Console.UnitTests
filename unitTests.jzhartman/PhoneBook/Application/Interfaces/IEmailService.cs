using PhoneBook.Domain.Validation;

namespace PhoneBook.Application.Interfaces;

public interface IEmailService
{
    Task<Result> SendEmailAsync(string recipient, string subject, string body);
}
