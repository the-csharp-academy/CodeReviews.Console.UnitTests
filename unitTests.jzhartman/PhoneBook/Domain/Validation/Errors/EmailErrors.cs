namespace PhoneBook.Domain.Validation.Errors;

public class EmailErrors
{
    public static readonly Error None = Error.None;

    public static readonly Error NullResponse = new("NullResponse", "The email service response returned null.");
}
