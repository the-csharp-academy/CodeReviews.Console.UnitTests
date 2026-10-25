using Microsoft.EntityFrameworkCore;
using PhoneBook.Domain.Entities;

namespace PhoneBook.Infrastructure.Database;

internal class DatabaseInitializer : IDatabaseInitializer
{
    private readonly PhoneBookDbContext _db;

    public DatabaseInitializer(PhoneBookDbContext db)
    {
        _db = db;
    }

    public async Task EnsureUncategorizedCategoryExistsAsync()
    {
        var exists = await _db.ContactCategories
                    .AnyAsync(c => c.Name == "Uncategorized");

        if (!exists)
        {
            _db.ContactCategories.Add(new ContactCategory
            {
                Id = 1,
                Name = "Uncategorized"
            });

            await _db.SaveChangesAsync();
        }
    }
}
