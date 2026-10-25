using PhoneBook.Application.Interfaces;
using PhoneBook.Domain.Entities;
using PhoneBook.Domain.Validation;
using PhoneBook.Domain.Validation.Errors;

namespace PhoneBook.Application.Categories.UpdateCategory;

public class UpdateCategoryNameHandler
{
    private readonly ICategoryRepository _repo;

    public UpdateCategoryNameHandler(ICategoryRepository repo)
    {
        _repo = repo;
    }

    public async Task<Result> HandleAsync(UpdateCategoryNameRequest category)
    {
        if (category.OriginalName.ToUpper() == "UNCATEGORIZED")
            return Result.Failure(CategoryRepositoryErrors.UpdateDefault);

        var result = await _repo.UpdateAsync(new ContactCategory { Id = category.Id, Name = category.NewName });

        if (result is null)
            return Result.Failure(CategoryRepositoryErrors.UpdateResponseNull);

        if (result.IsFailure)
            return Result.Failure(result.Errors);

        return Result.Success();
    }
}
