using Microsoft.EntityFrameworkCore;
using PhoneBook.Application.Interfaces;
using PhoneBook.Domain.Entities;
using PhoneBook.Domain.Validation;
using PhoneBook.Domain.Validation.Errors;

namespace PhoneBook.Infrastructure.Repositories;

public class ContactRepository : IContactRepository
{
    private readonly PhoneBookDbContext _context;

    public ContactRepository(PhoneBookDbContext context)
    {
        _context = context;
    }

    public async Task<Result<Contact>> AddAsync(Contact contact)
    {
        try
        {
            if (await ContactExists(contact))
                return Result<Contact>.Failure(ContactRepositoryErrors.ContactExists);

            await _context.Contacts.AddAsync(contact);

            return Result<Contact>.Success(contact);
        }
        catch (Exception ex)
        {
            return Result<Contact>.Failure(new Error("DatabaseError", ex.Message));
        }
    }

    public async Task<Result> DeleteAsync(Contact contact)
    {
        try
        {
            if (!(await ContactExists(contact)))
                return Result.Failure(ContactRepositoryErrors.ContactDoesNotExist);

            await _context.Contacts
                .Where(c => c.Id == contact.Id)
                .ExecuteDeleteAsync();

            return Result.Success();
        }
        catch (Exception ex)
        {

            return Result.Failure(new Error("DatabaseError", ex.Message));
        }
    }

    public async Task<Result<List<Contact>>> GetAllAsync()
    {
        try
        {
            var contacts = await _context.Contacts
                .AsNoTracking()
                .ToListAsync();

            return Result<List<Contact>>.Success(contacts);
        }
        catch (Exception ex)
        {
            return Result<List<Contact>>.Failure(new Error("DatabaseError", ex.Message));
        }
    }

    public async Task<Result<Contact>> GetByIdAsync(int id)
    {
        try
        {
            var contact = await _context.Contacts
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == id);

            return Result<Contact>.Success(contact);
        }
        catch (Exception ex)
        {
            return Result<Contact>.Failure(new Error("DatabaseError", ex.Message));
        }
    }

    public async Task<Result<List<Contact>>> GetByCategoryIdAsync(int id)
    {
        try
        {
            var contacts = await _context.Contacts
                .AsNoTracking()
                .Where(c => c.CategoryId == id)
                .ToListAsync();

            return Result<List<Contact>>.Success(contacts);
        }
        catch (Exception ex)
        {
            return Result<List<Contact>>.Failure(new Error("DatabaseError", ex.Message));
        }
    }

    public async Task<Result> UpdateAsync(Contact contact)
    {
        try
        {
            var response = await _context.Contacts
                .Where(c => c.Id == contact.Id)
                .ExecuteUpdateAsync(setters => setters
                    .SetProperty(c => c.FirstName, contact.FirstName)
                    .SetProperty(c => c.LastName, contact.LastName)
                    .SetProperty(c => c.PhoneNumber, contact.PhoneNumber)
                    .SetProperty(c => c.Email, contact.Email)
                    .SetProperty(c => c.CategoryId, contact.CategoryId)
                );

            if (response <= 0)
                return Result.Failure(ContactRepositoryErrors.UpdateDataFailed);

            return Result.Success();

        }
        catch (Exception ex)
        {
            return Result.Failure(new Error("DatabaseError", ex.Message));
        }
    }
    public async Task<Result> SetCategoryIdForContactsToDefaultByCategoryIdAsync(ContactCategory category)
    {
        try
        {
            if (!await _context.ContactCategories.AnyAsync(cat => cat.Name.ToUpper() == category.Name.ToUpper()))
                return Result.Failure(CategoryRepositoryErrors.CategoryDoesNotExist);

            // Note: If there are no contacts in this category, then no contacts need updated, therefore success
            if (!await _context.Contacts.AnyAsync(c => c.CategoryId == category.Id))
                return Result.Success();

            var response = await _context.Contacts
                                .Where(c => c.CategoryId == category.Id)
                                .ExecuteUpdateAsync(setters => setters
                                    .SetProperty(c => c.CategoryId, 1));

            if (response <= 0)
                return Result.Failure(ContactRepositoryErrors.UpdateDataFailed);

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

    private async Task<bool> ContactExists(Contact contact)
    {
        return await _context.Contacts.AnyAsync(
            c => c.FirstName == contact.FirstName &&
            c.LastName == contact.LastName &&
            c.PhoneNumber == contact.PhoneNumber &&
            c.Email == contact.Email &&
            c.CategoryId == contact.CategoryId);
    }
}
