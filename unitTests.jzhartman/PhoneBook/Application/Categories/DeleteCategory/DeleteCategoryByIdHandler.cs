using PhoneBook.Application.Categories.DTOs;
using PhoneBook.Application.Interfaces;
using PhoneBook.Domain.Entities;
using PhoneBook.Domain.Validation;
using PhoneBook.Domain.Validation.Errors;

namespace PhoneBook.Application.Categories.DeleteCategory;

public class DeleteCategoryByIdHandler
{
    private readonly ICategoryRepository _categoryRepo;

    public DeleteCategoryByIdHandler(ICategoryRepository categoryRepo)
    {
        _categoryRepo = categoryRepo;
    }

    public async Task<Result> HandleAsync(CategoryResponse categoryResponse)
    {
        if (categoryResponse.Name.ToUpper() == "UNCATEGORIZED")
            return Result.Failure(CategoryRepositoryErrors.DeleteDefault);

        var category = new ContactCategory
        {
            Id = categoryResponse.Id,
            Name = categoryResponse.Name
        };

        var deleteCategoryResponse = await _categoryRepo.DeleteAsync(category);

        if (deleteCategoryResponse is null)
            return Result.Failure(CategoryRepositoryErrors.DeleteResponseNull);

        if (deleteCategoryResponse.IsFailure)
            return Result.Failure(deleteCategoryResponse.Errors);

        return Result.Success();
    }
}
