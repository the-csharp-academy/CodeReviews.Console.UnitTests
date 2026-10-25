namespace PhoneBook.Domain.Validation.Errors;

public static class ContactRepositoryErrors
{
    public static readonly Error None = Error.None;

    public static readonly Error NullResponse = new("NullResponse", "The database response returned null.");
    public static readonly Error AddResponseNull = new("AddResponseNull", "Add action returned null value");
    public static readonly Error DeleteResponseNull = new("DeleteResponseNull", "Delete action returned null value");
    public static readonly Error UpdateResponseNull = new("UpdateResponseNull", "Update action returned null value");
    public static readonly Error SaveResponseNull = new("SaveResponseNull", "Save action returned null value");

    public static readonly Error ContactNotFound = new("ContactNotFound", "The selected contact was not found.");
    public static readonly Error ContactExists = new("ContactExists", "A contact with that information already exists.");
    public static readonly Error ContactDoesNotExist = new("ContactDoesNotExist", "A contact with that information does not exist.");
    public static readonly Error NoContactsInCategory = new("NoContactsInCategory", "No contacts where found in the selected category");

    public static readonly Error UpdateDataFailed = new("UpdateDataFailed", "Database returned no records were successfully updated");
}
