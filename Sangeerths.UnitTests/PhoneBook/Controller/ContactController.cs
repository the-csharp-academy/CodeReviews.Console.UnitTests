using PhoneBook.Models;
using PhoneBook.Services;
using PhoneBook.UI;
namespace PhoneBook.Controller;

public class ContactController
{
    private readonly ContactService _contactService;
    private readonly ConsoleUI _consoleUI;

    public ContactController()
    {
        _contactService = new ContactService();
        _consoleUI = new ConsoleUI();
    }
    public async Task InsertContactAsync(Contact contact)
    {
        try
        {
            await _contactService.InsertContactAsync(contact);
            _consoleUI.ShowSuccess("[green]Contact inserted successfully.[/]");
            _consoleUI.Pause();
        }
        catch (Exception ex)
        {
            _consoleUI.ShowError(ex.Message);
            _consoleUI.Pause();
        }
    }

    public async Task DeleteContactAsync(int id)
    {
        try
        {
           await _contactService.DeleteContactAsync(id);
            _consoleUI.ShowSuccess("[green]Contact Deleted successfully.[/]");
            _consoleUI.Pause();
        }
        catch (Exception ex)
        {
            _consoleUI.ShowError(ex.Message);
            _consoleUI.Pause();
        }
    }

    public async Task ModifyContact(Contact contact)
    {
        try
        {
           await _contactService.ModifyContactAsync(contact);
            _consoleUI.ShowSuccess("[green]Contact Modified successfully.[/]");
            _consoleUI.Pause();
        }
        catch (Exception ex)
        {
            _consoleUI.ShowError(ex.Message);
            _consoleUI.Pause();
        }
    }

    public async Task<List<Contact>> ViewAllContacts()
    {
        try
        {
            return await _contactService.ViewAllContactAsync();
        }
        catch (Exception ex)
        {
            _consoleUI.ShowError(ex.Message);
            _consoleUI.Pause();
            return new List<Contact>();
        }
    }

    public async Task<List<Contact>> SearchByNameAsync(string firstName, string lastName)
    {
        try
        {
            return await _contactService.SearchByNameAsync(firstName, lastName);
        }
        catch (Exception ex)
        {
            _consoleUI.ShowError(ex.Message);
            _consoleUI.Pause();
            return new List<Contact>();
        }
    }
    public void ImportContacts(string filePath)
    {
        try
        {
            _contactService.ImportContacts(filePath);
            _consoleUI.ShowSuccess("[green]Contacts imported successfully.[/]");
            _consoleUI.Pause();
        }
        catch (FileNotFoundException)
        {
            _consoleUI.ShowError("The specified file was not found.");
            _consoleUI.Pause();
        }
        catch (UnauthorizedAccessException)
        {
            _consoleUI.ShowError("Access denied. Check the file permissions.");
            _consoleUI.Pause();
        }
        catch (NotSupportedException ex)
        {
            _consoleUI.ShowError(ex.Message);
            _consoleUI.Pause();
        }
        catch (IOException ex)
        {
            _consoleUI.ShowError($"Unable to read the file: {ex.Message}");
            _consoleUI.Pause();
        }
        catch (Exception ex)
        {
            _consoleUI.ShowError(ex.Message);
            _consoleUI.Pause();
        }
    }
}
