using PhoneBook.Application.Categories.DTOs;
using PhoneBook.Application.Interfaces;
using PhoneBook.Domain.Entities;
using PhoneBook.Domain.Validation;
using PhoneBook.Domain.Validation.Errors;

namespace PhoneBook.Application.Contacts.SetCategoryIdToDefault;

public class SetCategoryIdForContactsToDefaultHandler
{
    private readonly IContactRepository _contactRepo;

    public SetCategoryIdForContactsToDefaultHandler(IContactRepository contactRepo)
    {
        _contactRepo = contactRepo;
    }

    public async Task<Result> HandleAsync(CategoryResponse category)
    {
        if (category.Name.ToUpper() == "UNCATEGORIZED")
            return Result.Failure(CategoryRepositoryErrors.DeleteDefault);

        var result = await _contactRepo.SetCategoryIdForContactsToDefaultByCategoryIdAsync(
                            new ContactCategory
                            {
                                Id = category.Id,
                                Name = category.Name
                            });

        if (result is null)
            return Result.Failure(ContactRepositoryErrors.NullResponse);
        if (result.IsFailure)
            return Result.Failure(result.Errors);

        return Result.Success();
    }
}
