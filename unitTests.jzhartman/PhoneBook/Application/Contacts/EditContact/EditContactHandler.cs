using PhoneBook.Application.Contacts.DTOs;
using PhoneBook.Application.Interfaces;
using PhoneBook.Domain.Entities;
using PhoneBook.Domain.Validation;
using PhoneBook.Domain.Validation.Errors;

namespace PhoneBook.Application.Contacts.EditContact;

public class EditContactHandler
{
    private readonly IContactRepository _repo;

    public EditContactHandler(IContactRepository repo)
    {
        _repo = repo;
    }

    public async Task<Result> HandleAsync(ContactResponse contact)
    {
        var result = await _repo.UpdateAsync(new Contact
        {
            Id = contact.ContactId,
            FirstName = contact.FirstName,
            LastName = contact.LastName,
            PhoneNumber = contact.PhoneNumber,
            Email = contact.Email,
            CategoryId = contact.CategoryId
        });

        if (result is null)
            return Result.Failure(ContactRepositoryErrors.UpdateResponseNull);

        if (result.IsFailure)
            return Result.Failure(result.Errors);

        return Result.Success();
    }
}
