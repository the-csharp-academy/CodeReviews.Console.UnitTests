using Phonebook.Models;
using Spectre.Console;

namespace Phonebook.UI;

public static class UserInterface
{
    public const string ExpectedEmailFormat = "name@example.com";
    public const string ExpectedPhoneFormats = "+27 82 123 4567 | 082 123 4567 | (011) 555-0198";

    public static void ShowContacts(List<Contact> contacts)
    {
        var table = new Table();
        table.AddColumn("Id");
        table.AddColumn("Name");
        table.AddColumn("Category");
        table.AddColumn("Email");
        table.AddColumn("Phone");

        foreach (var contact in contacts)
        {
            table.AddRow(
                contact.Id.ToString(),
                Markup.Escape(contact.Name),
                Markup.Escape(contact.Category.ToString()),
                Markup.Escape(contact.Email),
                Markup.Escape(contact.PhoneNumber));
        }

        AnsiConsole.Write(table);
    }

    public static string AskForName() =>
        AnsiConsole.Ask<string>("Contact name:");

    public static string AskForEmail()
    {
        AnsiConsole.MarkupLine($"Expected email format: [yellow]{ExpectedEmailFormat}[/]");
        return AnsiConsole.Ask<string>("Email:");
    }

    public static string AskForPhone()
    {
        AnsiConsole.MarkupLine($"Accepted phone formats: [yellow]{ExpectedPhoneFormats}[/]");
        return AnsiConsole.Ask<string>("Phone number:");
    }

    public static ContactCategory AskForCategory() =>
        AnsiConsole.Prompt(
            new SelectionPrompt<ContactCategory>()
                .Title("Category:")
                .AddChoices(Enum.GetValues<ContactCategory>()));

    public static int AskForContactId(string action) =>
        AnsiConsole.Ask<int>($"Enter the Id of the contact to {action}:");

    public static (string Subject, string Body) AskForEmailMessage()
    {
        var subject = AnsiConsole.Ask<string>("Subject:");
        var body = AnsiConsole.Ask<string>("Message body:");
        return (subject, body);
    }

    public static string AskForSmsText() =>
        AnsiConsole.Ask<string>("SMS text:");

    public static void ShowSuccess(string message) =>
        AnsiConsole.MarkupLine($"[green]{Markup.Escape(message)}[/]");

    public static void ShowError(string error) =>
        AnsiConsole.MarkupLine($"[red]{Markup.Escape(error)}[/]");

    public static void Pause()
    {
        AnsiConsole.MarkupLine("[grey]Press any key to continue...[/]");
        Console.ReadKey(intercept: true);
    }

    public static void Clear() => AnsiConsole.Clear();
}
