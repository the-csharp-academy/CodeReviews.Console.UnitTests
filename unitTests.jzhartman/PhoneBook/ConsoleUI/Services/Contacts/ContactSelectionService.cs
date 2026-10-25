using PhoneBook.Application.Categories.DTOs;
using PhoneBook.Application.Categories.GetCategoryById;
using PhoneBook.Application.Contacts.DTOs;
using PhoneBook.Application.Contacts.GetAllContacts;
using PhoneBook.Application.Contacts.GetContactsByCategoryId;
using PhoneBook.ConsoleUI.Input;
using PhoneBook.ConsoleUI.Models;
using PhoneBook.ConsoleUI.Output;
using PhoneBook.ConsoleUI.Views;
using PhoneBook.Domain.Validation;
using PhoneBook.Domain.Validation.Errors;


namespace PhoneBook.ConsoleUI.Services.Contacts;

internal class ContactSelectionService
{
    private readonly GetAllContactsHandler _getAllContactsHandler;
    private readonly GetAllContactsByCategoryIdHandler _getAllContactsByCategoryIdHandler;
    private readonly GetCategoryByIdHandler _getCategoryByIdHandler;
    private readonly ContactSelectionView _contactSelectionView;
    private readonly GenerateFullContactService _generateFullContactService;
    private readonly Messages _messages;
    private readonly UserInput _userInput;

    public ContactSelectionService(GetAllContactsHandler getAllContactsHandler, ContactSelectionView contactSelectionView,
                                    GetAllContactsByCategoryIdHandler getAllContactsByCategoryIdHandler,
                                    GetCategoryByIdHandler getCategoryByIdHandler, GenerateFullContactService generateFullContactService,
                                    Messages messages, UserInput userInput)
    {
        _getAllContactsHandler = getAllContactsHandler;
        _getAllContactsByCategoryIdHandler = getAllContactsByCategoryIdHandler;
        _getCategoryByIdHandler = getCategoryByIdHandler;
        _contactSelectionView = contactSelectionView;
        _generateFullContactService = generateFullContactService;

        _messages = messages;
        _userInput = userInput;
    }

    public async Task<FullContactViewModel?> RunAsync(CategoryResponse category)
    {
        var contactResult = await GetContactListAsync(category);

        if (contactResult is null)
            return null;

        var contact = _contactSelectionView.Render(contactResult);

        return await _generateFullContactService.RunAsync(contact);
    }

    private async Task<List<ContactResponse>?> GetContactListAsync(CategoryResponse category)
    {
        Result<List<ContactResponse>> contactResult;

        if (category.Id == -1 && category.Name.ToUpper() == "ALL")
            contactResult = await _getAllContactsHandler.HandleAsync();
        else
            contactResult = await _getAllContactsByCategoryIdHandler.HandleAsync(category);

        if (contactResult.IsFailure || contactResult.Value is null)
        {
            _messages.ErrorMessage(contactResult.Errors);
            _userInput.PressAnyKeyToContinue();
            return null;
        }
        if (contactResult.Value.Count == 0)
        {
            _messages.ErrorMessage(new Error[] { ContactRepositoryErrors.NoContactsInCategory });
            _userInput.PressAnyKeyToContinue();
            return null;
        }

        return contactResult.Value;
    }
}
