using PhoneBook.Application.Contacts.DTOs;
using PhoneBook.Application.Interfaces;
using PhoneBook.Domain.Validation;
using PhoneBook.Domain.Validation.Errors;

namespace PhoneBook.Application.GetById;

public class GetContactByIdHandler
{
    private readonly IContactRepository _repo;

    public GetContactByIdHandler(IContactRepository repo)
    {
        _repo = repo;
    }

    public async Task<Result<ContactResponse>> HandleAsync(int contactId)
    {
        var result = await _repo.GetByIdAsync(contactId);

        if (result is null)
            return Result<ContactResponse>.Failure(ContactRepositoryErrors.ContactNotFound);

        if (result.IsFailure)
            return Result<ContactResponse>.Failure(result.Errors);

        if (result.Value is null)
            return Result<ContactResponse>.Failure(ContactRepositoryErrors.ContactNotFound);


        return Result<ContactResponse>.Success(new ContactResponse
            (
                result.Value.Id,
                result.Value.FirstName,
                result.Value.LastName,
                result.Value.PhoneNumber,
                result.Value.Email,
                result.Value.CategoryId
            ));
    }
}
