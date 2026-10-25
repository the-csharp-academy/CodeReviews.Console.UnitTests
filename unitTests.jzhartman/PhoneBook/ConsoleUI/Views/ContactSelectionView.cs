using PhoneBook.Application.Contacts.DTOs;
using Spectre.Console;

namespace PhoneBook.ConsoleUI.Views;

internal class ContactSelectionView
{
    public ContactResponse Render(IEnumerable<ContactResponse> contacts)
    {
        contacts = contacts.OrderBy(c => c.LastName);

        var selection = AnsiConsole.Prompt(
                            new SelectionPrompt<ContactResponse>()
                            .Title("Select a contact from below: ")
                            .PageSize(15)
                            .WrapAround()
                            .UseConverter(c => $"{c.LastName}, {c.FirstName}")
                            .AddChoices(contacts)
                            .EnableSearch()
                            .SearchPlaceholderText("Type to search contacts by name"));

        return selection;
    }
}
