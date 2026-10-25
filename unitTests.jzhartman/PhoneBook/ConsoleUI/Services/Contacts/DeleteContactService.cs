using PhoneBook.Application.Contacts.DeleteContact;
using PhoneBook.Application.Contacts.DTOs;
using PhoneBook.Application.Contacts.SaveChanges;
using PhoneBook.ConsoleUI.Input;
using PhoneBook.ConsoleUI.Models;
using PhoneBook.ConsoleUI.Output;
using PhoneBook.Domain.Validation;
using PhoneBook.Domain.Validation.Errors;

namespace PhoneBook.ConsoleUI.Services;

internal class DeleteContactService
{
    private readonly DeleteContactHandler _deleteContactHandler;
    private readonly SaveChangesHandler _saveChangesHandler;
    private readonly Messages _messages;
    private readonly UserInput _userInput;

    public DeleteContactService(DeleteContactHandler deleteContactHandler, SaveChangesHandler saveChangesHandler,
                                Messages messages, UserInput userInput)
    {
        _deleteContactHandler = deleteContactHandler;
        _saveChangesHandler = saveChangesHandler;
        _messages = messages;
        _userInput = userInput;
    }

    internal async Task<bool> RunAsync(FullContactViewModel contact)
    {
        var contactRequest = new ContactResponse(contact.Id,
                                                contact.FirstName,
                                                contact.LastName,
                                                contact.PhoneNumber,
                                                contact.Email,
                                                contact.CategoryId);

        var confirmDelete = _userInput.GetDeleteConfirmationFromUser($"{contact.FirstName} {contact.LastName}", "contact list");

        if (confirmDelete)
        {
            var deleteResult = await _deleteContactHandler.HandleAsync(contactRequest);
            var errors = new List<Error>();

            if (deleteResult.IsSuccess)
            {
                var saveResult = await _saveChangesHandler.HandleAsync();

                if (saveResult is null)
                    errors.Add(ContactRepositoryErrors.SaveResponseNull);

                else if (saveResult.IsFailure)
                    errors.AddRange(saveResult.Errors);

                else if (saveResult.IsSuccess)
                {
                    _messages.DeleteSucessfulMessage($"{contact.FirstName} {contact.LastName}", "contact list");
                    _userInput.PressAnyKeyToContinue();
                    return true;
                }
            }

            if (deleteResult.IsFailure)
                errors.AddRange(deleteResult.Errors);

            if (deleteResult is null)
                errors.Add(ContactRepositoryErrors.DeleteResponseNull);

            _messages.ErrorMessage(errors);
        }
        else
        {
            _messages.DeleteCancelledMessage($"{contact.FirstName} {contact.LastName}", "contact list");
        }
        _userInput.PressAnyKeyToContinue();

        return false;
    }
}
