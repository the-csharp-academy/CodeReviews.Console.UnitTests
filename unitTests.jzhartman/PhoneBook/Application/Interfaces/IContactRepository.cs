using PhoneBook.Domain.Entities;
using PhoneBook.Domain.Validation;

namespace PhoneBook.Application.Interfaces;

public interface IContactRepository
{
    Task<Result<Contact>> GetByIdAsync(int id);
    Task<Result<List<Contact>>> GetAllAsync();
    Task<Result<Contact>> AddAsync(Contact contact);
    Task<Result> UpdateAsync(Contact contact);
    Task<Result> DeleteAsync(Contact contact);
    Task<Result> SaveChangesAsync();
    Task<Result<List<Contact>>> GetByCategoryIdAsync(int id);
    Task<Result> SetCategoryIdForContactsToDefaultByCategoryIdAsync(ContactCategory category);
}
