using Phonebook.Models;
using Phonebook.Services;
using Phonebook.UI;
using Spectre.Console;

namespace Phonebook.Controllers;

public class ContactController(ContactService contactService, EmailService emailService, SmsService smsService)
{
    public async Task RunAsync()
    {
        while (true)
        {
            UserInterface.Clear();

            var choice = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("Phonebook")
                    .AddChoices(
                        "View all contacts",
                        "Add contact",
                        "Update contact",
                        "Delete contact",
                        "Send email",
                        "Send SMS",
                        "Exit"));

            switch (choice)
            {
                case "View all contacts":
                    await ViewAllAsync();
                    UserInterface.Pause();
                    break;
                case "Add contact":
                    await AddAsync();
                    UserInterface.Pause();
                    break;
                case "Update contact":
                    await UpdateAsync();
                    UserInterface.Pause();
                    break;
                case "Delete contact":
                    await DeleteAsync();
                    UserInterface.Pause();
                    break;
                case "Send email":
                    await SendEmailAsync();
                    UserInterface.Pause();
                    break;
                case "Send SMS":
                    await SendSmsAsync();
                    UserInterface.Pause();
                    break;
                case "Exit": return;
            }
        }
    }

    private async Task ViewAllAsync()
    {
        var result = await contactService.GetAllAsync();
        if (!result.Success)
        {
            UserInterface.ShowError(result.Error!);
            return;
        }

        if (result.Data!.Count == 0)
        {
            UserInterface.ShowError("The phonebook is empty. Add a contact first.");
            return;
        }

        UserInterface.ShowContacts(result.Data);
    }

    private async Task AddAsync()
    {
        string name = ReadValidName();
        if (name == string.Empty) return;

        string email = ReadValidEmail();
        if (email == string.Empty) return;

        string phone = ReadValidPhone();
        if (phone == string.Empty) return;

        var category = UserInterface.AskForCategory();

        var result = await contactService.AddAsync(new Contact { Name = name, Email = email, PhoneNumber = phone, Category = category });

        if (result.Success)
            UserInterface.ShowSuccess("Contact added.");
        else
            UserInterface.ShowError(result.Error!);
    }

    private async Task UpdateAsync()
    {
        var all = await contactService.GetAllAsync();
        if (!all.Success || all.Data!.Count == 0)
        {
            UserInterface.ShowError(all.Success ? "The phonebook is empty." : all.Error!);
            return;
        }

        UserInterface.ShowContacts(all.Data);
        int id = UserInterface.AskForContactId("update");

        var found = await contactService.GetByIdAsync(id);
        if (!found.Success)
        {
            UserInterface.ShowError(found.Error!);
            return;
        }

        string name = ReadValidName();
        if (name == string.Empty) return;
        string email = ReadValidEmail();
        if (email == string.Empty) return;
        string phone = ReadValidPhone();
        if (phone == string.Empty) return;
        var category = UserInterface.AskForCategory();

        var result = await contactService.UpdateAsync(new Contact { Id = id, Name = name, Email = email, PhoneNumber = phone, Category = category });

        if (result.Success)
            UserInterface.ShowSuccess($"Contact {id} updated.");
        else
            UserInterface.ShowError(result.Error!);
    }

    private async Task DeleteAsync()
    {
        var all = await contactService.GetAllAsync();
        if (!all.Success || all.Data!.Count == 0)
        {
            UserInterface.ShowError(all.Success ? "The phonebook is empty." : all.Error!);
            return;
        }

        UserInterface.ShowContacts(all.Data);
        int id = UserInterface.AskForContactId("delete");

        var result = await contactService.DeleteAsync(id);

        if (result.Success)
            UserInterface.ShowSuccess($"Contact {id} deleted.");
        else
            UserInterface.ShowError(result.Error!);
    }

    private async Task SendEmailAsync()
    {
        var all = await contactService.GetAllAsync();
        if (!all.Success || all.Data!.Count == 0)
        {
            UserInterface.ShowError(all.Success ? "The phonebook is empty." : all.Error!);
            return;
        }

        UserInterface.ShowContacts(all.Data);
        int id = UserInterface.AskForContactId("email");

        var found = await contactService.GetByIdAsync(id);
        if (!found.Success)
        {
            UserInterface.ShowError(found.Error!);
            return;
        }

        var (subject, body) = UserInterface.AskForEmailMessage();
        var result = await emailService.SendEmailAsync(found.Data!, subject, body);

        if (result.Success)
            UserInterface.ShowSuccess(result.Message!);
        else
            UserInterface.ShowError(result.Message!);
    }

    private async Task SendSmsAsync()
    {
        var all = await contactService.GetAllAsync();
        if (!all.Success || all.Data!.Count == 0)
        {
            UserInterface.ShowError(all.Success ? "The phonebook is empty." : all.Error!);
            return;
        }

        UserInterface.ShowContacts(all.Data);
        int id = UserInterface.AskForContactId("SMS");

        var found = await contactService.GetByIdAsync(id);
        if (!found.Success)
        {
            UserInterface.ShowError(found.Error!);
            return;
        }

        var text = UserInterface.AskForSmsText();
        var result = await smsService.SendSmsAsync(found.Data!, text);

        if (result.Success)
            UserInterface.ShowSuccess(result.Message!);
        else
            UserInterface.ShowError(result.Message!);
    }

    // Validation loops: re-ask until valid; empty input cancels the current flow.
    private static string ReadValidName()
    {
        while (true)
        {
            var name = UserInterface.AskForName().Trim();
            if (name.Length == 0) return string.Empty; // empty = cancel
            if (ValidationService.IsValidName(name)) return name;
            UserInterface.ShowError("A name can't be empty.");
        }
    }

    private static string ReadValidEmail()
    {
        while (true)
        {
            var email = UserInterface.AskForEmail().Trim();
            if (email.Length == 0) return string.Empty;
            if (ValidationService.IsValidEmail(email)) return email;
            UserInterface.ShowError($"Invalid email. Expected format: {UserInterface.ExpectedEmailFormat}");
        }
    }

    private static string ReadValidPhone()
    {
        while (true)
        {
            var phone = UserInterface.AskForPhone().Trim();
            if (phone.Length == 0) return string.Empty;
            if (ValidationService.IsValidPhoneNumber(phone)) return phone;
            UserInterface.ShowError($"Invalid phone number. Accepted formats: {UserInterface.ExpectedPhoneFormats}");
        }
    }
}
