using PhoneBook.Application.Categories.GetCategoryById;
using PhoneBook.Application.Contacts.DTOs;
using PhoneBook.ConsoleUI.Input;
using PhoneBook.ConsoleUI.Models;
using PhoneBook.ConsoleUI.Output;
using PhoneBook.Domain.Validation;
using PhoneBook.Domain.Validation.Errors;

namespace PhoneBook.ConsoleUI.Services.Contacts;

internal class GenerateFullContactService
{
    private readonly GetCategoryByIdHandler _getCategoryByIdHandler;
    private readonly Messages _messages;
    private readonly UserInput _userInput;

    public GenerateFullContactService(GetCategoryByIdHandler getCategoryByIdHandler, Messages messages, UserInput userInput)
    {
        _getCategoryByIdHandler = getCategoryByIdHandler;
        _messages = messages;
        _userInput = userInput;
    }

    public async Task<FullContactViewModel?> RunAsync(ContactResponse contact)
    {
        var categoryResult = await _getCategoryByIdHandler.HandleAsync(contact.CategoryId);

        var errors = new List<Error>();

        if (categoryResult.Value is null || categoryResult is null)
            errors.Add(ContactRepositoryErrors.NullResponse);

        else if (categoryResult.IsFailure)
            errors.AddRange(categoryResult.Errors);

        else if (categoryResult.Value.Id != contact.CategoryId)
            errors.Add(new Error("CategoryIdMismatch", "The category id of the returned data did not match"));

        if (errors.Count > 0)
        {
            _messages.ErrorMessage(errors);
            _userInput.PressAnyKeyToContinue();
            return null;
        }

        return new FullContactViewModel(contact, categoryResult.Value);
    }
}
