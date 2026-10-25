using PhoneBook.Domain.Entities;
using PhoneBook.Domain.Validation;

namespace PhoneBook.Application.Interfaces;

public interface ICategoryRepository
{
    Task<Result> AddAsync(ContactCategory category);
    Task<Result> DeleteAsync(ContactCategory category);
    Task<Result<List<ContactCategory>>> GetAllAsync();
    Task<Result<ContactCategory>> GetByIdAsync(int id);
    Task<Result> SaveChangesAsync();
    Task<Result> UpdateAsync(ContactCategory category);
}