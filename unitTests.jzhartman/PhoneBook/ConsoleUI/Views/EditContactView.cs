using PhoneBook.ConsoleUI.Models;
using Spectre.Console;

namespace PhoneBook.ConsoleUI.Views;

internal class EditContactView
{
    public void Render(EditContactViewModel contact)
    {
        var firstNameTextColor = (contact.ChangedFirstName) ? "[green]" : "[white]";
        var lastNameTextColor = (contact.ChangedLastName) ? "[green]" : "[white]";
        var phoneNumberTextColor = (contact.ChangedPhoneNumber) ? "[green]" : "[white]";
        var emailTextColor = (contact.ChangedEmail) ? "[green]" : "[white]";
        var categoryTextColor = (contact.ChangedCategory) ? "[green]" : "[white]";


        AnsiConsole.MarkupInterpolated($"[bold blue]Contact Entry:[/]");

        AnsiConsole.WriteLine();
        AnsiConsole.WriteLine();

        var table = new Table()
                .HideHeaders()
                .NoBorder();

        table.AddColumn("Key");
        table.AddColumn("Value");

        table.AddRow("[blue]Name:[/]", $"{firstNameTextColor}{contact.FirstName}[/] {lastNameTextColor}{contact.LastName}[/]");
        table.AddRow("[blue]Phone Number:[/]", $"{phoneNumberTextColor}{contact.PhoneNumber}[/]");
        table.AddRow("[blue]Email:[/]", $"{emailTextColor}{contact.Email}[/]");
        table.AddRow("[blue]Category:[/]", $"{categoryTextColor}{contact.CategoryName}[/]");

        AnsiConsole.Write(table);
    }
}
