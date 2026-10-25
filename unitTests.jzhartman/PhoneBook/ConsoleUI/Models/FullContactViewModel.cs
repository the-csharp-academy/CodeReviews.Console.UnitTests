using PhoneBook.Application.Categories.DTOs;
using PhoneBook.Application.Contacts.DTOs;

namespace PhoneBook.ConsoleUI.Models;

internal class FullContactViewModel
{
    public int Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string PhoneNumber { get; set; }
    public string Email { get; set; }
    public int CategoryId { get; set; }
    public string CategoryName { get; set; }

    public FullContactViewModel(ContactResponse contact, CategoryResponse category)
    {
        Id = contact.ContactId;
        FirstName = contact.FirstName;
        LastName = contact.LastName;
        PhoneNumber = contact.PhoneNumber;
        Email = contact.Email;
        CategoryId = category.Id;
        CategoryName = category.Name;
    }
}
