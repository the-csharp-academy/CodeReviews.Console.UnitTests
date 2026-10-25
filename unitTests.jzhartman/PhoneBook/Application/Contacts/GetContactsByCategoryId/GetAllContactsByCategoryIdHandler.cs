using PhoneBook.Application.Categories.DTOs;
using PhoneBook.Application.Contacts.DTOs;
using PhoneBook.Application.Interfaces;
using PhoneBook.Domain.Validation;
using PhoneBook.Domain.Validation.Errors;

namespace PhoneBook.Application.Contacts.GetContactsByCategoryId;

public class GetAllContactsByCategoryIdHandler
{
    private readonly IContactRepository _repo;

    public GetAllContactsByCategoryIdHandler(IContactRepository repo)
    {
        _repo = repo;
    }

    public async Task<Result<List<ContactResponse>>> HandleAsync(CategoryResponse category)
    {
        var result = await _repo.GetByCategoryIdAsync(category.Id);

        if (result is null)
            return Result<List<ContactResponse>>.Failure(ContactRepositoryErrors.ContactNotFound);

        if (result.IsFailure)
            return Result<List<ContactResponse>>.Failure(result.Errors);

        if (result.Value is null || result.Value.Count < 1)
            return Result<List<ContactResponse>>.Failure(ContactRepositoryErrors.ContactNotFound);

        return Result<List<ContactResponse>>.Success(ContactResponseHelper.MapToContactResponse(result.Value));
    }
}
