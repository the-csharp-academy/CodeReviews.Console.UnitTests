using PhoneBook.Controller;
using PhoneBook.Messaging;
using PhoneBook.Models;
using PhoneBook.Validation;
using Spectre.Console;
namespace PhoneBook.UI;

public class ConsoleMenu
{
    private readonly ContactController _contactController;
    private readonly ReportController _reportController;
    private readonly ConsoleUI _consoleUI;
    private enum MenuOptions
    {
        InsertContact,
        DeleteContact,
        ModifyContact,
        ViewContact,
        ViewAllContacts,
        SendMessage,
        Report,
        ImportContacts,
        Exit
    }

    public ConsoleMenu()
    {
        _contactController = new ContactController();
        _consoleUI = new ConsoleUI();
        _reportController = new ReportController();
    }

    public async Task OnStart()
    {
        bool isRunning = true;
        while  (isRunning)
        {
            _consoleUI.ShowHeader();
            var choice = AnsiConsole.Prompt(new SelectionPrompt<MenuOptions>().Title("Choose your operation").AddChoices(
                MenuOptions.InsertContact,
                MenuOptions.DeleteContact,
                MenuOptions.ModifyContact,
                MenuOptions.ViewContact,
                MenuOptions.ViewAllContacts,
                MenuOptions.SendMessage,
                MenuOptions.Report,
                MenuOptions.ImportContacts,
                MenuOptions.Exit));

            switch (choice)
            {
                case MenuOptions.InsertContact:
                    await NewContactPrompt();  
                    break;
                case MenuOptions.DeleteContact:
                    await DeleteContactFlowAsync();
                    break;
                case MenuOptions.ModifyContact:
                    await ModifyContactFlowAsync();
                    break;
                case MenuOptions.ViewContact:
                    await ViewContactFlowAsync();
                    break;
                case MenuOptions.ViewAllContacts:
                    await ViewAllContactsFlowAsync();
                    break;
                case MenuOptions.SendMessage:
                    await SendMessageFlowAsync();
                    break;
                case MenuOptions.Report:
                    await _reportController.ExportReportFlowAsync();
                    break;
                case MenuOptions.ImportContacts:
                    await ImportContactsFlowAsync();
                    break;
                case MenuOptions.Exit:
                    isRunning = false;
                    _consoleUI.ShowGoodbye();
                    break;
            }
        }
    }

    private async Task NewContactPrompt()
    {
        var contact = new Contact
        {
            FirstName = PromptValidated("Enter [green]first name[/]:", InputValidator.IsValidName),
            LastName = PromptValidated("Enter [green]last name[/]:", InputValidator.IsValidName),
            OrganizationName = AnsiConsole.Ask("Enter [green]organization[/] (optional):", string.Empty),
            JobTitle = AnsiConsole.Ask("Enter [green]job title[/] (optional):", string.Empty),
            Notes = AnsiConsole.Ask("Enter [green]notes[/] (optional):", string.Empty)
        };

        contact.PhoneNumbers.Add(new PhoneNumberDetail
        {
            Number = PromptValidated("Enter [green]phone number[/]:", InputValidator.IsValidPhoneNumber),
            Label = AnsiConsole.Prompt(new SelectionPrompt<LabelType>().Title("Select [green]label type[/]:").AddChoices(LabelType.Mobile, LabelType.Home, LabelType.Work, LabelType.Other))
        });
        while (AnsiConsole.Confirm("Add another phone number?", false))
        {
            contact.PhoneNumbers.Add(new PhoneNumberDetail
            {
                Number = PromptValidated("Enter [green]phone number[/]:", InputValidator.IsValidPhoneNumber),
                Label = AnsiConsole.Prompt(new SelectionPrompt<LabelType>().Title("Select [green]label type[/]:").AddChoices(LabelType.Mobile, LabelType.Home, LabelType.Work, LabelType.Other))
            });
        }

        while (AnsiConsole.Confirm("Add an email address?", false))
        {
            contact.Emails.Add(new EmailDetail
            {
                EmailAddress = PromptValidated("Enter [green]email[/]:", InputValidator.IsValidEmail),
                Label = AnsiConsole.Prompt(new SelectionPrompt<LabelType>().Title("Select [green]label type[/]:").AddChoices(LabelType.Mobile, LabelType.Home, LabelType.Work, LabelType.Other))
            });
        }
        AnsiConsole.MarkupLine("[grey]Please review the new contact:[/]");
        DisplayContacts(new List<Contact> { contact });

        var confirmInsert = AnsiConsole.Confirm("Save this contact?", true);
        if (!confirmInsert)
        {
            AnsiConsole.MarkupLine("[grey]Contact insertion cancelled.[/]");
            _consoleUI.Pause();
            return;
        }

        await _contactController.InsertContactAsync(contact);
    }

    private async Task DeleteContactFlowAsync()
    {
        var firstName = AnsiConsole.Ask<string>("Enter [green]first name[/]:");
        var lastName = AnsiConsole.Ask<string>("Enter [green]last name[/]:");

        var matches = await _contactController.SearchByNameAsync(firstName, lastName);

        if (matches.Count == 0)
        {
            AnsiConsole.MarkupLine("[yellow]No matching contact found.[/]");
            _consoleUI.Pause();
            return;
        }

        Contact contactToDelete;

        if (matches.Count == 1)
        {
            contactToDelete = matches[0];
        }
        else
        {
            AnsiConsole.MarkupLine($"[yellow]{matches.Count} contacts found with that name:[/]");
            contactToDelete = AnsiConsole.Prompt(
                new SelectionPrompt<Contact>()
                    .Title("Select the contact to delete:")
                    .UseConverter(c => $"Id: {c.Id} | {c.OrganizationName ?? "-"}")
                    .AddChoices(matches));
        }

        var confirm = AnsiConsole.Confirm(
            $"Are you sure you want to delete [red]{contactToDelete.FirstName} {contactToDelete.LastName}[/]?", false);

        if (!confirm)
        {
            AnsiConsole.MarkupLine("[grey]Delete cancelled.[/]");
            _consoleUI.Pause();
            return;
        }

        await _contactController.DeleteContactAsync(contactToDelete.Id);
    }

    private async Task ModifyContactFlowAsync()
    {
        var firstName = AnsiConsole.Ask<string>("Enter [green]first name[/]:");
        var lastName = AnsiConsole.Ask<string>("Enter [green]last name[/]:");

        var matches = await _contactController.SearchByNameAsync(firstName, lastName);

        if (matches.Count == 0)
        {
            AnsiConsole.MarkupLine("[yellow]No matching contact found.[/]");
            _consoleUI.Pause();
            return;
        }

        Contact contact;

        if (matches.Count == 1)
        {
            contact = matches[0];
        }
        else
        {
            AnsiConsole.MarkupLine($"[yellow]{matches.Count} contacts found with that name:[/]");
            contact = AnsiConsole.Prompt(
                new SelectionPrompt<Contact>()
                    .Title("Select the contact to modify:")
                    .UseConverter(c => $"Id: {c.Id} | {c.OrganizationName ?? "-"}")
                    .AddChoices(matches));
        }

        AnsiConsole.MarkupLine("[grey]Current details:[/]");
        DisplayContacts(new List<Contact> { contact }); 

        var fieldsToUpdate = AnsiConsole.Prompt(
            new MultiSelectionPrompt<string>()
                .Title("What would you like to [green]update[/]?")
                .AddChoices("First Name", "Last Name", "Organization", "Job Title", "Notes", "Phone Numbers", "Emails"));

        if (fieldsToUpdate.Count == 0)
        {
            AnsiConsole.MarkupLine("[yellow]No changes selected.[/]");
            _consoleUI.Pause();
            return;
        }

        foreach (var field in fieldsToUpdate)
        {
            switch (field)
            {
                case "First Name":
                    contact.FirstName = PromptValidated(
                        $"Enter new [green]first name[/] (current: {contact.FirstName}):",
                        InputValidator.IsValidName);
                    break;

                case "Last Name":
                    contact.LastName = PromptValidated(
                        $"Enter new [green]last name[/] (current: {contact.LastName}):",
                        InputValidator.IsValidName);
                    break;

                case "Organization":
                    contact.OrganizationName = AnsiConsole.Ask(
                        $"Enter new [green]organization[/] (current: {contact.OrganizationName ?? "-"}):",
                        contact.OrganizationName ?? string.Empty);
                    break;

                case "Job Title":
                    contact.JobTitle = AnsiConsole.Ask(
                        $"Enter new [green]job title[/] (current: {contact.JobTitle ?? "-"}):",
                        contact.JobTitle ?? string.Empty);
                    break;

                case "Notes":
                    contact.Notes = AnsiConsole.Ask(
                        $"Enter new [green]notes[/] (current: {contact.Notes ?? "-"}):",
                        contact.Notes ?? string.Empty);
                    break;

                case "Phone Numbers":
                    contact.PhoneNumbers.Add(new PhoneNumberDetail
                    {
                        Number = PromptValidated("Enter [green]new phone number[/]:", InputValidator.IsValidPhoneNumber),
                        Label = PromptForLabelType()
                    });
                    break;

                case "Emails":
                    contact.Emails.Add(new EmailDetail
                    {
                        EmailAddress = PromptValidated("Enter [green]new email[/]:", InputValidator.IsValidEmail),
                        Label = PromptForLabelType()
                    });
                    break;
            }
        }
        AnsiConsole.MarkupLine("[grey]Updated details:[/]");
        DisplayContacts(new List<Contact> { contact });

        var confirmUpdate = AnsiConsole.Confirm("Save these changes?", true);
        if (!confirmUpdate)
        {
            AnsiConsole.MarkupLine("[grey]Update cancelled.[/]");
            _consoleUI.Pause();
            return;
        }
        await _contactController.ModifyContact(contact);
    }

    private async Task ViewContactFlowAsync()
    {
        var firstName = AnsiConsole.Ask<string>("Enter [green]first name[/]:");
        var lastName = AnsiConsole.Ask<string>("Enter [green]last name[/]:");

        var matches = await _contactController.SearchByNameAsync(firstName, lastName);

        if (matches.Count == 0)
        {
            AnsiConsole.MarkupLine("[yellow]No matching contact found.[/]");
            _consoleUI.Pause();
            return;
        }

        Contact contact;

        if (matches.Count == 1)
        {
            contact = matches[0];
        }
        else
        {
            AnsiConsole.MarkupLine($"[yellow]{matches.Count} contacts found with that name:[/]");
            contact = AnsiConsole.Prompt(
                new SelectionPrompt<Contact>()
                    .Title("Select the contact to view:")
                    .UseConverter(c => $"Id: {c.Id} | {c.OrganizationName ?? "-"}")
                    .AddChoices(matches));
        }

        DisplayContacts(new List<Contact> { contact });
        _consoleUI.Pause();
    }

    private async Task ViewAllContactsFlowAsync()
    {
        var contacts = await _contactController.ViewAllContacts();
        DisplayContacts(contacts);
        _consoleUI.Pause();
    }

    private void DisplayContacts(List<Contact> contacts)
    {
        if (contacts.Count == 0)
        {
            AnsiConsole.MarkupLine("[yellow]No contacts found.[/]");
            _consoleUI.Pause();
            return;
        }

        var table = new Table()
            .Border(TableBorder.Rounded)
            .BorderColor(Color.Aqua)
            .Title("[bold cyan]Contact Details[/]")
            .ShowRowSeparators()

            .Expand();

        table.AddColumn(new TableColumn("[bold]Id[/]").Centered());
        table.AddColumn(new TableColumn("[bold]Name[/]"));
        table.AddColumn(new TableColumn("[bold]Organization[/]"));
        table.AddColumn(new TableColumn("[bold]Job Title[/]"));
        table.AddColumn(new TableColumn("[bold]Phone(s)[/]"));
        table.AddColumn(new TableColumn("[bold]Email(s)[/]"));
        table.AddColumn(new TableColumn("[bold]Notes[/]"));

        foreach (var contact in contacts)
        {
            var phones = contact.PhoneNumbers.Count > 0
                ? string.Join("\n", contact.PhoneNumbers.ConvertAll(p => $"[grey]{p.Label}:[/] {p.Number}"))
                : "[grey]-[/]";

            var emails = contact.Emails.Count > 0
                ? string.Join("\n", contact.Emails.ConvertAll(e => $"[grey]{e.Label}:[/] {e.EmailAddress}"))
                : "[grey]-[/]";

            table.AddRow(
                $"[aqua]{contact.Id}[/]",
                $"[bold]{contact.FirstName} {contact.LastName}[/]",
                contact.OrganizationName ?? "[grey]-[/]",
                contact.JobTitle ?? "[grey]-[/]",
                phones,
                emails,
                contact.Notes ?? "[grey]-[/]"
            );
        }

        AnsiConsole.Write(table);
        AnsiConsole.MarkupLine($"[grey]{contacts.Count} contact(s) found.[/]");
    }

    private async Task ImportContactsFlowAsync()
    {
        AnsiConsole.MarkupLine("[bold cyan]Import Contacts[/]");

        AnsiConsole.MarkupLine("[grey]Supported formats: .csv, .xls, .xlsx[/]");

        string filePath = AnsiConsole.Ask<string>("Enter the [green]file path[/]:");
        filePath = filePath.Trim().Trim('"');

        if (string.IsNullOrWhiteSpace(filePath))
        {
            AnsiConsole.MarkupLine("[red]File path cannot be empty.[/]");
            _consoleUI.Pause();
            return;
        }

        if (!File.Exists(filePath))
        {
            AnsiConsole.MarkupLine("[red]File not found.[/]");
            AnsiConsole.MarkupLine( $"[grey]Path:[/] {filePath}");
            _consoleUI.Pause();
            return;
        }

        string extension = Path.GetExtension(filePath).ToLowerInvariant();

        if (extension != ".csv" &&extension != ".xls" && extension != ".xlsx")
        {
            AnsiConsole.MarkupLine("[red]Unsupported file format.[/]");
            AnsiConsole.MarkupLine("[grey]Supported formats: .csv, .xls, .xlsx[/]");
            _consoleUI.Pause();
            return;
        }

        AnsiConsole.MarkupLine($"[green]File found:[/] {Path.GetFileName(filePath)}");
        bool confirm = AnsiConsole.Confirm("Do you want to import this file?",true);

        if (!confirm)
        {
            AnsiConsole.MarkupLine( "[grey]Import cancelled.[/]");
            _consoleUI.Pause();
            return;
        }

        _contactController.ImportContacts(filePath);
    }

    private LabelType PromptForLabelType()
    {
        return AnsiConsole.Prompt(
            new SelectionPrompt<LabelType>()
                .Title("Select [green]label type[/]:")
                .AddChoices(Enum.GetValues<LabelType>()));
    }

    private async Task SendMessageFlowAsync()
    {
        var firstName = AnsiConsole.Ask<string>("Enter [green]first name[/]:");
        var lastName = AnsiConsole.Ask<string>("Enter [green]last name[/]:");

        var matches = await _contactController.SearchByNameAsync(firstName, lastName);
        if (matches.Count == 0)
        {
            AnsiConsole.MarkupLine("[yellow]No matching contact found.[/]");
            _consoleUI.Pause();
            return;
        }

        var contact = matches.Count == 1 ? matches[0] : AnsiConsole.Prompt(
            new SelectionPrompt<Contact>().Title("Select the contact:")
                .UseConverter(c => $"{c.FirstName} {c.LastName}").AddChoices(matches));

        var channel = AnsiConsole.Prompt(
            new SelectionPrompt<MessageChannel>().Title("Send via:")
                .AddChoices(MessageChannel.Email, MessageChannel.Sms));

        string recipient = channel == MessageChannel.Email
            ? contact.Emails.FirstOrDefault()?.EmailAddress ?? ""
            : contact.PhoneNumbers.FirstOrDefault()?.Number ?? "";

        if (string.IsNullOrEmpty(recipient))
        {
            AnsiConsole.MarkupLine($"[yellow]No {channel} found for this contact.[/]");
            _consoleUI.Pause();
            return;
        }

        var subject = channel == MessageChannel.Email ? AnsiConsole.Ask<string>("Enter [green]subject[/]:") : "";
        var body = AnsiConsole.Ask<string>("Enter [green]message[/]:");

        try
        {
            await MessageSender.SendAsync(channel, recipient, subject, body);
            _consoleUI.ShowSuccess($"[green]{channel} sent successfully.[/]");
        }
        catch (Exception ex)
        {
            _consoleUI.ShowError(ex.Message);
        }
        _consoleUI.Pause();
    }

    private string PromptValidated(string promptText, Func<string, bool> isValid)
    {
        string input;
        do
        {
            input = AnsiConsole.Ask<string>(promptText);
            if (!isValid(input))
                AnsiConsole.MarkupLine("[red]Invalid input, please try again.[/]");
        } while (!isValid(input));

        return input;
    }
}
