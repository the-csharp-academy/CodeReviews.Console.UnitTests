namespace PhoneBook.Application.Contacts.AddContact;

public record AddContactRequest
(
    string FirstName,
    string LastName,
    string PhoneNumber,
    string Email,
    int CategoryId = 1
);