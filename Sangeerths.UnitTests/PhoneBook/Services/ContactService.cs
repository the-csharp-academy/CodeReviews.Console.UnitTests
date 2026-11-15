using Microsoft.EntityFrameworkCore;
using PhoneBook.Models;
using PhoneBook.Repository;
namespace PhoneBook.Services;

public class ContactService
{
    private readonly PhoneBookContext _context;

    public ContactService()
    {
        _context = new PhoneBookContext();
    }

    public ContactService(PhoneBookContext context)
    {
        _context = context;
    }
    internal async Task DeleteContactAsync(int id)
    {
        var contact = await _context.Contacts.FindAsync(id);
        _context.Contacts.Remove(contact!);
        await _context.SaveChangesAsync();
    }

    internal async Task InsertContactAsync(Contact contact)
    {
        _context.Contacts.Add(contact);
        await _context.SaveChangesAsync();
    }

    public async Task ModifyContactAsync(Contact contact)
    {
        contact.UpdatedAt = DateTime.UtcNow;

        bool exists = await _context.Contacts.AnyAsync(c => c.Id == contact.Id);
        if (!exists)
            throw new InvalidOperationException($"No contact found with Id {contact.Id}.");

        _context.Contacts.Update(contact);
        await _context.SaveChangesAsync();
    }

    internal async Task<List<Contact>> SearchByNameAsync(string firstName, string lastName)
    {
        return await _context.Contacts
            .Include(c => c.PhoneNumbers)
            .Include(c => c.Emails)
            // Use direct equality and rely on the database collation for case-insensitive
            // comparisons. This avoids repeated ToLower allocations and lets SQL perform
            // the comparison efficiently.
            .Where(c => c.FirstName == firstName && c.LastName == lastName)
            .ToListAsync();
    }

    internal async Task<List<Contact>> ViewAllContactAsync()
    {
        return await _context.Contacts
    .Include(c => c.PhoneNumbers)
    .Include(c => c.Emails)
    .ToListAsync();
    }

    internal void ImportContacts(string filePath)
    {
        _context.ImportDataFromFile(filePath);
    }
}
