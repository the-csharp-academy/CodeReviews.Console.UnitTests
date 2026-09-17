using Microsoft.EntityFrameworkCore;
using Phonebook.Data;
using Phonebook.Models;

namespace Phonebook.Services;

/// <summary>
/// All database access for contacts. Every method catches EF/database failures
/// and returns a result tuple so the UI can show a friendly message instead of crashing.
/// </summary>
public class ContactService(AppDbContext context)
{
    public async Task<(bool Success, List<Contact>? Data, string? Error)> GetAllAsync()
    {
        try
        {
            var contacts = await context.Contacts.AsNoTracking().OrderBy(c => c.Name).ToListAsync();
            return (true, contacts, null);
        }
        catch (Exception ex)
        {
            return (false, null, $"Could not load contacts: {ex.Message}");
        }
    }

    public async Task<(bool Success, Contact? Data, string? Error)> GetByIdAsync(int id)
    {
        try
        {
            var contact = await context.Contacts.AsNoTracking().FirstOrDefaultAsync(c => c.Id == id);
            return contact is null
                ? (false, null, $"No contact with Id {id} was found.")
                : (true, contact, null);
        }
        catch (Exception ex)
        {
            return (false, null, $"Could not load contact {id}: {ex.Message}");
        }
    }

    public async Task<(bool Success, string? Error)> AddAsync(Contact contact)
    {
        try
        {
            context.Contacts.Add(contact);
            await context.SaveChangesAsync();
            return (true, null);
        }
        catch (Exception ex)
        {
            return (false, $"Could not add contact: {ex.Message}");
        }
    }

    public async Task<(bool Success, string? Error)> UpdateAsync(Contact contact)
    {
        try
        {
            // EF only allows ONE tracked instance per key. If a previous operation
            // (e.g. AddAsync) left the same entity tracked, detach it first so
            // Update() can attach the edited copy.
            var tracked = context.ChangeTracker.Entries<Contact>()
                .Select(e => e.Entity)
                .FirstOrDefault(c => c.Id == contact.Id);
            if (tracked is not null)
                context.Entry(tracked).State = EntityState.Detached;

            context.Contacts.Update(contact);
            await context.SaveChangesAsync();
            return (true, null);
        }
        catch (Exception ex)
        {
            return (false, $"Could not update contact {contact.Id}: {ex.Message}");
        }
    }

    public async Task<(bool Success, string? Error)> DeleteAsync(int id)
    {
        try
        {
            var contact = await context.Contacts.FindAsync(id);
            if (contact is null)
                return (false, $"No contact with Id {id} was found.");

            context.Contacts.Remove(contact);
            await context.SaveChangesAsync();
            return (true, null);
        }
        catch (Exception ex)
        {
            return (false, $"Could not delete contact {id}: {ex.Message}");
        }
    }
}
