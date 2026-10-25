namespace PhoneBook.Domain.Validation.Errors;

public static class Errors
{
    public static readonly Error None = Error.None;

    public static readonly Error GenericNull = new("GenericNull", "");
    public static readonly Error InvalidKeyPress = new("InvalidKeyPress", "The selected key was not a valid option.");
}