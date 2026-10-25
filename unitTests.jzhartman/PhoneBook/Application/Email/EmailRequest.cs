namespace PhoneBook.Application.Email;

public record EmailRequest(string recipient, string subject, string body);
