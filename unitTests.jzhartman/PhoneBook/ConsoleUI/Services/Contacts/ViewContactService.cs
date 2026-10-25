using PhoneBook.Application.GetById;
using PhoneBook.ConsoleUI.Enums;
using PhoneBook.ConsoleUI.Input;
using PhoneBook.ConsoleUI.Models;
using PhoneBook.ConsoleUI.Output;
using PhoneBook.ConsoleUI.Services.Categories;
using PhoneBook.ConsoleUI.Services.Email;
using PhoneBook.ConsoleUI.Views;
using PhoneBook.Domain.Validation;
using PhoneBook.Domain.Validation.Errors;
using Spectre.Console;

namespace PhoneBook.ConsoleUI.Services.Contacts;

internal class ViewContactService
{
    private readonly CategorySelectionService _categorySelectionService;
    private readonly ContactSelectionService _contactSelectionService;
    private readonly DeleteContactService _deleteContactService;
    private readonly GenerateFullContactService _generateFullContactService;
    private readonly GetContactByIdHandler _getContactByIdHandler;
    private readonly SendEmailService _sendEmailService;
    private readonly EditContactService _editContactService;
    private readonly ContactDetailsView _contactDetailsView;
    private readonly Messages _messages;
    private readonly UserInput _userInput;

    public ViewContactService(ContactSelectionService contactSelectionService, DeleteContactService deleteContactService,
                                    CategorySelectionService categorySelectionService, GenerateFullContactService generateFullContactService,
                                    GetContactByIdHandler getContactByIdHandler, SendEmailService sendEmailService,
                                    EditContactService editContactService, ContactDetailsView contactDetailsView,
                                    Messages messages, UserInput userInput)
    {
        _categorySelectionService = categorySelectionService;
        _contactSelectionService = contactSelectionService;
        _deleteContactService = deleteContactService;
        _editContactService = editContactService;
        _generateFullContactService = generateFullContactService;
        _sendEmailService = sendEmailService;

        _getContactByIdHandler = getContactByIdHandler;

        _contactDetailsView = contactDetailsView;

        _messages = messages;
        _userInput = userInput;
    }

    internal async Task RunAsync()
    {
        bool returnToMainMenu = false;
        ManageSubMenuOptions[] menuOptions = Enum.GetValues<ManageSubMenuOptions>();

        Console.Clear();
        var category = await _categorySelectionService.RunAsync(true);

        var contact = await _contactSelectionService.RunAsync(category ?? new(-1, "ALL"));

        if (contact is null)
            return;

        while (returnToMainMenu == false)
        {
            Console.Clear();

            AnsiConsole.WriteLine($"Viewing contact entry for {contact.FirstName} {contact.LastName}:");
            AnsiConsole.WriteLine();

            _contactDetailsView.Render(contact);
            AnsiConsole.WriteLine();

            RenderContactDetailKeyOptions();

            var keyInfo = Console.ReadKey(true);
            var operation = await ManageKeyPressMenuAsync(keyInfo, contact);

            if (operation == ManageSubMenuOptions.Exit || operation == ManageSubMenuOptions.Delete)
                returnToMainMenu = true;

            if (operation == ManageSubMenuOptions.Edit)
            {
                var updatedContact = await ReloadUpdatedContactAsync(contact.Id);

                if (updatedContact is null)
                    return;

                contact = updatedContact;
            }
        }
    }

    private async Task<FullContactViewModel?> ReloadUpdatedContactAsync(int contactId)
    {
        var updatedContact = await _getContactByIdHandler.HandleAsync(contactId);

        var errors = new List<Error>();

        if (updatedContact is null || updatedContact.Value is null)
            errors.Add(ContactRepositoryErrors.NullResponse);
        else if (updatedContact.IsFailure)
            errors.AddRange(updatedContact.Errors);

        if (errors.Count > 0)
        {
            _messages.ErrorMessage(errors);
            _userInput.PressAnyKeyToContinue();
            return null;
        }

        return await _generateFullContactService.RunAsync(updatedContact.Value);
    }

    private void RenderContactDetailKeyOptions()
    {
        var table = new Table()
                        .RoundedBorder()
                        .BorderColor(Spectre.Console.Color.Blue)
                        .ShowRowSeparators();

        table.AddColumn("Key");
        table.AddColumn("Operation");

        table.AddRow("1", "Delete Contact");
        table.AddRow("2", "Edit Contact");
        table.AddRow("3", "Send Email");
        table.AddRow("4", "Return to Main Menu");

        AnsiConsole.Write(table);
    }

    private async Task<ManageSubMenuOptions> ManageKeyPressMenuAsync(ConsoleKeyInfo keyInfo, FullContactViewModel contact)
    {
        switch (keyInfo.Key)
        {
            case ConsoleKey.D1:
                var contactDeleted = await _deleteContactService.RunAsync(contact);
                return (contactDeleted) ? ManageSubMenuOptions.Delete : ManageSubMenuOptions.Unknown;
            case ConsoleKey.D2:
                await _editContactService.RunAsync(contact);
                return ManageSubMenuOptions.Edit;
            case ConsoleKey.D3:
                await _sendEmailService.RunAsync(contact);
                return ManageSubMenuOptions.Email;
            case ConsoleKey.D4:
                return ManageSubMenuOptions.Exit;
            default:
                _messages.ErrorMessage(new[] { Errors.InvalidKeyPress });
                Console.WriteLine("ERROR! Invalid key press");
                _userInput.PressAnyKeyToContinue();
                return ManageSubMenuOptions.Unknown;
        }
    }
}
