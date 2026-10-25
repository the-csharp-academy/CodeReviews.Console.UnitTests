using Microsoft.EntityFrameworkCore;
using PhoneBook.Application.Interfaces;
using PhoneBook.Domain.Entities;
using PhoneBook.Domain.Validation;
using PhoneBook.Domain.Validation.Errors;

namespace PhoneBook.Infrastructure.Repositories;

public class CategoryRepository : ICategoryRepository
{
    private readonly PhoneBookDbContext _context;

    public CategoryRepository(PhoneBookDbContext context)
    {
        _context = context;
    }

    public async Task<Result<List<ContactCategory>>> GetAllAsync()
    {
        try
        {
            var categories = await _context.ContactCategories.AsNoTracking().ToListAsync();

            return Result<List<ContactCategory>>.Success(categories);
        }
        catch (Exception ex)
        {
            return Result<List<ContactCategory>>.Failure(new Error("DatabaseError", ex.Message));
        }
    }
    public async Task<Result<ContactCategory>> GetByIdAsync(int id)
    {
        try
        {
            var category = await _context.ContactCategories.AsNoTracking().FirstOrDefaultAsync(cat => cat.Id == id);

            return Result<ContactCategory>.Success(category);
        }
        catch (Exception ex)
        {
            return Result<ContactCategory>.Failure(new Error("DatabaseError", ex.Message));
        }
    }

    public async Task<Result> AddAsync(ContactCategory category)
    {
        try
        {
            if (await CategoryExists(category))
                return Result.Failure(CategoryRepositoryErrors.CategoryExists);

            await _context.ContactCategories.AddAsync(category);
            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure(new Error("DatabaseError", ex.Message));
        }
    }

    public async Task<Result> DeleteAsync(ContactCategory category)
    {
        try
        {
            if (!(await CategoryExists(category)))
                return Result.Failure(CategoryRepositoryErrors.CategoryDoesNotExist);

            await _context.ContactCategories
                .Where(cat => cat.Id == category.Id)
                .ExecuteDeleteAsync();
            return Result.Success();
        }
        catch (Exception ex)
        {

            return Result.Failure(new Error("DatabaseError", ex.Message));
        }
    }
    public async Task<Result> UpdateAsync(ContactCategory category)
    {
        try
        {
            if (await CategoryExistsByNameCaseIndependent(category))
                return Result.Failure(CategoryRepositoryErrors.CategoryExists);

            var response = await _context.ContactCategories
                .Where(cat => cat.Id == category.Id)
                .ExecuteUpdateAsync(setters => setters
                    .SetProperty(c => c.Name, category.Name)
                );

            if (response <= 0)
                return Result.Failure(CategoryRepositoryErrors.UpdateDataFailed);

            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure(new Error("DatabaseError", ex.Message));
        }
    }

    public async Task<Result> SaveChangesAsync()
    {
        try
        {
            var response = await _context.SaveChangesAsync();

            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure(new Error("DatabaseError", ex.Message));
        }
    }

    private async Task<bool> CategoryExistsByNameCaseIndependent(ContactCategory category)
    {
        return await _context.ContactCategories.AnyAsync(cat => cat.Name == category.Name);
    }

    private async Task<bool> CategoryExists(ContactCategory category)
    {
        return await _context.ContactCategories.AnyAsync(cat => cat.Name.ToUpper() == category.Name.ToUpper());
    }
}
