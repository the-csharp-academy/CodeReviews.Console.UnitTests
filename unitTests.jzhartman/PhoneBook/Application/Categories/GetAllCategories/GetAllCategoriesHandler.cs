using PhoneBook.Application.Categories.DTOs;
using PhoneBook.Application.Interfaces;
using PhoneBook.Domain.Entities;
using PhoneBook.Domain.Validation;
using PhoneBook.Domain.Validation.Errors;

namespace PhoneBook.Application.Categories.GetAllCategories;

public class GetAllCategoriesHandler
{
    private readonly ICategoryRepository _repo;

    public GetAllCategoriesHandler(ICategoryRepository repo)
    {
        _repo = repo;
    }

    public async Task<Result<List<CategoryResponse>>> HandleAsync()
    {
        var result = await _repo.GetAllAsync();

        if (result is null)
            return Result<List<CategoryResponse>>.Failure(new Error[] { CategoryRepositoryErrors.CategoryNotFound });

        if (result.IsFailure)
            return Result<List<CategoryResponse>>.Failure(result.Errors);

        if (result.Value is null || result.Value.Count < 1)
            return Result<List<CategoryResponse>>.Failure(new Error[] { CategoryRepositoryErrors.CategoryNotFound });

        return Result<List<CategoryResponse>>.Success(MapToCategoryResponse(result.Value));
    }

    private List<CategoryResponse> MapToCategoryResponse(List<ContactCategory> categories)
    {
        var categoryResponseList = new List<CategoryResponse>();

        foreach (var category in categories)
        {
            categoryResponseList.Add(new CategoryResponse
            (
                category.Id,
                category.Name
            ));
        }

        return categoryResponseList;
    }
}
