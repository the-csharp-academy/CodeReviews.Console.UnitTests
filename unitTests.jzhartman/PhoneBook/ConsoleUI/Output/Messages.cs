using PhoneBook.Domain.Validation;
using Spectre.Console;

namespace PhoneBook.ConsoleUI.Output;

internal class Messages
{
    internal void ErrorMessage(IEnumerable<Error> errors)
    {
        foreach (var error in errors)
        {
            AnsiConsole.MarkupLineInterpolated($"[red]ERROR:[/] {error.Description}");
        }
    }

    internal void AddCancelledMessage(string name, string addType)
    {
        AnsiConsole.MarkupLineInterpolated($"Cancelled adding [green]{name}[/] to {addType}.");
    }
    internal void AddSucessfulMessage(string name, string addType)
    {
        AnsiConsole.MarkupLineInterpolated($"Successfully added [green]{name}[/] to {addType}");
    }
    internal void DeleteCancelledMessage(string name, string addType)
    {
        AnsiConsole.MarkupLineInterpolated($"Cancelled deleting [green]{name}[/] from {addType}.");
    }
    internal void DeleteSucessfulMessage(string name, string addType)
    {
        AnsiConsole.MarkupLineInterpolated($"Successfully deleted [green]{name}[/] from {addType}");
    }
    internal void RenameCategorySuccessfulMessage(string originalName, string newName)
    {
        AnsiConsole.MarkupLineInterpolated($"Successfully changed [yellow]{originalName}[/] to [green]{newName}[/]");
    }
    internal void RenameCategoryCancelledMessage(string originalName, string newName)
    {
        AnsiConsole.MarkupLineInterpolated($"Cancelled changing [yellow]{originalName}[/] to [green]{newName}[/]");
    }

    internal void EditContactCancelledMessage(string name)
    {
        AnsiConsole.MarkupLineInterpolated($"Cancelled editing [green]{name}[/]");
    }
    internal void EditContactSuccessfulMessage(string name)
    {
        AnsiConsole.MarkupLineInterpolated($"Successfully updated [green]{name}[/]");
    }
    internal void EditContactCancelSaveMessage(string name)
    {
        AnsiConsole.MarkupLineInterpolated($"Changes to [green]{name}[/] not saved");
    }

    internal void EmailSendSuccessfulMessage()
    {
        AnsiConsole.MarkupLineInterpolated($"Email successfully sent!");
    }
    internal void EmailSendCancelledMessage()
    {
        AnsiConsole.MarkupLineInterpolated($"Email cancelled!");
    }

    internal void RetryingSendEmailMessage(int retryCount)
    {
        AnsiConsole.MarkupLineInterpolated($"Attempting to resend email. Total attempts: {retryCount}...");
    }

    internal void SettingDefaultCategory()
    {
        AnsiConsole.MarkupLineInterpolated($"Category will be set to the default value: [yellow]UNCATEGORIZED[/].");

    }
}
