using PhoneBook.Application.Contacts.DTOs;
using PhoneBook.Application.Interfaces;
using PhoneBook.Domain.Validation;
using PhoneBook.Domain.Validation.Errors;

namespace PhoneBook.Application.Contacts.GetAllContacts;

public class GetAllContactsHandler
{
    private readonly IContactRepository _repo;

    public GetAllContactsHandler(IContactRepository repo)
    {
        _repo = repo;
    }

    public async Task<Result<List<ContactResponse>>> HandleAsync()
    {
        var result = await _repo.GetAllAsync();

        if (result is null)
            return Result<List<ContactResponse>>.Failure(ContactRepositoryErrors.ContactNotFound);

        if (result.IsFailure)
            return Result<List<ContactResponse>>.Failure(result.Errors);

        if (result.Value is null || result.Value.Count < 1)
            return Result<List<ContactResponse>>.Failure(ContactRepositoryErrors.ContactNotFound);

        return Result<List<ContactResponse>>.Success(ContactResponseHelper.MapToContactResponse(result.Value));
    }
}
