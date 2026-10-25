using PhoneBook.ConsoleUI.Models;
using Spectre.Console;

namespace PhoneBook.ConsoleUI.Input;

internal class UserInput
{
    internal string GetNameFromUser(string message)
    {
        var namePrompt = new TextPrompt<string>(message)
            .AllowEmpty()
            .Validate(input =>
            {
                if (string.IsNullOrWhiteSpace(input))
                    return ValidationResult.Error("[red]Required Field:[/] Name cannot be empty.");

                return ValidationResult.Success();
            });

        return AnsiConsole.Prompt(namePrompt);
    }

    internal string GetEmailAddressFromUser()
    {
        var emailPrompt = new TextPrompt<string>("Enter your [green]EMAIL[/] (format must match: name@example.com):")
            .AllowEmpty()
            .Validate(input =>
            {
                if (input.Contains("@") && input.IndexOf("@") >= 1 && input.Count('@') == 1 &&
                    input.Contains(".") && input.IndexOf(".") > input.IndexOf("@" + 1) &&
                    input.Length >= 5)
                    return ValidationResult.Success();

                if (string.IsNullOrWhiteSpace(input))
                    return ValidationResult.Error("[red]Required Field:[/] Email cannot be empty.");

                return ValidationResult.Error("[red]Invalid Entry:[/] Please enter a valid email address with the format [yellow]name@domain.com[/].");
            });

        return AnsiConsole.Prompt(emailPrompt);
    }

    internal string GetPhoneNumberFromUser()
    {
        var invalidChars = new[] { '-', '(', ')', '+' };

        var phoneNumberPrompt = new TextPrompt<string>("Enter your [green]PHONE NUMBER[/] (7 to 15 digits, '-', '(', ')', and '+' will be ignored):")
            .AllowEmpty()
            .Validate(input =>
            {
                var inputMinusCommonSymbols = new string(input.Where(c => !invalidChars.Contains(c)).ToArray());

                if (inputMinusCommonSymbols.Any(char.IsNumber) == false)
                    return ValidationResult.Error("[red]Invalid Phone Number[/] Phone number cannot contain letters or symbols.");

                if (string.IsNullOrWhiteSpace(inputMinusCommonSymbols))
                    return ValidationResult.Error("[red]Required Field:[/] Phone number cannot be empty.");

                if (inputMinusCommonSymbols.Length < 7 || inputMinusCommonSymbols.Length > 15)
                    return ValidationResult.Error("[red]Invalid Phone Number[/] Phone number length must be between 7 and 15 numbers long.");

                return ValidationResult.Success();
            });

        return new string(AnsiConsole.Prompt(phoneNumberPrompt).Where(c => !invalidChars.Contains(c)).ToArray()).Replace(" ", "");
    }
    internal string GetEmailSubjectFromUser()
    {
        var subjectPrompt = new TextPrompt<string>("Enter email [green]SUBJECT[/]:")
            .AllowEmpty()
            .Validate(input =>
            {
                if (string.IsNullOrWhiteSpace(input))
                    return ValidationResult.Error("[red]Required Field:[/] Subject cannot be empty.");

                if (input.Length > 50)
                    return ValidationResult.Error("[red]Exceeded Max Lenght:[/] Subject must be 50 characters or less.");

                return ValidationResult.Success();
            });

        return AnsiConsole.Prompt(subjectPrompt);
    }
    internal string GetEmailBodyFromUser()
    {
        var bodyPrompt = new TextPrompt<string>("Enter email [green]BODY[/]:")
            .AllowEmpty()
            .Validate(input =>
            {
                if (string.IsNullOrWhiteSpace(input))
                    return ValidationResult.Error("[red]Required Field:[/] Body cannot be empty.");

                return ValidationResult.Success();
            });

        return AnsiConsole.Prompt(bodyPrompt);
    }

    internal bool GetAddConfirmationFromUser(string name, string addType)
    {
        return AnsiConsole.Confirm($"Confirm adding [green]{name}[/] to the {addType}?");
    }
    internal bool GetDeleteConfirmationFromUser(string name, string deleteType)
    {
        return AnsiConsole.Confirm($"Confirm deleting [green]{name}[/] from the {deleteType}?");
    }
    internal bool GetRenameCategoryConfirmationFromUser(string originalName, string newName)
    {
        return AnsiConsole.Confirm($"Confirm renaming [yellow]{originalName}[/] to [green]{newName}[/]?");
    }
    internal bool GetEditContactConfirmationFromUser(FullContactViewModel originalContact, EditContactViewModel newContact)
    {
        string preamble = $"Confirm the following changes for the contact {originalContact.FirstName} {originalContact.LastName}:";
        string changes = string.Empty;

        if (newContact.ChangedFirstName) changes += $"\t[yellow]{originalContact.FirstName}[/] to [green]{newContact.FirstName}[/]\r\n";
        if (newContact.ChangedLastName) changes += $"\t[yellow]{originalContact.LastName}[/] to [green]{newContact.LastName}[/]\r\n";
        if (newContact.ChangedPhoneNumber) changes += $"\t[yellow]{originalContact.PhoneNumber}[/] to [green]{newContact.PhoneNumber}[/]\r\n";
        if (newContact.ChangedEmail) changes += $"\t[yellow]{originalContact.Email}[/] to [green]{newContact.Email}[/]\r\n";
        if (newContact.ChangedCategory) changes += $"\t[yellow]{originalContact.CategoryName}[/] to [green]{newContact.CategoryName}[/]\r\n";

        return AnsiConsole.Confirm($"{preamble}\r\n\r\n{changes}\r\nConfirm:");
    }
    internal bool GetEmailContentsConfirmationFromUser(FullContactViewModel contact, string subject, string body)
    {
        string email = "Please confirm the following email:\r\n\r\n";
        email += $"[green]To:[/] [yellow]{contact.Email}[/] ({contact.FirstName} {contact.LastName})\r\n";
        email += $"[green]Subject:[/] {subject}\r\n\r\n";
        email += $"[green]Body:[/]\r\n{body}\r\n\r\n";

        return AnsiConsole.Confirm($"{email}\r\nConfirm send:");
    }
    internal bool GetRetrySendConfirmationFromUser()
    {
        return AnsiConsole.Confirm($"Retry sending email?");
    }
    internal void PressAnyKeyToContinue()
    {
        Console.Write("Press any key to continue...");
        Console.ReadLine();
    }
}
