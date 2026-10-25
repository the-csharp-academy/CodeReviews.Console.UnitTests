using Microsoft.EntityFrameworkCore;
using PhoneBook.Application.Interfaces;
using PhoneBook.Domain.Entities;
using PhoneBook.Infrastructure;
using PhoneBook.Infrastructure.Repositories;

namespace PhoneBook.IntegrationTests.Fixtures;

public class DatabaseFixture : IDisposable
{
    public string DbFile { get; }
    public PhoneBookDbContext Context { get; }
    public ICategoryRepository CategoryRepository { get; }
    public IContactRepository ContactRepository { get; }

    public DatabaseFixture()
    {
        DbFile = Path.GetTempFileName();

        var options = new DbContextOptionsBuilder<PhoneBookDbContext>()
            .UseSqlite($"Data Source={DbFile}")
            .Options;

        Context = new PhoneBookDbContext(options, disableSeedData: true);
        Context.Database.EnsureCreated();

        CategoryRepository = new CategoryRepository(Context);
        ContactRepository = new ContactRepository(Context);
    }

    public void Dispose()
    {
        Context.Dispose();
        File.Delete(DbFile);
    }

    public void SeedCategoryTable(ContactCategory category)
    {
        var input = new ContactCategory { Id = category.Id, Name = category.Name };
        Context.ContactCategories.Add(input);
        Context.SaveChanges();
    }

    public void SeedCategoryTable(List<ContactCategory> categories)
    {
        Context.ContactCategories.AddRange(categories);
        Context.SaveChanges();
    }

    public void SeedContactTable(string firstName, string lastName, string phoneNumber, string email, int categoryId)
    {
        var contact = new Contact
        {
            FirstName = firstName,
            LastName = lastName,
            PhoneNumber = phoneNumber,
            Email = email,
            CategoryId = categoryId
        };

        SeedContactTable(contact);
    }

    public void SeedContactTable(Contact contact)
    {
        Context.Contacts.Add(contact);
        Context.SaveChanges();
    }

    public void SeedContactTable(List<Contact> contacts)
    {
        Context.Contacts.AddRange(contacts);
        Context.SaveChanges();
    }

    public void ResetDatabase()
    {
        Context.ContactCategories.RemoveRange(Context.ContactCategories);
        Context.Contacts.RemoveRange(Context.Contacts);
        Context.SaveChanges();
    }

}
