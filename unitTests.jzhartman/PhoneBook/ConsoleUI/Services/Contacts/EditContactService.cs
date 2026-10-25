using PhoneBook.Application.Contacts.DTOs;
using PhoneBook.Application.Contacts.EditContact;
using PhoneBook.Application.Contacts.SaveChanges;
using PhoneBook.ConsoleUI.Enums;
using PhoneBook.ConsoleUI.Input;
using PhoneBook.ConsoleUI.Models;
using PhoneBook.ConsoleUI.Output;
using PhoneBook.ConsoleUI.Services.Categories;
using PhoneBook.ConsoleUI.Views;
using PhoneBook.Domain.Validation;
using PhoneBook.Domain.Validation.Errors;
using Spectre.Console;

namespace PhoneBook.ConsoleUI.Services;

internal class EditContactService
{
    private readonly EditContactView _editContactView;
    private readonly UserInput _userInput;
    private readonly Messages _messages;
    private readonly EditContactHandler _editContactHandler;
    private readonly SaveChangesHandler _saveChangesHandler;
    private readonly CategorySelectionService _categorySelectionService;

    public EditContactService(EditContactView editContactView, UserInput userInput, Messages messages,
                                EditContactHandler editContactHandler, SaveChangesHandler saveChangesHandler,
                                CategorySelectionService categorySelectionService)
    {
        _editContactView = editContactView;
        _userInput = userInput;
        _messages = messages;
        _editContactHandler = editContactHandler;
        _saveChangesHandler = saveChangesHandler;
        _categorySelectionService = categorySelectionService;
    }

    public async Task RunAsync(FullContactViewModel originalContact)
    {
        bool stillEditing = true;
        var contact = new EditContactViewModel(originalContact);

        while (stillEditing)
        {
            Console.Clear();
            AnsiConsole.MarkupLine($"[green]Editing[/] contact entry for {contact.FirstName} {contact.LastName}:");
            AnsiConsole.WriteLine();

            _editContactView.Render(contact);
            AnsiConsole.WriteLine();

            RenderEditContactKeyOptions();

            var keyInfo = Console.ReadKey(true);
            var exitCode = await ManageKeyPressMenuFromCategoryAsync(keyInfo, originalContact, contact);

            if (exitCode == EditContactExitCode.Save)
            {
                var confirmEdits = _userInput.GetEditContactConfirmationFromUser(originalContact, contact);
                if (confirmEdits)
                {
                    _messages.EditContactSuccessfulMessage($"{contact.FirstName} {contact.LastName}");
                    stillEditing = false;
                    await UpdateContactAsync(contact);
                }
                else
                {
                    _messages.EditContactCancelSaveMessage($"{originalContact.FirstName} {originalContact.LastName}");
                    _userInput.PressAnyKeyToContinue();
                }
            }

            if (exitCode == EditContactExitCode.Cancel)
            {
                stillEditing = false;
                _messages.EditContactCancelledMessage($"{originalContact.FirstName} {originalContact.LastName}");
            }
        }
        _userInput.PressAnyKeyToContinue();
    }

    private void RenderEditContactKeyOptions()
    {
        var table = new Table()
                        .RoundedBorder()
                        .BorderColor(Spectre.Console.Color.Blue)
                        .Title("Select an field to edit:")
                        .ShowRowSeparators();

        table.AddColumn("Key");
        table.AddColumn("Operation");

        table.AddRow("1", "First Name");
        table.AddRow("2", "Last Name");
        table.AddRow("3", "Phone Number");
        table.AddRow("4", "Email");
        table.AddRow("5", "Category");
        table.AddRow("6", "Save Changes");
        table.AddRow("7", "Cancel");

        AnsiConsole.Write(table);
    }

    private async Task<EditContactExitCode> ManageKeyPressMenuFromCategoryAsync(ConsoleKeyInfo keyInfo,
                                                                            FullContactViewModel originalContact,
                                                                            EditContactViewModel contact)
    {
        switch (keyInfo.Key)
        {
            case ConsoleKey.D1:
                ManageUpdateFirstName(originalContact, contact);
                break;
            case ConsoleKey.D2:
                ManageUpdateLastName(originalContact, contact);
                break;
            case ConsoleKey.D3:
                ManageUpdatePhoneNumber(originalContact, contact);
                break;
            case ConsoleKey.D4:
                ManageUpdateEmail(originalContact, contact);
                break;
            case ConsoleKey.D5:
                await ManageUpdateCategoryAsync(originalContact, contact);
                break;
            case ConsoleKey.D6:
                return EditContactExitCode.Save;
            case ConsoleKey.D7:
                return EditContactExitCode.Cancel;
            default:
                return EditContactExitCode.None;
        }
        return EditContactExitCode.None;
    }

    private async Task UpdateContactAsync(EditContactViewModel contact)
    {
        var updateResult = await _editContactHandler.HandleAsync(new ContactResponse(contact.Id,
                                                                        contact.FirstName,
                                                                        contact.LastName,
                                                                        contact.PhoneNumber,
                                                                        contact.Email,
                                                                        contact.CategoryId));

        var errors = new List<Error>();

        if (updateResult.IsSuccess)
            return;

        if (updateResult.IsFailure)
            errors.AddRange(updateResult.Errors);

        if (updateResult is null)
            errors.Add(ContactRepositoryErrors.UpdateResponseNull);

        _messages.ErrorMessage(errors);
        _userInput.PressAnyKeyToContinue();
    }

    private async Task ManageUpdateCategoryAsync(FullContactViewModel originalContact, EditContactViewModel contact)
    {
        var newCategory = await _categorySelectionService.RunAsync(true);
        contact.CategoryName = newCategory.Name;
        contact.CategoryId = newCategory.Id;
        contact.ChangedCategory = (contact.CategoryName == originalContact.CategoryName) ? false : true;
    }

    private void ManageUpdateFirstName(FullContactViewModel originalContact, EditContactViewModel contact)
    {
        contact.FirstName = _userInput.GetNameFromUser("Please enter the new first name:");
        contact.ChangedFirstName = (contact.FirstName == originalContact.FirstName) ? false : true;
    }
    private void ManageUpdateLastName(FullContactViewModel originalContact, EditContactViewModel contact)
    {
        contact.LastName = _userInput.GetNameFromUser("Please enter the new last name:");
        contact.ChangedLastName = (contact.LastName == originalContact.LastName) ? false : true;
    }

    private void ManageUpdatePhoneNumber(FullContactViewModel originalContact, EditContactViewModel contact)
    {
        contact.PhoneNumber = _userInput.GetPhoneNumberFromUser();
        contact.ChangedPhoneNumber = (contact.PhoneNumber == originalContact.PhoneNumber) ? false : true;
    }

    private void ManageUpdateEmail(FullContactViewModel originalContact, EditContactViewModel contact)
    {
        contact.Email = _userInput.GetEmailAddressFromUser();
        contact.ChangedEmail = (contact.Email == originalContact.Email) ? false : true;
    }
}
