using Microsoft.EntityFrameworkCore;
using silvermax.PhoneBook.DbAcess;
using silvermax.PhoneBook.Dtos;
using Spectre.Console;

namespace silvermax.PhoneBook.Services;

public class ContactService(ContactDbContext dbContext, UserInput userInput, CancellationToken ct)
{
    public async Task AddContactToDb()
    {
        var response = userInput.GetInfo();

        if (await dbContext.Contacts.AnyAsync(c => c.PhoneNumber == response.PhoneNumber, ct))
        {
            AnsiConsole.MarkupLine("[red]This phone number already exists[/]");
            return;
        }

        var contact = new Contact
        {
            Id = Guid.NewGuid(),
            Name = response.Name!.ToLower(),
            Email = response.Email!,
            PhoneNumber = response.PhoneNumber!,
            AddedAt = DateTime.UtcNow
        };

        dbContext.Add(contact);
        try
        {
            await dbContext.SaveChangesAsync(ct);
        }
        catch (DbUpdateException)
        {
            AnsiConsole.MarkupLine("[red]Failed to add phone number: Phone number already exists[/]");
            return;
        }

        UIHelper.ClearScreen();

        AnsiConsole.MarkupLine("[green]Contact added successfully[/]");
        UIHelper.ContinueMessage();
    }

    public async Task<List<ResponseDto>> ListContacts()
    {
        var contacts = await dbContext.Contacts
            .Select(c => new ResponseDto { Id = c.Id, Name = c.Name, Email = c.Email, PhoneNumber = c.PhoneNumber})
            .ToListAsync(ct);

        UIHelper.ClearScreen();

        return contacts;
    }
    
    public async Task DeleteContact()
    {
        await DisplayContacts();

        var contactToDelete = AnsiConsole.Ask<string>("[DarkOrange3_1]Please write the first name of the contact you want to delete:[/]");

        if (string.IsNullOrWhiteSpace(contactToDelete))
        {
            AnsiConsole.MarkupLine("[red]Invalid name[/]");
            UIHelper.ContinueMessage();
            return;
        }

        bool confirmation = AnsiConsole.Confirm("[red]Are you sure you want to delete contact {contactToDelete}?[/]");

        if (!confirmation)
        {
            AnsiConsole.MarkupLine("[DarkOrange3_1]Cancelled[/]");
            UIHelper.ContinueMessage();
            return;
        }

        var parts = contactToDelete.Trim().Split(" ", StringSplitOptions.RemoveEmptyEntries) ?? Array.Empty<string>();

        var firstName = parts[0];

        var contact = await dbContext.Contacts.FirstOrDefaultAsync(c => c.Name == firstName.ToLower() || c.Name.StartsWith(firstName.ToLower() + " ") , ct);

        if (contact is null)
        {
            AnsiConsole.MarkupLine("[red]Contact not found[/]");
            return;
        }

        dbContext.Contacts.Remove(contact);
        await dbContext.SaveChangesAsync(ct);

        UIHelper.ClearScreen();

        AnsiConsole.MarkupLine("[green]Contact deleted successfully[/]");
        UIHelper.ContinueMessage();
    }

    public async Task UpdateContact()
    {
        await DisplayContacts();

        var contactToEdit = AnsiConsole.Ask<string>("[DarkOrange3_1]Please write the name of the contact you want to update:[/]");

        if (string.IsNullOrWhiteSpace(contactToEdit))
        {
            AnsiConsole.MarkupLine("[red]Invalid name[/]");
            UIHelper.ContinueMessage();
            return;
        }

        var parts = contactToEdit.Trim().Split(" ", StringSplitOptions.RemoveEmptyEntries) ?? Array.Empty<string>();

        var firstName = parts[0];

        var contact = await dbContext.Contacts.FirstOrDefaultAsync(c => c.Name == firstName.ToLower() || c.Name.StartsWith(firstName.ToLower() + " "), ct);

        if (contact is null)
        {
            AnsiConsole.MarkupLine("[red]Contact not found[/]");
            return;
        }

        var response = userInput.GetInfo();

        contact.Name = response.Name!;
        contact.Email = response.Email!;
        contact.PhoneNumber = response.PhoneNumber!;

        await dbContext.SaveChangesAsync(ct);

        UIHelper.ClearScreen();

        AnsiConsole.MarkupLine("[green]Contact updated successfully[/]");
        UIHelper.ContinueMessage();
    }

    public async Task DisplayContacts()
    {
        var contacts = await ListContacts();

        var table = new Table { Border = TableBorder.Rounded };
        table.AddColumn("[yellow]Name[/]");
        table.AddColumn("[yellow]Email[/]");
        table.AddColumn("[yellow]Phone Number[/]");

        foreach (var c in contacts)
        {
            table.AddRow(
                $"[purple]{c.Name}[/]",
                $"[purple]{c.Email}[/]",
                $"[purple]{c.PhoneNumber}[/]"
                );
        }

        AnsiConsole.Write(table);
    }
}
