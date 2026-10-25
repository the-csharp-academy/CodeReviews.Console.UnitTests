namespace PhoneBook.Domain.Validation.Errors;

public class CategoryRepositoryErrors
{
    public static readonly Error None = Error.None;

    public static readonly Error NullResponse = new("NullResponse", "The database response returned null.");
    public static readonly Error AddResponseNull = new("AddResponseNull", "Add action returned null value");
    public static readonly Error DeleteResponseNull = new("DeleteResponseNull", "Delete action returned null value");
    public static readonly Error UpdateResponseNull = new("UpdateResponseNull", "Update action returned null value");

    public static readonly Error DeleteDefault = new("DeleteDefault", "Cannot delete the default category UNCATEGORIZED!");
    public static readonly Error UpdateDefault = new("UpdateDefault", "Cannot rename default category.");

    public static readonly Error CategoryNotFound = new("CategoryNotFound", "The selected category was not found.");
    public static readonly Error CategoryExists = new("CategoryExists", "A category with that name already exists.");
    public static readonly Error CategoryDoesNotExist = new("CategoryDoesNotExist", "A contact with that name does not exist.");

    public static readonly Error UpdateDataFailed = new("UpdateDataFailed", "Database returned no records were successfully updated");

}
