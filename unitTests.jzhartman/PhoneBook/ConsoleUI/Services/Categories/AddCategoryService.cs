using PhoneBook.Application.Categories.AddCategory;
using PhoneBook.Application.Contacts.SaveChanges;
using PhoneBook.ConsoleUI.Input;
using PhoneBook.ConsoleUI.Output;
using PhoneBook.Domain.Validation;
using PhoneBook.Domain.Validation.Errors;

namespace PhoneBook.ConsoleUI.Services.Categories;

internal class AddCategoryService
{
    private readonly AddCategoryHandler _addCategoryHandler;
    private readonly SaveChangesHandler _saveChangesHandler;
    private readonly UserInput _userInput;
    private readonly Messages _messages;

    public AddCategoryService(AddCategoryHandler addCategoryHandler, SaveChangesHandler saveChangesHandler,
                                UserInput userInput, Messages messages)
    {
        _addCategoryHandler = addCategoryHandler;
        _saveChangesHandler = saveChangesHandler;
        _userInput = userInput;
        _messages = messages;
    }

    internal async Task RunAsync()
    {
        var name = _userInput.GetNameFromUser($"Enter category [green]NAME[/]:");

        bool confirmAdd = _userInput.GetAddConfirmationFromUser(name, "category list");

        if (confirmAdd)
        {
            var addResult = await _addCategoryHandler.HandleAsync(new AddCategoryRequest(name));
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
                    _messages.AddSucessfulMessage(name, "category list");
                    _userInput.PressAnyKeyToContinue();
                    return;
                }
            }

            if (addResult.IsFailure)
                errors.AddRange(addResult.Errors);

            if (addResult is null)
                errors.Add(CategoryRepositoryErrors.AddResponseNull);

            _messages.ErrorMessage(errors);
        }
        else
        {
            _messages.AddCancelledMessage(name, "category list");
        }

        _userInput.PressAnyKeyToContinue();
    }
}
