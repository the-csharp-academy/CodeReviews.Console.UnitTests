namespace PhoneBook.Application.Contacts.DTOs;

public record ContactResponse
(
    int ContactId,
    string FirstName,
    string LastName,
    string PhoneNumber,
    string Email,
    int CategoryId
);