namespace PhoneBook.ConsoleUI.Models;

internal class EditContactViewModel
{
    internal bool ChangedFirstName { get; set; } = false;
    internal bool ChangedLastName { get; set; } = false;
    internal bool ChangedPhoneNumber { get; set; } = false;
    internal bool ChangedEmail { get; set; } = false;
    internal bool ChangedCategory { get; set; } = false;

    internal int Id { get; set; }
    internal string FirstName { get; set; }
    internal string LastName { get; set; }
    internal string PhoneNumber { get; set; }
    internal string Email { get; set; }
    internal int CategoryId { get; set; }
    internal string CategoryName { get; set; }

    internal EditContactViewModel(FullContactViewModel originalContact)
    {
        Id = originalContact.Id;
        FirstName = originalContact.FirstName;
        LastName = originalContact.LastName;
        PhoneNumber = originalContact.PhoneNumber;
        Email = originalContact.Email;
        CategoryId = originalContact.CategoryId;
        CategoryName = originalContact.CategoryName;
    }
}
