using PhoneBook.Application.Interfaces;
using PhoneBook.Domain.Entities;
using PhoneBook.Domain.Validation;
using PhoneBook.Domain.Validation.Errors;

namespace PhoneBook.Application.Categories.AddCategory;

public class AddCategoryHandler
{
    private readonly ICategoryRepository _repo;

    public AddCategoryHandler(ICategoryRepository repo)
    {
        _repo = repo;
    }

    public async Task<Result> HandleAsync(AddCategoryRequest category)
    {
        var result = await _repo.AddAsync(
            new ContactCategory
            {
                Name = category.Name
            });

        if (result is null)
            return Result.Failure(CategoryRepositoryErrors.AddResponseNull);

        if (result.IsFailure)
            return Result.Failure(result.Errors);

        return Result.Success();
    }
}
