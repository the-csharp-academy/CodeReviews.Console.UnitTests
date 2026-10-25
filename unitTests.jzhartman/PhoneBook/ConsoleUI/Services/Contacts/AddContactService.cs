using PhoneBook.Application.Contacts.AddContact;
using PhoneBook.Application.Contacts.SaveChanges;
using PhoneBook.ConsoleUI.Input;
using PhoneBook.ConsoleUI.Output;
using PhoneBook.ConsoleUI.Services.Categories;
using PhoneBook.Domain.Validation;
using PhoneBook.Domain.Validation.Errors;

namespace PhoneBook.ConsoleUI.Services.Contacts;

internal class AddContactService
{
    private readonly CategorySelectionService _categorySelectionService;
    private readonly AddContactHandler _addContactHandler;
    private readonly SaveChangesHandler _saveChangesHandler;
    private readonly UserInput _userInput;
    private readonly Messages _messages;

    public AddContactService(AddContactHandler addContactHandler, SaveChangesHandler saveChangesHandler,
                                CategorySelectionService categorySelectionService, UserInput userInput, Messages messages)
    {
        _categorySelectionService = categorySelectionService;
        _addContactHandler = addContactHandler;
        _saveChangesHandler = saveChangesHandler;
        _userInput = userInput;
        _messages = messages;
    }

    internal async Task RunAsync()
    {
        var firstName = _userInput.GetNameFromUser($"Enter your [green]FIRST NAME[/]:");
        var lastName = _userInput.GetNameFromUser($"Enter your [green]LAST NAME[/]:");
        var phoneNumber = _userInput.GetPhoneNumberFromUser();
        var email = _userInput.GetEmailAddressFromUser();
        var categoryId = await GetCategoryIdFromUserAsync();

        bool confirmAdd = _userInput.GetAddConfirmationFromUser($"{firstName} {lastName}", "contact list");

        if (confirmAdd)
        {
            var addResult = await _addContactHandler.HandleAsync(new(firstName, lastName, phoneNumber, email, categoryId));
            var errors = new List<Error>();

            if (addResult.IsSuccess)
            {
                var saveResult = await _saveChangesHandler.HandleAsync();

                if (saveResult is null)
                    errors.Add(ContactRepositoryErrors.SaveResponseNull);

                else if (saveResult.IsFailure)
                    errors.AddRange(saveResult.Errors);

                else if (saveResult.IsSuccess)
                {
                    _messages.AddSucessfulMessage($"{firstName} {lastName}", "contact list");
                    _userInput.PressAnyKeyToContinue();
                    return;
                }
            }

            if (addResult.IsFailure)
                errors.AddRange(addResult.Errors);

            if (addResult is null)
                errors.Add(ContactRepositoryErrors.AddResponseNull);

            _messages.ErrorMessage(errors);
        }
        else
        {
            _messages.AddCancelledMessage($"{firstName} {lastName}", "contact list");
        }
        _userInput.PressAnyKeyToContinue();
    }

    private async Task<int> GetCategoryIdFromUserAsync()
    {
        bool categoryIdIsValid = false;
        int categoryId = 0;

        while (categoryIdIsValid == false)
        {
            var category = await _categorySelectionService.RunAsync();

            if (category is not null)
            {
                categoryIdIsValid = true;
                categoryId = category.Id;
            }
            else
            {
                _messages.SettingDefaultCategory();
            }
        }

        return categoryId;
    }
}
